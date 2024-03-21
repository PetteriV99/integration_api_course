string newFilePath = "C:/Users/pjetu/Documents/IntegraatioHarjoitus/In";
string destinationDirectory = "C:/Users/pjetu/Documents/IntegraatioHarjoitus/Out";

string[] files = Directory.GetFiles(newFilePath);

foreach (string file in files)
{
    string fileName = Path.GetFileName(file);
    string destinationPath = Path.Combine(destinationDirectory, fileName);
    File.Copy(file, destinationPath);
    Console.WriteLine($"Copied: {file} -> {destinationPath}");
}