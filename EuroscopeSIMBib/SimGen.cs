using ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroscopeSIMBib
{
    public class SimGen
    {
        private int _duration;
        private int _inboundseparation;
        private int _outboundseparation;
        private int _groundorairstart;
        private string _outboundstartposition;
        private string _outboundinitialaltitude;
        private int _runwaydirectionindex;
        private int _selectedPseudopilot;
        private List<string> SimFile = new List<string>();

        private AirportJson _airport = new AirportJson();

        private List<ACFT> AircraftList = new List<ACFT>();
        private List<ACFT> Outbounds = new List<ACFT>();
        private List<ACFT> Inbounds = new List<ACFT>();

        private Random Random = new Random();
        private List<int> OutboundDistribution = new List<int>();
        private List<int> ArrivalDistribution = new List<int>();
        private static List<ACFT> CreateAircraft(List<string> Input)
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

        public void GenerateSIM()
        {
            AirportSelection(out string ICAO, out _runwaydirectionindex);
            // serialize _airport
            AircraftList = CreateAircraft(InputOutput.ImportFlightData(ICAO));
            SplitAircraftList(ICAO, AircraftList, out Outbounds, out Inbounds);
            DurationAndIntervalSelection(Outbounds.Count, Inbounds.Count, out _duration);
            ESSimSettings();
            GenerateSimFileStandardText();
            GenerateOutbounds();
            GenerateInbounds();
        }

        private void AirportSelection(out string ICAO, out int RunwayDirectionIndex)
        {
            ICAO = "";
            RunwayDirectionIndex = 0;
            string _input;
            do
            {
                Console.WriteLine("For which airport do you want to create a Euroscope Scenario file?\n 1. EDDL \n or \n 2. EDDK");
                Console.WriteLine("Input 1 or 2");
                _input = Console.ReadLine();
            } while (_input != "1" && _input != "2");

            string _runwaydirection;
            if (_input == "1")
            {
                ICAO = "EDDL";
                do
                {
                    Console.WriteLine("Which runway direction?\n 23 or 05");
                    _runwaydirection = Console.ReadLine();
                    if(_runwaydirection == "23")
                    {
                        RunwayDirectionIndex = 0;
                    }
                    else if (_runwaydirection == "05")
                    {
                        RunwayDirectionIndex = 1;
                    }

                } while (RunwayDirectionIndex != 0 && RunwayDirectionIndex != 1);
            }
            else if(_input == "EDDK")
            {
                ICAO = "EDDK";
                do
                {
                    Console.WriteLine("Which runway direction?\n 14 or 32");
                    _runwaydirection = Console.ReadLine();
                    if (_runwaydirection == "14")
                    {
                        RunwayDirectionIndex = 0;
                    }
                    else if (_runwaydirection == "32")
                    {
                        RunwayDirectionIndex = 1;
                    }
                } while (RunwayDirectionIndex != 0 && RunwayDirectionIndex != 1);
            }
            else
            {
                throw new ArgumentException("ArgumentException at AirportSelection()");
            }
        }

        private void SplitAircraftList(string ICAO, List<ACFT> AircraftList, out List<ACFT> Outbounds, out List<ACFT> Inbounds)
        {
            Outbounds = new List<ACFT>();
            Inbounds = new List<ACFT>();

            for (int i = 0; i < AircraftList.Count; i++)
            {
                if (AircraftList[i].DEP == ICAO)
                {
                    Outbounds.Add(AircraftList[i]);
                }
                else
                {
                    Inbounds.Add(AircraftList[i]);
                }
            }
        }

        private void DurationAndIntervalSelection(int OutboundCount, int InboundCount, out int _duration)
        {
            bool _limitreached;
            do
            {
                _limitreached = false;
                Console.WriteLine("Select sim duration in minutes");
                bool _durationbool = Int32.TryParse(Console.ReadLine(), out _duration);

                Console.WriteLine("Select inbound separation in minutes \nSelect 0 for none");
                bool _inboundseparationbool = Int32.TryParse(Console.ReadLine(), out _inboundseparation);

                Console.WriteLine("Select outbound separation in minutes \nSelect 0 for none");
                bool _outboundseparationbool = Int32.TryParse(Console.ReadLine(), out _outboundseparation);

                if (_durationbool && _inboundseparationbool && _outboundseparationbool)
                {
                    if(_inboundseparation != 0)
                    {
                        int _inboundaircraftcount = _duration / _inboundseparation;
                        if (_inboundaircraftcount > InboundCount)
                        {
                            Console.WriteLine("You want to create more aircraft than callsigns are availabe for inbounds");
                            _limitreached = true;
                        }
                    }
                    if(_outboundseparation != 0)
                    {
                        int _outboundaircraftcount = _duration / _outboundseparation;
                        if (_outboundaircraftcount > InboundCount)
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
    
        private void ESSimSettings()
        {
            #region selection of pseudopilot

            Console.WriteLine("Which station should control the aircrafts");
            for(int i = 0; i < _airport.Pseudopilots.Count(); i++)
            {
                Console.WriteLine($"{i + 1}. {_airport.Pseudopilots[i]}");
            }
            bool _erfolgreich;
            do
            {
                _erfolgreich = Int32.TryParse(Console.ReadLine(), out _selectedPseudopilot);
                _selectedPseudopilot--;
                _erfolgreich = _erfolgreich == true && _selectedPseudopilot < _airport.Pseudopilots.Count() && _selectedPseudopilot >= 0;
            } while (!_erfolgreich);

            #endregion

            if (_outboundseparation > 0)
            {
                #region ground or air start
                do
                {
                    Console.WriteLine("Should outbounds start on 1) ground or 2) in air");
                    Int32.TryParse(Console.ReadLine(), out _groundorairstart);
                } while (_groundorairstart != 1 && _groundorairstart != 2);
                if(_groundorairstart == 1)
                {
                    Console.WriteLine("Outbound aircraft are starting on ground and have to be commanded to takeoff manually");
                    if (_runwaydirectionindex == 1)
                    {
                        _outboundstartposition = _airport.DeparturePositionGroundStart[1];
                    }
                    else
                    {
                        _outboundstartposition = _airport.DeparturePositionGroundStart[0];
                    }
                    _outboundinitialaltitude = _airport.Elevation;
                }
                else
                {
                    Console.WriteLine("Outbound aircraft are starting in air");
                    if (_runwaydirectionindex == 1)
                    {
                        _outboundstartposition = _airport.DeparturePositionAirStart[1];
                    }
                    else
                    {
                        _outboundstartposition = _airport.DeparturePositionAirStart[0];
                    }
                    _outboundinitialaltitude = "2000";
                }
                #endregion
            }
        }

        private void GenerateSimFileStandardText()
        {
            #region ILS/runway definition

            for(int i = 0; i < 4; i++)
            {
                SimFile.Add($"{_airport.ILSDefinitions[_runwaydirectionindex, i]}");
            }

            #endregion

            #region Holding definitions

            for(int i = 0; i < _airport.HoldingDefinitions.Length; i++)
            {
                SimFile.Add($"{_airport.HoldingDefinitions[i]}");
            }

            #endregion

            SimFile.Add($"AIRPORT_ALT:{_airport.Elevation}");
        }

        private string ConvertHeadingtoEuroscopeHeadingString(int heading)
        {
            string _headingstring = ((int)((heading * 2.88 + 0.5) * 4)).ToString();
            return _headingstring;
        }

        private void GenerateOutbounds()
        {
            if(_outboundseparation != 0)
            {
                #region distribution

                OutboundDistribution = DistributionInput(_airport.SIDSflightplan, "SID");


                #endregion

                #region Convert airport runway heading to ES specific runway heading
                string _runwayheading = ConvertHeadingtoEuroscopeHeadingString(_airport.RunwayHeadings[_runwaydirectionindex]);
                #endregion

                #region generation
                string simroute;
                string flightplan;
                for (int i = 0; i < _duration; i += _outboundseparation)
                {
                    // Empty line for readability
                    SimFile.Add("");

                    #region selecting random callsign from outbound list
                    int CallsignNumber = Random.Next(Outbounds.Count());

                    #endregion

                    SimFile.Add($"@N:{Outbounds[CallsignNumber].Callsign}:{Squawks.AssignSquawk()}:1:{_outboundstartposition}:{_airport.Elevation}:0:{_runwayheading}:0");

                    int SIDnumber = Random.Next(OutboundDistribution.Count);
                    SIDnumber = OutboundDistribution[SIDnumber];

                    simroute = _airport.SIDSroute[_runwaydirectionindex, SIDnumber];
                    flightplan = _airport.SIDSflightplan[_runwaydirectionindex, SIDnumber];

                    SimFile.Add($"$FP{Outbounds[CallsignNumber].Callsign}:*A:I:{Outbounds[CallsignNumber].ATYP.Substring(0, Outbounds[CallsignNumber].ATYP.Length - 2)}:420:{Outbounds[CallsignNumber].DEP}:::0:{flightplan}");
                    SimFile.Add($"SIMDATA:{Outbounds[CallsignNumber].Callsign}:*:*:25:1:0");
                    SimFile.Add($"$ROUTE:{simroute}");
                    SimFile.Add($"START:{i}");
                    SimFile.Add($"DELAY:1:2");
                    SimFile.Add($"REQALT:5000");
                    SimFile.Add($"INITIALPSEUDOPILOT:{_selectedPseudopilot}");

                    Outbounds.RemoveAt(CallsignNumber);
                }
                
                #endregion
            }
        }

        private void GenerateInbounds()
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
                    validEntry = validEntry == true && _selection <= 2 && _selection >= 1;
                    STARorTRANS = _selection - 1;
                } while (validEntry == false);
                #endregion

                #region distribution selection

                string _SIDorSTAR = "";
                //int _distributionlength = 0;
                string[,] RouteList = new string[_airport.RunwayDirections[0].Count(), _airport.STARSroute.GetLength(_runwaydirectionindex)];
                string[,] FlightplanList = new string[_airport.RunwayDirections[0].Count(), _airport.STARSroute.GetLength(_runwaydirectionindex)];
                string[,] AltitudeList = new string[_airport.RunwayDirections[0].Count(), _airport.STARSroute.GetLength(_runwaydirectionindex)];
                if (STARorTRANS == 0)
                {
                    _SIDorSTAR = "STAR";
                    //_distributionlength = _airport.STARSroute.GetLength(_runwaydirectionindex);
                    RouteList = _airport.STARSroute;
                    FlightplanList = _airport.STARSflightplan;
                    AltitudeList = _airport.STARSArrivalAltitude;
                }
                else
                {
                    _SIDorSTAR = "TRANS";
                    //_distributionlength = _airport.TRANSITIONroute.GetLength(_runwaydirectionindex);
                    RouteList = _airport.TRANSITIONroute;
                    FlightplanList = _airport.TRANSITIONflightplan;
                    AltitudeList = _airport.TRANSITIONArrivalAltitude;
                }

                ArrivalDistribution = DistributionInput(FlightplanList, _SIDorSTAR);

                #endregion

                #region generation of inbounds

                string simroute = "";
                string fplnroute = "";
                string InboundStartPosition = "";
                string InboundAltitude = "";
                string InitialHeading = "";
                for(int i = 0; i < _duration ; i++)
                {
                    SimFile.Add("");
                    // Get random callsign from inbound list
                    int CallsignNumber = Random.Next(Inbounds.Count());
                    // Assign random star / transition
                    int ArrivalSelection = Random.Next(OutboundDistribution.Count);
                    ArrivalSelection = OutboundDistribution[ArrivalSelection];


                    simroute = RouteList[_runwaydirectionindex, ArrivalSelection];
                    fplnroute = FlightplanList[_runwaydirectionindex, ArrivalSelection];
                    InboundStartPosition = _airport.ArrivalStartPosition[ArrivalSelection];
                    InboundAltitude = AltitudeList[_runwaydirectionindex, ArrivalSelection];
                    InitialHeading = ConvertHeadingtoEuroscopeHeadingString(_airport.ArrivalInitialHeading[ArrivalSelection]);

                    SimFile.Add($"@N:{Inbounds[CallsignNumber].Callsign}:{Squawks.AssignSquawk()}:1:{InboundStartPosition}:{InboundAltitude}:0:{InitialHeading}");
                }

                #endregion
            }
        }

        private List<int> DistributionInput(string[,] SIDorSTARflightplanarray, string IsSIDorSTAR)
        {
            List<int> _distributionkey = new List<int>();
            List<int> _distribution = new List<int>();
            string _input = Console.ReadLine();
            bool _validEntry = false;
            do
            {
                Console.WriteLine($"Enter {IsSIDorSTAR} distribution key: i.e. 5:1:2:5");
                Console.WriteLine($"{_airport.ICAO} needs {SIDorSTARflightplanarray.GetLength(_runwaydirectionindex)} numbers");
                Console.WriteLine("or type \"random\" for a random distribution");
                Console.WriteLine("Distribtuon numbers have to be in this order: ");

                WriteDepartureorArrivalName(SIDorSTARflightplanarray, IsSIDorSTAR);

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
                    if (_distributionkey.Count != SIDorSTARflightplanarray.GetLength(_runwaydirectionindex))
                    {
                        _validEntry = false;
                        _distributionkey.Clear();
                    }
                    else
                    {
                        _validEntry = true;
                    }
                }
            } while (_validEntry == false);

            if(_input.ToUpper() == "RANDOM")
            {
                for (int i = 0; i < SIDorSTARflightplanarray.GetLength(_runwaydirectionindex); i++)
                {
                    _distribution.Add(1);
                }
            }
            else
            {
                for (int i = 0; i < _distributionkey.Count; i++)
                {
                    _distribution.Add(_distributionkey[i]);
                }
            }

            return _distributionkey;
        }

        private void WriteDepartureorArrivalName(string[,] SIDorSTARflightplanarray, string IsSIDorSTAR)
        {
            string _name;
            for (int i = 0; i < SIDorSTARflightplanarray.GetLength(_runwaydirectionindex); i++)
            {
                if (IsSIDorSTAR == "SID")
                {
                    _name = SIDorSTARflightplanarray[_runwaydirectionindex, i].Split(' ').Last();
                }
                else
                {
                    _name = SIDorSTARflightplanarray[_runwaydirectionindex, i].Split(' ').First();
                }
                if (i < SIDorSTARflightplanarray.GetLength(_runwaydirectionindex) - 1)
                {
                    Console.Write(_name + ":");
                }
                else
                {
                    Console.Write(_name + "\n");
                }
            }
        }
    }
}
