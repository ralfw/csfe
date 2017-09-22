using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace digitvalues
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var inputFilenames = Directory.GetFiles("input");
            foreach (var f in inputFilenames) {
                var romanNumber = File.ReadAllText(f);

                var digits = romanNumber.Trim().ToUpper().ToCharArray();
                var values = digits.Select(Map_digit_to_value);
                
                var outputFilename = "output/" + Path.GetFileName(f);
                File.WriteAllLines(outputFilename, values.Select(v => v.ToString()));
            }


            int Map_digit_to_value(char d) {
                switch (d) {
                    case 'I': return 1;
                    case 'V': return 5;
                    case 'X': return 10;
                    case 'L': return 50;
                    case 'C': return 100;
                    case 'D': return 500;
                    case 'M': return 1000;
                    default: throw new ApplicationException($"Invalid roman digit: {d}");
                }
            }
        }
    }
}