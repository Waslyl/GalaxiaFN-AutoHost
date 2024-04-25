using System;
using GalaxiaFN_Console.Properties;
using static GalaxiaFN_Console.Injection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxiaFN_Console
{
    internal class Program
    {
        private delegate bool EventHandler(CtrlType sig);
        static SetConsoleCtrlEventHandler _handler;
        static void Main(string[] args)
        {
            Console.WriteLine("[+] Bienvenue sur GalaxiaFN Console, un programme pour lancer des anciennes Saisons de Fortnite et pour jouer dessus.\n Ce programme à été programmer par Waslyl\n"); Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] Que voulez vous faire?"); Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[0] Ajouter une version");
            Console.WriteLine("[1] ");
        }
    }
}
