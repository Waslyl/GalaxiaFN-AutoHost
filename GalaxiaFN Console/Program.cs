using System;
using GalaxiaFN_Console.Properties;
using static GalaxiaFN_Console.Injection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IniParser;
using IniParser.Model;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace GalaxiaFN_Console
{
    internal class Program
    {
        private delegate bool EventHandler(CtrlType sig);
        static SetConsoleCtrlEventHandler _handler;
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("[+] Bienvenue sur GalaxiaFN Console, un programme pour lancer des anciennes Saisons de Fortnite et pour jouer dessus.\n Ce programme à été programmer par Waslyl\n");

            Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("[?] Que voulez vous faire?\n");

            Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("[0] Ajouter une version");

            Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("[1] Lancer une version"); 

            Console.Write(">> ");
            string output = Console.ReadLine();

            if (output.ToLower() == "0" && output.ToLower() != "1") 
            {
                Console.WriteLine("Veuillez repondre uniquement par 0 ou 1");
                //goto start;
            }

            if (output.ToLower() == "0")
            {
                Console.WriteLine("Veuillez mettre le path de votre jeu ici:");
                Console.Write(">> ");

                string path = Console.ReadLine();
                Settings.Default.path = path;
                Settings.Default.Save();
                Console.WriteLine("Votre chemin à bien été enregistré");
                


                /*string path = Console.ReadLine();
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("[+] Le path n'est pas valide, l'application va se fermer dans 5seconde..."); Console.ForegroundColor = ConsoleColor.Red;
                    Thread.Sleep(5000);
                    Environment.Exit(0);
                }*/
            }
            if (output.ToLower() == "1")
            {
                Console.WriteLine("Quel pseudo voulez vous mettre?");
                Console.Write(">> ");
                string username = Console.ReadLine();
                Settings.Default.username = username;
                Settings.Default.Save();
                Console.WriteLine("Votre pseudo à bien été enregistré");
            }   
        }
    }
}
