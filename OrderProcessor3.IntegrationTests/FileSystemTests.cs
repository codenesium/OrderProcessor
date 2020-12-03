using FluentAssertions;
using System.IO;
using System.Reflection;
using Xunit;

namespace OrderProcessor.IntegrationTests
{
    public class FileSystemTests
    {
        [Fact]
        public void ReadAllText_FileIsRead()
        {
            // Given
            string testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(testDirectory, $"ReadAllLinesTest.txt");
            File.Delete(path);
            File.WriteAllLines(path,
                new string[]
                {
                    "line0"
                });

            FileSystem fileSystem = new FileSystem();

            // When
            string text = fileSystem.ReadAllText(path);

            // Then
            text.Should().Be("line0");
        }

        [Fact]
        public void MoveFile_FileIsMoved()
        {
            // Given
            string testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string existingPath = Path.Combine(testDirectory, $"ReadAllLinesTest.txt");
            string newPath = Path.Combine(testDirectory, $"Moved_ReadAllLinesTest.txt");
            File.Delete(existingPath);
            File.Delete(newPath);
            File.WriteAllLines(existingPath,
                new string[]
                {
                    "line0",
                    "line1",
                });

            FileSystem fileSystem = new FileSystem();

            // When
            fileSystem.MoveFile(existingPath, newPath);

            // Then
            File.Exists(existingPath).Should().BeFalse();
            File.Exists(newPath).Should().BeTrue();
        }
    }
}