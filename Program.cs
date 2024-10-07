using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Lolcat;

namespace StealerFind
{
    class Program
    {
        static readonly string ASCII = @"
  ██████ ▄▄▄█████▓▓█████ ▄▄▄       ██▓    ▓█████  ██▀███    █████▒██▓ ███▄    █ ▓█████▄ 
▒██    ▒ ▓  ██▒ ▓▒▓█   ▀▒████▄    ▓██▒    ▓█   ▀ ▓██ ▒ ██▒▓██   ▒▓██▒ ██ ▀█   █ ▒██▀ ██▌
░ ▓██▄   ▒ ▓██░ ▒░▒███  ▒██  ▀█▄  ▒██░    ▒███   ▓██ ░▄█ ▒▒████ ░▒██▒▓██  ▀█ ██▒░██   █▌
  ▒   ██▒░ ▓██▓ ░ ▒▓█  ▄░██▄▄▄▄██ ▒██░    ▒▓█  ▄ ▒██▀▀█▄  ░▓█▒  ░░██░▓██▒  ▐▌██▒░▓█▄   ▌
▒██████▒▒  ▒██▒ ░ ░▒████▒▓█   ▓██▒░██████▒░▒████▒░██▓ ▒██▒░▒█░   ░██░▒██░   ▓██░░▒████▓ 
▒ ▒▓▒ ▒ ░  ▒ ░░   ░░ ▒░ ░▒▒   ▓▒█░░ ▒░▓  ░░░ ▒░ ░░ ▒▓ ░▒▓░ ▒ ░   ░▓  ░ ▒░   ▒ ▒  ▒▒▓  ▒ 
░ ░▒  ░ ░    ░     ░ ░  ░ ▒   ▒▒ ░░ ░ ▒  ░ ░ ░  ░  ░▒ ░ ▒░ ░      ▒ ░░ ░░   ░ ▒░ ░ ▒  ▒ 
░  ░  ░    ░         ░    ░   ▒     ░ ░      ░     ░░   ░  ░ ░    ▒ ░   ░   ░ ░  ░ ░  ░ 
      ░              ░  ░     ░  ░    ░  ░   ░  ░   ░             ░           ░    ░    
By @ivyfrost";
        static readonly string APIURL = "https://cavalier.hudsonrock.com/api/json/v2/osint-tools/search-by-email?email=";

        static async Task Main(string[] args)
        {
            var style = new RainbowStyle();
            var rainbow = new Rainbow(style);
            rainbow.WriteLineWithMarkup(ASCII);

            if (args.Length < 1)
            {
                Console.WriteLine($"use: ./stealerfind email");
                return;
            }

            var targetEmail = args[0].Trim();
            if (string.IsNullOrEmpty(targetEmail))
            {
                Console.WriteLine("Error: Email cannot be empty.");
                return;
            }

            if (!IsValidEmail(targetEmail))
            {
                Console.WriteLine("Error: Invalid email format.");
                return;
            }

            rainbow.WriteLineWithMarkup($"Searching: {targetEmail}");
            var results = await SearchMail(targetEmail);
            foreach (var result in results)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"IP: {result.ip}");
                Console.WriteLine($"OS: {result.operating_system}");
                Console.WriteLine($"Antiviruses: {result.antiviruses}");
                Console.WriteLine($"Date Compromised: {result.date_compromised}");
                Console.WriteLine($"Top Logins: {string.Join(", ", result.top_logins)}");
                Console.WriteLine($"Top Passwords: {string.Join(", ", result.top_passwords)}");
                Console.WriteLine($"Malware Path: {result.malware_path}");
                Console.WriteLine($"Total User Services: {result.total_user_services}");
                Console.WriteLine($"Total Corporate Services: {result.total_corporate_services}");
                Console.ResetColor();
                Console.WriteLine(new string('-', 40));
            }
        }

        static async Task<List<Stealer>> SearchMail(string email)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"{APIURL}{email}");
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var root = JsonSerializer.Deserialize<Root>(jsonResponse);
                    return root?.stealers;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        static bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }
    }

    public class Root
    {
        public string message { get; set; }
        public List<Stealer> stealers { get; set; }
        public int total_corporate_services { get; set; }
        public int total_user_services { get; set; }
    }

    public class Stealer
    {
        public int total_corporate_services { get; set; }
        public int total_user_services { get; set; }
        public DateTime date_compromised { get; set; }
        public string computer_name { get; set; }
        public string operating_system { get; set; }
        public string malware_path { get; set; }
        public object antiviruses { get; set; }
        public string ip { get; set; }
        public List<string> top_passwords { get; set; }
        public List<string> top_logins { get; set; }
    }

}