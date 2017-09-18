using System;
using System.Diagnostics;
using System.IO;

namespace csfe.adapters
{
    //TODO: optionally pass into Service input/output directories to create/use
    internal class Service {
        private readonly string _path;
        private readonly ServiceAdapter _service;
        
        public Service(string path, string executablename, string arguments) {
            _path = path;
            Directory.CreateDirectory(this.InputPath);
            Directory.CreateDirectory(this.OutputPath);

            arguments = arguments.Replace("%input", this.InputPath).Replace("%output", this.OutputPath);
            _service = new ServiceAdapter(path, executablename, arguments);
        }
        
        private string InputPath => System.IO.Path.Combine(_path, "input");
        private string OutputPath => System.IO.Path.Combine(_path, "output");
                    

        public void RunSync() {
            _service.RunSync();
        }

        public void AddInput(string filename) {
            var inputFilename = Path.Combine(this.InputPath, Filesystem.Unique_sortable_filename());
            Filesystem.Copy_file(filename, inputFilename);
        }

        public string[] Output => Filesystem.List_files_in_chronological_order(this.OutputPath);
    }
    
    
    internal class ServiceAdapter {
        private readonly string _path;
        private readonly string _executablename;
        private readonly string _arguments;

        public ServiceAdapter(string path, string executablename, string arguments) {
            _path = path;
            _executablename = executablename;
            _arguments = arguments;
        }
            
        public void RunSync() {
            var pi = new ProcessStartInfo {
                FileName = _executablename,
                Arguments = _arguments,
                WorkingDirectory = _path,
                CreateNoWindow = true
            };
            var p = Process.Start(pi);
            p.WaitForExit();
            switch (p.ExitCode) {
                case 0: return;
                case 2: throw new ApplicationException($"Missing service '{_executablename} {_arguments}' in '{_path}'! Exit code {p.ExitCode}.");
                default:
                    throw new ApplicationException($"Cannot run service '{_executablename} {_arguments}' in '{_path}'! Exit code {p.ExitCode}");
            }
        }
    }
}