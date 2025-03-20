using System;
using System.Collections.Generic;
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
                        Console.WriteLine("Tryk på en tast for at gå tilbage...");
                        Console.ReadKey();
                        break;
                    case "5":
                        // søgefunktion
                        // inventory.SearchGame(); ??
                        // tænker menuen går gennem de forskellige søgekriterier, og hvis man ikke taster noget så er det ikke relevant til søgningen. Nem måde at kombinere søgninger på
                        GameSearch search = new GameSearch(inventory);
                        search.SearchAndDisplay();
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

    public class GameInventory
    {
        private List<Game> games = new List<Game>(); // Liste over spil i hukommelsen (Maise og Anders)
        private static string FilePath = "games.json"; // Placering af vores JSON-fil

        public GameInventory() // Constructor
        {
            // Indlæs spil fra JSON-filen 
            games = LoadGamesFromJson();
        }
        public List<Game> GetGames() 
        {
            return LoadGamesFromJson();
        }


        // Tilføj spil til listen (opdaterer lager, hvis spillet findes)
        public void AddGame(Game game)
        {
            // Tjek om spillet allerede findes med samme navn og stand man kunne vælge pris også, men går ud fra samme spil samme stand = samme pris
            Game existingGame = games.FirstOrDefault(g => g.Name == game.Name && g.GameCondition == game.GameCondition);

            if (existingGame != null)
            {
                // Hvis spillet findes, opdater lageret istedet for at tilføje en ny entry, siden vi ikke har et id er det dog case sensitive
                existingGame.Stock += game.Stock;
                Console.WriteLine($"Lager opdateret! {game.Name} ({game.GameCondition}) har nu {existingGame.Stock} stk.");
            }
            else
            {
                // Hvis spillet ikke findes, tilføj det til listen
                games.Add(game);
                Console.WriteLine($"Nyt spil tilføjet: {game.Name} ({game.GameCondition}) med {game.Stock} stk.");
            }

            // Gem den opdaterede liste til JSON-filen
            SaveGamesToJson();
        }

        // Fjern et spil fra listen
        // skal laves lidt om, så man kan fjerne 1 stk med en stand fx istedet for at fjerne alle med samme navn
        public void RemoveGame(String nameToRemove)
        {
            Game gameToRemove = games.FirstOrDefault(g => g.Name == nameToRemove);

            if (gameToRemove != null)
            {
                if (games.Remove(gameToRemove))
                {
                    Console.WriteLine($"Spillet {gameToRemove.Name} er fjernet fra listen.");
                    SaveGamesToJson(); // Gem ændringerne
                }
                else
                {
                    Console.WriteLine($"Spillet {gameToRemove.Name} blev ikke fundet.");
                }
            }
            else
            {
                Console.WriteLine("Spillet blev ikke fundet.");
            }


        }

        // Vis alle spil i listen
        public void ListGames()
        {
            if (games.Count == 0)
            {
                Console.WriteLine("Der er ingen spil i listen.");
            }
            else
            {
                Console.WriteLine("######SPILOVERSIGT#######");
                foreach (Game game in games)
                {
                    Console.WriteLine($"Spil: {game.Name}, Stand: {game.GameCondition}, Pris: {game.Price} kr, Antal: {game.Stock} stk.");
                }
            }
        }

        // Tilføj et spil via brugerinput
        public void AddGameFromUserInput()
        {
            Console.WriteLine("Tilføj et nyt spil:");

            // Navn
            Console.Write("Navn på spil: ");
            string name = Console.ReadLine();

            // Stand
            Console.Write("Stand (God, OK, Slidt): ");
            string conditionInput = Console.ReadLine();
            Game.Condition condition;
            while (!Enum.TryParse(conditionInput, true, out condition))
            {
                //Du er en klovn hvis du har skrevet forkert, prøv igen
                Console.Write("Skriv dog det rigtige ind!. Vælg mellem: " + string.Join(", ", Enum.GetNames(typeof(Game.Condition))) + ". Prøv igen: ");
                conditionInput = Console.ReadLine();
            }

            // Pris
            Console.Write("Pris: ");
            double price;
            //loop rundt, så vi kan blive ved med at tage imod input indtil vi får en double værdi der er større end 0
            while (!double.TryParse(Console.ReadLine(), out price) || price < 0)
            {
                Console.Write("Ugyldig pris. Indtast en positiv værdi: ");
            }

            // Antal der skal tilføjes til lageret samme koncept som above
            Console.Write("Antal: ");
            int stock;
            while (!int.TryParse(Console.ReadLine(), out stock) || stock < 0)
            {
                Console.Write("Ugyldigt antal. Indtast et positivt tal: ");
            }

            // Opret spillet og tilføj det til listen (spillet bliver tilføjet til vores liste i AddGame metoden)
            //Game newGame = new Game(name, condition, price, stock);
            //AddGame(newGame);
        }

        // Gem listen til JSON-filen
        private void SaveGamesToJson()
        {
            string json = JsonConvert.SerializeObject(games, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        // Indlæs spil fra JSON-filen
        private List<Game> LoadGamesFromJson()
        {
            if (File.Exists(FilePath))
            {
                
                string json = File.ReadAllText(FilePath);
                Console.WriteLine("Debug" + json);
                return JsonConvert.DeserializeObject<List<Game>>(json) ?? new List<Game>();

            }
            return new List<Game>();
        }

        // Eriks arbejde







        // Vis spil fra JSON fil
        private void ShowGamesFromJson()
        {
            var loadedGames = LoadGamesFromJson();
            if (loadedGames.Count == 0)
            {
                Console.WriteLine("Der er ingen spil i JSON filen.");
            }
            else
            {
                Console.WriteLine("#####SPIL FRA JSON FIL#####");
                foreach (var game in loadedGames)
                {
                    Console.WriteLine($"Spil: {game.Name}, Stand: {game.GameCondition}, Pris: {game.Price} kr, Antal: {game.Stock} stk.");
                }
            }
        }
    }



}
