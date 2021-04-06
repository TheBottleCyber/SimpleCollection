using System;
using System.Diagnostics;
using System.Linq;
using SimpleCollection;

namespace Usage
{
    class Program
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var simpleCollectionNumeric = new SimpleCollection<int, string, string>(false);
            for (int i = 0; i < 1000; i++)
            {
                simpleCollectionNumeric.Add(i, RandomString(16), $"{i}");
            }

            stopwatch.Stop();
            Console.WriteLine($"Elapsed time for create collection with onlyUniqueKey = false is {stopwatch.Elapsed}");

            stopwatch = new Stopwatch();
            stopwatch.Start();

            var simpleCollectionGuid = new SimpleCollection<Guid, string, string>();
            for (int i = 0; i < 1000; i++)
            {
                simpleCollectionGuid.Add(Guid.NewGuid(), RandomString(16), $"{i}");
            }

            stopwatch.Stop();
            Console.WriteLine($"Elapsed time for create collection with onlyUniqueKey = true  is {stopwatch.Elapsed}\n");

            // Методы добавления и удаления значений Value по ключу (Id, Name);
            var itemIndex = random.Next(0, 1000 - 1);
            Console.WriteLine($"Random index is {itemIndex}");
            var item = simpleCollectionNumeric.Find(x => x.Id == itemIndex);
            Console.WriteLine($"Current item Value is \"{item.Value}\"");
            item.Value = "hello world";

            simpleCollectionNumeric.UpdateItem(itemIndex, item);
            Console.WriteLine($"Current item Value is \"{item.Value}\" after update");

            item.Value = string.Empty;
            simpleCollectionNumeric.UpdateItem(itemIndex, item);
            Console.WriteLine($"Current item Value is \"{item.Value}\" after update");

            item.Value = $"{itemIndex}";
            simpleCollectionNumeric.UpdateItem(itemIndex, item);
            Console.WriteLine($"Current item Value is \"{item.Value}\" after update");

            //методы получения элементов Value по составному ключу (Id, Name), по частям
            // составного ключа, отдельно по (Id) или (Name);
            var valueId = simpleCollectionNumeric.FindValue(item.Id);
            var valueName = simpleCollectionNumeric.FindValue(item.Name);
            var valueIdName = simpleCollectionNumeric.FindValue(item.Id, item.Name);

            Console.WriteLine($"Current item Value from Id: \"{valueId}\", Name: \"{valueName}\", Key: \"{valueIdName}\"\n");

            //Продемонстрируйте использование класса-коллекции, где один из типов составного ключа
            // (Id) или (Name) реализован в виде пользовательского типа, объекты которого сравниваются
            // по значению.

            foreach (var itemScheme in simpleCollectionNumeric)
            {
                if (itemScheme.Id == item.Id || itemScheme.Name == item.Name) Console.WriteLine("Passed");
            }
        }
    }
}
