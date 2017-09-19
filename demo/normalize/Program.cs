using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace normalize
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var inputfile = Directory.GetFiles("input").First();
            var charsText = File.ReadAllText(inputfile);
            File.Delete(inputfile);

            charsText = charsText.ToLower();

            File.WriteAllText($"output/{Path.GetFileName(Path.GetTempFileName())}", charsText);
        }
    }
}