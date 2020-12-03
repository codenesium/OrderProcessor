using System.IO;

namespace OrderProcessor
{
    public interface IFileSystem
    {
        void MoveFile(string source, string destination);

        string ReadAllText(string filename);

        string[] GetFiles(string directory);
    }

    public class FileSystem : IFileSystem
    {
        public string ReadAllText(string filename)
        {
            return File.ReadAllText(filename).Trim();
        }

        public string[] GetFiles(string directory)
        {
            return Directory.GetFiles(directory);
        }

        public void MoveFile(string source, string destination)
        {
            File.Move(source, destination);
        }
    }
}