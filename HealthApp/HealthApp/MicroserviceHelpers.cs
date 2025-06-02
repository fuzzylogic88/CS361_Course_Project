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
        public static readonly string pypath = @"C:/Python313/python.exe";

        /// <summary>
        /// Small helper function to spin up a python script
        /// </summary>
        /// <param name="path"></param>
        public static void StartPythonScript(string script_path)
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
            msgProc.Start();
        }
    }
}
