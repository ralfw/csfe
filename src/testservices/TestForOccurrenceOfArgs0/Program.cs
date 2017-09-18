using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestForOccurrenceOfArgs0
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Directory.CreateDirectory("input");
            Directory.CreateDirectory("output");

            var inputFilename = Directory.GetFiles("input").FirstOrDefault();
            if (inputFilename == null) return;
            
            var text = File.ReadAllText(inputFilename);
            if (text.IndexOf(args[0]) >= 0)
            {
                var outputFilename = "output/" + Unique_sortable_filename() + "-SUCCESS";
                File.Move(inputFilename, outputFilename);
            }
            else
            {
                File.Delete(inputFilename);
                var outputFilename = "output/" + Unique_sortable_filename() + "-FAILURE";
                File.WriteAllText(outputFilename, $"Pattern '{args[0]}' not found in <<<\n{text}\n>>>");
            }


            string Unique_sortable_filename() => File_sequence_number().ToString("00000000000000");
            long File_sequence_number() => (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}