using System;

namespace Lab2_0110D.Common
{
    public class Accessory : Device
    {
        public string Type { get; set; }

        public Accessory(string name, double price, string type) : base(name, price)
        {
            Type = type;
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"Тип аксесуара: {Type}");
        }

        public static Accessory CreateNew()
        {
            var rnd = new Random();
            string[] types = { "Мишка", "Клавіатура", "Навушники", "Коврик" };
            return new Accessory("Аксесуар " + rnd.Next(1000), rnd.Next(300, 2000), types[rnd.Next(types.Length)]);
        }
    }
}
