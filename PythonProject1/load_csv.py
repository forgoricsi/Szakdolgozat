import csv
import re
import mysql.connector
from dbconnect import connect
from typing import List
from datetime import datetime


def normalize_date(date_str: str) -> str:
    if not date_str or date_str.strip() == '':
        return None  # Explicit None for empty values

    for fmt in ('%Y.%m.%d', '%Y-%m-%d', '%Y/%m/%d', '%m/%d/%Y', '%d.%m.%Y'):
        try:
            dt = datetime.strptime(date_str.strip(), fmt)
            return dt.strftime('%Y-%m-%d')
        except ValueError:
            continue
    return date_str  # Return original if no format matched


def get_table_columns(conn, table_name: str) -> List[str]:
    """Get table columns"""
    cursor = conn.cursor()
    try:
        cursor.execute(f"SHOW COLUMNS FROM {table_name}")
        return [column[0] for column in cursor.fetchall()]
    except mysql.connector.Error as err:
        print(f"❌ Error getting columns: {err}")
        return []
    finally:
        cursor.close()


def create_table(conn, table_name: str, columns: List[str], sample_row: List[str]):
    """Create table with columns"""
    cursor = conn.cursor()

    column_definitions = []
    for col, sample in zip(columns, sample_row):
        sample = str(sample)
        if not sample:
            col_type = "VARCHAR(255)"
        elif sample.isdigit():
            col_type = "INT"
        elif re.match(r'^\d+\.\d+$', sample):
            col_type = "DECIMAL(10,2)"
        elif sample.lower() in ('true', 'false'):
            col_type = "BOOLEAN"
        else:
            if re.match(r'^\d{4}[\.\-/]\d{2}[\.\-/]\d{2}$', sample):
                col_type = "DATE"
            else:
                col_type = "TEXT" if len(sample) > 255 else "VARCHAR(255)"

        column_definitions.append(f"`{col}` {col_type}")

    if 'id' in [col.lower() for col in columns]:
        column_definitions.append("PRIMARY KEY (`id`)")

    create_query = f"""
    CREATE TABLE IF NOT EXISTS `{table_name}` (
        {','.join(column_definitions)}
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci
    """

    try:
        cursor.execute(create_query)
        conn.commit()
    except mysql.connector.Error as err:
        print(f"❌ Error creating table: {err}")
    finally:
        cursor.close()


def upsert_data(conn, table_name: str, columns: List[str], row: List[str]):
    """UPSERT operation with proper NULL handling"""
    cursor = conn.cursor()

    # Normalize data - convert empty strings to None for date fields
    processed_row = []
    for col, value in zip(columns, row):
        if any(d in col.lower() for d in ['date', 'time']):
            if value is None or str(value).strip() == '':
                processed_row.append(None)
            else:
                processed_row.append(normalize_date(value))
        else:
            processed_row.append(value)

    col_names = ','.join([f"`{col}`" for col in columns])
    placeholders = ','.join(['%s'] * len(columns))
    update_set = ','.join([f"`{col}`=VALUES(`{col}`)" for col in columns if col.lower() != 'id'])

    query = f"""
    INSERT INTO `{table_name}` ({col_names}) 
    VALUES ({placeholders})
    ON DUPLICATE KEY UPDATE {update_set}
    """

    try:
        cursor.execute(query, processed_row)
        conn.commit()
    except mysql.connector.Error as err:
        print(f"❌ Error inserting data: {err}")
        print(f"Query: {query}")
        print(f"Row: {processed_row}")
    finally:
        cursor.close()


