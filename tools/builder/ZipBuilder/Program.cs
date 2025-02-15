using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace ZipBuilder
{
    internal class Program
    {
        static string localPath = ".";
        static void Main()
        {
            if (Directory.Exists(localPath + "\\loc"))
            {
                Directory.Delete(localPath + "\\loc", true);
            }
            Directory.CreateDirectory(localPath + "\\loc");

            ReplaceMainNodeFromBloodMoneyFileToAllFiles();
            CreateMainLocks();
            CreatePostMissionLocs();
            UpdateZip();

            Directory.Delete(localPath + "\\loc", true);
        }

        static void UpdateZip()
        {
            string sourceFolder = localPath + @"\loc"; // Папка з .loc файлами
            string archiveFolder = localPath + @"\release-sources";  // Папка з архівами

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

            if (File.Exists("Hitman_Ukr.zip"))
            {
                File.Delete("Hitman_Ukr.zip");
            }

            using (FileStream zipToCreate = new FileStream("Hitman_Ukr.zip", FileMode.Create))
            using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
            {
                var scenesFolder = archiveFolder + "\\Scenes";
                string rootFolderName = Path.GetFileName(scenesFolder.TrimEnd(Path.DirectorySeparatorChar));

                // Додаємо всі файли з папки до архіву разом із папкою
                foreach (string file in Directory.GetFiles(scenesFolder, "*", SearchOption.AllDirectories))
                {
                    Console.WriteLine($"Processing - {file}");
                    string relativePath = Path.GetRelativePath(scenesFolder, file);
                    string entryScene = Path.Combine(rootFolderName, relativePath).Replace("\\", "/"); // Відносний шлях у архіві
                    archive.CreateEntryFromFile(file, entryScene);
                }


                string additionalFile = archiveFolder + "\\HitmanBloodMoney.exe";
                archive.CreateEntryFromFile(additionalFile, "HitmanBloodMoney.exe");

            }

            Console.WriteLine("Архів успішно створено!");
        }
        static void CreateMainLocks()
        {
            string jsonDirectory = localPath + @"\json";
            string locDirectory = localPath + @"\loc";
            string[] jsonFiles = Directory.GetFiles(jsonDirectory, "*.json");

            foreach (string jsonFile in jsonFiles)
            {

                string fileName = Path.GetFileName(jsonFile);
                if (fileName.ToUpperInvariant() == "M00_MAIN.JSON")
                {
                    continue;
                }
                string outputLocFile = Path.ChangeExtension(fileName, ".loc");
                Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "locc",
                        Arguments = $"--from=\"{jsonDirectory}\\{fileName}\" --to=\"{locDirectory}\\{outputLocFile}\" --mode=compile", //--mode=decompile --pretty-json=true
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                Console.WriteLine($"Processing: {fileName} -> {outputLocFile}");
                process.Start();
                process.WaitForExit();

                string output1 = process.StandardOutput.ReadToEnd();
                string error1 = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(output1))
                    Console.WriteLine(output1);
                if (!string.IsNullOrEmpty(error1))
                    Console.WriteLine($"Error: {error1}");
            }

            //M00_MAIN.JSON
            string locFileM00 = locDirectory + "\\" + $"M00_main.loc";
            string jsonFileM00 = jsonDirectory + "\\" + $"M00_main.json";
            Process process2 = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "hitmanbe",
                    Arguments = $"\"{jsonFileM00}\" \"{locFileM00}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            Console.WriteLine($"Processing: {jsonFileM00} -> {locFileM00}");
            process2.Start();
            process2.WaitForExit();

            string output = process2.StandardOutput.ReadToEnd();
            string error = process2.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(output))
                Console.WriteLine(output);
            if (!string.IsNullOrEmpty(error))
                Console.WriteLine($"Error: {error}");


        }

        static void CreatePostMissionLocs()
        {
            string postmission = localPath + @"\json\postmission\postmission.json";

            string locFile = localPath + @"\loc" + "\\" + $"M01_postmission.loc";
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "hitmanbe",
                    Arguments = $"\"{postmission}\" \"{locFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            Console.WriteLine($"Processing: {postmission} -> {locFile}");
            process.Start();
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(output))
                Console.WriteLine(output);
            if (!string.IsNullOrEmpty(error))
                Console.WriteLine($"Error: {error}");



            for (var i = 2; i < 13; i++)
            {
                if (i == 7)
                {
                    continue;
                }

                string newLocFile = localPath + @"\loc" + "\\" + $"M{i.ToString("D2")}_postmission.loc";

                Console.WriteLine($"Copying: {postmission} -> {newLocFile}");

                File.Copy(locFile, newLocFile, false);
            }
        }

        static void ReplaceMainNodeFromBloodMoneyFileToAllFiles()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string jsonFolder = localPath + @"\json";
            string mainUkr = localPath + @"\json\main\main.json";

            var jsonUkr = File.ReadAllTextAsync(mainUkr, Encoding.UTF8).GetAwaiter().GetResult();

            var ukr = JsonConvert.DeserializeObject<HLocClass>(jsonUkr);

            string[] files = Directory.GetFiles(jsonFolder, "*.JSON");

            for (int i1 = 0; i1 < files.Length; i1++)
            {
                string file = files[i1];
                string fileName = Path.GetFileName(file);

                if (fileName.ToUpperInvariant() == "M00_MAIN.JSON" || fileName.ToUpperInvariant().Contains("POSTMISSION"))
                {
                    continue;
                }

                var jsonText = File.ReadAllTextAsync(file, Encoding.UTF8).GetAwaiter().GetResult();

                var hloc = JsonConvert.DeserializeObject<HLocClass>(jsonText);

                hloc.children[0] = ukr;


                //GoThrouClass(hloc);

                //for (var i = 1; i < hloc.children.Count; i++)
                //{
                //    GoThrouClass(hloc.children[i]);
                //}

                var newJsonText = JsonConvert.SerializeObject(hloc, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                string destinationFilePath = Path.Combine(jsonFolder, fileName);
                File.WriteAllText(destinationFilePath, newJsonText, Encoding.UTF8);
                Console.WriteLine($"Processing - {destinationFilePath}");
            }
        }

        private static void GoThrouClass(HLocClass root)
        {
            if (root == null) return;
            Stack<HLocClass> stack = new Stack<HLocClass>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                HLocClass current = stack.Pop();
                if (!string.IsNullOrEmpty(current.value))
                {
                    current.value = "";//TranslateText(current.value, current.name);
                }

                if (current.children != null)
                {
                    for (int i = current.children.Count - 1; i >= 0; i--)
                    {
                        if (current.children[i] != null) // Переконуємося, що елемент не null
                        {
                            stack.Push(current.children[i]);
                        }
                    }
                }
            }
        }
    }
}
