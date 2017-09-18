using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestServiceToUpper
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
            text = text.ToUpper();

            var outputFilename = "output/" + Unique_sortable_filename() + ".txt";
            File.WriteAllText(outputFilename, text);
            
            File.Delete(inputFilename);
            
            string Unique_sortable_filename() => File_sequence_number().ToString("00000000000000");
            long File_sequence_number() => (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}