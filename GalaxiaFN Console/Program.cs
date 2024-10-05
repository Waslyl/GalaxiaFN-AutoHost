using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using IniParser;
using IniParser.Model;

namespace GalaxiaFN_AutoRestart
{
    internal class Program
    {
        static String idkkk = AppDomain.CurrentDomain.BaseDirectory;
        static Process FortniteClient = new Process();
        static Process FortniteEAC = new Process();
        static Process FortniteLauncher = new Process();
        static Dictionary<string, (string Path, string Username, string Password)> seasons = new Dictionary<string, (string, string, string)>();


        //Kill Process pour restart le GS
        //Kill Process for restart the GS
        public static void SafeKillProcess(string processName) 
        {
            try
            {
                Process[] processesByName = Process.GetProcessesByName(processName);
                for (int i = 0; i < processesByName.Length; i++)
                {
                    processesByName[i].Kill();
                }
            }
            catch
            {
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "GalaxiaFN Console Launcher";
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
                    Console.WriteLine("Veuillez répondre uniquement par 0, 1 ou 2");
                    Thread.Sleep(2000);
                    Console.Clear();
                }
            }
        }

        static void ShowMainMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] Bienvenue sur GalaxiaFN Console, un programme pour lancer des anciennes Saisons de Fortnite et pour jouer dessus.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[0] Ajouter une version de Fortnite");
            Console.WriteLine("[1] Lancer une version de Fortnite");
            Console.WriteLine("[2] Supprimer une version de Fortnite");
            Console.Write(">> ");
        }

        static void AddSeason()
        {
            Console.Clear();
            Console.WriteLine("Veuillez mettre le chemin de votre version ici:"); //Path
            Console.Write(">> ");
            string path = Console.ReadLine();

            Console.WriteLine("Veuillez donner un nom pour cette saison:"); //Name
            Console.Write(">> ");
            string name = Console.ReadLine();

            Console.WriteLine("Quel email voulez-vous mettre?"); //Email (pseudo@. if on lawinV1)
            Console.Write(">> ");
            string email = Console.ReadLine();

            Console.WriteLine("Quel mot de passe voulez-vous ajouter?"); //Password (whatever you want if on lawinV1)
            Console.Write(">> ");
            string password = Console.ReadLine();

            seasons[name] = (path, email, password);
            SaveSeasons();

            Console.WriteLine("Votre chemin, pseudo et mot de passe ont bien été enregistrés correctement!"); //Everything is saved in seasons.ini
            Console.Clear();
        }

        static void LaunchSeason()
        {
            if (seasons.Count == 0)
            {
                Console.WriteLine("Aucune saison n'a été ajoutée.");
                return;
            }

            Console.Clear();
            Console.WriteLine("Saisons disponibles:");
            foreach (var season in seasons.Keys)
            {
                Console.WriteLine($"- {season}");
            }

            Console.WriteLine("Veuillez séléctionner la saison que vous voulez lancer par son nom:");
            Console.Write(">> ");
            string selectedSeason = Console.ReadLine();

            if (seasons.ContainsKey(selectedSeason))
            {
                Console.Clear();
                var season = seasons[selectedSeason];
                Console.WriteLine($"Lancement de la saison: {selectedSeason} avec le chemin: {season.Path}");

                while (true)
                {
                    LaunchGameInstance(season);
                    Thread.Sleep(2000); // Attendre 2 secondes avant de relancer   Wait 2 seconds before restarting
                }
            }
            else
            {
                Console.WriteLine("Saison non trouvée.");
            }

            Console.Clear();
            ShowMainMenu();
        }
        public static void Inject(int pid, string path)
        {
            if (!File.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Could not find {path.Split('\\').Last()}. Exiting in 3 seconds.");
                Thread.Sleep(3000);
                Environment.Exit(0);
                return;
            }

            try
            {
                IntPtr hProcess = Win32.OpenProcess(1082, false, pid);
                IntPtr procAdress = Win32.GetProcAddress(Win32.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                uint num = checked((uint)((path.Length + 1) * Marshal.SizeOf(typeof(char))));
                IntPtr intPtr = Win32.VirtualAllocEx(hProcess, IntPtr.Zero, num, 12288U, 4U);
                UIntPtr uintPtr;
                Win32.WriteProcessMemory(hProcess, intPtr, Encoding.Default.GetBytes(path), num, out uintPtr);
                Win32.CreateRemoteThread(hProcess, IntPtr.Zero, 0U, procAdress, intPtr, 0U, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Injection failed: {ex.Message}"); //retour d'erreur  Callback error
            }
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


                FortniteLauncher.StartInfo.FileName = launcherPath;
                FortniteLauncher.Start();
                ProcessExtension.Suspend(FortniteLauncher);

                string eacPath = Path.Combine(season.Path, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping_EAC.exe");
                if (!File.Exists(eacPath))
                {
                    Console.WriteLine($"Le fichier {eacPath} est introuvable.");
                    return;
                }


                FortniteEAC.StartInfo.FileName = eacPath;
                FortniteEAC.Start();
                ProcessExtension.Suspend(FortniteEAC);

                

                Process Fortnite = new Process();
                Fortnite.StartInfo.FileName = Path.Combine(season.Path, "FortniteGame/Binaries/Win64/FortniteClient-Win64-Shipping.exe");
                Fortnite.StartInfo.RedirectStandardOutput = true;  
                Fortnite.StartInfo.UseShellExecute = false;
                Fortnite.StartInfo.Arguments = $@"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -log -nosplash -nosound -nullrhi -useolditemcards -fltoken=3db3ba5dcbd2e16703f3978d -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={season.Username} -AUTH_PASSWORD={season.Password} -AUTH_TYPE=epic";
                Fortnite.Start();

                
                Process injector = new Process();
                injector.StartInfo.FileName = $@"{idkkk}\\inj\\injector.exe";
                injector.StartInfo.Arguments = $@"-p {Fortnite.Id} -i redirect.dll "; // remplacer son ssl bypass dans \inj\redirect.dll  Replace your ssl in \inj\redirect.dll
                injector.StartInfo.CreateNoWindow = true; injector.StartInfo.RedirectStandardOutput = false;
                injector.StartInfo.UseShellExecute = false;
                injector.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                injector.Start(); //inject redirect dll

                string outputLine;
                while ((outputLine = Fortnite.StandardOutput.ReadLine()) != null)
                {
                    if (outputLine.Contains("Region "))
                    {
                        Thread.Sleep(3500);

                        Process inj = new Process();
                        inj.StartInfo.FileName = $@"{idkkk}\\inj\\injector.exe";
                        inj.StartInfo.Arguments = $@"-p {Fortnite.Id} -i gameserver.dll "; // remplacer son ssl dans \inj\gameserver.dll  Replace your ssl in \inj\gameserver.dll
                        inj.StartInfo.CreateNoWindow = true; inj.StartInfo.RedirectStandardOutput = false;
                        inj.StartInfo.UseShellExecute = false;
                        inj.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        inj.Start();

                        Console.WriteLine("Le dll a été injecte");
                    }
                }

                /*Fortnite.BeginOutputReadLine();
                Fortnite.BeginErrorReadLine();*/

                Fortnite.WaitForExit();

                SafeKillProcess("EpicGamesLauncher");
                SafeKillProcess("EpicWebHelper");
                SafeKillProcess("CrashReportClient");
                SafeKillProcess("FortniteLauncher");
                SafeKillProcess("FortniteClient-Win64-Shipping");
                SafeKillProcess("EasyAntiCheat_EOS");
                SafeKillProcess("EpicGamesLauncher");
                SafeKillProcess("EpicWebHelper");
                SafeKillProcess("CrashReportClient");
                SafeKillProcess("FortniteLauncher");
                SafeKillProcess("FortniteClient-Win64-Shipping");
                SafeKillProcess("EasyAntiCheat_EOS");
                SafeKillProcess("EasyAntiCheat_Launcher");
                Console.WriteLine("La game est fini. Restart...");
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
                Console.WriteLine("Aucune saison n'a été ajoutée.");
                return;
            }

            Console.Clear();
            Console.WriteLine("Saisons disponibles:");
            foreach (var season in seasons.Keys)
            {
                Console.WriteLine($"- {season}");
            }

            Console.WriteLine("Veuillez sélectionner la saison que vous voulez supprimer par son nom:");
            Console.Write(">> ");
            string selectedSeason = Console.ReadLine();

            if (seasons.ContainsKey(selectedSeason))
            {
                seasons.Remove(selectedSeason);
                SaveSeasons();
                Console.WriteLine($"La saison {selectedSeason} a été supprimée avec succès.");
            }
            else
            {
                Console.WriteLine("Saison non trouvée.");
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

        //ini-parser seasons.ini
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