using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoLotDAL.DataOperations;
using AutoLotDAL.Models;
using AutoLotDAL.Bulklmport;

namespace AutoLotClient
{
    class Program
    {
        static void Main(string[] args)
        {
            MoveCustomer();
            DoBulkCopy();
            InventoryDAL dal = new InventoryDAL();
            var list = dal.GetAllInventory();
            Console.WriteLine("CarId \t Make \t Color \t Pet Name");
            foreach (var itm in list)
            {
                Console.WriteLine($" {itm.Carld} \t {itm.Make} \t{itm.Color} \t{itm.PetName} ");
            }
            Console.WriteLine();
            //var car = dal.GetCar(list.OrderBy(x => x.Color).Select(x => x.Carld).First());
            //Console.WriteLine(" **************First Car By Color * ************* ");
            //Console.WriteLine("CarId \t Make \t Color \t Pet Name");
            //Console.WriteLine($"{ car.Carld} \t { car.Make} \t { car.Color} \t { car.PetName}");
            //try
            //{
            //    dal.DeleteCar(5);
            //    Console.WriteLine("Car deleted.");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"An exception occurred: {ex.Message}");
            //    // Возникло исключение
            //}
            //dal.InsertAuto(new Car
            //{
            //    Color = "Blue",
            //    Make ="Pilot", PetName = "TowMonster"}) ;
            //list = dal.GetAllInventory();
            //var newCar = list.First(x => x.PetName == "TowMonster");
            //Console.WriteLine(" ************** New Car ************** ");
            //Console.WriteLine("CarId \t Make \t Color \t Pet Name");
            //Console.WriteLine($"{newCar.Carld} \t {newCar.Make} \t {newCar.Color} \t { newCar.PetName}"); 
            //dal.DeleteCar(newCar.Carld);
            //var petName = dal.LookUpPetName(car.Carld);
            //Console.WriteLine(" ************** New Car ************** ");
            //Console.WriteLine($"Car pet name: {petName}");
            //Console.Write("Press enter to continue...");
            //Console.ReadLine();
        }

        public static void MoveCustomer()
        {
            Console.WriteLine("***** Simple Transaction Example *****\n");
            // Простой способ позволить транзакции успешно завершиться или отказать,
            bool throwEx = true;
            Console.Write("Do you want to throw an exception (Y or N) : ");
            // Хотите ли вы сгенерировать исключение?
            var userAnswer = Console.ReadLine();
            if (userAnswer?.ToLower() == "n")
                throwEx = false;
            var dal = new InventoryDAL();
                // Обработать клиента 1 - ввести идентификатор клиента,
                // подлежащего перемещению.
            dal.ProcessCreditRisk(throwEx, 1);
            Console.WriteLine("Check CreditRisk table for results");
                // Результаты ищите в таблице CreditRisk
            Console.ReadLine();
            
        }

        public static void DoBulkCopy()
        {
            Console.WriteLine(" ************** Do Bulk Copy ************** ");
            var cars = new List<Car>
            {
                new Car() {Color = "Blue", Make = "Honda", PetName = "MyCarl"},
                new Car() {Color = "Red", Make = "Volvo", PetName = "MyCar2" },
                new Car() { Color = "White", Make = "VW", PetName = "МуСагЗ" },
                new Car() { Color = "Yellow", Make = "Toyota", PetName = "MyCar4" }
            };

            ProcessBulkImpor.ExecuteBulkImport(cars, "Test");
        }
    }
}
