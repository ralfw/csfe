using System;
using System.IO;
using System.Runtime.InteropServices;
using csfe.adapters;

namespace csfe.operations
{
    public class InboxOperation {
        private readonly string _path;

        public InboxOperation(string path, string name = "input") {
            _path = System.IO.Path.Combine(path, name);
            Directory.CreateDirectory(_path);
        }

        public void AddText(string text) {
            var itemFilename = System.IO.Path.Combine(_path, Filesystem.Unique_sortable_filename());
            File.WriteAllText(itemFilename, text);
            OnItemArrived(itemFilename);
        }

        public void AddFile(string filename) {
            var itemFilename = System.IO.Path.Combine(_path, Filesystem.Unique_sortable_filename());
            Filesystem.Copy_file(filename, itemFilename);
            OnItemArrived(itemFilename);
        }

        public event Action<string> OnItemArrived;
    }
}