using System;

namespace Lab2_0110D.Common
{
    public class Laptop : Device
    {
        public string Processor { get; set; }

        public Laptop(string name, double price, string processor) : base(name, price)
        {
            Processor = processor;
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"Процесор: {Processor}");
        }

        public static Laptop CreateNew()
        {
            var rnd = new Random();
            string[] processors = { "Intel i5", "Intel i7", "AMD Ryzen 5", "Apple M2" };
            return new Laptop("Ноутбук " + rnd.Next(1000), rnd.Next(15000, 60000), processors[rnd.Next(processors.Length)]);
        }
    }
}
