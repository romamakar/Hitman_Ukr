using System.IO.Compression;

namespace ZipBuilder
{
    internal class Program
    {
        static void Main()
        {
            string sourceFolder = @"C:\Users\roman\OneDrive\Desktop\Hitman_Ukr\loc"; // Папка з .loc файлами
            string archiveFolder = @"C:\Users\roman\OneDrive\Desktop\Hitman_Ukr\hitman";  // Папка з архівами

            if (!Directory.Exists(sourceFolder) || !Directory.Exists(archiveFolder))
            {
                Console.WriteLine("Одна з директорій не існує.");
                return;
            }

            foreach (string filePath in Directory.GetFiles(sourceFolder, "*.loc"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string archivePath, entryPath;

                if (fileName.Contains("_"))
                {
                    var parts = fileName.Split('_');
                    string level = parts[0];
                    string desc = string.Join("_", parts.Skip(1));
                    string subfolder = Path.Combine("Scenes", level);
                    archivePath = Path.Combine(archiveFolder, "Scenes", level, $"{level}_{desc}.zip");
                    entryPath = Path.Combine(subfolder, Path.GetFileName(filePath));
                }
                else
                {
                    archivePath = Path.Combine(archiveFolder, "Scenes", $"{fileName}.zip");
                    entryPath = Path.Combine("Scenes", Path.GetFileName(filePath));
                }

                entryPath = entryPath.Replace("\\", "/");              

                if (!File.Exists(archivePath))
                {
                    continue;
                }

                using (var archive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
                {
                    var existingEntry = archive.GetEntry(entryPath);
                    existingEntry?.Delete();
                    archive.CreateEntryFromFile(filePath, entryPath, CompressionLevel.Optimal);
                }

                Console.WriteLine($"Файл {filePath} додано до архіву {archivePath}");
            }
        }
        }
}
