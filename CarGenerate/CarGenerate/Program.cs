using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        List<Car> cars = GenerateCars();
        string filePath = "Cars.csv";

        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            foreach (var car in cars)
            {
                writer.WriteLine($"{car.Id},{car.Brand},{car.Model},{car.Year},{car.LicensePlate},{car.Mileage},{car.CurrentUserId}," +
                                 $"{car.Status},{car.InsuranceDetails},{car.ServiceInfo},{car.FuelType},{car.Condition},{car.StorageLocation}," +
                                 $"{car.Accessories},{car.UseLog}");
            }
        }

        Console.WriteLine($"CSV fájl sikeresen létrehozva: {filePath}");
    }

    static List<Car> GenerateCars()
    {
        Random random = new Random();
        List<Car> carList = new List<Car>();

        string[,] carModels = {
            { "Ford", "Transit" },
            { "Peugeot", "Boxer" },
            { "Fiat", "Ducato" },
            { "Volkswagen", "Crafter" },
            { "Renault", "Master" }
        };

        string[] conditions = { "Uj", "Ujszeru", "Hasznalt" };
        string insurance = "Generali Biztosito";

        List<int> userIds = new List<int> { 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        userIds = ShuffleList(userIds, random);

        int activeCarIndex = 0;

        for (int i = 1; i <= 15; i++)
        {
            int brandIndex = (i - 1) % carModels.GetLength(0);
            string brand = carModels[brandIndex, 0];
            string model = carModels[brandIndex, 1];
            int year = random.Next(2015, 2024);
            string licensePlate = $"ABC-{random.Next(100, 999)}";
            int mileage = random.Next(5000, 200000);
            string status = i <= 9 ? "Aktiv" : "Inaktiv";

            int currentUserId = status == "Aktiv" ? userIds[activeCarIndex++] : 0;

            string serviceInfo = status == "Aktiv" ? (random.Next(2) == 0 ? "Kivalo" : "Jo") :
                                 (i % 3 == 0 ? "Hibas" : (i % 2 == 0 ? "Jo" : "Kivalo"));
            string fuelType = random.Next(2) == 0 ? "Dizel" : "Benzin";
            string condition = year > 2020 ? "Uj" : (year > 2017 ? "Ujszeru" : "Hasznalt");
            string storage = status == "Inaktiv" ? "Budapest" : (i == 15 ? "Debrecen" : "Vac");

            string[] basicAccessories = { "Potkerek", "Mentolada", "Elakadasjelzo haromszog", "Lathatosagi melleny" };
            string accessories = status == "Inaktiv" ? "" : string.Join(", ", basicAccessories);
            if (random.Next(3) == 0) accessories += ", Terminal alkatreszek";

            string[] useLogs = { "Hasznalatban", "Szervizeles alatt", "Pihenoben" };
            string useLog = useLogs[random.Next(useLogs.Length)];

            carList.Add(new Car
            {
                Id = i,
                Brand = brand,
                Model = model,
                Year = year,
                LicensePlate = licensePlate,
                Mileage = mileage,
                CurrentUserId = currentUserId,
                Status = status,
                InsuranceDetails = insurance,
                ServiceInfo = serviceInfo,
                FuelType = fuelType,
                Condition = condition,
                StorageLocation = storage,
                Accessories = accessories,
                UseLog = useLog
            });
        }

        return carList;
    }

    static List<int> ShuffleList(List<int> list, Random random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }
}

class Car
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string LicensePlate { get; set; }
    public int Mileage { get; set; }
    public int CurrentUserId { get; set; }
    public string Status { get; set; }
    public string InsuranceDetails { get; set; }
    public string ServiceInfo { get; set; }
    public string FuelType { get; set; }
    public string Condition { get; set; }
    public string StorageLocation { get; set; }
    public string Accessories { get; set; }
    public string UseLog { get; set; }
}
