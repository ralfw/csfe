using System;
using System.Collections.Generic;
using System.IO;
using CLAP;

namespace csfe
{
    public class App
    {
        public static void Run(string[] args) {
            Parser.Run<App>(args);
        }
        
        
        [Verb(IsDefault = true)]
        public static void Run(
            [Required,Aliases("s,source")] string flowSourceFilename, 
            [DefaultValue("."),Aliases("p,path,fp")] string flowPath, 
            [Required,Aliases("i,input,ip")] string inputPath)
        {
            Validate_parameters();
            var inputFilenames = Compile_files_to_process();
            Process__files(inputFilenames);

            
            void Validate_parameters() {
                if (inputPath != "") return;
                Console.WriteLine("No input source specified! Use -i= to point to a file as input or a directory with input files.");
                Environment.Exit(2);
            }

            IEnumerable<string> Compile_files_to_process() {
                if (File.Exists(inputPath)) {
                    yield return inputPath;
                }
                else if (Directory.Exists(inputPath)) {
                    foreach (var f in Directory.GetFiles(inputPath))
                        yield return f;
                }
                else {
                    Console.WriteLine($"Input path {inputPath} is neither a file nor a directory. Nothing to process.");
                    Environment.Exit(2);
                }
            }

            void Process__files(IEnumerable<string> filenames) {
                var fex = new FlowExecutionEngine(flowSourceFilename, flowPath);    
                foreach (var f in filenames)
                    fex.ProcessFile(f);
            }
        }
    }
}