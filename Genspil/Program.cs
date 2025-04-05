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
        static RequestInventory requests = new RequestInventory();

        static void Main(string[] args)
        {

            // Tilføjelse af spil til listen
            //inventory.AddGame(new Game("Sequence", Game.Condition.God, 150, 1));
            //inventory.AddGame(new Game("Ticket to ride", Game.Condition.God, 150, 3));
            //inventory.AddGame(new Game("7 Wonders", Game.Condition.OK, 100, 2));

            // Tilføj nyt spil fra brugerinput (valgte lige en menu, ellers så kan man kun tilføje et spil også stopper programmet)
            ShowMenu();


        }

        public static void ShowMenu() // første menu hvor du kan vælge mellem lagerliste og forespørgsler
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

        public static void ShowInventoryMenu() // Lagerliste menu
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
                    case "1": // kalder spil tilføjelses metode
                        inventory.AddGameFromUserInput();
                        break;
                    case "2": // kalder spil fjerne metode
                        Console.Write("Indtast navnet på spillet, der skal fjernes: ");
                        string nameToRemove = Console.ReadLine();
                        inventory.RemoveGame(nameToRemove);
                        break;
                    case "3": // kalder spil redigerings metode
                        Console.WriteLine("Rediger spil");
                        Console.Write("Indtast navnet på spillet du vil redigere: ");
                        string gameName = Console.ReadLine();

                        // Hent listen over spil fra inventory
                        var games = inventory.LoadGamesFromJson();

                        // Find alle spil med det samme navn
                        var matchingGames = games.Where(g => g.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase)).ToList();

                        if (matchingGames.Count() == 0)  // Brug Count() som metode
                        {
                            Console.WriteLine("Ingen spil fundet med det navn.");
                            break;
                        }

                        string gameVersion = "";

                        if (matchingGames.Count() == 1)
                        {
                            gameVersion = matchingGames[0].Version;
                            Console.WriteLine($"Spillet {gameName} ({gameVersion}) er fundet.");
                        }
                        else
                        {
                            Console.WriteLine("Flere spil fundet. Vælg en version:");
                            for (int i = 0; i < matchingGames.Count(); i++)
                            {
                                Console.WriteLine($"{i + 1}. {matchingGames[i].Name} ({matchingGames[i].Version}) - Lager: {matchingGames[i].Stock}");
                            }

                            int selectedIndex;
                            while (true)
                            {
                                Console.Write("Indtast nummeret på den version, du vil redigere: ");
                                if (int.TryParse(Console.ReadLine(), out selectedIndex) && selectedIndex > 0 && selectedIndex <= matchingGames.Count())
                                {
                                    gameVersion = matchingGames[selectedIndex - 1].Version;
                                    break;
                                }
                                Console.WriteLine("Ugyldigt valg, prøv igen.");
                            }
                        }

                        Console.WriteLine("Vælg hvad du vil ændre:");
                        Console.WriteLine("1. Fjern 1 fra lager");
                        Console.WriteLine("2. Fjern spillet helt");
                        Console.WriteLine("3. Rediger navn");
                        Console.WriteLine("4. Rediger version");
                        Console.WriteLine("5. Rediger lagerbeholdning");
                        Console.WriteLine("6. Rediger genre");
                        Console.WriteLine("7. Rediger pris");
                        Console.Write("Indtast dit valg: ");
                        string editChoice = Console.ReadLine();

                        string newValue = "";
                        string action = "";

                        switch (editChoice)
                        {
                            case "1":
                                action = "remove_one";
                                break;
                            case "2":
                                action = "remove_all";
                                break;
                            case "3":
                                Console.Write("Indtast det nye navn: ");
                                newValue = Console.ReadLine();
                                action = "edit_name";
                                break;
                            case "4":
                                Console.Write("Indtast den nye version: ");
                                newValue = Console.ReadLine();
                                action = "edit_version";
                                break;
                            case "5":
                                Console.Write("Indtast den nye lagerbeholdning: ");
                                newValue = Console.ReadLine();
                                action = "edit_stock";
                                break;
                            case "6":
                                Console.Write("Indtast den nye genre: ");
                                newValue = Console.ReadLine();
                                action = "edit_genre";
                                break;
                            case "7":
                                Console.Write("Indtast den nye pris: ");
                                newValue = Console.ReadLine();
                                action = "edit_price";
                                break;
                            default:
                                Console.WriteLine("Ugyldigt valg.");
                                break;
                        }

                        if (!string.IsNullOrEmpty(action))
                        {
                            inventory.EditGame(gameName, gameVersion, action, newValue);
                        }
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
                    case "6": // afslutter lykken 
                        slut = true;
                        break;
                    default: // respons til forkert input
                        Console.WriteLine("fix");
                        break;
                }
            }
        }



        public static void ShowRequestMenu() // request menu
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
                        // tilføj forespørgsel
                        requests.AddRequest(inventory);  
                        break;
                    case "2":
                        // fjern forespørgsel
                        requests.RemoveRequest();
                        break;
                    case "3":
                        // vis forespørgseler
                        requests.ListRequests();
                        break;
                    case "4": 
                        // tilbage funktion
                        slut = true;
                        break;
                    default:
                        // respons til forkert input
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
        public string Username { get; set; }

        // Constructor
        public Request(string name, double maxprice, string username)
        {
            Name = name;
            MaxPrice = maxprice;
            Username = username;
        }

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
