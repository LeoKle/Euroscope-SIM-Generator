using ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Deployment.Internal;
using System.Xml.Linq;

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
        private static List<string> ScenarioFile = new List<string>();
        private static List<int> _distribution = new List<int>();

        private static Airport _selectedairport;

        private static List<ACFT> AircraftList = new List<ACFT>();
        private static List<ACFT> Outbounds = new List<ACFT>();
        private static List<ACFT> Inbounds = new List<ACFT>();
        private static Random Random = new Random();

        public static void GenerateSim()
        {
            InputOutput.CheckFolder();
            InputOutput.ReadJSON(out List<Airport> airports);

            PresentandSelectAirport(airports);
            Console.WriteLine();
            SelectAirportConfig(_selectedairport);
            Console.WriteLine();
            CreateAircraft();
            DurationAndIntervalSelection();
            Console.WriteLine();
            EuroScopeScenarioSettings();
            Console.WriteLine();
            GenerateScenarioFileStandardText();
            Console.WriteLine();
            GenerateOutbounds();
            Console.WriteLine();
            GenerateInbounds();
            Console.WriteLine();
            InputOutput.ExportSimFile(ScenarioFile);
            Console.WriteLine("Program end");
            Console.ReadLine();
        }

        private static void GenerateInbounds()
        {
            if (_inboundseparation != 0)
            {
                #region STAR or Transition selection
                bool validEntry = false;
                int STARorTRANS = 0;
                do
                {
                    Console.WriteLine("Use 1. STAR or 2. TRANS?");

                    validEntry = Int32.TryParse(Console.ReadLine(), out int _selection);
                    Console.WriteLine();
                    validEntry = validEntry == true && _selection <= 2 && _selection >= 1;
                    STARorTRANS = _selection - 1;
                } while (validEntry == false);
                #endregion

                #region distribution selection

                string _SIDorSTAR = "";
                //int _distributionlength = 0;
                string[][] RouteList = new string[1][];
                string[][] FlightplanList = new string[1][];
                string[][] AltitudeList = new string[1][];
                if (STARorTRANS == 0)
                {
                    _SIDorSTAR = "STAR";
                    //_distributionlength = _airport.STARSroute.GetLength(_runwaydirectionindex);
                    RouteList = _selectedairport.STARSroute;
                    FlightplanList = _selectedairport.STARSflightplan;
                    AltitudeList = _selectedairport.STARSAltitude;
                }
                else
                {
                    _SIDorSTAR = "TRANS";
                    //_distributionlength = _airport.TRANSITIONroute.GetLength(_runwaydirectionindex);
                    RouteList = _selectedairport.TRANSITIONroute;
                    FlightplanList = _selectedairport.TRANSITIONflightplan;
                    AltitudeList = _selectedairport.TRANSITIONAltitude;
                }

                DistributionInput(FlightplanList, _SIDorSTAR);

                #endregion

                #region generation of inbounds

                string simroute = "";
                string fplnroute = "";
                string InboundStartPosition = "";
                string InboundAltitude = "";
                string InitialHeading = "";
                for (int i = 0; i < _duration; i+=_inboundseparation)
                {
                    ScenarioFile.Add("");
                    // Get random callsign from inbound list
                    int CallsignNumber = Random.Next(Inbounds.Count());
                    // Assign random star / transition
                    int ArrivalSelection = Random.Next(_distribution.Count);
                    ArrivalSelection = _distribution[ArrivalSelection];


                    simroute = RouteList[_runwaydirectionindex][ArrivalSelection];
                    fplnroute = FlightplanList[_runwaydirectionindex][ArrivalSelection];
                    InboundStartPosition = _selectedairport.ArrivalStartPosition[ArrivalSelection];
                    InboundAltitude = AltitudeList[_runwaydirectionindex][ArrivalSelection];
                    InitialHeading = ConvertHeadingtoEuroscopeHeadingString(_selectedairport.ArrivalInitialHeading[ArrivalSelection]);
                    string firstSIDpointname = GetFirstPointofArrival(fplnroute);

                    ScenarioFile.Add($"@N:{Inbounds[CallsignNumber].Callsign}:1000:1:{InboundStartPosition}:{InboundAltitude}:0:{InitialHeading}:0");
                    ScenarioFile.Add($"$FP{Inbounds[CallsignNumber].Callsign}:*A:I:{Inbounds[CallsignNumber].ATYP.Substring(0, Inbounds[CallsignNumber].ATYP.Length - 2)}:420:{Inbounds[CallsignNumber].DEP}:::350:{Inbounds[CallsignNumber].DEST}:00:00:0:0:::{fplnroute}");
                    ScenarioFile.Add($"SIMDATA:{Inbounds[CallsignNumber].Callsign}:*:*:25:1:0");
                    ScenarioFile.Add($"$ROUTE:{simroute}");
                    ScenarioFile.Add($"START:{i}");
                    ScenarioFile.Add("DELAY:4:10");
                    ScenarioFile.Add($"REQALT:{firstSIDpointname}:{InboundAltitude}");
                    ScenarioFile.Add($"INITIALPSEUDOPILOT:{_selectedairport.Pseudopilots[_selectedPseudopilot]}");

                    Inbounds.RemoveAt(CallsignNumber);
                }

                #endregion
            }
        }

        private static string GetFirstPointofArrival(string fplnroute)
        {
            string _name = fplnroute.Split(' ').First();
            return _name;
        }

        private static void GenerateOutbounds()
        {
            if(_outboundseparation != 0)
            {
                #region distribution

                DistributionInput(_selectedairport.SIDSflightplan, "SID");


                #endregion

                #region Convert airport runway heading to ES specific runway heading
                string _runwayheading = ConvertHeadingtoEuroscopeHeadingString(_selectedairport.RunwayHeadings[_runwaydirectionindex]);
                #endregion

                #region generation
                string simroute;
                string flightplan;
                for (int i = 0; i < _duration; i += _outboundseparation)
                {
                    // Empty line for readability
                    ScenarioFile.Add("");

                    #region selecting random callsign from outbound list
                    int CallsignNumber = Random.Next(Outbounds.Count());

                    #endregion
                    // ScenarioFile.Add($"@N:{Outbounds[CallsignNumber].Callsign}:1000:1:{_outboundstartposition}:{_selectedairport.Elevation}:0:{_runwayheading}:0");
                    ScenarioFile.Add($"@N:{Outbounds[CallsignNumber].Callsign}:1000:1:{_outboundstartposition}:2000:0:{_runwayheading}:0");

                    int SIDnumber = Random.Next(_distribution.Count);
                    SIDnumber = _distribution[SIDnumber];

                    simroute = _selectedairport.SIDSroute[_runwaydirectionindex][SIDnumber];
                    flightplan = _selectedairport.SIDSflightplan[_runwaydirectionindex][SIDnumber];
                    
                    ScenarioFile.Add($"$FP{Outbounds[CallsignNumber].Callsign}:*A:I:{Outbounds[CallsignNumber].ATYP.Substring(0, Outbounds[CallsignNumber].ATYP.Length - 2)}:420:{Outbounds[CallsignNumber].DEP}:::350:{Outbounds[CallsignNumber].DEST}:00:00:0:0:::{flightplan}");
                    ScenarioFile.Add($"SIMDATA:{Outbounds[CallsignNumber].Callsign}:*:*:25:1:0");
                    ScenarioFile.Add($"$ROUTE:{simroute}");
                    ScenarioFile.Add($"START:{i}");
                    ScenarioFile.Add($"DELAY:1:2");
                    ScenarioFile.Add($"REQALT:5000");
                    ScenarioFile.Add($"INITIALPSEUDOPILOT:{_selectedairport.Pseudopilots[_selectedPseudopilot]}");

                    Outbounds.RemoveAt(CallsignNumber);
                }

                #endregion
            }
        }

        private static void WriteDepartureorArrivalNames(string[][] SIDorSTARflightplanarray, string IsSIDorSTAR)
        {
            string _name;
            for (int i = 0; i < SIDorSTARflightplanarray[_runwaydirectionindex].Length; i++)
            {
                if (IsSIDorSTAR == "SID")
                {
                    _name = SIDorSTARflightplanarray[_runwaydirectionindex][i].Split(' ').Last();
                }
                else
                {
                    _name = SIDorSTARflightplanarray[_runwaydirectionindex][i].Split(' ').First();
                }
                if (i < SIDorSTARflightplanarray[_runwaydirectionindex].Length - 1)
                {
                    Console.Write(_name + ":");
                }
                else
                {
                    Console.Write(_name + "\n");
                }
            }
        }

        private static void DistributionInput(string[][] SIDorSTARflightplanarray, string IsSIDorSTAR)
        {
            _distribution = new List<int>();
            List<int> _distributionkey = new List<int>();
            string _input = "";
            bool _validEntry = false;
            do
            {
                Console.WriteLine($"Enter {IsSIDorSTAR} distribution key: i.e. 5:1:2:5");
                Console.WriteLine($"{_selectedairport.ICAO} needs {SIDorSTARflightplanarray[_runwaydirectionindex].Length} numbers");
                Console.WriteLine("or type \"random\" for a random distribution");
                Console.WriteLine("Distribution numbers have to be in this order: ");

                WriteDepartureorArrivalNames(SIDorSTARflightplanarray, IsSIDorSTAR);

                _input = Console.ReadLine();

                if (_input.ToUpper() != "RANDOM")
                {
                    string[] _subStrings = _input.Split(':');
                    foreach (string substring in _subStrings)
                    {
                        bool _isNumber = Int32.TryParse(substring, out int _convertedInt);
                        if (_isNumber == true)
                        {
                            _distributionkey.Add(_convertedInt);
                        }
                    }
                    if (_distributionkey.Count != SIDorSTARflightplanarray[_runwaydirectionindex].Length)
                    {
                        _validEntry = false;
                        _distributionkey.Clear();
                    }
                    else
                    {
                        _validEntry = true;
                    }
                }
                else
                {
                    _validEntry = true;
                }
            } while (_validEntry == false);

            if (_input.ToUpper() == "RANDOM")
            {
                // SIDorSTARflightplanarray.GetLength(_runwaydirectionindex)
                for (int i = 0; i < SIDorSTARflightplanarray[_runwaydirectionindex].Length; i++)
                {
                    _distribution.Add(1);
                }
            }
            else
            {
                for (int i = 0; i < _distributionkey.Count; i++)
                {
                    for (int j = 0; j < _distributionkey[i]; j++)
                    {
                        _distribution.Add(i);
                    }
                }
            }
        }

        private static string ConvertHeadingtoEuroscopeHeadingString(int heading)
        {
            string _headingstring = ((int)((heading * 2.88 + 0.5) * 4)).ToString();
            return _headingstring;
        }

        private static void GenerateScenarioFileStandardText()
        {
            #region ILS definition

            if (_selectedairport.ILSDefinitions.Length == 1)
            {
            for (int i = 0; i < 4; i++)
            {
                    ScenarioFile.Add($"{_selectedairport.ILSDefinitions[0][i]}");
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    ScenarioFile.Add($"{_selectedairport.ILSDefinitions[_runwaydirectionindex][i]}");
                }
            }

            #endregion

            #region Holding definitions

            for (int i = 0; i < _selectedairport.HoldingDefinitions.Length; i++)
            {
                ScenarioFile.Add($"{_selectedairport.HoldingDefinitions[i]}");
            }

            #endregion

            #region airport elevation

            ScenarioFile.Add($"AIRPORT_ALT:{_selectedairport.Elevation}");

            #endregion
        }

        private static void EuroScopeScenarioSettings()
        {
            #region Selection of Pseudopilot

            Console.WriteLine("Which station should control the aircraft");
            for (int i = 0; i < _selectedairport.Pseudopilots.Count(); i++)
            {
                Console.WriteLine($"{i + 1}. {_selectedairport.Pseudopilots[i]}");
            }
            bool _erfolgreich;
            do
            {
                _erfolgreich = Int32.TryParse(Console.ReadLine(), out _selectedPseudopilot);
                _selectedPseudopilot--;
                _erfolgreich = _erfolgreich == true && _selectedPseudopilot < _selectedairport.Pseudopilots.Count() && _selectedPseudopilot >= 0;
            } while (!_erfolgreich);

            #endregion

            #region ground or airstart

            if(_outboundseparation > 0)
            {
                do
                {
                    Console.WriteLine("Should outbounds start on 1) ground or 2) in air");
                    Int32.TryParse(Console.ReadLine(), out _groundorairstart);
                } while (_groundorairstart != 1 && _groundorairstart != 2);
                if (_groundorairstart == 1)
                {
                    Console.WriteLine("Outbound aircraft are starting on ground and have to be commanded to takeoff manually");
                    if (_runwaydirectionindex == 1)
                    {
                        _outboundstartposition = _selectedairport.DeparturePositionGroundStart[1];
                    }
                    else
                    {
                        _outboundstartposition = _selectedairport.DeparturePositionGroundStart[0];
                    }
                    _outboundinitialaltitude = _selectedairport.Elevation;
                }
                else
                {
                    Console.WriteLine("Outbound aircraft are starting in air");
                    if (_runwaydirectionindex == 1)
                    {
                        _outboundstartposition = _selectedairport.DeparturePositionAirStart[1];
                    }
                    else
                    {
                        _outboundstartposition = _selectedairport.DeparturePositionAirStart[0];
                    }
                    _outboundinitialaltitude = "2000";
                }
            }

            #endregion
        }

        private static void DurationAndIntervalSelection()
        {
            bool _limitreached;
            do
            {
                _limitreached = false;
                Console.WriteLine("Select sim duration in minutes");
                bool _durationbool = Int32.TryParse(Console.ReadLine(), out _duration);
                Console.WriteLine();
                Console.WriteLine("Select inbound separation in minutes \nSelect 0 for none");
                bool _inboundseparationbool = Int32.TryParse(Console.ReadLine(), out _inboundseparation);
                Console.WriteLine();
                Console.WriteLine("Select outbound separation in minutes \nSelect 0 for none");
                bool _outboundseparationbool = Int32.TryParse(Console.ReadLine(), out _outboundseparation);
                Console.WriteLine();
                if (_durationbool && _inboundseparationbool && _outboundseparationbool)
                {
                    if (_inboundseparation != 0)
                    {
                        int _inboundaircraftcount = _duration / _inboundseparation;
                        if (_inboundaircraftcount > Inbounds.Count())
                        {
                            Console.WriteLine("You want to create more aircraft than callsigns are availabe for inbounds");
                            _limitreached = true;
                        }
                    }
                    if (_outboundseparation != 0)
                    {
                        int _outboundaircraftcount = _duration / _outboundseparation;
                        if (_outboundaircraftcount > Outbounds.Count())
                        {
                            _limitreached = true;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error: sim duration and inbound separation have to be numbers \n");
                    _limitreached = true;
                }

            } while (_limitreached);
        }

        private static void CreateAircraft()
        {
            List<string> _csvinput = InputOutput.ImportFlightData(_selectedairport.ICAO.ToLower());
            for(int i = 1; i < _csvinput.Count; i++)
            {
                string[] _subStrings = _csvinput[i].Split(';');
                ACFT _currentAircraft = new ACFT(_subStrings[0], _subStrings[1], _subStrings[2], _subStrings[3], "1000");
                AircraftList.Add(_currentAircraft);
            }

            for (int i = 0; i < AircraftList.Count; i++)
            {
                if (AircraftList[i].DEP == _selectedairport.ICAO)
                {
                    Outbounds.Add(AircraftList[i]);
                }
                else
                {
                    Inbounds.Add(AircraftList[i]);
                }
            }
        }

        private static void PresentandSelectAirport(List<Airport> airports)
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
            _selectedairport = airports[_input-1];
        }

        private static void SelectAirportConfig(Airport airport)
        {
            int _input = 0;
            do
            {
                int i = 1;
                foreach (string config in airport.RunwayConfigText)
                {
                    Console.WriteLine($"{i}. {config}");
                    i++;
                }
                Console.WriteLine("Select config:");
                Int32.TryParse(Console.ReadLine(), out _input);
            } while (_input > airport.RunwayConfigText.Length || _input < 1);
            _runwaydirectionindex = _input - 1;
        }
    }
}
