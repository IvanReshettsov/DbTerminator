using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTerminator
{
    public class Repository
    {
        public void SaveViewedFile(string file)
        {
            var files = LoadViewedFiles();

            if (files.Contains(file))
            {
                files.Remove(file);
            }
            if (files.Count() > 2) {
                files.Remove(files.First());
            }
            if (file != @"..\..\Resources\AdventureWorks2012.mdf")
            {
                files.Add(file);
            }
            
            File.WriteAllLines(@"..\..\Resources\files.txt", files);
        }

        public IList<string> LoadViewedFiles()
        {
            return File.ReadAllLines(@"..\..\Resources\files.txt").ToList();
        }
    }
}
