using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace EmployeGenerate
{
    internal class Program
    {
        public class listaelemek
        {
            public int id;
            public string first_name;
            public string last_name;
            public string position;
            public DateTime birth_date;
            public DateTime hire_date;
            public int salary;
            public string email;
            public string phone_number;
            public string location;
            public string department;
            public string address;
            public string password;
            public int supervisor_id;
            public string employment_status;
            public DateTime contract_date;
        }

        // Segédmetódus ékezetes karakterek eltávolításához
        static string RemoveAccents(string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormKD);
            Encoding removal = Encoding.GetEncoding(Encoding.ASCII.CodePage,
                                                  new EncoderReplacementFallback(""),
                                                  new DecoderReplacementFallback(""));
            byte[] bytes = removal.GetBytes(normalized);
            return Encoding.ASCII.GetString(bytes);
        }

        // Segédmetódus érvényes e-mail cím létrehozásához
        static string GenerateEmail(string firstName, string lastName, int id)
        {
            // Ékezetek eltávolítása és kisbetűssé alakítás
            string cleanFirstName = RemoveAccents(firstName).ToLower();
            string cleanLastName = RemoveAccents(lastName).ToLower();

            // Speciális karakterek eltávolítása
            cleanFirstName = Regex.Replace(cleanFirstName, @"[^a-z]", "");
            cleanLastName = Regex.Replace(cleanLastName, @"[^a-z]", "");

            return $"{cleanFirstName[0]}{cleanLastName}{id}@gmail.com";
        }

        // WriteCSV metódus, amely elvégzi a CSV fájlba írást
        static void WriteCSV(string filename, List<listaelemek> lista)
        {
            using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
            {
                // CSV fejléc
                sw.WriteLine("id,first_name,last_name,position,birth_date,hire_date,salary,email,phone_number,department,address,supervisor_id,employment_status,contract_date");

                // Minden elem kiírása
                foreach (var e in lista)
                {
                    sw.WriteLine($"{e.id},{e.first_name},{e.last_name},{e.position},{e.birth_date:yyyy-MM-dd},{e.hire_date:yyyy-MM-dd},{e.salary},{e.email},{e.phone_number},{e.department},{e.address},{e.supervisor_id},{e.employment_status},{e.contract_date:yyyy-MM-dd},{e.password}");
                }
            }
        }

        static void Main(string[] args)
        {
            string[] locations = {   "Budapest", "Debrecen", "Szeged", "Miskolc", "Pécs", "Győr", "Nyíregyháza", "Kecskemét", "Székesfehérvár", "Szombathely",
                                    "Szolnok", "Tatabánya", "Kaposvár", "Eger", "Veszprém", "Békéscsaba", "Zalaegerszeg", "Sopron", "Érd", "Nagykanizsa",
                                    "Dunaújváros", "Hódmezővásárhely", "Szigetszentmiklós", "Cegléd", "Baja", "Esztergom", "Komárom", "Gyöngyös", "Sárvár", "Vác",
                                    "Orosháza", "Mosonmagyaróvár", "Salgótarján", "Hajdúböszörmény", "Pápa", "Kazincbarcika", "Balassagyarmat", "Kiskunfélegyháza", "Jászberény", "Makó",
                                    "Ózd", "Budaörs", "Szentendre", "Gyula", "Kiskunhalas", "Dunakeszi", "Gödöllő", "Törökszentmiklós", "Vecsés", "Ajka",
                                    "Fót", "Gyál", "Komló", "Dabas", "Hatvan", "Tiszaújváros", "Karcag", "Sárbogárd", "Kalocsa", "Csongrád",
                                    "Mezőkövesd", "Püspökladány", "Tapolca", "Marcali", "Berettyóújfalu", "Nagykőrös", "Bonyhád", "Kisvárda", "Celldömölk", "Lenti",
                                    "Balatonfüred", "Szekszárd", "Kőszeg", "Siklós", "Mór", "Mohács", "Balatonalmádi", "Zirc", "Szentgotthárd", "Tata",
                                    "Körmend", "Füzesabony", "Tiszafüred", "Gyömrő", "Nyírbátor", "Kaba", "Hajdúszoboszló", "Pilisvörösvár", "Nagymaros", "Budakeszi",
                                    "Pilisszentiván", "Csömör", "Gárdony", "Ráckeve", "Kunszentmiklós", "Sümeg", "Lőrinci", "Nagykáta", "Tolna", "Jászapáti"};
            string[] first_name = { "László", "István", "József", "János", "Zoltán", "Sándor", "Gábor", "Ferenc", "Attila", "Péter", "Anna", "Mária", "Katalin", "Éva", "Zsuzsanna", "Eszter", "Krisztina", "Andrea", "Gabriella", "Tímea" };
            string[] last_name = { "Nagy", "Kovács", "Tóth", "Szabó", "Horváth", "Varga", "Kiss", "Molnár", "Németh", "Farkas", "Balogh", "Papp", "Takács", "Juhász", "Lakatos", "Oláh", "Simon", "Rácz", "Fekete", "Fehér" };
            string[] address = { "Petőfi utca", "Kossuth utca", "Széchenyi utca", "Rákóczi utca", "Dózsa György utca", "Ady Endre utca", "Bartók Béla utca", "Jókai utca", "Hunyadi utca", "Kossuth tér" };

            string[] departmentOptions = { "Kezdő", "Haladó" };

            List<listaelemek> lista = new List<listaelemek>();
            Random random = new Random();

            // Először legyártunk 20 születési dátumot úgy, hogy az első ember biztos a legidősebb legyen
            List<DateTime> birthDates = new List<DateTime>();

            birthDates.Add(new DateTime(1940, 1, 1)); // id = 1, legidősebb
            for (int i = 1; i < 20; i++)
            {
                int year = random.Next(1943, 2003); // hogy 20+ éves legyen 2024-ben
                int month = random.Next(1, 13);
                int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);
                birthDates.Add(new DateTime(year, month, day));
            }

            for (int i = 1; i <= 20; i++)
            {
                listaelemek e = new listaelemek();
                e.id = i;
                e.first_name = first_name[random.Next(first_name.Length)];
                e.last_name = last_name[random.Next(last_name.Length)];
                e.position = (i <= 11) ? "irodista" : "kintis";
                e.location = locations[random.Next(locations.Length)];
                // Születési dátum
                e.birth_date = birthDates[i - 1];

                e.password = "jelszo";
                // Hire date csak ha elmúlt 20 éves
                DateTime minHireDate = e.birth_date.AddYears(20);
                int startYear = Math.Max(minHireDate.Year, 2000);
                int endYear = 2015;

                if (startYear > endYear)
                {
                    // Ha a legkorábbi lehetséges hire_date már 2015 után lenne, akkor fixen 2015-re állítjuk
                    startYear = endYear;
                }

                int year = random.Next(startYear, endYear + 1);
                int month = random.Next(1, 13);
                int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);
                e.hire_date = new DateTime(year, month, day);

                // Department és fizetés
                if (i == 1)
                {
                    e.department = "Szakértő";
                    e.salary = 1000000;
                }
                else
                {
                    e.department = departmentOptions[random.Next(departmentOptions.Length)];
                    e.salary = e.department == "Kezdő" ? 250000 : 600000;
                }

                // Email és telefonszám - most már az új metódussal
                e.email = GenerateEmail(e.first_name, e.last_name, e.id);
                e.phone_number = "06" + random.Next(1, 8) + "0" + random.Next(1000000, 9999999);
                e.address = address[random.Next(address.Length)] + " " + random.Next(1, 99);
                e.supervisor_id = 1;
                e.employment_status = "aktív";

                // Contract date: 1 héttel a felvétel előtt
                e.contract_date = e.hire_date.AddDays(-7);

                lista.Add(e);
            }

            // WriteCSV metódus hívása
            WriteCSV("Employees.csv", lista);

            Console.WriteLine("Employees.csv sikeresen létrehozva.");
        }
    }
}