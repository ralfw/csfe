using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace count
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var inputfile = Directory.GetFiles("input").First();
            var chars = File.ReadAllLines(inputfile);
            File.Delete(inputfile);
            
            var counts = new Dictionary<string,int>();
            foreach (var c in chars) {
                if (!counts.ContainsKey(c)) counts[c] = 0;
                counts[c] += 1;
            }

            var index = counts.Select(cn => $"{cn.Key};{cn.Value}").OrderBy(i => i);
            File.AppendAllLines("output/" + Path.GetFileName(Path.GetTempFileName()), index);
        }
    }
}