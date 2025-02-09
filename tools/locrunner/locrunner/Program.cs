// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

string currentDirectory = Directory.GetCurrentDirectory();
string[] locFiles = Directory.GetFiles(currentDirectory, "*.loc");

foreach (string locFile in locFiles)
{
    string fileName = Path.GetFileName(locFile);
    string outputJsonFile = Path.ChangeExtension(fileName, ".json");
    Process process = new Process()
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "locc",
            Arguments = $"--from=\"{locFile}\" --to=\"{outputJsonFile}\" --mode=decompile",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

    Console.WriteLine($"Processing: {locFile} -> {outputJsonFile}");
    process.Start();
    process.WaitForExit();

    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();

    if (!string.IsNullOrEmpty(output))
        Console.WriteLine(output);
    if (!string.IsNullOrEmpty(error))
        Console.WriteLine($"Error: {error}");
}