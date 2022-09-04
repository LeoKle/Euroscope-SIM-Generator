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

            Airport2 airport;
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
                Console.WriteLine("Which _inboundinterval mode should be used?");
                Console.WriteLine("1. Constant: A constant _inboundinterval allowing for a steady workflow");
                /*
                Console.WriteLine("2. Decreasing: Selection of 2 intervals, _inboundinterval is decreasing therby increasing the amount of aircraft per time");
                Console.WriteLine("3. Alternating: Interval alternates between 2 intervals"); */

                erfolgreich = Int32.TryParse(Console.ReadLine(), out _auswahl) && (_auswahl == 1 || _auswahl == 2 || _auswahl == 3);
            } while (erfolgreich == false);
            int _inboundinterval = 1;


            if (_auswahl == 1)
            {
                Console.WriteLine("Select constant _inboundinterval: ");
                Int32.TryParse(Console.ReadLine(), out _inboundinterval);
            }
            /*
            float interval2 = 0;
            if (_auswahl == 2)
            {
                Console.WriteLine("Select the first/start _inboundinterval");
                float.TryParse(Console.ReadLine(), out _inboundinterval);
                Console.WriteLine("Select the second/end _inboundinterval");
                float.TryParse(Console.ReadLine(), out interval2);
            }
            if (_auswahl == 3)
            {
                Console.WriteLine("Select the first _inboundinterval");
                float.TryParse(Console.ReadLine(), out _inboundinterval);
                Console.WriteLine("Select the second _inboundinterval");
                float.TryParse(Console.ReadLine(), out interval2);
            } */
            #endregion

            #region further SIM settings
            // Activation of outbounds
            int outboundsactive;
            string OutboundStartPosition = "";
            do
            {
                Console.WriteLine("Should outbounds be included? \n 1. Yes \n2. No");
                Int32.TryParse(Console.ReadLine(), out outboundsactive);
            } while (outboundsactive != 1 && outboundsactive != 2);
            int _outboundinterval = 2; //initialization to avoid problems in region "Generate outbound flow
            if (outboundsactive == 1)
            {
                #region outbound interval
                do
                {
                    Console.WriteLine("Should outbounds have the same _inboundinterval as inbounds? \n 1. Yes \n2. No");
                    Int32.TryParse(Console.ReadLine(), out _outboundinterval);
                } while (_outboundinterval != 1 && _outboundinterval != 2);
                if (_outboundinterval == 2)
                {
                    bool outboundintervaltryparse;
                    do
                    {
                        Console.WriteLine("Which _inboundinterval should outbounds have?");
                        outboundintervaltryparse = Int32.TryParse(Console.ReadLine(), out _outboundinterval);
                    } while (outboundintervaltryparse != true);
                }
                else
                {
                    _outboundinterval = _inboundinterval;
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

            // Selection of Initialpseudopilot
            Console.WriteLine("Which station should control the aircraft");
            for (int i = 0; i < airport.Pseudopilots.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {airport.Pseudopilots[i]}");
            }
            do
            {
                erfolgreich = Int32.TryParse(Console.ReadLine(), out _auswahl);
                erfolgreich = erfolgreich == true && _auswahl <= airport.Pseudopilots.Count && _auswahl >= 0;
            } while (!erfolgreich);
            string InitialPseudopilot = airport.Pseudopilots[_auswahl - 1];
            #endregion

            #region Definition text
            List<string> SimFile = new List<string>();
            SimFile = GenerateSimFileStandardText(SimFile, airport);
            #endregion
            
            #region Generate outbound flow
            string simroute;
            bool validEntry = false;
            Random rnd = new Random();
            if (outboundsactive == 1)
            {
                #region SID Distribution
                List<int> SIDSelectionDistribution = new List<int>();
                Console.WriteLine($"Enter SID selection/outbounddistribution key: i.e. 5:1:2:5. {airport.ICAO} needs {airport.SIDSRoute[airport.SelectedRunwayDirectionIndex].Count} numbers \n or type \"random\" for a random outbounddistribution");
                string _lastword = "";
                Console.WriteLine("Distribution numbers have to be in this order: \n");
                for (int i = 0; i < airport.SIDSRoute[airport.SelectedRunwayDirectionIndex].Count; i++)
                {
                    _lastword = airport.SIDSRoute[airport.SelectedRunwayDirectionIndex][i].Split(' ').Last();
                    if (i < airport.SIDSRoute[airport.SelectedRunwayDirectionIndex].Count - 1)
                    {
                        Console.Write(_lastword + ":");
                    }
                    else
                    {
                        Console.Write(_lastword + "\n");
                    }
                }
                // reading and computing outbounddistribution key

                string _distributionkey;
                
                List<int> outbounddistribution = new List<int>();
                do
                {
                    _distributionkey = Console.ReadLine();

                    string[] _subStrings = _distributionkey.Split(':');
                    foreach (string substring in _subStrings)
                    {
                        if (substring != ":")
                        {
                            Int32.TryParse(substring, out int _convertedInt);
                            outbounddistribution.Add(_convertedInt);
                            //Console.Write(_convertedInt + " ");
                        }
                    }
                    if (outbounddistribution.Count == airport.SIDSRoute[airport.SelectedRunwayDirectionIndex].Count)
                    {
                        validEntry = true;
                    }
                    if (validEntry == false)
                    {
                        Console.Write($"Wrong input, input has to look like this 5:1:3:4 -> It needs {airport.SIDSRoute[airport.SelectedRunwayDirectionIndex].Count} numbers, separated by : \n");
                        //Console.Write(outbounddistribution.Count);
                        outbounddistribution.Clear();
                    }
                } while (validEntry != true);

                #endregion
                
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
                foreach (int i in outbounddistribution)
                {
                    Console.Write($"{i}:");
                }
                List<int> SolutionSID = new List<int>();
                for(int i = 0; i < outbounddistribution.Count; i++)
                {
                    for (int j = 0; j < outbounddistribution[i]; j++)
                    {
                        SolutionSID.Add(i);
                    }
                }
                
                for (int i = 0; i < _duration; i += _outboundinterval)
                {
                    // Empty Line to separate each aircraft in .txt file
                    SimFile.Add("");

                    // random callsign from outbound list
                    
                    int RandomNumber = rnd.Next(Outbounds.Count);

                    // First line
                    SimFile.Add($"@N:{Outbounds[RandomNumber].Callsign}:{Squawks.AssignSquawk()}:1:{OutboundStartPosition}:{airport.Elevation}:0:{RWYHeading}:0");

                    // Select SID
                   
                    int randomnumber = rnd.Next(SolutionSID.Count);

                    int SelectedSID = SolutionSID[randomnumber];
                    Console.WriteLine(SelectedSID);
                    //1:2:3:4:5:6:7:8:9:10:11

                    simroute = airport.SIDSRoute[airport.SelectedRunwayDirectionIndex][SelectedSID];



                    SimFile.Add($"$FP{Outbounds[RandomNumber].Callsign}:*A:I:{Outbounds[RandomNumber].ATYP.Substring(0, Outbounds[RandomNumber].ATYP.Length - 2)}:420:{Outbounds[RandomNumber].DEP}:::0:{Outbounds[RandomNumber].DEST}:00:00:0:0:::{airport.SIDSFpln[airport.SelectedRunwayDirectionIndex][SelectedSID]}");
                    SimFile.Add($"SIMDATA:{Outbounds[RandomNumber].Callsign}:*:*:25:1:0");
                    SimFile.Add($"$ROUTE:{simroute}");
                    SimFile.Add($"START:{i}");
                    SimFile.Add($"DELAY:1:2");
                    SimFile.Add($"REQALT:5000");
                    SimFile.Add($"INITIALPSEUDOPILOT:{InitialPseudopilot}");
                }
            }
            #endregion
            ///////
            #region Generate inbound flow

            string InboundHeading = "";
            int STARorTRANS = 0;
            do
            {
                Console.WriteLine("Use 1. STAR or 2. TRANS?");
                
                erfolgreich = Int32.TryParse(Console.ReadLine(), out _auswahl);
                erfolgreich = erfolgreich == true && _auswahl <= 2 && _auswahl >= 1;
                STARorTRANS = _auswahl - 1;
                
            } while (!erfolgreich);

            #region STAR/TRANS distribution

            List<int> ArrivalDistribution = new List<int>();

            #region text settings

            string key = "";
            string distributionlengthstring = "";
            int distributionlengthint = 0;
            List<string>[] RouteList = { new List<string>(), new List<string>() };
            if(STARorTRANS == 0)
            {
                key = "STAR";
                distributionlengthint = airport.STARSRoute[airport.SelectedRunwayDirectionIndex].Count;
                distributionlengthstring = $"{distributionlengthint}";
                RouteList[0] = airport.STARSRoute[0];
                RouteList[1] = airport.STARSRoute[1];
            }
            else
            {
                key = "TRANS";
                distributionlengthint = airport.TRANSITIONRoute[airport.SelectedRunwayDirectionIndex].Count;
                distributionlengthstring = $"{distributionlengthint}";
                RouteList[0] = airport.TRANSITIONRoute[0];
                RouteList[1] = airport.TRANSITIONRoute[1];
            }

            #endregion

            Console.WriteLine($"Enter {key} outbounddistribution key: {airport.ICAO} needs {distributionlengthstring} numbers \n or type \"random\" for a random outbounddistribution");
            string _firstword = "";

            Console.WriteLine("Distribution numbers have to be in this order: \n");

            for (int i = 0; i < distributionlengthint; i++)
            {
                _firstword = RouteList[airport.SelectedRunwayDirectionIndex][i].Split(' ').First();
                if (i < RouteList[airport.SelectedRunwayDirectionIndex].Count - 1)
                {
                    Console.Write(_firstword + ":");
                }
                else
                {
                    Console.Write(_firstword + "\n");
                }
            }

            #endregion

            // reading and computing outbounddistribution key

            string _inbounddistributionkey = "";
            validEntry = false;

            List<int> distribution = new List<int>();

            do
            {
                _inbounddistributionkey = Console.ReadLine();

                string[] _subStrings = _inbounddistributionkey.Split(':');
                foreach(string substring in _subStrings)
                {
                    if(substring != ":")
                    {
                        Int32.TryParse(substring, out int _convertedInt);
                        ArrivalDistribution.Add(_convertedInt);
                    }
                }
                if(ArrivalDistribution.Count == RouteList[airport.SelectedRunwayDirectionIndex].Count)
                {
                    validEntry = true;
                }
                if(validEntry == false)
                {
                    Console.Write($"Wrong input, input has to look like this 5:1:3:4 -> It needs {distributionlengthstring} numbers, separated by : \n");
                    ArrivalDistribution.Clear();
                }

            } while (validEntry != true);


            // EVERYTHING BELOW IS NOT CHECKED

            


            


            simroute = "";
            string fplnroute = "";
            string InboundStartPosition = "";
            string InboundAltitude = "";
            List<int> SolutionSTAR = new List<int>();
            for (int i = 0; i < ArrivalDistribution.Count; i++)
            {
                for (int j = 0; j < ArrivalDistribution[i]; j++)
                {
                    SolutionSTAR.Add(i);
                }
            }
            // Debug
            foreach(int i in SolutionSTAR)
            {
                Console.Write($"{i} ");
            }

            for (int i = 0; i < _duration; i += _inboundinterval)
            {
                SimFile.Add("");

                // random callsign from inbound list
                int RNDintCallsign = rnd.Next(Inbounds.Count);

                // Select STAR / TRANS

                int RANDSTARorTRANSNumber = 0;
                

                int randomnumber = rnd.Next(SolutionSTAR.Count);

                RANDSTARorTRANSNumber = SolutionSTAR[randomnumber];

                if (STARorTRANS == 1)
                {
                    //RANDSTARorTRANSNumber = rnd.Next(airport.STARSRoute.Length); obsolete due to line 511?
                    simroute = airport.STARSRoute[airport.SelectedRunwayDirectionIndex][RANDSTARorTRANSNumber];
                    fplnroute = airport.STARSFpln[airport.SelectedRunwayDirectionIndex][RANDSTARorTRANSNumber];

                }
                else
                {
                    //RANDSTARorTRANSNumber = rnd.Next(airport.TRANSITIONRoute.Length); obsolete due to line 511?
                    simroute = airport.TRANSITIONRoute[airport.SelectedRunwayDirectionIndex][RANDSTARorTRANSNumber];
                    fplnroute = airport.TRANSITIONFpln[airport.SelectedRunwayDirectionIndex][RANDSTARorTRANSNumber];
                }
                Console.WriteLine(fplnroute);



                InboundStartPosition = airport.InboundStartPosition[RANDSTARorTRANSNumber];
                InboundAltitude = airport.InboundAltitude[RANDSTARorTRANSNumber];
                // First line
                // Fix spawn ALTITUDE -> should be fixed
                SimFile.Add($"@N:{Inbounds[RNDintCallsign].Callsign}:{Squawks.AssignSquawk()}:1:{InboundStartPosition}:{InboundAltitude}:0:{InboundHeading}:0");

                // Second line
                SimFile.Add($"$FP{Inbounds[RNDintCallsign].Callsign}:*:I:{Inbounds[RNDintCallsign].ATYP.Substring(0, Inbounds[RNDintCallsign].ATYP.Length - 2)}:420:{Inbounds[RNDintCallsign].DEP}:::0:{Inbounds[RNDintCallsign].DEST}:00:00:0:0:::{fplnroute}");

                // Rest of lines
                SimFile.Add($"SIMDATA:{Inbounds[RNDintCallsign].Callsign}:*:*:25:1:0");
                SimFile.Add($"$ROUTE:{simroute}");
                SimFile.Add($"START:{i}");
                SimFile.Add($"DELAY:1:2");
                SimFile.Add($"REQALT:{InboundAltitude}");
                SimFile.Add($"INITIALPSEUDOPILOT:{InitialPseudopilot}");

            }

            #endregion

            #region Output SimFile as txt

            InputOutput.ExportSimFile(SimFile);
            Console.ReadLine();
            #endregion
        }

        internal static List<string> GenerateSimFileStandardText(List<string> SimFile, Airport2 airport)
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
