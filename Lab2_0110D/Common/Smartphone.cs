using System;

namespace Lab2_0110D.Common
{
    public class Smartphone : Device
    {
        public string OS { get; set; }

        public Smartphone(string name, double price, string os) : base(name, price)
        {
            OS = os;
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"ОС: {OS}");
        }

        public static Smartphone CreateNew()
        {
            var rnd = new Random();
            string[] osOptions = { "Android", "iOS", "HarmonyOS" };
            return new Smartphone("Смартфон " + rnd.Next(1000), rnd.Next(5000, 30000), osOptions[rnd.Next(osOptions.Length)]);
        }
    }
}
