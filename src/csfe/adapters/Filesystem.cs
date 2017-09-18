using System;
using System.IO;
using System.Linq;

namespace csfe.adapters
{
    internal static class Filesystem
    {
        public static void Delete_directory(string dirPath) {
            if (Directory.Exists(dirPath)) Directory.Delete(dirPath, true);
        }
        
        public static string[] List_files_in_chronological_order(string path) {
            var filenames = Directory.GetFiles(path);
            return filenames.Select(f => new FileInfo(f))
                .OrderBy(fi => fi.CreationTime)
                .ThenBy(fi => fi.Name)
                .Select(fi => fi.FullName)
                .ToArray();
        }

        public static void Copy_file(string sourceFilename, string destinationFilename) {
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFilename));
            File.Copy(sourceFilename, destinationFilename);
        }

        public static void Move_file(string sourceFilename, string destinationFilename) {
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFilename));
            File.Move(sourceFilename, destinationFilename);
        }
        
        
        public static string Unique_sortable_filename() => File_sequence_number().ToString("00000000000000");
        private static long File_sequence_number() => (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
}