using System;
using System.Diagnostics;

namespace GalaxiaFN_AutoRestart.ProcessKill
{
    public class ProcessKiller
    {
        /// <summary>
        /// Force stop un process
        /// </summary>
        /// <param name="processName">Nom d'un process comme "FortniteLauncher" par ex.</param>
        public static void KillProcessByName(string processName)
        {
            try
            {
                if (processName.EndsWith(".exe"))
                {
                    processName = processName.Substring(0, processName.Length - 4);
                }

                Process[] processes = Process.GetProcessesByName(processName);

                foreach (Process process in processes)
                {
                    process.Kill();
                    process.WaitForExit();
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{processName}.exe has been stoped by launcher. please contact galaxia staff for more infos");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error when trying to stop process: {ex.Message} at {ex.StackTrace}");
            }
        }
        public static bool IsProcessRunning(string processName)
        {
            if (processName.EndsWith(".exe"))
            {
                processName = processName.Substring(0, processName.Length - 4);
            }
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }
    }
}
