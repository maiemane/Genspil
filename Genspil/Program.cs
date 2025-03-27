using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Schema;
using Newtonsoft.Json; //tilføj json.net via NuGet

namespace Genspil
{
    internal class Program
    {
        // Inventory liste
        static GameInventory inventory = new GameInventory();

        static void Main(string[] args)
        {

            // Tilføjelse af spil til listen
            //inventory.AddGame(new Game("Sequence", Game.Condition.God, 150, 1));
            //inventory.AddGame(new Game("Ticket to ride", Game.Condition.God, 150, 3));
            //inventory.AddGame(new Game("7 Wonders", Game.Condition.OK, 100, 2));

            // Tilføj nyt spil fra brugerinput (valgte lige en menu, ellers så kan man kun tilføje et spil også stopper programmet)
            ShowMenu();


        }

        public static void ShowMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("#####Spil Inventory Menu######");
                Console.WriteLine("1. Lagerliste");
                Console.WriteLine("2. Forespørgsler");
                Console.WriteLine("3. Afslut");
                Console.Write("Vælg en mulighed: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ShowInventoryMenu();
                        break;
                    case "2":
                        ShowRequestMenu();
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("fix");
                        break;
                }
            }
        }

        public static void ShowInventoryMenu()
        {
            bool slut = false;

            while (!slut)
            {
                Console.Clear();
                Console.WriteLine("#####Lagerliste Menu######");
                Console.WriteLine("1. Tilføj");
                Console.WriteLine("2. Fjern");
                Console.WriteLine("3. Rediger");
                Console.WriteLine("4. Vis lagerliste");
                Console.WriteLine("5. Søg i lagerliste");
                Console.WriteLine("6. Tilbage");
                Console.Write("Vælg en mulighed: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        inventory.AddGameFromUserInput();
                        break;
                    case "2":
                        Console.Write("Indtast navnet på spillet, der skal fjernes: ");
                        string nameToRemove = Console.ReadLine();
                        inventory.RemoveGame(nameToRemove);
                        break;
                    case "3":
                        // redgier lager
                        // inventory.EditGame(); ??
                        break;
                    case "4":
                        inventory.ListGames();
                        break;
                    case "5":
                        // søgefunktion
                        // inventory.SearchGame(); ??
                        // tænker menuen går gennem de forskellige søgekriterier, og hvis man ikke taster noget så er det ikke relevant til søgningen. Nem måde at kombinere søgninger på
                        inventory.SearchAndDisplay();
                        break;
                    case "6":
                        slut = true;
                        break;
                    default:
                        Console.WriteLine("fix");
                        break;
                }
            }
        }

        public static void ShowRequestMenu()
        {
            bool slut = false;

            while (!slut)
            {
                Console.Clear();
                Console.WriteLine("#####Forespørgsler Menu######");
                Console.WriteLine("1. Opret forespørgsel");
                Console.WriteLine("2. Fjern forespørgsel");
                Console.WriteLine("3. Vis forespørgsler");
                Console.WriteLine("4. Tilbage");
                Console.Write("Vælg en mulighed: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        // opret forespørgsel
                        // request.CreateRequest(); ??
                        break;
                    case "2":
                        // fjern forespørgsel
                        // request.RemoveRequest(); ??
                        break;
                    case "3":
                        // vis forespørgseler
                        // request.RemoveRequest(); ??
                        break;
                    case "4":
                        slut = true;
                        break;
                    default:
                        Console.WriteLine("fix");
                        break;
                }
            }
        }


    }

    public class Request
    {
        // Properties
        public string Name { get; set; }
        public double MaxPrice { get; set; }

        // Constructor
        public Request(string name, double maxprice)
        {
            Name = name;
            MaxPrice = maxprice;
        }

    }

    public class RequestList {
    }

    public class Game
    {
        // Properties
        public string Name { get; set; }
        public Condition GameCondition { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public string Genre { get; set; }
        public int GroupSize { get; set; }
        public string Version { get; set; }

        // Constructor
        public Game(string name, Condition condition, double price, int stock, string genre, int groupSize, string version)
        {
            Name = name;
            GameCondition = condition;
            Price = price;
            Stock = stock;
            Genre = genre;
            GroupSize = groupSize;
            Version = version;
        }

        // Enum for stand
        public enum Condition
        {
            God,
            OK,
            Slidt,
            TilReperation,
        }
    }
}
