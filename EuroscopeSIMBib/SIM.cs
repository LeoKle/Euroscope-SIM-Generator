using EuroscopeSIMGen;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ES
{
    public class SIM
    {
        internal static List<string> ImportACFT(string suffix)
        {
            #region Read flights.csv
            List<string> output = new List<string>();
            try
            {
                StreamReader reader = new StreamReader($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/{suffix}.csv");

                using (reader)
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        output.Add(line);
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
                Console.WriteLine("Fehler beim Einlesen der CSV: ");
                Console.WriteLine(e.Message);
            }
            return output;
            #endregion
        }

        internal static List<ACFT> CreateAircraft(List<string> Input)
        {
            List<ACFT> AircraftList = new List<ACFT>();
            //Squawks SquawkAssigner = new Squawks(); not needed if static
            for (int i = 1; i < Input.Count; i++)
            {
                string[] subs = Input[i].Split(';');
                ACFT CurrentAircraft = new ACFT(subs[0], subs[1], subs[2], subs[3], Squawks.AssignSquawk());
                AircraftList.Add(CurrentAircraft);
            }

            return AircraftList;
        }

        public static void GenerateSIM()
        {
            #region Selection for airport
            string _input;
            do
            {
                Console.WriteLine("Which airport? \n EDDL or EDDK");
                _input = Console.ReadLine();
                _input = _input.ToUpper();
            } while (_input != "EDDL" && _input != "EDDK");

            IAirport airport;
            string _runwaydirection;
            if (_input == "EDDL")
            {
                do
                {
                    Console.WriteLine("Which runway direction? \n 23 or 05");
                    _runwaydirection = Console.ReadLine();
                } while (_runwaydirection != "05" && _runwaydirection != "23");
                airport = new EDDL(_runwaydirection);
            }
            else if (_input == "EDDK")
            {
                {
                    Console.WriteLine("Which runway direction? \n 14 or 32");
                    _runwaydirection = Console.ReadLine();
                } while (_runwaydirection != "14" && _runwaydirection != "32") ;

                airport = new EDDL(_runwaydirection);
            }
            else
            {
                throw new ArgumentException();
            }
            #endregion

            #region Get aircraft list of selected airport
            List<string> import = SIM.ImportACFT(airport.ICAO);
            #endregion

            #region Create aircraft list and split by arrival and departure
            List<ACFT> AircraftList = SIM.CreateAircraft(import);
            List<ACFT> Outbounds = new List<ACFT>();
            List<ACFT> Inbounds = new List<ACFT>();

            for (int i = 0; i < AircraftList.Count; i++)
            {
                if (AircraftList[i].DEP == airport.ICAO)
                {
                    Outbounds.Add(AircraftList[i]);
                }
                else
                {
                    Inbounds.Add(AircraftList[i]);
                }
            }
            #endregion

            #region Interval selection and mode selection
            Console.WriteLine("Select sim duration in minutes");
            Int32.TryParse(Console.ReadLine(), out int _duration);
            bool erfolgreich;
            int _auswahl;
            do
            {
                Console.WriteLine("Which interval mode should be used?");
                Console.WriteLine("1. Constant: A constant interval allowing for a steady workflow");
                /*
                Console.WriteLine("2. Decreasing: Selection of 2 intervals, interval is decreasing therby increasing the amount of aircraft per time");
                Console.WriteLine("3. Alternating: Interval alternates between 2 intervals"); */

                erfolgreich = Int32.TryParse(Console.ReadLine(), out _auswahl) && (_auswahl == 1 || _auswahl == 2 || _auswahl == 3);
            } while (erfolgreich == false);
            int interval = 1;


            if (_auswahl == 1)
            {
                Console.WriteLine("Select constant interval: ");
                Int32.TryParse(Console.ReadLine(), out interval);
            }
            /*
            float interval2 = 0;
            if (_auswahl == 2)
            {
                Console.WriteLine("Select the first/start interval");
                float.TryParse(Console.ReadLine(), out interval);
                Console.WriteLine("Select the second/end interval");
                float.TryParse(Console.ReadLine(), out interval2);
            }
            if (_auswahl == 3)
            {
                Console.WriteLine("Select the first interval");
                float.TryParse(Console.ReadLine(), out interval);
                Console.WriteLine("Select the second interval");
                float.TryParse(Console.ReadLine(), out interval2);
            } */
            #endregion

            #region further SIM settings
            int outboundsactive;
            string OutboundStartPosition = "";
            do
            {
                Console.WriteLine("Should outbounds be included? \n 1. Yes \n2. No");
                Int32.TryParse(Console.ReadLine(), out outboundsactive);
            } while (outboundsactive != 1 && outboundsactive != 2);
            int outboundinterval = 500; //initialization to avoid problems in region "Generate outbound flow
            if (outboundsactive == 1)
            {
                #region outbound interval
                do
                {
                    Console.WriteLine("Should outbounds have the same interval as inbounds? \n 1. Yes \n2. No");
                    Int32.TryParse(Console.ReadLine(), out outboundinterval);
                } while (outboundinterval != 1 && outboundinterval != 2);
                if (outboundinterval == 2)
                {
                    bool outboundintervaltryparse;
                    do
                    {
                        Console.WriteLine("Which interval should outbounds have?");
                        outboundintervaltryparse = Int32.TryParse(Console.ReadLine(), out outboundinterval);
                    } while (outboundintervaltryparse == true);
                }
                else
                {
                    outboundinterval = interval;
                }
                #endregion
                #region Ground or Air start ?
                int groundorairstart;
                do
                {
                    Console.WriteLine("Should outbounds start on 1) ground or 2) in air");
                    Int32.TryParse(Console.ReadLine(), out groundorairstart);
                } while (groundorairstart != 1 && groundorairstart != 2);
                if (groundorairstart == 1)
                {
                    Console.WriteLine("Outbound aircraft are starting on ground and have to be commanded to takeoff manually");
                    if (airport.SelectedRunwayDirection == "05")
                    {
                        OutboundStartPosition = airport.DepartureGroundstart[1];
                    }
                    else
                    {
                        OutboundStartPosition = airport.DepartureGroundstart[0];
                    }

                }
                else
                {
                    Console.WriteLine("Outbound aircraft are starting in air");
                    if (airport.SelectedRunwayDirection == "05")
                    {
                        OutboundStartPosition = airport.DepartureAirstart[1];
                    }
                    else
                    {
                        OutboundStartPosition = airport.DepartureAirstart[0];
                    }
                }
                #endregion
            }
            #endregion

            #region Definition text
            List<string> SimFile = new List<string>();
            SimFile = GenerateSimFileStandardText(SimFile, airport);
            #endregion

            #region Generate outbound flow
            if (outboundsactive == 1)
            {
                #region SID Distribution
                List<int> SIDSelectionDistribution = new List<int>();
                Console.WriteLine($"Enter SID selection/distribution key: i.e. 5:1:2:5. {airport.ICAO} needs {airport.SIDS[airport.SelectedRunwayDirectionIndex].Count} numbers \n or type \"random\" for a random distribution");
                string _lastword = "";
                Console.WriteLine("Distribution numbers have to be in this order: \n");
                for (int i = 0; i < airport.SIDS[airport.SelectedRunwayDirectionIndex].Count; i++)
                {
                    _lastword = airport.SIDS[airport.SelectedRunwayDirectionIndex][i].Split(' ').Last();
                    if (i < airport.SIDS[airport.SelectedRunwayDirectionIndex].Count - 1)
                    {
                        Console.Write(_lastword + ":");
                    }
                    else
                    {
                        Console.Write(_lastword + "\n");
                    }
                }
                // reading and computing distribution key

                string _distributionkey;
                bool validEntry = false;
                List<int> distribution = new List<int>();
                do
                {
                    _distributionkey = Console.ReadLine();

                    string[] _subStrings = _distributionkey.Split(':');
                    foreach (string substring in _subStrings)
                    {
                        if (substring != ":")
                        {
                            Int32.TryParse(substring, out int _convertedInt);
                            distribution.Add(_convertedInt);
                            //Console.Write(_convertedInt + " ");
                        }
                    }
                    if (distribution.Count == airport.SIDS[airport.SelectedRunwayDirectionIndex].Count)
                    {
                        validEntry = true;
                    }
                    if (validEntry == false)
                    {
                        Console.Write($"Wrong input, input has to look like this 5:1:3:4 -> It needs {airport.SIDS[airport.SelectedRunwayDirectionIndex].Count} numbers, separated by : \n");
                        //Console.Write(distribution.Count);
                        distribution.Clear();
                    }
                } while (validEntry != true);
                #endregion
                for (int i = 0; i < _duration; i++)
                {
                    // Empty Line to separate each aircraft in .txt file
                    SimFile.Add("");
                    // random callsign from outbound list
                    Random rnd = new Random();
                    int r = rnd.Next(Outbounds.Count);
                    string RWYHeading = "";
                    if (Int32.TryParse(airport.SelectedRunwayDirection + airport.RunwayHeadingLastNumber, out int heading))
                    {
                        RWYHeading = ((int)((heading * 2.88 + 0.5) * 4)).ToString();
                    }
                    else
                    {
                        Console.WriteLine("Error occured while converting runway heading to string");
                        Console.ReadLine();
                        System.Environment.Exit(0);
                    }

                    int SelectedSID = 0; // TODO

                    string simroute = ""; //TODO

                    SimFile.Add($"@N:{Outbounds[r].Callsign}:{Squawks.AssignSquawk()}:1:{OutboundStartPosition}:{airport.Elevation}:0:{RWYHeading}:0");
                    SimFile.Add($"$FP{Outbounds[r].Callsign}:*A:I:{Outbounds[r].ATYP.Substring(0, Outbounds[r].ATYP.Length - 2)}:420:{Outbounds[r].DEP}:::0:{Outbounds[r].DEST}:00:00:0:0:::{airport.SIDS[SelectedSID]}");
                    SimFile.Add($"SIMDATA:{Outbounds[r].Callsign}:*:*:25:1:0");
                    SimFile.Add($"$ROUTE:{simroute}");
                    SimFile.Add($"START:{i}");
                    SimFile.Add($"DELAY:1:2");
                    SimFile.Add($"REQALT:5000");
                    SimFile.Add($"INITIALPSEUDOPILOT:EDGG_P_CTR");
                }
            }
            #endregion
        }

        internal static List<string> GenerateSimFileStandardText(List<string> SimFile, IAirport airport)
        {
            #region ILS/runway definitions
            for (int i = 0; i < 4; i++)
            {
                SimFile.Add($"{airport.ILSDefinition[airport.SelectedRunwayDirectionIndex][i]}");
            }
            #endregion

            #region Holding definitions
            for (int i = 0; i < airport.Holdings.Length; i++)
            {
                SimFile.Add($"{airport.Holdings[i]}");
            }
            #endregion
            #region airport elevation
            SimFile.Add($"AIRPORT_ALT:{airport.Elevation}");
            #endregion


            return SimFile;
        }
    }
}
