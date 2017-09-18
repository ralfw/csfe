using System;
using System.IO;
using csfe.adapters;

namespace csfe.operations
{
    public class OutboxOperation {
        private readonly string _path;

        public OutboxOperation(string path, string name = "output") {
            _path = System.IO.Path.Combine(path, name);
            Directory.CreateDirectory(_path);
        }
        
        public void AddFile(string filename) {
            var itemFilename = System.IO.Path.Combine(_path, Filesystem.Unique_sortable_filename());
            Filesystem.Move_file(filename, itemFilename);
            OnItemArrived(itemFilename);
        }

        public string[] Items => Directory.GetFiles(_path);
        
        public event Action<string> OnItemArrived = _ => { };
    }
}