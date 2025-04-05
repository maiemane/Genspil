using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genspil
{
    class RequestInventory
    {
        private List<Request> requests = new List<Request>(); // Liste over spil i hukommelsen
        private static string FilePath = "requests.json"; // Placering af vores JSON-fil

        // Indlæs requests fra JSON-fil
        public RequestInventory() // Constructor
        {
            requests = LoadRequestsFromJson();
        }

        // Tilføj request til listen
        public void AddRequest(GameInventory inventory)
        {
            Console.WriteLine("Tilføj et nyt spil:");

            Console.Write("Navn på spil: ");
            string name = Console.ReadLine();

            Console.Write("Dit navn: ");
            string username = Console.ReadLine();

            // tjek om requesten er en positiv integer
            Console.Write("Pris: ");
            double maxPris;
            while (!double.TryParse(Console.ReadLine(), out maxPris) || maxPris < 0)
            {
                Console.Write("Ugyldig pris. Indtast en positiv værdi: ");
            }

            // tjekker om det requestede spil allerede er i lageret
            List<Game> goodGames = inventory.SearchGame(name, "", null, null, null, maxPris);

            if (goodGames.Count == 0)
            {
                // Opret ny request i request listen
                Request newRequest = new Request(name, maxPris, username);
                requests.Add(newRequest);

                // Gemmer spillet til JSON filen
                SaveRequestsToJson();

            }
            else
            {
                Console.WriteLine("Der findes allerede et spil");

            }

            Console.ReadKey();

        }

        // fjerner request fra listen
        public void RemoveRequest()
        {
            Console.WriteLine("Tilføj et nyt spil:");

            // spilnavn
            Console.Write("Navn på spil: ");
            string name = Console.ReadLine();

            // brugernavn
            Console.Write("Dit navn: ");
            string username = Console.ReadLine();
            
            // spørger efter navn og brugernavn af request der skal fjernes
            Request requestToRemove = requests.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && r.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (requestToRemove != null)
            {
                requests.Remove(requestToRemove);
                SaveRequestsToJson();
                Console.WriteLine($"Request for {requestToRemove.Name} removed.");
            }
            else
            {
                Console.WriteLine("Request not found.");
            } 
            Console.ReadLine();
        }

        // viser alle requests
        public void ListRequests()
        {
            if (requests.Count == 0)
            {
                Console.WriteLine("No requests found.");
            }
            else
            {
                Console.WriteLine("##### Requests #####");
                foreach (var request in requests)
                {
                    Console.WriteLine($"Name: {request.Name}, Max Price: {request.MaxPrice}");
                }
            } Console.ReadLine();
        }

        // metode til at gemme requests til JSON fil
        private void SaveRequestsToJson()
        {
            string json = JsonConvert.SerializeObject(requests, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        // metode til at indlæse requests fra JSON fil
        public List<Request> LoadRequestsFromJson()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    string json = File.ReadAllText(FilePath);
                    return JsonConvert.DeserializeObject<List<Request>>(json) ?? new List<Request>();
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
            return new List<Request>();
        }
    }
}

