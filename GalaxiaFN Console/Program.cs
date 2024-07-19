using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaxiaFN_Console.Properties;
using IniParser;
using IniParser.Model;
using static GalaxiaFN_Console.ProcessExtension;

namespace GalaxiaFN_Console
{
    internal class Program
    {
        static Process FortniteClient = new Process();
        static Process FortniteEAC = new Process();
        static Process FortniteLauncher = new Process();
        static Dictionary<string, (string Path, string Username, string Password)> seasons = new Dictionary<string, (string, string, string)>();
        static string selectedLanguage = "fr"; // Valeur par défaut en français

        static void Main(string[] args)
        {
            ShowLanguageMenu();
            Console.Title = "GalaxiaFN Console Launcher";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(selectedLanguage == "en" ? "Waiting for the backend to be connected to port3551..." : "Attente de la connexion au backend sur le port 3551...");
            Console.WriteLine(selectedLanguage == "en" ? "If your backend is not listening on port 3551, please change it (for most of them, in index.js)" : "Si votre backend n'écoute pas sur le port 3551, veuillez le modifier (la plupart dans index.js)");

            while (!IsPortOpen("localhost", 3551))
            {
                Thread.Sleep(1000); // Attendre 1 seconde avant de réessayer
            }
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(selectedLanguage == "en" ? "Backend successfully connected! Please wait 5 seconds!" : "Connexion au backend réussie! Veuillez patienter 5 seconde");
            Thread.Sleep(5000);

            LoadSeasons();

            Console.Clear();

            while (true)
            {
                ShowMainMenu();
                string output = Console.ReadLine();

                if (output == "0")
                {
                    AddSeason();
                }
                else if (output == "1")
                {
                    LaunchSeason();
                }
                else if (output == "2")
                {
                    RemoveSeason();
                }
                else
                {
                    Console.WriteLine(selectedLanguage == "en" ? "Please answer only with 0, 1 or 2" : "Veuillez répondre uniquement par 0, 1 ou 2");
                    Thread.Sleep(2000);
                    Console.Clear();
                }
            }
        }

