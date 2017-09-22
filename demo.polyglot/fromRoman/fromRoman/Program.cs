using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace fromRoman
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var residueFilenames = Directory.GetFiles(".", "*.*", SearchOption.AllDirectories)
                                            .Where(f => f.IndexOf("input/") > 0 || f.IndexOf("output/") > 0)
                                            .ToList();
            residueFilenames.ForEach(File.Delete);
            
            var flow = new csfe.FlowExecutionEngine("fromRoman.csfe", ".");
            
            while (true) {
                Console.Write("Roman number: ");
                var roman = Console.ReadLine();
                if (roman == "") break;
                
                flow.ProcessText(roman);

                var resultFilename = flow.Output.First();
                var result = File.ReadAllText(resultFilename);
                Console.WriteLine($"  = {result}");
            }
        }
    }
}