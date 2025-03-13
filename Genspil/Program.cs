namespace Genspil
{
    internal class Program
    {

        static void Main(string[] args)
        {
            //Inventory liste
            GameInventory inventory = new GameInventory();

            //Tilføjelse af spil til listen
            inventory.AddGame(new Game("Sequence", Game.Condition.God, 150, 1));
            inventory.AddGame(new Game("Ticket to ride", Game.Condition.God, 150, 3));
            inventory.AddGame(new Game("7 Wonders", Game.Condition.OK, 100, 2));

            //Udskriv alle spillene i listen
            Console.WriteLine("Spil i listen: ");
            inventory.ListGames();
            Console.ReadLine();
        }

    }
    
    public class Game
    {
        //Properties
        public string Name { get; set; }
        public Condition GameCondition { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }

        //Constructor
        public Game(string name, Condition condition, double price, int stock)
        {
            Name = name;
            GameCondition = condition;
            Price = price;
            Stock = stock;
        }

        //Enum for stand
        public enum Condition
        {
            God,
            OK,
            Slidt
        }
    }

    public class GameInventory
    {
        List<Game> games = new List<Game>();

        //Tilføj spil til listen
        public void AddGame(Game game)
        {
            games.Add(game);
            Console.WriteLine($"Spillet {game.Name} er tilføjet til listen.");
        }

        //Fjern spil fra listen
        public void RemoveGame(Game game)
        {
            games.Remove(game);
            Console.WriteLine($"Spillet {game.Name} er fjernet fra listen.");
        }

        //Overblik over spil i listen
        public void ListGames()
        {
            if(games.Count == 0)
            { 
             Console.WriteLine("Der er ingen spil i listen.");
            }
            else
            {
                foreach(Game game in games)
                {
                    Console.WriteLine($"Spil: {game.Name}, Stand: {game.GameCondition}, Pris: {game.Price}, Antal på lager: {game.Stock}");
                }
            }
        }
    }
}
