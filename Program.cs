using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;

namespace iCloudPhotosBackupToFolders
{
    public class CSVObject
    {
        public string Images { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string outputFolder = "output";

            // Set photos directory path
            Console.WriteLine("Enter the full path of the folder that contains the photo's:");
            string photosFolderPath = Console.ReadLine();
            if (!Directory.Exists(photosFolderPath)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Directory '{photosFolderPath}' does not exist.");
                return;
            }

            // Set CSV mapping file path
            Console.WriteLine("Enter the full path to the CSV mapping file, including the CSV file name itself:");
            string csvPath = Console.ReadLine();
            if (!File.Exists(csvPath)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File '{csvPath}' does not exist.");
                return;
            }

            // Set destination folder
            Console.WriteLine("Enter the destination folder name:");
            string targetFolderName = Console.ReadLine();

            // Create destination folder
            DirectoryInfo targetFolderInfo = Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/{outputFolder}/{targetFolderName}");

            // Get files based on mapping
            using (StreamReader reader = new StreamReader(csvPath))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                IEnumerable<CSVObject> records = csv.GetRecords<CSVObject>();
                List<string> files = new List<string>();
                Boolean error = false;

                // Check if all files exist, otherwise do not execute
                foreach (CSVObject record in records) {
                    string filePath = $"{photosFolderPath}/{record.Images}";
                    if (!File.Exists(filePath)) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"File '{record.Images}' does not exist.");
                        break;
                    }
                    files.Add(record.Images);
                }
                if (error) {
                    return;
                }

                // If all files exist, copy files to target folder
                foreach (string fileName in files) {
                    string filePath = $"{photosFolderPath}/{fileName}";
                    string targetPath = $"{Directory.GetCurrentDirectory()}/{outputFolder}/{targetFolderInfo.Name}/{fileName}";
                    File.Copy(filePath, targetPath);
                }
            }
        }
    }
}
