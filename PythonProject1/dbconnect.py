import mysql.connector

def connect():
    try:
        conn = mysql.connector.connect(
            host='localhost',
            user='admin',
            password='root1234',
            database='terminalservice'
        )
        return conn
    except mysql.connector.Error as err:
        print(f"❌ Hiba történt: {err}")
        return None

# Teszteléshez:
if __name__ == "__main__":
    connection = connect()
    if connection:
        connection.close()
