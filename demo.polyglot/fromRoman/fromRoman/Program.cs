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
            var flow = new csfe.FlowExecutionEngine("fromRoman.csfe", ".");
            
            while (true) {
                Clear_input_output();
                
                Console.Write("Roman number: ");
                var roman = Console.ReadLine();
                if (roman == "") break;
                
                flow.ProcessText(roman);

                var resultFilename = flow.Output.FirstOrDefault();
                if (resultFilename != null) {
                    var result = File.ReadAllText(resultFilename);
                    Console.WriteLine($"  = {result}");
                }
                else
                    Console.WriteLine("  No output was produced!");
            }
        }

        static void Clear_input_output()
        {
            var residueFilenames = Directory.GetFiles(".", "*.*", SearchOption.AllDirectories);
            residueFilenames = residueFilenames.Where(f => f.IndexOf("input/") > 0 || f.IndexOf("output/") > 0).ToArray();
            residueFilenames.ToList().ForEach(File.Delete);
        }
    }
}