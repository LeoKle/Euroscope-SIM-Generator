using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EuroscopeSIMBib
{
    public class InputOutput
    {
        public static List<string> ReadCSV(string suffix)
        {
            StreamReader reader = new StreamReader($"C:\\Users\\Leon\\Desktop\\Vatsim\\SIM\\{suffix}.csv");
            List<string> output = new List<string>();
            try
            {
                using (reader)
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        output.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Fehler beim Einlesen der CSV: ");
                Console.WriteLine(e.Message);
            }
            return output;
        }

        public static void ExportSimFile(List<string> Output)
        {
            DateTime dateTime = DateTime.Now;
            string date = $"{dateTime.Day}.{dateTime.Month}";
            var File = new StreamWriter($"C:\\Users\\Leon\\Desktop\\Vatsim\\SIM\\{date}.txt");
            foreach(string line in Output)
            {
                File.WriteLine(line);
            }
            File.Flush();
            File.Close();
        }
    }
}
