/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 * 
 * Various methods to facilitate microservice execution
 */

using System.Diagnostics;
using System.IO;

namespace HealthApp
{
    internal class MicroserviceHelpers
    {
        public static List<Process> RunningMicroservices = new();

        public static readonly string pypath = @"C:/Python313/python.exe";

        /// <summary>
        /// Small helper function to spin up a python script
        /// </summary>
        /// <param name="path"></param>
        public static void StartPythonScript(string script_path)
        {
            // check that we're not starting a dupe process
            bool running = false;
            foreach (Process p in MicroserviceHelpers.RunningMicroservices)
            {
                if (p.StartInfo.Arguments.ToString().Contains(script_path))
                {
                    running = true;
                    break;
                }
            }
            if (!running)
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = pypath,
                    Arguments = $"\"{script_path}\"",  // Properly quoted in case of spaces
                    WorkingDirectory = Path.GetDirectoryName(script_path), // Set working directory
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    CreateNoWindow = false,
                };
                var msgProc = new Process { StartInfo = processInfo };
                RunningMicroservices.Add(msgProc);

                msgProc.Start();
            }
        }

        /// <summary>
        /// Terminates any running python scripts on app exit
        /// </summary>
        public static void StopMicroservices()
        {
            foreach (Process p in RunningMicroservices)
            {
                if (!p.HasExited) {
                    p.Kill(true);
                }
            }
        }
    }
}
