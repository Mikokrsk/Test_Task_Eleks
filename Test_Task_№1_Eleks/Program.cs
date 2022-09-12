using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
namespace DuplicateFileFinder
{
    class Program
    {
        static int bufferSize = 1 * 1024 * 1024;

        static void Main(string[] args)
        {
            

            Stopwatch stopWatch = new Stopwatch();
            
            stopWatch.Start();

            string path; //= @"D:\";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Path :");
            path =@""+Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            //додавання адрес всіх файлів до ліста fileLists
            var fileLists = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
              
            //

            int totalFiles = fileLists.Length;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(totalFiles);
            Console.ForegroundColor = ConsoleColor.White;

            List<FileDetails> finalDetails = new List<FileDetails>();
            List<string> Dublicats = new List<string>();
            finalDetails.Clear();

            //Додавання в ліст адреса файлів та їх адрес
            Parallel.ForEach(fileLists, (item) =>
            {
                using (var stream = new BufferedStream(File.OpenRead(item), bufferSize))
                {
                    finalDetails.Add(new FileDetails()
                    {
                        FileName = item,
                        //хешування файлів
                        FileHash = BitConverter.ToString(SHA1.Create().ComputeHash(stream)),
                    });
                }

            });      
            
            //групування
            var similarList = finalDetails.GroupBy(f => f.FileHash)
                .Select(g => new { FileHash = g.Key, Files = g.Select(z => z.FileName).ToList() });

            Dublicats.AddRange(similarList.SelectMany(f => f.Files.Skip(1)).ToList());

            //вивід всіх файлів дублікатів

            Console.ForegroundColor = ConsoleColor.Red;
             Console.WriteLine("Total duplicate files - {0}", Dublicats.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Dublicats.Count > 0)
            {

                foreach (var item in Dublicats)
                {
                    Console.WriteLine(item);
                    FileInfo fi = new FileInfo(item);
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            stopWatch.Stop();
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss\\.fffffff}", stopWatch.Elapsed);
            Console.ForegroundColor = ConsoleColor.White;

        }

    }
    public class FileDetails
    {
        public string FileName { get; set; }
        public string FileHash { get; set; }
    }


}