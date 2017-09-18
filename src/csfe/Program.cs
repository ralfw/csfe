using System;
using System.Diagnostics;
using System.Security.Claims;
using CLAP;

namespace csfe
{
    internal class Program
    {
        public static void Main(string[] args) {
            Print_usage(args);
            Parser.Run<App>(args);
        }

        private static void Print_usage(string[] args) {
            if (args.Length > 0) return;
            Console.WriteLine($"CSFE - Console Service Flow Engine v{Version()}");
            Console.WriteLine();
            Console.WriteLine("Usage: csfe.exe -s=<source file> -p=<flow path> -i=<input source>");
            Console.WriteLine("  -s, -source: name of file with flow definition");
            Console.WriteLine("  -p, -path: path to directory with services");
            Console.WriteLine("  -i, -input: name of file to process or directory with files to process");
            Environment.Exit(1);

            string Version() {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }
    }
}