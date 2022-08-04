using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroscopeSIMBib
{
    public class SIM
    {
        #region EDDL TRANS
        string DOMUX23 { get; set; } = "DOMUX DL402 DL403 DL404 DL405 DL406 DL407 DL408 DL409 DL410";
        string DOMUX05 { get; set; } = "DOMUX DL402 DL403 DL404 DL405 DL406 DL407 DL408 DL409 DL410";
        string HALME23 { get; set; } = "HALME XAMOD DL420 DL421 DL422 DL426 DL428 DL429 DL430";
        string HALME05 { get; set; } = "HALME BOMBA DL517 Dl518 DL519 GAPNU DL522 RONAD DL524 DL525 DL530";
        string BIKMU23 { get; set; } = "BIKMU DL426 DL428 DL429 DL430";
        string BIKMU05 { get; set; } = "BIKMU RONAD DL513 DL514 IBIKO GAPNU DL522 DL524 DL525 DL530";
        string PISAP23 { get; set; } = "PISAP AGEDA XAMOD DL420 DL421 DL422 DL426 DL428 DL429 DL430";
        string PISAP05 { get; set; } = "PISAP VALSU GAPNU DL522 RONAD DL524 DL525 DL530";
        #endregion
        #region EDDK TRANS

        #endregion
        #region EDDK STAR

        #endregion
        static int arrivalpoints { get; set; }
        public SIM(string airport)
        {
            if(airport == "EDDL")
            {
                arrivalpoints = 4;
            }
            else
            {
                arrivalpoints = 4;
            }
        }

        public static List<ACFT> CreateAircraft(List<string> Input)
        {
            List<ACFT> ACFTListe = new List<ACFT>();
            for (int i = 1; i < Input.Count; i++)
            {
                ACFT aktuellesFlugzeug = new ACFT();
                string[] subs = Input[i].Split(';');

                aktuellesFlugzeug.Callsign = subs[0];
                aktuellesFlugzeug.ADEP = subs[1];
                aktuellesFlugzeug.ADES = subs[2];
                aktuellesFlugzeug.TypeCat = subs[3];
                ACFTListe.Add(aktuellesFlugzeug);
            }


            return ACFTListe;
        }

        public static List<string> CreateSimFile(List<ACFT> Input)
        {
            List<ACFT> Outbounds = new List<ACFT>();
            List<ACFT> Inbounds = new List<ACFT>();
            /* Outbounds nach Arrival sortieren */
            for (int i = 0; i < Input.Count; i++)
            {
                if (Input[i].ADEP == "EDDK" || Input[i].ADEP == "EDDL")
                {
                    Outbounds.Add(Input[i]);
                }
                else
                {
                    Inbounds.Add(Input[i]);
                }
            }
            Int32.TryParse(Console.ReadLine(), out int Abstand);
            List<string> SimFile = new List<string>();
            #region Standard TXT

            SimFile.Add("PSEUDOPILOT:ALL");
            SimFile.Add("");

            SimFile.Add("AIRPORT_ALT:302.0");
            SimFile.Add("");

            SimFile.Add("ILS24:50.9336103:7.3699104:50.8695234:7.1548344");
            SimFile.Add("ILS06:50.8162496:6.9815123:50.8612187:7.1274431");
            SimFile.Add("ILS32R:50.8551957:7.1657231:50.8804612:7.1290705");
            SimFile.Add("ILS32L:50.8585091:7.1387503:50.8708337:7.1208554");
            SimFile.Add("");

            SimFile.Add("HOLDING:WYP:261:-1");
            SimFile.Add("HOLDING:COL:298:-1");
            SimFile.Add("HOLDING:KBO:140:1");
            SimFile.Add("HOLDING:NVO:68:1");
            SimFile.Add("HOLDING:ELDAR:356:-1");
            SimFile.Add("HOLDING: ERUKI:93:1");
            SimFile.Add("HOLDING:GULKO:276:-1");
            SimFile.Add("HOLDING:KOPAG:205:-1");
            SimFile.Add("");

            SimFile.Add("PSEUDOPILOT:ALL");
            SimFile.Add("CONTROLLER:UNICOM:122.800");
            SimFile.Add("CONTROLLER:EDDL_APP:128.550");
            SimFile.Add("PSEUDOPILOT:ALL");
            SimFile.Add("CONTROLLER:EDDG_APP:129.300");
            SimFile.Add("PSEUDOPILOT:ALL");
            SimFile.Add("CONTROLLER:EDGG_P_CTR:135.650");
            SimFile.Add("PSEUDOPILOT:ALL");
            SimFile.Add("CONTROLLER:EDDK_APP:135.350");
            SimFile.Add("PSEUDOPILOT:ALL");
            SimFile.Add("CONTROLLER:EDDK_TWR:124.875");
            SimFile.Add("PSEUDOPILOT:ALL");
            SimFile.Add("CONTROLLER:EDGG_G_CTR:124.725");
            SimFile.Add("PSEUDOPILOT:ALL");
            SimFile.Add("CONTROLLER:EBBU_E_CTR:131.100");
            #endregion
            for (int i = 0; i <= 120; i += Abstand)
            {
                SimFile.Add("");
                /* Erste Zeile: */
                SimFile.Add("PSEUDOPILOT:ALL");

                /* Zweite Zeile: Random CS */
                Random rnd = new Random();
                int r = rnd.Next(Inbounds.Count);

                Console.WriteLine($"{Inbounds[r].ADEP}: Welche STAR? 1 KOPAG \n 2 ERNEP \n 3 GULKO \n 4 DEPOK");


                Int32.TryParse(Console.ReadLine(), out int STAR);
                // Zeile 1:
                string Wpt = "";
                string Lat = "";
                string Long = "";
                string Alt = "";
                string Heading = "";
                string simroute = "";
                string fplroute = "";

                if (STAR == 1)
                {
                    Wpt = "KOPAG";
                    Lat = "51.1560671";
                    Long = "8.2183844";
                    Alt = "11000";
                    Heading = "2848";

                    simroute = "KOPAG COL COL85 RARIX IKE44 ILS32R";
                    fplroute = "KOPAG KOPAG2C";
                }
                if (STAR == 2)
                {
                    Wpt = "ERNEP";
                    Lat = "50.9941231";
                    Long = "8.7143664";
                    Alt = "18000";
                    Heading = "3016";

                    simroute = "ERNEP COL COL85 RARIX IKE44 ILS32R";
                    fplroute = "ERNEP ERNEP1C";
                }
                if (STAR == 3)
                {
                    Wpt = "GULKO";
                    Lat = "50.6220895";
                    Long = "8.1815946";
                    Alt = "11000";
                    Heading = "3532";

                    simroute = "GULKO COL COL85 RARIX IKE44 ILS32R";
                    fplroute = "GULKO GULKO2C";
                }
                else
                {
                    Wpt = "DEPOK";
                    Lat = "50.4566440";
                    Long = "5.7003220";
                    Alt = "17000";
                    Heading = "3532";

                    simroute = "DEPOK KBO COL COL85 RARIX IKE44 ILS32R";
                    fplroute = "DEPOK DEPOK1C";
                }
                Squawks Squawkbox = new Squawks();
                SimFile.Add($"@N:{Inbounds[r].Callsign}:{Squawkbox.GetSquawk()}:1:{Lat}:{Long}:{Alt}:0:{Heading}:0");


                // Zweite Zeile:
                // 
                SimFile.Add($"$FP{Inbounds[r].Callsign}:*A:I:{Inbounds[r].TypeCat.Substring(0, Inbounds[r].TypeCat.Length - 2)}:420:{Inbounds[r].ADEP}:::0:{Inbounds[r].ADES}:00:00:0:0:::{fplroute}");

                // Dritte Zeile:
                // SIMDATA:<callsign>:<plane type>:<livery>:<maximum taxi speed>:<taxiway usage>:<object extent>
                SimFile.Add($"SIMDATA:{Inbounds[r].Callsign}:*:*:25:1:0");

                // Vierte Zeile:
                SimFile.Add($"$ROUTE:{simroute}");

                // Fünfte Zeile:
                SimFile.Add($"START:{i}");

                // Sechste Zeile:
                SimFile.Add($"DELAY:1:2");

                // Siebte Zeile:
                if(Wpt == "KOPAG")
                {
                    SimFile.Add($"REQALT:KOPAG:11000");
                }
                if(Wpt == "ERNEP")
                {
                    SimFile.Add($"REQALT:ERNEP:12000");
                }
                if(Wpt == "GULKO")
                {
                    SimFile.Add($"REQALT:GULKO:11000");
                }
                if(Wpt == "DEPOK")
                {
                    SimFile.Add($"REQALT:DEPOK:17000");
                }

                // Achte Zeile:
                SimFile.Add($"INITALPSEUDOPILOT:EDDK_M_APP");



                Inbounds.Remove(Inbounds[r]);
            }

            return SimFile;
        }
    }
}
