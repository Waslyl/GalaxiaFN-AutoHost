using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using GalaxiaFN_AutoRestart.ProcessKill;

namespace GalaxiaFN_AutoRestart
{
    internal class Program
    {
        static Process FortniteEAC = new Process();
        static Process FortniteLauncher = new Process();
        static Dictionary<string, (string Path, string Username, string Password)> seasons = new Dictionary<string, (string, string, string)>();
        static string currentLanguage = "en";

        //Dictionary will be used to add different language support, more languages will be added in the future if I have time :)
        static Dictionary<string, Dictionary<string, string>> messages = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "en", new Dictionary<string, string>
                {
                    { "Welcome", "[+] Welcome to GalaxiaFN Console, a program for launching older Fortnite seasons and playing on them." },
                    { "AddVersion", "[0] Add a Fortnite version" },
                    { "LaunchVersion", "[1] Launch a Fortnite version" },
                    { "RemoveVersion", "[2] Remove a Fortnite version" },
                    { "SelectAction", "Please respond only with 0, 1, or 2" },
                    { "AddPath", "Please enter the path to your version:" },
                    { "AddName", "Please give a name for this season:" },
                    { "AddEmail", "What email would you like to use?" },
                    { "AddPassword", "What password would you like to add?" },
                    { "SaveConfirmation", "Your path, username, and password have been saved successfully!" },
                    { "NoSeasons", "No seasons have been added." },
                    { "AvailableSeasons", "Available seasons:" },
                    { "SelectSeason", "Please select the season you want to launch by name:" },
                    { "Launching", "Launching season:" },
                    { "FileNotFound", "The file is not found:" },
                    { "GameFinished", "The game has ended. Restarting..." },
                    { "RemoveSeasonPrompt", "Please select the season you want to remove by name:" },
                    { "RemoveSuccess", "The season has been successfully removed." },
                    { "SeasonNotFound", "Season not found." },
                    { "InjectionFailed", "Injection failed:" },
                    { "LanguageSelection", "Please select your language / Veuillez sélectionner votre langue:" },
                    { "English", "[1] English" },
                    { "French", "[2] Français" },
                    { "DllMoveSuccess", "DLL successfully moved and renamed to the target path." },
                    { "DllMoveFailed", "Failed to move and rename the DLL to the target path." },
                    { "RedirectDllInjection", "The SSL have been injected into the game." },
                    { "RedirectDllInjectionFailed", "The SSL is missing." },
                    { "GameserverDllInjection", "The Gameserver have been injected into the game." },
                    { "GameserverDllInjectionFailed", "The Gameserver is missing." },
                    { "Restart", "The game is finished, restarting..." }
                }
            },
            {
                "fr", new Dictionary<string, string>
                {
                    { "Welcome", "[+] Bienvenue sur GalaxiaFN Console, un programme pour lancer des anciennes saisons de Fortnite et pour jouer dessus." },
                    { "AddVersion", "[0] Ajouter une version de Fortnite" },
                    { "LaunchVersion", "[1] Lancer une version de Fortnite" },
                    { "RemoveVersion", "[2] Supprimer une version de Fortnite" },
                    { "SelectAction", "Veuillez répondre uniquement par 0, 1 ou 2" },
                    { "AddPath", "Veuillez mettre le chemin de votre version ici:" },
                    { "AddName", "Veuillez donner un nom pour cette saison:" },
                    { "AddEmail", "Quel email voulez-vous mettre?" },
                    { "AddPassword", "Quel mot de passe voulez-vous ajouter?" },
                    { "SaveConfirmation", "Votre chemin, pseudo et mot de passe ont bien été enregistrés correctement!" },
                    { "NoSeasons", "Aucune saison n'a été ajoutée." },
                    { "AvailableSeasons", "Saisons disponibles:" },
                    { "SelectSeason", "Veuillez sélectionner la saison que vous voulez lancer par son nom:" },
                    { "Launching", "Lancement de la saison:" },
                    { "FileNotFound", "Le fichier est introuvable:" },
                    { "GameFinished", "La game est finie. Redémarrage..." },
                    { "RemoveSeasonPrompt", "Veuillez sélectionner la saison que vous voulez supprimer par son nom:" },
                    { "RemoveSuccess", "La saison a été supprimée avec succès." },
                    { "SeasonNotFound", "Saison non trouvée." },
                    { "InjectionFailed", "Injection échouée:" },
                    { "LanguageSelection", "Please select your language / Veuillez sélectionner votre langue:" },
                    { "English", "[1] English" },
                    { "French", "[2] Français" },
                    { "DllMoveSuccess", "DLL déplacé et renommé avec succès vers le chemin cible." },
                    { "DllMoveFailed", "Le déplacement et le renommage du DLL vers le chemin cible ont échoué." },
                    { "RedirectDllInjection", "Le SSL a ete injecte au jeu." },
                    { "RedirectDllInjectionFailed", "Le SSL est manquant." },
                    { "GameserverDllInjection", "Le Gameserveur a ete injecte au jeu." },
                    { "GameserverDllInjectionFailed", "Le Gameserveur est manquant." },
                    { "Restart", "La partie est termine, restart..." }
                }
            }
        };

        public static string GetMessage(string key)
        {
            return messages[currentLanguage].ContainsKey(key) ? messages[currentLanguage][key] : key;
        }

        static void Main(string[] args)
        {
            Console.Title = "GalaxiaFN Console Launcher";
            SelectLanguage();
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
                else if (output == "2")
                {
                    RemoveSeason();
                }
                else
                {
                    //Choisir l'action à faire
                    //Choose which action to do
                    Console.WriteLine(GetMessage("SelectAction"));
                    Thread.Sleep(2000);
                    Console.Clear();
                }
            }
        }

        static void SelectLanguage()
        {
            Console.WriteLine(GetMessage("LanguageSelection"));
            Console.WriteLine(GetMessage("English"));
            Console.WriteLine(GetMessage("French"));
            Console.Write(">> ");
            string langChoice = Console.ReadLine();

            if (langChoice == "2")
            {
                currentLanguage = "fr";
            }
            else
            {
                currentLanguage = "en";
            }

            Console.Clear();
        }

        static void ShowMainMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(GetMessage("Welcome"));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetMessage("AddVersion"));
            Console.WriteLine(GetMessage("LaunchVersion"));
            Console.WriteLine(GetMessage("RemoveVersion"));
            Console.Write(">> ");
        }

        static void AddSeason()
        {
            Console.Clear();
            Console.WriteLine(GetMessage("AddPath"));
            Console.Write(">> ");
            string path = Console.ReadLine();

            Console.WriteLine(GetMessage("AddName"));
            Console.Write(">> ");
            string name = Console.ReadLine();

            Console.WriteLine(GetMessage("AddEmail"));
            Console.Write(">> ");
            string email = Console.ReadLine();

            Console.WriteLine(GetMessage("AddPassword"));
            Console.Write(">> ");
            string password = Console.ReadLine();

            seasons[name] = (path, email, password);
            SaveSeasons();

            Console.WriteLine(GetMessage("SaveConfirmation"));
            Console.Clear();
        }

        static void LaunchSeason()
        {
            if (seasons.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(GetMessage("NoSeasons"));
                Thread.Sleep(2000);
                Console.Clear();
                return;
            }

            Console.Clear();
            Console.WriteLine(GetMessage("AvailableSeasons"));
            foreach (var season in seasons.Keys)
            {
                Console.WriteLine($"- {season}");
            }

            Console.WriteLine(GetMessage("SelectSeason"));
            Console.Write(">> ");
            string selectedSeason = Console.ReadLine();

            if (seasons.ContainsKey(selectedSeason))
            {
                Console.Clear();
                var season = seasons[selectedSeason];
                Console.WriteLine($"{GetMessage("Launching")} {selectedSeason}");

                while (true)
                {
                    LaunchGameInstance(season);
                    Thread.Sleep(2000); // Wait 2 seconds before restarting
                }
            }
            else
            {
                Console.WriteLine(GetMessage("SeasonNotFound"));
            }

            Console.Clear();
            ShowMainMenu();
        }

        static void LaunchGameInstance((string Path, string Username, string Password) season)
        {
            try
            {

                string dllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!File.Exists(Path.Combine(dllDirectory, "redirect.dll")))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(GetMessage("RedirectDllInjectionFailed"));
                    return;
                }

                if (!File.Exists(Path.Combine(dllDirectory, "gameserver.dll")))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(GetMessage("GameserverDllInjectionFailed"));
                    return;
                }

                string launcherPath = Path.Combine(season.Path, "FortniteGame", "Binaries", "Win64", "FortniteLauncher.exe");
                if (!File.Exists(launcherPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Le fichier {launcherPath} est introuvable.");
                    return;
                }


                FortniteLauncher.StartInfo.FileName = launcherPath;
                FortniteLauncher.Start();
                ProcessExtension.Suspend(FortniteLauncher); // Will suspend it to make it work

                string eacPath = Path.Combine(season.Path, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping_EAC.exe");
                if (!File.Exists(eacPath))
                {
                    Console.WriteLine($"Le fichier {eacPath} est introuvable.");
                    return;
                }


                FortniteEAC.StartInfo.FileName = eacPath;
                FortniteEAC.Start();
                ProcessExtension.Suspend(FortniteEAC); // Will suspend it to make it work

                Process Fortnite = new Process();
                Fortnite.StartInfo.FileName = Path.Combine(season.Path, "FortniteGame/Binaries/Win64/FortniteClient-Win64-Shipping.exe");
                Fortnite.StartInfo.RedirectStandardOutput = true;
                Fortnite.StartInfo.RedirectStandardError = true;
                Fortnite.StartInfo.UseShellExecute = false;
                Fortnite.StartInfo.CreateNoWindow = true;
                Fortnite.StartInfo.Verb = "runas";
                Fortnite.StartInfo.Arguments = $@"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -log -nosplash -nosound -nullrhi -useolditemcards -fltoken=3db3ba5dcbd2e16703f3978d -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={season.Username} -AUTH_PASSWORD={season.Password} -AUTH_TYPE=epic";
                //Fortnite.StartInfo.Arguments = $@"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={season.Username} -AUTH_PASSWORD={season.Password} -AUTH_TYPE=epic";
                // if you want to enable the Game UI, delete -log -nosplash -nosound -nullrhi -useolditemcards from Fortnite.StartInfo.Arguments :)
                

                bool injectedGameserver = false;
                var injectionTask = new TaskCompletionSource<bool>();

                Fortnite.OutputDataReceived += async (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        if (e.Data.Contains("Region ") && !injectedGameserver)
                        {
                            injectedGameserver = true;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Output Region defined");

                            // Attendre 10 secondes avant l'injection
                            await Task.Delay(10000);

                            await Task.Run(() =>
                            {
                                try
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Win32.InjectDll(Fortnite.Id, Path.Combine(dllDirectory, "gameserver.dll"));
                                    Console.WriteLine(GetMessage("GameserverDllInjection"));
                                    injectionTask.SetResult(true);
                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"Erreur lors de l'injection du gameserver: {ex.Message}");
                                    injectionTask.SetResult(false);
                                }
                            });
                        }

                        string[] LoginFailureErrs = { "port 3551 failed: Connection refused", "Unable to login to Fortnite servers", "HTTP 400 response from ", "Network failure when attempting to check platform restrictions", "UOnlineAccountCommon::ForceLogout" };
                        if (LoginFailureErrs.Any(err => e.Data.Contains(err)))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"A token error occured, restarting server, Error: {e.Data}");
                            await Task.Delay(5000);
                            ProcessKiller.KillProcessByName(Path.GetFileNameWithoutExtension(Fortnite.StartInfo.FileName));
                        }
                    }
                };

                Fortnite.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error: {e.Data}");
                    }
                };

                Fortnite.Start();
                Fortnite.BeginOutputReadLine();
                Fortnite.BeginErrorReadLine();

                try
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Win32.InjectDll(Fortnite.Id, Path.Combine(dllDirectory, "redirect.dll"));
                    Console.WriteLine(GetMessage("RedirectDllInjection"));
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Erreur lors de l'injection: {ex.Message}");
                }

                Fortnite.WaitForExit(); // The game will restart if the client is kill, don't forget to add a function in your gs to make the process kill when your game is finished (:

                ProcessKiller.KillProcessByName("EpicGamesLauncher");
                ProcessKiller.KillProcessByName("EpicWebHelper");
                ProcessKiller.KillProcessByName("CrashReportClient");
                ProcessKiller.KillProcessByName("FortniteLauncher");
                ProcessKiller.KillProcessByName("FortniteClient-Win64-Shipping");
                ProcessKiller.KillProcessByName("EasyAntiCheat_EOS");
                ProcessKiller.KillProcessByName("FortniteClient-Win64-Shipping_EAC");

                Console.WriteLine(GetMessage("Restart"));

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occured: {ex.Message}");
            }
        }

        static void RemoveSeason()
        {
            if (seasons.Count == 0)
            {
                Console.WriteLine(GetMessage("NoSeasons"));
                return;
            }

            Console.Clear();
            Console.WriteLine(GetMessage("AvailableSeasons"));
            foreach (var season in seasons.Keys)
            {
                Console.WriteLine($"- {season}");
            }

            Console.WriteLine(GetMessage("RemoveSeasonPrompt"));
            Console.Write(">> ");
            string selectedSeason = Console.ReadLine();

            if (seasons.ContainsKey(selectedSeason))
            {
                seasons.Remove(selectedSeason);
                SaveSeasons();
                Console.WriteLine(GetMessage("RemoveSuccess"));
            }
            else
            {
                Console.WriteLine(GetMessage("SeasonNotFound"));
            }

            Console.Clear();
        }


        //ini-parser seasons.ini
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

        // Save your info
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