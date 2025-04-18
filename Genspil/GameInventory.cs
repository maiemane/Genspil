using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json; //tilføj json.net via NuGet

namespace Genspil
{
    public class GameInventory
    {
        private List<Game> games = new List<Game>(); // Liste over spil i hukommelsen, som så bliver tilføjet til vores json fil
        private static string FilePath = "games.json"; // Placering af vores JSON-fil
        private RequestInventory requestInventory = new RequestInventory();

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

            var matchingRequests = requestInventory.LoadRequestsFromJson()
                .Where(r => r.Name.Equals(game.Name, StringComparison.OrdinalIgnoreCase) && r.MaxPrice >= game.Price)
                .ToList();

            foreach (var request in matchingRequests)
            {
                requestInventory.RemoveRequestAuto(request.Name, request.Username);
                Console.WriteLine($"Request fjernet: {request.Name} fra {request.Username} (pris ≤ {request.MaxPrice} kr)");
            }
            // Gem den opdaterede liste til JSON-filen
            SaveGamesToJson();
        }

        // Fjern et spil fra listen
        // skal laves lidt om, så man kan fjerne 1 stk med en stand fx istedet for at fjerne alle med samme navn
        // funktion bliver ikke brugt længere, da den er rykket til "EditGame" istedet for
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
                Console.Write("Skriv dog det rigtige ind! Vælg mellem: " + string.Join(", ", Enum.GetNames(typeof(Game.Condition))) + ". Prøv igen: ");
                conditionInput = Console.ReadLine();
            }

            // GroupSize
            Console.Write("Antal spillere (GroupSize): ");
            int groupSize;
            while (!int.TryParse(Console.ReadLine(), out groupSize) || groupSize < 1)
            {
                Console.Write("Ugyldigt antal spillere. Indtast et positivt tal: ");
            }

            // Genre
            Console.Write("Genre: ");
            string genre = Console.ReadLine();

            // Pris
            Console.Write("Pris: ");
            double price;
            while (!double.TryParse(Console.ReadLine(), out price) || price < 0)
            {
                Console.Write("Ugyldig pris. Indtast en positiv værdi: ");
            }

            // Antal der skal tilføjes til lageret
            Console.Write("Antal på lager: ");
            int stock;
            // skal nok ændres til at må inkludere 0, da vi godt kan have spil som er på "efterspørgsel"
            while (!int.TryParse(Console.ReadLine(), out stock) || stock < 0)
            {
                Console.Write("Ugyldigt antal. Indtast et positivt tal: ");
            }

            // Version 
            Console.Write("Version fx Original, Hyggespil: ");
            string version = Console.ReadLine();

            // Opret liste med spil data
            Game newGame = new Game(name, condition, price, stock, genre, groupSize, version);
            // kald AddGame, med vores liste så den kan tilføjes til lageret, eller få opdateret stock
            AddGame(newGame);

            Console.WriteLine("Spillet er blevet tilføjet!");
        }


        public void EditGame(string name, string version, string action, string newValue = "")
        {
            Game gameToEdit = games.FirstOrDefault(g => g.Name == name && g.Version == version);

            if (gameToEdit == null)
            {
                Console.WriteLine("Spillet blev ikke fundet.");
                return;
            }
            //switch case skal kaldes med en string fra menuen fx "remove_one" som action EditGame(name,version, action, newValue) 
            switch (action.ToLower())
            {
                    // fjern en fra lager beholdningen
                case "remove_one":
                    // stock skal selvfølgelig være > 0 for at kunne fjerne 1
                    if (gameToEdit.Stock > 0)
                    {
                        gameToEdit.Stock--;
                        Console.WriteLine($"Der blev fjernet 1 fra lageret af {gameToEdit.Name} ({gameToEdit.Version}). Ny lagerstatus: {gameToEdit.Stock}");
                    }
                    else
                    {
                        Console.WriteLine("Lageret er allerede tomt.");
                    }
                    break;
                // fjern alle med navn & version
                case "remove_all":
                    games.Remove(gameToEdit);
                    Console.WriteLine($"Spillet {gameToEdit.Name} ({gameToEdit.Version}) er fjernet helt fra listen.");
                    break;
                // rediger navn, til "newValue" som sendes fra 
                case "edit_name":
                    gameToEdit.Name = newValue;
                    Console.WriteLine($"Spillets navn er ændret til {newValue}.");
                    break;

                case "edit_version":
                    gameToEdit.Version = newValue;
                    Console.WriteLine($"Spillets version er ændret til {newValue}.");
                    break;
                // rediger "stock" eller lager beholdning
                case "edit_stock":
                    if (int.TryParse(newValue, out int newStock) && newStock >= 0)
                    {
                        gameToEdit.Stock = newStock;
                        Console.WriteLine($"Lageret er opdateret til {newStock}.");
                    }
                    else
                    {
                        Console.WriteLine("Ugyldig værdi for lager.");
                    }
                    break;
                // rediger genre
                case "edit_genre":
                    gameToEdit.Genre = newValue;
                    Console.WriteLine($"Genren er ændret til {newValue}.");
                    break;
                // rediger prisen på "stocken" eller lager beholdningen.
                case "edit_price":
                    if (double.TryParse(newValue, out double newPrice) && newPrice >= 0)
                    {
                        gameToEdit.Price = newPrice;
                        Console.WriteLine($"Prisen er opdateret til {newPrice} kr.");
                    }
                    else
                    {
                        Console.WriteLine("Ugyldig pris.");
                    }
                    break;

                default:
                    Console.WriteLine("Ugyldig handling.");
                    return;
            }

            SaveGamesToJson(); // Gem ændringerne
        }

        
        // Gem listen til JSON-filen
        private void SaveGamesToJson()
        {
            string json = JsonConvert.SerializeObject(games, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        // Indlæs spil fra JSON-filen
        public List<Game> LoadGamesFromJson()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    string json = File.ReadAllText(FilePath);
                    return JsonConvert.DeserializeObject<List<Game>>(json) ?? new List<Game>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fejl ved indlæsning af JSON-fil: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("JSON-filen blev ikke fundet: " + Path.GetFullPath(FilePath));
            }
            return new List<Game>();
        }

        public List<Game> SearchGame(string name = "", string genre = "", int? MinPlayers = null, int? MaxPlayers = null, double? MinPrice = null, double? MaxPrice = null, Game.Condition? condition = null) 
        {
            return games.Where(game =>
                (game != null) &&
                (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(game.Name) && game.Name.Contains(name, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrEmpty(genre) || (!string.IsNullOrEmpty(game.Genre) && game.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))) &&
                (!MinPlayers.HasValue || game.GroupSize >= MinPlayers.Value) &&
                (!MaxPlayers.HasValue || game.GroupSize <= MaxPlayers.Value) &&
                (!condition.HasValue || (int)game.GameCondition == (int)condition.Value) &&
                (!MinPrice.HasValue || game.Price >= MinPrice.Value) &&
                (!MaxPrice.HasValue || game.Price <= MaxPrice.Value)
            ).ToList();
        }

        public void SearchAndDisplay() // metode til at søge i listen
        {
            Console.WriteLine("Søg i liste: ");
            Console.Write("Navn (valgfrit): ");
            string name = Console.ReadLine();

            Console.Write("Genre (valgfrit): ");
            string genre = Console.ReadLine();

            Console.Write("Minimum Antal Spillere (valgfrit): ");
            int? MinPlayers = int.TryParse(Console.ReadLine(), out int Min) ? Min : (int?)null;

            Console.Write("Maximum Antal Spillere (valgfrit): ");
            int? MaxPlayers = int.TryParse(Console.ReadLine(), out int Max) ? Max : (int?)null;

            Console.Write("Minimum Pris (valgfrit): ");
            double? MinPrice = double.TryParse(Console.ReadLine(), out double MinP) ? MinP : (double?)null;

            Console.Write("Maximum Pris (valgfrit): ");
            double? MaxPrice = double.TryParse(Console.ReadLine(), out double MaxP) ? MaxP : (double?)null;

            Console.Write("Stand (0 = God, 1 = Lidt Slidt, 2 = Meget Slidt, 3 = Til Reperation) (valgfrit): ");
            string conditionInput = Console.ReadLine();
            Game.Condition? condition = Enum.TryParse(conditionInput, out Game.Condition ParsedCondition) ? ParsedCondition : (Game.Condition?)null;

            var results = SearchGame(name, genre, MinPlayers, MaxPlayers, MinPrice, MaxPrice, condition);
            Console.WriteLine("Søgeresultat: ");
            if (results.Count == 0)
            {
                Console.WriteLine("Ingen resultater fundet.");
            }
            else
            {
                foreach (var game in results)
                {
                    Console.WriteLine($"Navn: {game.Name}, Genre: {game.Genre}, Antal Spillere: {game.GroupSize}, Pris: {game.Price}, Stand: {game.GameCondition}");
                }
            }

            Console.WriteLine("Tryk for at gå tilbage. ");
            Console.ReadKey();
        }


        private string Pad(string input, int width)
        {
            if (input.Length > width)
                return input.Substring(0, width - 3) + "...";
            return input.PadRight(width);
        }

        public void ListGames() // metode til at vise spil på lager med sortering
        {
            if (games.Count == 0)
            {
                Console.WriteLine("Der er ingen spil i listen.");
            }
            else
            {
                Console.WriteLine("######hvad vil du sortere efter#######");
                Console.WriteLine("Vælg en mulighed: ");
                Console.WriteLine("1. Navn");
                Console.WriteLine("2. Pris");
                Console.WriteLine("3. Tilstand");
                Console.WriteLine("4. Antal spillere");
                Console.WriteLine("5. Genre");
                Console.WriteLine("6. Antal på lager");
                Console.Write("vælg en mulighed: ");
                string valg = Console.ReadLine();

                switch (valg)
                {
                    case "1":
                        games.Sort( // sorterings algoritme efter navn
                            (game1, game2) =>
                            {
                                int sort1 = game1.Name.CompareTo(game2.Name);
                                if (sort1 != 0) return sort1;
                                return game1.Stock.CompareTo(game2.Stock); // hvis de er ens, så sorteres efter navn
                            }
                            );
                        break;
                    case "2":
                        games.Sort( // sorterings algoritme efter pris
                            (game1, game2) =>
                            {
                                int sort1 = game1.Price.CompareTo(game2.Price);
                                if (sort1 != 0) return sort1;
                                return game1.Stock.CompareTo(game2.Stock); // hvis de er ens, så sorteres efter navn
                            }
                            );
                        break;
                    case "3":
                        games.Sort( // sorterings algoritme efter tilstand
                            (game1, game2) =>
                            {
                                int sort1 = game1.GameCondition.CompareTo(game2.GameCondition);
                                if (sort1 != 0) return sort1;
                                return game1.Stock.CompareTo(game2.Stock); // hvis de er ens, så sorteres efter navn
                            }
                            );
                        break;
                    case "4":
                        games.Sort( // sorterings algoritme efter antal spillere
                            (game1, game2) =>
                            {
                                int sort1 = game1.GroupSize.CompareTo(game2.GroupSize);
                                if (sort1 != 0) return sort1;
                                return game1.Stock.CompareTo(game2.Stock); // hvis de er ens, så sorteres efter navn
                            }
                            );
                        break;
                    case "5":
                        games.Sort( // sorterings algoritme efter genre
                        (game1, game2) =>
                        {
                            int sort1 = game1.Genre.CompareTo(game2.Genre);
                            if (sort1 != 0) return sort1;
                            return game1.Stock.CompareTo(game2.Stock); // hvis de er ens, så sorteres efter navn
                        }
                        );
                        break;
                    case "6":
                        games.Sort( // sorterings algoritme efter antal på lager
                        (game1, game2) =>
                        {
                            int sort1 = game1.Stock.CompareTo(game2.Stock);
                            if (sort1 != 0) return sort1; 
                            return game1.Name.CompareTo(game2.Name); // hvis de er ens, så sorteres efter navn
                        }
                        );
                        break;
                    default:
                        Console.WriteLine("fix");
                        break;
                }

                // find maksimmal længde på kolonner
                int nameWidth = Math.Min(Math.Max("Navn".Length, games.Max(g => g.Name.Length)), 40);
                int conditionWidth = Math.Max("Stand".Length, games.Max(g => g.GameCondition.ToString().Length));
                int priceWidth = Math.Max("Pris".Length, games.Max(g => g.Price.ToString("0.00" + " kr").Length));
                int groupSizeWidth = Math.Max("Spillere".Length, games.Max(g => g.GroupSize.ToString().Length));
                int genreWidth = Math.Min(Math.Max("Genre".Length, games.Max(g => g.Genre.Length)), 25);
                int stockWidth = Math.Max("Antal".Length, games.Max(g => g.Stock.ToString().Length));

                Console.WriteLine($"{Pad("Navn", nameWidth)} {Pad("Stand", conditionWidth)} {Pad("Pris", priceWidth)} {Pad("Spillere", groupSizeWidth)}      {Pad("Genre", genreWidth)} {Pad("Antal", stockWidth)}");

                //seperatorlinje
                int totalWidth = nameWidth + conditionWidth + priceWidth + groupSizeWidth + genreWidth + stockWidth + 13;
                Console.WriteLine(new string('-', totalWidth));

                foreach (Game game in games)
                {
                    Console.WriteLine($"{Pad(game.Name, nameWidth)} {Pad(game.GameCondition.ToString(), conditionWidth)} {Pad(game.Price.ToString("0.00") + " kr", nameWidth)} {Pad(game.GroupSize.ToString(), groupSizeWidth)} {Pad(game.Genre, genreWidth)} {Pad(game.Stock.ToString(), stockWidth)}");
                }

                Console.WriteLine(new string('-', totalWidth));

                int TotalUnikkeSpil = games.Count;
                int TotalStock = games.Sum(g => g.Stock);
                double TotalValue = games.Sum(g => g.Price * g.Stock);

                Console.WriteLine($"Total antal unikke spil: {TotalUnikkeSpil}");
                Console.WriteLine($"Total antal spil på lager: {TotalStock}");
                Console.WriteLine($"Total værdi af lager: {TotalValue:0.00} kr");
                Console.WriteLine("\n Tryk på en tast for at gå tilbage...");
                Console.ReadKey();
            }
        }
    }
}
