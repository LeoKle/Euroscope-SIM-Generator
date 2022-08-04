using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EuroscopeSIMBib;

namespace Konsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> Input = InputOutput.ReadCSV("eddk");
            List<ACFT> ListACFT = SIM.CreateAircraft(Input);
            List<string> Ausgabe = SIM.CreateSimFile(ListACFT);
            InputOutput.ExportSimFile(Ausgabe);
        }
    }
}
