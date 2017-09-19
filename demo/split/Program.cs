using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace split
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var inputfile = Directory.GetFiles("input").First();
            var text = File.ReadAllText(inputfile);
            File.Delete(inputfile);
            
            var chars = text.ToCharArray().Where(char.IsLetterOrDigit).Select(c => c.ToString());
            
            var charsText = string.Join("\n", chars);
            File.WriteAllText($"output/{Path.GetFileName(Path.GetTempFileName())}", charsText);
        }
    }
}