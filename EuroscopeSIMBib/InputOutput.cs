using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES
{
    internal class InputOutput
    {
        internal static List<string> ImportFlightData(string ICAO)
        {
            List<string> FlightsData = new List<string>();
            try
            {
                StreamReader reader = new StreamReader($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\Euroscope\\SIMGEN\\{ICAO}.csv");

                using (reader)
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        FlightsData.Add(line);
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(".csv file not found \n Make sure you put the flights.csv in MyDocuments");
                Console.WriteLine(e.Message);
                Console.ReadLine();
                System.Environment.Exit(0);
                /* string fileName;
                OpenFileDialog fd = new OpenFileDialog();
                fd.ShowDialog();
                fileName = fd.FileName;
                Console.Write(fileName); */
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while reading.csv");
                Console.WriteLine(e.Message);
            }
            return FlightsData;
        }

        public static void ExportSimFile(List<string> Output)
        {
            DateTime dateTime = DateTime.Now;
            string date = $"{dateTime.Day}.{dateTime.Month}";
            System.IO.Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\Euroscope\\SIM\\SIMGEN");
            var File = new StreamWriter($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\Euroscope\\SIMGEN\\{date}.txt");
            foreach (string line in Output)
            {
                File.WriteLine(line);
            }
            File.Flush();
            File.Close();
        }
    }
}
