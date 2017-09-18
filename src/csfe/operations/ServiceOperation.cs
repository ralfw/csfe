using System;
using System.IO;
using csfe.adapters;

namespace csfe.operations
{
    public class ServiceOperation {
        private readonly Service _service;

        public ServiceOperation(string path, string executablename, string arguments) {
            _service = new Service(path, executablename, arguments);
        }

        public void Enqueue(string filename)
        {
            Move_file_to_input();
            _service.RunSync();
            Report_all_output_files_to_subscribers();
            

            void Move_file_to_input() {
                _service.AddInput(filename);
                File.Delete(filename);
            }
            void Report_all_output_files_to_subscribers() {
                foreach (var outputFilename in _service.Output)
                    OnOutput(outputFilename);
            }
        }

        public Action<string> OnOutput = _ => { };
    }
}