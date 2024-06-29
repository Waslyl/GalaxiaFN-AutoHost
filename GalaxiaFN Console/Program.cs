using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        static void Main(string[] args)
        {
            Console.Title = "GalaxiaFN Console Launcher";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Attente de la connexion de l'outil backend sur le port 3551...");

            while (!IsPortOpen("localhost", 3551))
            {
                Thread.Sleep(1000); // Attendre 1 seconde avant de réessayer
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Connexion de l'outil backend réussie!");

            LoadSeasons();

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
                else
                {
                    Console.WriteLine("Veuillez répondre uniquement par 0 ou 1");
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

        static void ShowMainMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] Bienvenue sur GalaxiaFN Console, un programme pour lancer des anciennes Saisons de Fortnite et pour jouer dessus.\n Ce programme à été programmé par Waslyl\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[?] Que voulez vous faire?\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[0] Ajouter une version");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[1] Lancer une version");

            Console.Write(">> ");
        }

        static void AddSeason()
        {
            Console.WriteLine("Veuillez mettre le chemin de votre jeu ici:");
            Console.Write(">> ");
            string path = Console.ReadLine();

            Console.WriteLine("Veuillez donner un nom pour cette saison:");
            Console.Write(">> ");
            string name = Console.ReadLine();

            Console.WriteLine("Quel email/pseudo (si vous êtes sur lawin v1) voulez-vous mettre?");
            Console.Write(">> ");
            string email = Console.ReadLine();

            Console.WriteLine("Quel est votre mot de passe?");
            Console.Write(">> ");
            string password = Console.ReadLine();

            seasons[name] = (path, email, password);
            SaveSeasons();

            Console.WriteLine("Votre chemin, pseudo et mot de passe ont bien été enregistrés.");
        }

        static void LaunchSeason()
        {
            if (seasons.Count == 0)
            {
                Console.WriteLine("Aucune saison n'a été ajoutée.");
                return;
            }

            Console.WriteLine("Saisons disponibles:");
            foreach (var season in seasons.Keys)
            {
                Console.WriteLine($"- {season}");
            }

            Console.WriteLine("Veuillez sélectionner la saison que vous voulez lancer par son nom:");
            Console.Write(">> ");
            string selectedSeason = Console.ReadLine();

            if (seasons.ContainsKey(selectedSeason))
            {
                var season = seasons[selectedSeason];
                Console.WriteLine($"Lancement de la saison: {selectedSeason} avec le chemin: {season.Path}");

                Console.WriteLine("Combien de jeux souhaitez-vous lancer?");
                Console.Write(">> ");
                if (!int.TryParse(Console.ReadLine(), out int numberOfGames) || numberOfGames <= 0)
                {
                    Console.WriteLine("Veuillez entrer un nombre valide.");
                    return;
                }

                for (int i = 0; i < numberOfGames; i++)
                {
                    LaunchGameInstance(season);
                }
            }
            else
            {
                Console.WriteLine("Saison non trouvée.");
            }
        }

        static void LaunchGameInstance((string Path, string Username, string Password) season)
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
                ProcessHelper.InjectDll(Fortnite.Id, fortniteX64DllPath);
            }
            else
            {
                Console.WriteLine($"Le fichier {fortniteX64DllPath} est introuvable.");
            }
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