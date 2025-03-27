using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genspil
{
    public class GameSort
    {
        private List<Game> games;

        public GameSort(List<Game> gameList)
        {
            games = gameList;
        }

        private string Pad(string input, int width)
        {
            if (input.Length > width)
                return input.Substring(0, width - 3) + "...";
            return input.PadRight(width);
        }
        public void ListGames()
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
                        games.Sort(
                            (game1, game2) =>
                            {
                                int sort1 = game1.Name.CompareTo(game2.Name);
                                if (sort1 != 0) return sort1;
                                return game1.Stock.CompareTo(game2.Stock);
                            }
                            );
                        break;
                    case "2":
                        games.Sort(
                            (game1, game2) =>
                            {
                                int sort1 = game1.Price.CompareTo(game2.Price);
                                if (sort1 != 0) return sort1;
                                return game1.Stock.CompareTo(game2.Stock);
                            }
                            );
                        break;
                    case "3":
                        games.Sort(
                            (game1, game2) =>
                            {
                                int sort1 = game1.GameCondition.CompareTo(game2.GameCondition);
                                if (sort1 != 0) return sort1;
                                return game1.Stock.CompareTo(game2.Stock);
                            }
                            );
                        break;
                    case "4":
                        games.Sort(
                            (game1, game2) =>
                            {
                                int sort1 = game1.GroupSize.CompareTo(game2.GroupSize);
                                if (sort1 != 0) return sort1;
                                return game1.Stock.CompareTo(game2.Stock);
                            }
                            );
                        break;
                    case "5":
                        games.Sort(
                        (game1, game2) =>
                        {
                            int sort1 = game1.Genre.CompareTo(game2.Genre);
                            if (sort1 != 0) return sort1;
                            return game1.Stock.CompareTo(game2.Stock);
                        }
                        );
                        break;
                    case "6":
                        games.Sort(
                        (game1, game2) =>
                        {
                            int sort1 = game1.Stock.CompareTo(game2.Stock);
                            if (sort1 != 0) return sort1;
                            return game1.Name.CompareTo(game2.Name);
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
