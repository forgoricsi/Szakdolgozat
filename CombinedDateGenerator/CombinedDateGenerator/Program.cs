using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

class Program
{
    static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2")); // hexadecimális formátum
            return sb.ToString();
        }
    }

    static string GenerateCustomerEmail(string firstName, string lastName, int id)
    {
        // Remove accents and special characters
        string cleanFirstName = RemoveAccents(firstName).ToLower();
        string cleanLastName = RemoveAccents(lastName).ToLower();

        // Remove any remaining non-letter characters
        cleanFirstName = Regex.Replace(cleanFirstName, @"[^a-z]", "");
        cleanLastName = Regex.Replace(cleanLastName, @"[^a-z]", "");

        // Generate email with first initial + lastname + id
        return $"{cleanFirstName[0]}{cleanLastName}{id}@gmail.com";
    }

    // Add this helper method for accent removal
    static string RemoveAccents(string input)
    {
        string normalized = input.Normalize(NormalizationForm.FormKD);
        Encoding removal = Encoding.GetEncoding(Encoding.ASCII.CodePage,
                                            new EncoderReplacementFallback(""),
                                            new DecoderReplacementFallback(""));
        byte[] bytes = removal.GetBytes(normalized);
        return Encoding.ASCII.GetString(bytes);
    }

    static Random rnd = new Random();

    static string[] firstNames = { "László", "István", "József", "János", "Zoltán", "Sándor", "Gábor", "Ferenc", "Attila", "Péter",
                                    "Tamás", "Miklós", "Zsolt", "András", "Imre", "Csaba", "Béla", "Balázs", "György", "Róbert",
                                    "Dániel", "Márton", "Ádám", "Bence", "Levente", "Máté", "Dominik", "Benedek", "Kristóf", "Gergő",
                                    "Anna", "Mária", "Katalin", "Erzsébet", "Ilona", "Júlia", "Éva", "Zsuzsanna", "Eszter", "Krisztina",
                                    "Hajnalka", "Melinda", "Andrea", "Gabriella", "Tímea", "Ágnes", "Nikolett", "Orsolya", "Réka", "Rita",
                                    "Noémi", "Emese", "Beáta", "Nóra", "Vivien", "Barbara", "Viktória", "Anikó", "Boglárka", "Dóra",
                                    "Enikő", "Ildikó", "Judit", "Margit", "Zsófia", "Hanna", "Lili", "Panna", "Laura", "Dorina",
                                    "Vera", "Virág", "Bianka", "Izabella", "Nina", "Luca", "Jázmin", "Fanni", "Kincső", "Eszter",
                                    "Olívia", "Lilla", "Szonja", "Zselyke", "Natasa", "Adrienn", "Ramóna", "Szilvia", "Bernadett", "Flóra",
                                    "Árpád", "Erik", "Tibor", "Roland", "Ákos", "Albert", "Mátyás", "Arnold", "Ottó", "Zsigmond"};
    static string[] lastNames = { "Nagy", "Kovács", "Tóth", "Szabó", "Horváth", "Varga", "Kiss", "Molnár", "Németh", "Farkas",
                                    "Balogh", "Papp", "Takács", "Juhász", "Lakatos", "Mészáros", "Oláh", "Simon", "Rácz", "Fekete",
                                    "Szilágyi", "Török", "Fehér", "Balázs", "Gál", "Kis", "Szűcs", "Kocsis", "Orsós", "Pintér",
                                    "Fodor", "Szalai", "Sipos", "Magyar", "Lukács", "Gulyás", "Bíró", "Király", "László", "Katona",
                                    "Balog", "Bogdán", "Jakab", "Sándor", "Boros", "Váradi", "Fazekas", "Kelemen", "Antal", "Orosz",
                                    "Somogyi", "Fülöp", "Veres", "Vincze", "Budai", "Hegedűs", "Deák", "Pap", "Bálint", "Pál",
                                    "Illés", "Vass", "Szőke", "Vörös", "Fábián", "Lengyel", "Bognár", "Bodnár", "Szűcs", "Jónás",
                                    "Hajdu", "Halász", "Máté", "Székely", "Kozma", "Gáspár", "Pásztor", "Bakos", "Dudás", "Major",
                                    "Orbán", "Virág", "Hegedüs", "Barna", "Novák", "Soós", "Tamás", "Nemes", "Pataki", "Balla",
                                    "Faragó", "Kerekes", "Borbély", "Barta", "Péter", "Csonka", "Mezei", "Szekeres", "Sárközi", "Márton"};
    static string[] locations = {   "Budapest", "Debrecen", "Szeged", "Miskolc", "Pécs", "Győr", "Nyíregyháza", "Kecskemét", "Székesfehérvár", "Szombathely",
                                    "Szolnok", "Tatabánya", "Kaposvár", "Eger", "Veszprém", "Békéscsaba", "Zalaegerszeg", "Sopron", "Érd", "Nagykanizsa",
                                    "Dunaújváros", "Hódmezővásárhely", "Szigetszentmiklós", "Cegléd", "Baja", "Esztergom", "Komárom", "Gyöngyös", "Sárvár", "Vác",
                                    "Orosháza", "Mosonmagyaróvár", "Salgótarján", "Hajdúböszörmény", "Pápa", "Kazincbarcika", "Balassagyarmat", "Kiskunfélegyháza", "Jászberény", "Makó",
                                    "Ózd", "Budaörs", "Szentendre", "Gyula", "Kiskunhalas", "Dunakeszi", "Gödöllő", "Törökszentmiklós", "Vecsés", "Ajka",
                                    "Fót", "Gyál", "Komló", "Dabas", "Hatvan", "Tiszaújváros", "Karcag", "Sárbogárd", "Kalocsa", "Csongrád",
                                    "Mezőkövesd", "Püspökladány", "Tapolca", "Marcali", "Berettyóújfalu", "Nagykőrös", "Bonyhád", "Kisvárda", "Celldömölk", "Lenti",
                                    "Balatonfüred", "Szekszárd", "Kőszeg", "Siklós", "Mór", "Mohács", "Balatonalmádi", "Zirc", "Szentgotthárd", "Tata",
                                    "Körmend", "Füzesabony", "Tiszafüred", "Gyömrő", "Nyírbátor", "Kaba", "Hajdúszoboszló", "Pilisvörösvár", "Nagymaros", "Budakeszi",
                                    "Pilisszentiván", "Csömör", "Gárdony", "Ráckeve", "Kunszentmiklós", "Sümeg", "Lőrinci", "Nagykáta", "Tolna", "Jászapáti"};
    static string[] addresses = {    "Petőfi utca", "Kossuth utca", "Széchenyi utca", "Rákóczi utca", "Dózsa György utca",
                                    "Arany János utca", "Ady Endre utca", "Bartók Béla utca", "Jókai utca", "Hunyadi utca",
                                    "Kossuth tér", "Szabadság utca", "Temető utca", "Bem utca", "Szent István utca",
                                    "Árpád utca", "Vörösmarty utca", "Béke utca", "Deák Ferenc utca", "Táncsics Mihály utca",
                                    "Tavasz utca", "Rózsa utca", "Kert utca", "Új utca", "Iskola utca",
                                    "Fő utca", "Hársfa utca", "Jázmin utca", "Orgona utca", "Mátyás király utca",
                                    "Nyár utca", "Akácfa utca", "Vasút utca", "Közép utca", "Gyöngyvirág utca",
                                    "Zrínyi utca", "Csokonai utca", "Honvéd utca", "Madách utca", "Hegyalja utca",
                                    "Alkotmány utca", "Rét utca", "Tavaszmező utca", "Szőlő utca", "Munkás utca",
                                    "Rákóczi tér", "Virág utca", "Templom utca", "Sport utca", "Erkel Ferenc utca"};

    class Terminal
    {
        public int Id;
        public string Location;
        public string Address;
        public DateTime CreationDate;
        public string ReleaseVersion;
        public int InstallerId;
        public string Status;
        public DateTime LastMaintenance;
        public string TerminalType;
        public string SoftwareVersion;
        public string IpAddress;
        public string OperatingSystem;
        public string SimCard;
    }

    class Customer
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public string Email;
        public string Phone;
        public string Location;
        public string Address;
        public DateTime BirthDate;
        public DateTime ContractDate;
        public DateTime RegistrationDate;
        public string Status;
        public int TerminalId;
        public string Password;
    }

    class Ticket
    {
        public int Id;
        public int CreatorId;
        public int CustomerId;
        public string Description;
        public string Address;
        public int TerminalId;
        public string Location;
        public string Status;
        public string Priority;
        public DateTime CreateTime;
        public DateTime? ResolutionDate;
        public string IssueType;
        public int TechnicianId;
        public DateTime ServiceDate;
    }

    static void Main()
    {
        var terminals = new List<Terminal>();
        var customers = new List<Customer>();
        var tickets = new List<Ticket>();

        for (int i = 1; i <= 20000; i++)
        {
            var location = locations[rnd.Next(locations.Length)];
            var address = addresses[rnd.Next(addresses.Length)] + " " + rnd.Next(1, 100);
            var creationDate = RandomDate(new DateTime(2015, 1, 1), DateTime.Now.AddMonths(-6));

            terminals.Add(new Terminal
            {
                Id = i,
                Location = location,
                Address = address,
                CreationDate = creationDate,
                ReleaseVersion = "v" + rnd.Next(1, 10) + "." + rnd.Next(0, 10),
                InstallerId = rnd.Next(12, 21),
                Status = "-",
                LastMaintenance = creationDate.AddMonths(rnd.Next(1, 24)),
                TerminalType = "ATM",
                SoftwareVersion = "Soft " + rnd.Next(1, 5) + "." + rnd.Next(0, 10),
                IpAddress = $"192.168.{rnd.Next(0, 256)}.{rnd.Next(0, 256)}",
                OperatingSystem = "Windows",
                SimCard = rnd.Next(100000000, 999999999).ToString()
            });
        }

        var usedTerminalIds = new HashSet<int>();
        for (int i = 1; i <= 10000; i++)
        {
            int terminalId;
            do { terminalId = rnd.Next(1, 20001); } while (!usedTerminalIds.Add(terminalId));
            var term = terminals.First(t => t.Id == terminalId);
            var first = firstNames[rnd.Next(firstNames.Length)];
            var last = lastNames[rnd.Next(lastNames.Length)];

            var contract = RandomDate(term.CreationDate, DateTime.Now);
            customers.Add(new Customer
            {
                Id = i,
                FirstName = first,
                LastName = last,
                Email = GenerateCustomerEmail(first, last, i), // Modified line
                Phone = "+36" + rnd.Next(20, 100) + rnd.Next(1000000, 9999999),
                Location = term.Location,
                Address = term.Address,
                BirthDate = RandomDate(new DateTime(1960, 1, 1), new DateTime(2003, 12, 31)),
                Password = HashPassword("jelszo"),
                ContractDate = contract,
                RegistrationDate = contract,
                Status = rnd.Next(10000) == 0 ? "inaktív" : "aktív",
                TerminalId = terminalId
            });
        }

        int ticketId = 1;
        DateTime today = DateTime.Today;
        DateTime threeDaysLater = today.AddDays(3);

        foreach (var customer in customers)
        {
            var term = terminals.First(t => t.Id == customer.TerminalId);
            var installDate = customer.ContractDate;

            // Determine status for installation ticket
            string installStatus = "elvégezve";
            DateTime? installResolution = installDate.AddDays(14);
            if (installDate > today)
            {
                installStatus = (rnd.Next(2) == 0 && installDate <= threeDaysLater) ? "kiosztva" : "új";
                installResolution = installStatus == "kiosztva" ? installDate.AddDays(14) : (DateTime?)null;
            }

            // Installation ticket
            tickets.Add(new Ticket
            {
                Id = ticketId++,
                CreatorId = rnd.Next(1, 12),
                CustomerId = customer.Id,
                Description = "El kell végezni a telepítést",
                Address = customer.Address,
                TerminalId = customer.TerminalId,
                Location = term.Location, // Set terminal location
                Status = installStatus,
                Priority = RandomPriority(),
                CreateTime = installDate,
                ResolutionDate = installResolution,
                IssueType = "telepítés",
                TechnicianId = installStatus == "új" ? 0 : rnd.Next(12, 21),
                ServiceDate = installDate.AddDays(14)
            });

            int extraTickets = rnd.Next(0, 3);
            for (int i = 0; i < extraTickets; i++)
            {
                string issue = i == extraTickets - 1 && rnd.Next(2) == 0 ? "leszerelés" : "szerviz";
                string desc = issue == "leszerelés" ? "El kell végezni a leszerelést" : "El kell végezni a szervizelést";

                DateTime createTime = tickets.Last().ServiceDate.AddDays(14);
                DateTime serviceDate = createTime.AddDays(14);

                string status;
                DateTime? resolutionDate;

                if (serviceDate > today)
                {
                    status = (rnd.Next(2) == 0 && serviceDate <= threeDaysLater) ? "kiosztva" : "új";
                    resolutionDate = status == "kiosztva" ? serviceDate : (DateTime?)null;
                }
                else
                {
                    status = rnd.Next(10000) == 0 ? "új" : (rnd.Next(10000) == 1 ? "kiosztva" : "elvégezve");
                    resolutionDate = status == "elvégezve" ? serviceDate : (status == "kiosztva" ? serviceDate : (DateTime?)null);
                }

                tickets.Add(new Ticket
                {
                    Id = ticketId++,
                    CreatorId = rnd.Next(1, 12),
                    CustomerId = customer.Id,
                    Description = desc,
                    Address = customer.Address,
                    TerminalId = customer.TerminalId,
                    Location = term.Location, // Set terminal location
                    Status = status,
                    Priority = RandomPriority(),
                    CreateTime = createTime,
                    ResolutionDate = resolutionDate,
                    IssueType = issue,
                    TechnicianId = status == "új" ? 0 : rnd.Next(12, 21),
                    ServiceDate = serviceDate
                });
            }

            // Status update logic remains the same
            var lastTicket = tickets.LastOrDefault(t => t.CustomerId == customer.Id);
            if (lastTicket != null && lastTicket.IssueType == "leszerelés")
            {
                customer.Status = "inaktív";
                terminals.First(t => t.Id == customer.TerminalId).Status = "raktárban";
            }
            else
            {
                terminals.First(t => t.Id == customer.TerminalId).Status =
                    customer.Status == "aktív" && rnd.Next(10000) != 0 ? "aktív" : "hibás";
            }
        }

        WriteCSV("terminals.csv", terminals);
        WriteCSV("customers.csv", customers);
        WriteCSV("service_tickets.csv", tickets);

        Console.WriteLine("CSV fájl létrehozva!");
    }

    static DateTime RandomDate(DateTime start, DateTime end) => start.AddDays(rnd.Next((end - start).Days));
    static string RandomPriority() => new[] { "alacsony", "közepes", "magas" }[rnd.Next(3)];

    static void WriteCSV<T>(string filename, List<T> data)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
        using (var writer = new StreamWriter(path, false, Encoding.UTF8))
        {
            var props = typeof(T).GetFields();
            writer.WriteLine(string.Join(";", props.Select(p => p.Name)));
            foreach (var item in data)
            {
                var values = props.Select(p =>
                {
                    var val = p.GetValue(item);
                    return val == null ? "" : val.ToString();
                });
                writer.WriteLine(string.Join(";", values));
            }
        }
    }
}
