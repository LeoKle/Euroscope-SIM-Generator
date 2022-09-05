using ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Deployment.Internal;

namespace EuroscopeSIMBib
{
    public static class ScenarioFileGenerator
    {
        private static int _duration;
        private static int _inboundseparation;
        private static int _outboundseparation;
        private static int _groundorairstart;
        private static string _outboundstartposition;
        private static string _outboundinitialaltitude;
        private static int _runwaydirectionindex;
        private static int _selectedPseudopilot;
        private static List<string> SimFile = new List<string>();

        private static Airport airport;

        private static List<ACFT> AircraftList = new List<ACFT>();
        private static List<ACFT> Outbounds = new List<ACFT>();
        private static List<ACFT> Inbounds = new List<ACFT>();

        public static void GenerateSim()
        {
            InputOutput.CheckFolder();
            InputOutput.ReadJSON(out List<Airport> airports);

            PresentandSelectAirport(airports, out Airport _selectedAirport);
            SelectAirportConfig(_selectedAirport);



            Console.ReadLine();
        }

        private static void PresentandSelectAirport(List<Airport> airports, out Airport _selectedAirport)
        {
            int _input = 0;
            int i = 1;
            List<string> _airportnames = new List<string>();
            do
            {
                _airportnames.Clear();

                Console.WriteLine("Airports in config file: ");
                i = 1;
                foreach (Airport airport in airports)
                {
                    Console.WriteLine($"{i}. {airport.ICAO}");
                    i++;
                    _airportnames.Add(airport.ICAO);
                }
                Console.WriteLine("Select airport:");
                Int32.TryParse(Console.ReadLine(), out _input);

            } while (_input > i || _input < 1);

            Console.WriteLine($"Selected: {airports[_input-1].ICAO}");
            _selectedAirport = airports[_input-1];
        }

        private static void SelectAirportConfig(Airport airport)
        {
            foreach(string config in airport.RunwayConfigText)
            {
                Console.WriteLine(config);
            }
        }
    }
}
