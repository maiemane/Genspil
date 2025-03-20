using System;
using System.Collections.Generic;
using Newtonsoft.Json; //tilføj json.net via NuGet

namespace Genspil
{
    public class  GameSearch
    {
        private GameInventory inventory;
        public GameSearch(GameInventory gameInventory)
        {

            inventory = gameInventory;
        }

        //Definerer en metode til at søge i listen af spillene ud fra kriterer
        public List<Game> SearchGame(string name = "", string genre = "", int? MinPlayers = null, int? MaxPlayers = null, double? MinPrice = null, double? MaxPrice = null, Game.Condition? condition = null)
        {
            var games = inventory.GetGames();
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

        public void SearchAndDisplay()
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
    }
}