def load_csv_to_db(filename: str, table_name: str, columns: List[str], delimiter: str = ';'):
    """Load CSV into database with foreign key validation"""
    conn = connect()
    if not conn:
        return

    cursor = conn.cursor()
    try:
        # Disable foreign key checks temporarily
        cursor.execute("SET FOREIGN_KEY_CHECKS=0")
        conn.commit()

        with open(filename, mode='r', encoding='utf-8') as file:
            reader = csv.reader(file, delimiter=delimiter)
            _ = next(reader)  # Skip header

            existing_columns = get_table_columns(conn, table_name)
            if not existing_columns:
                try:
                    sample_row = next(reader)
                    create_table(conn, table_name, columns, sample_row)
                    existing_columns = get_table_columns(conn, table_name)
                except StopIteration:
                    print(f"❌ Empty CSV file: {filename}")
                    return

            common_columns = [col for col in columns
                              if any(col.lower() == existing.lower() for existing in existing_columns)]

            processed_rows = 0
            for row in reader:
                if not row:  # Skip empty rows
                    continue

                # Adjust row length
                if len(row) < len(common_columns):
                    row += [''] * (len(common_columns) - len(row))
                elif len(row) > len(common_columns):
                    row = row[:len(common_columns)]

                # Validate foreign keys
                if table_name == 'servicetickets':
                    creator_idx = common_columns.index('CreatorId')
                    technician_idx = common_columns.index('TechnicianId')

                    # Check CreatorId exists
                    cursor.execute("SELECT 1 FROM employees WHERE id = %s", (row[creator_idx],))
                    if not cursor.fetchone():
                        print(f"⚠️ Skipping row - invalid CreatorId: {row[creator_idx]}")
                        continue

                    # Check TechnicianId if not empty
                    if row[technician_idx] and row[technician_idx] != '0':
                        cursor.execute("SELECT 1 FROM employees WHERE id = %s", (row[technician_idx],))
                        if not cursor.fetchone():
                            print(f"⚠️ Skipping row - invalid TechnicianId: {row[technician_idx]}")
                            continue

                upsert_data(conn, table_name, common_columns, row)
                processed_rows += 1

            print(f"{filename}: {processed_rows} sor betöltve a {table_name} táblába")

    except FileNotFoundError:
        print(f"❌ File not found: {filename}")
    except Exception as e:
        print(f"❌ Unexpected error: {e}")
    finally:
        # Re-enable foreign key checks
        cursor.execute("SET FOREIGN_KEY_CHECKS=1")
        conn.commit()
        cursor.close()
        conn.close()


if __name__ == "__main__":
    # Load tables in proper dependency order
    tables_to_load = [
        ('employees.csv', 'employees', [
            'id', 'first_name', 'last_name', 'position', 'birth_date',
            'hire_date', 'salary', 'email', 'phone_number', 'department',
            'address', 'supervisor_id', 'employment_status', 'contract_date', 'password', 'location'
        ]),
        ('terminals.csv', 'terminals', [
            'Id', 'Location', 'Address', 'CreationDate', 'ReleaseVersion',
            'InstallerId', 'Status', 'LastMaintenance', 'TerminalType',
            'SoftwareVersion', 'IpAddress', 'OperatingSystem', 'SimCard'
        ]),
        ('customers.csv', 'customers', [
            'Id', 'FirstName', 'LastName', 'Email', 'Phone', 'Location',
            'Address', 'BirthDate', 'ContractDate', 'RegistrationDate',
            'Status', 'TerminalId', 'Password'
        ]),
        ('cars.csv', 'cars', [
            'Id', 'Brand', 'Model', 'Year', 'LicensePlate', 'Mileage',
            'CurrentUserId', 'Status', 'InsuranceDetails', 'ServiceInfo',
            'FuelType', 'Conditionn', 'StorageLocation', 'Accessories', 'UseLog'
        ]),
        ('service_tickets.csv', 'servicetickets', [
            'Id', 'CreatorId', 'CustomerId', 'Description', 'Address',
            'TerminalId', 'Status', 'Priority', 'CreateTime',
            'ResolutionDate', 'IssueType', 'TechnicianId', 'ServiceDate'
        ])
    ]

    for filename, table_name, columns in tables_to_load:
        print(f"Adatok feldolgozása {filename}-ből")
        load_csv_to_db(filename, table_name, columns)