        static bool IsPortOpen(string host, int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    if (!success)
                    {
                        return false;
                    }

                    client.EndConnect(result);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        static void ShowLanguageMenu()
        {
            Console.Clear();
            Console.WriteLine("Select Language with a valid / Sélectionnez la langue avec un nombre valide:");
            Console.WriteLine("[1] English");
            Console.WriteLine("[2] Français");
            Console.Write(">> ");
            string languageChoice = Console.ReadLine();

            if (languageChoice == "1")
            {
                selectedLanguage = "en";
            }
            else if (languageChoice == "2")
            {
                selectedLanguage = "fr";
            }
            else
            {
                Console.WriteLine("Invalid choice. Defaulting to French / Choix invalide. Par défaut en français.");
            }
        }

        static void ShowMainMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(selectedLanguage == "en" ? "[+] Welcome to GalaxiaFN Console, a programm to launch old fortnite versions and play it. \n This program was made by Waslyl \n link: \n discord: https://discord.gg/galaxiafn \n github: https://github.com/waslyl" : "[+] Bienvenue sur GalaxiaFN Console, un programme pour lancer des anciennes Saisons de Fortnite et pour jouer dessus.\n Ce programme à été programmé par Waslyl \n link: \n discord: https://discord.gg/galaxiafn \n github: https://github.com/waslyl");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(selectedLanguage == "en" ? "[?] What do you want to do?\n" : "[?] Que voulez vous faire?\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(selectedLanguage == "en" ? "[0] Add a Fortnite version\n" : "[0] Ajouter une version de Fortnite\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(selectedLanguage == "en" ? "[1] Start a Fortnite version\n" : "[1] Lancer une version de Fortnite\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(selectedLanguage == "en" ? "[2] Delete a Fortnite version\n" : "[2] Supprimer une version de Fortnite\n");

            Console.Write(">> ");
        }

        static void AddSeason()
        {
            Console.Clear();
            Console.WriteLine(selectedLanguage == "en" ? "Please put the path of the version here:\n" : "Veuillez mettre le chemin de votre version ici:\n");
            Console.Write(">> ");
            string path = Console.ReadLine();

            Console.WriteLine(selectedLanguage == "en" ? "Please give a name for this season:\n" : "Veuillez donner un nom pour cette saison:\n");
            Console.Write(">> ");
            string name = Console.ReadLine();

            Console.WriteLine(selectedLanguage == "en" ? "Which email (name if you're on lawin v1) do you want to put?\n" : "Quel email (pseudo si vous êtes sur lawin v1) voulez-vous mettre?\n");
            Console.Write(">> ");
            string email = Console.ReadLine();

            Console.WriteLine(selectedLanguage == "en" ? "Which password do you want to put?\n" : "Quel mot de pass veut tu ajouter\n");
            Console.Write(">> ");
            string password = Console.ReadLine();

            seasons[name] = (path, email, password);
            SaveSeasons();

            Console.WriteLine(selectedLanguage == "en" ? "The path, email/pseudo and password has been saved correctly!\n" : "Votre chemin, pseudo et mot de passe ont bien été enregistrés correctement!\n");
            Console.Clear();
        }

        static void LaunchSeason()
        {
            if (seasons.Count == 0)
            {
                Console.WriteLine(selectedLanguage == "en" ? "No seasons has been saved.\n" : "Aucune saison n'a été ajoutée.\n");
                return;
            }

            Console.Clear();

            Console.WriteLine(selectedLanguage == "en" ? "Available seasons:\n" : "Saisons disponibles:\n");
            foreach (var season in seasons.Keys)
            {
                Console.WriteLine($"- {season}");
            }

            Console.WriteLine(selectedLanguage == "en" ? "Please select the season that you want to play wit it's name:\n" : "Veuillez sélectionner la saison que vous voulez lancer par son nom:\n");
            Console.Write(">> ");
            string selectedSeason = Console.ReadLine();

            if (seasons.ContainsKey(selectedSeason))
            {
                var season = seasons[selectedSeason];
                Console.WriteLine(selectedLanguage == "en" ? $"Launching the season: {selectedSeason} with path: {season.Path}\n" : $"Lancement de la saison: {selectedSeason} avec le chemin: {season.Path}\n");

                Console.WriteLine(selectedLanguage == "en" ? "How many games do you want to start?\n" : "Combien de jeux souhaitez-vous lancer?\n");
                Console.Write(">> ");
                if (!int.TryParse(Console.ReadLine(), out int numberOfGames) || numberOfGames <= 0)
                {
                    Console.WriteLine(selectedLanguage == "en" ? "Please enter a valid number\n" : "Veuillez entrer un nombre valide.\n");
                    return;
                }

                for (int i = 0; i < numberOfGames; i++)
                {
                    LaunchGameInstance(season);
                }
            }
            else
            {
                Console.WriteLine(selectedLanguage == "en" ? "Season not found.\n" : "Saison non trouvée.\n");
            }
            Console.Clear();
            ShowMainMenu();
        }

        static void LaunchGameInstance((string Path, string Username, string Password) season)
        {
            try
            {
                string launcherPath = Path.Combine(season.Path, "FortniteGame", "Binaries", "Win64", "FortniteLauncher.exe");
                if (!File.Exists(launcherPath))
                {
                    Console.WriteLine($"Le fichier {launcherPath} est introuvable.");
                    return;
                }

                var FortniteLauncher = new Process();
                FortniteLauncher.StartInfo.FileName = launcherPath;
                FortniteLauncher.Start();
                ProcessExtension.Suspend(FortniteLauncher); //launch and suspend because after the game crash

                string eacPath = Path.Combine(season.Path, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping_EAC.exe");
                if (!File.Exists(eacPath))
                {
                    Console.WriteLine($"Le fichier {eacPath} est introuvable.");
                    return;
                }

                var FortniteEAC = new Process();
                FortniteEAC.StartInfo.FileName = eacPath;
                FortniteEAC.Start();
                ProcessExtension.Suspend(FortniteEAC); //same

                string clientPath = Path.Combine(season.Path, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping.exe");
                if (!File.Exists(clientPath))
                {
                    Console.WriteLine($"Le fichier {clientPath} est introuvable.");
                    return;
                }

                var Fortnite = new Process();
                Fortnite.StartInfo.FileName = clientPath;
                Fortnite.StartInfo.UseShellExecute = false;
                Fortnite.StartInfo.RedirectStandardOutput = true;
                Fortnite.StartInfo.RedirectStandardError = true;
                Fortnite.StartInfo.Arguments = $@"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={season.Username} -AUTH_PASSWORD={season.Password} -AUTH_TYPE=epic";

                Fortnite.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null && e.Data.Contains("Region "))
                    {
                        Thread.Sleep(5000);
                        string consoleDllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Console.dll");
                        if (File.Exists(consoleDllPath))
                        {
                            Console.WriteLine($"Injection de {consoleDllPath}.");
                            ProcessHelper.InjectDll(((Process)sender).Id, consoleDllPath);
                        }
                        else
                        {
                            Console.WriteLine($"Le fichier {consoleDllPath} est introuvable.");
                        }
                    }
                };
                Fortnite.ErrorDataReceived += (sender, e) =>
                {
                    Console.WriteLine($"Erreur: {e.Data}");
                };

                Fortnite.Start();
                Fortnite.BeginOutputReadLine();
                Fortnite.BeginErrorReadLine();

                string fortniteX64DllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Redirect.dll");
                if (File.Exists(fortniteX64DllPath))
                {
                    Console.WriteLine($"Injection de {fortniteX64DllPath}.");
                    ProcessHelper.InjectDll(Fortnite.Id, fortniteX64DllPath);
                }
                else
                {
                    Console.WriteLine($"Le fichier {fortniteX64DllPath} est introuvable.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une exception s'est produite: {ex.Message}");
            }
        }

        static void RemoveSeason()
        {
            if (seasons.Count == 0)
            {
                Console.WriteLine(selectedLanguage == "en" ? "No season have been added\n" : "Aucune saison n'a été ajoutée.\n");
                return;
            }
            Console.Clear();
            Console.WriteLine(selectedLanguage == "en" ? "Available season:\n" : "Saisons disponibles:\n");
            foreach (var season in seasons.Keys)
            {
                Console.WriteLine($"- {season}");
            }

            Console.WriteLine("Veuillez sélectionner la saison que vous voulez supprimer par son nom:");
            Console.WriteLine(selectedLanguage == "en" ? "Please select the season that you want to delete with his name:\n" : "Veuillez sélectionner la saison que vous voulez supprimer par son nom:\n");
            Console.Write(">> ");
            string selectedSeason = Console.ReadLine();

            if (seasons.ContainsKey(selectedSeason))
            {
                seasons.Remove(selectedSeason);
                SaveSeasons();
                Console.WriteLine(selectedLanguage == "en" ? $"The season {selectedSeason} have been successfully delete.\n" : $"La saison {selectedSeason} a été supprimée avec succès.\n");
            }
            else
            {
                Console.WriteLine(selectedLanguage == "en" ? $"The season {selectedSeason} have been successfully delete.\n" : $"La saison {selectedSeason} a été supprimée avec succès.\n");
            }
            Console.Clear();
        }

        static void LoadSeasons()
        {
            if (File.Exists("seasons.ini"))
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("seasons.ini");

                foreach (var section in data.Sections)
                {
                    string name = section.SectionName;
                    string path = section.Keys["Path"];
                    string email = section.Keys["Email"];
                    string password = section.Keys["Password"];

                    seasons[name] = (path, email, password);
                }
            }
        }

        static void SaveSeasons()
        {
            var parser = new FileIniDataParser();
            var data = new IniData();

            foreach (var season in seasons)
            {
                data[season.Key]["Path"] = season.Value.Path;
                data[season.Key]["Email"] = season.Value.Username;
                data[season.Key]["Password"] = season.Value.Password;
            }

            parser.WriteFile("seasons.ini", data);
        }
    }
}