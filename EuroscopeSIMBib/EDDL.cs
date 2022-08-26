using ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroscopeSIMGen
{
    internal class EDDL : IAirport
    {
        public string ICAO { get; } = "EDDL";
        public string[] RunwayDirections { get; } = { "23", "05" };
        public int SelectedRunwayDirectionIndex { get; } = 0;
        public string SelectedRunwayDirection { get; }
        public List<string>[] SIDS { get; set; }
        public List<string>[] STARS { get; set; }
        public List<string>[] TRANSITION { get; set; }
        #region EDDL SIDS 05 
        internal string COL4Z = "DL050 DL058 BAM LIPMI COL";
        internal string DODEN6Z = "DL050 DL058 BAM ELBAL BETZO DODEN";
        internal string GMH5Z = "DL050 DL058 BAM ANAVI GMH";
        internal string KUMIK4Z = "DL050 DL058 BAM ANAVI DEGOM KUMIK";
        internal string LMA1Z = "DL050 DL051 DL052 LMA";
        internal string MEVEL9Z = "DL050 DL053 NIKOG LUSIX MEVEL";
        internal string MODRU6Z = "DL050 DL053 NIKOG ORSOV VEBAK NETEX MODRU";
        internal string NETEX3Z = "DL050 DL053 NIKOG ORSOV VEBAK NETEX";
        internal string NVO2Z = "DL050 DL058 BAM NVO";
        internal string NUDGO4Z = "DL050 DL058 BAM ANAVI MAMIB KULIX NUDGO";
        internal string SONEB5Z = "DL050 DL053 NIKOG LUSIX DL054 SONEB";
        #endregion
        #region EDDL SIDS 23
        internal string LMA2T = "DL236 LMA";
        internal string COL5T = "DL239 DL241 DL244 LIPMI COL";
        internal string DODEN9T = "DL239 DL241 DL242 ELBAL BETZO DODEN";
        internal string GMH9T = "DL239 DL241 ELBAL GMH";
        internal string KUMIK6T = "DL239 DL241 ELBAL DEGOM KUMIK";
        internal string MEVEL3T = "DL230 DL235 DL248 ERKUM LUSIX MEVEL";
        internal string MODRU1T = "DL243 UBORO NETEX MODRU";
        internal string NETEX5T = "DL236 LMA NETEX";
        internal string NVO1T = "DL239 DL241 NVO";
        internal string NUDGO5T = "DL239 DL241 ELBAL MAMIB KULIX NUDGO";
        internal string SONEB7T = "DL230 DL235 DL248 ERKUM DL245 SONEB";
        #endregion
        #region EDDL STAR 05
        internal string LMA9X = "LMA IBIKO GAPNU RONAD";
        internal string PISAP1X = "PISAP VALSU GAPNU RONAD";
        internal string DOMUX2X = "DOMUX DOLAV LEBTI";
        internal string HALME1X = "HALME BOMBA GAPNU LEBTI";
        internal string BIKMU1X = "BIKMU RONAD";
        #endregion
        #region EDDL STAR 23
        internal string LMA8G = "LMA BOT";
        internal string PISAP1G = "PISAP AGEDA XAMOD BOT";
        internal string HALME1G = "HALME XAMOD BOT";
        internal string DOMUX2G = "DOMUX BAM";
        internal string BIKMU1G = "BIKMU BOT";
        #endregion
        #region EDDL TRANS 05
        internal string LMA05 = "LMA IBIKO GAPNU DL522 RONAD DL524 DL525 DL530";
        internal string PISAP05 = "PISAP VALSU GAPNU DL522 RONAD DL524 DL525 DL530";
        internal string DOMUX05 = "DOMUX DOLAV DL502 DL503 DL504 DL505 DL510";
        internal string HALME05 = "HALME BOMBA DL517 DL518 DL519 GAPNU DL522 RONAD DL524 DL525 DL530";
        internal string BIKMU05 = "BIKMU RONAD DL513 DL514 IBIKO GAPNU DL522 RONAD DL524 DL525 DL530";
        internal string ELDAR05 = "ELDAR DL501 DL502 DL503 DL504 DL505 DL510";
        #endregion
        #region EDDL TRANS 23
        internal string LMA23 = "LMA DL426 DL428 DL429 DL430";
        internal string PISAP23 = "PISAP AGEDA XAMOD DL420 DL421 DL422 DL426 DL428 DL429 DL430";
        internal string DOMUX23 = "DOMUX DL402 DL403 DL404 DL405 DL406 DL407 DL408 DL409 DL410";
        internal string HALME23 = "HALME XAMOD DL420 DL421 DL422 DL426 DL428 DL429 DL430";
        internal string BIKMU23 = "BIKMU DL426 DL428 DL429 DL430";
        #endregion

        #region SIM settings / defintions
        public List<string> Pseudopilots { get; set; } = new List<string> { "EDDL_APP", "EDDL_M_APP", "EDGG_P_CTR" };
        public string Pseudopilot { get; set; }
        public string[] Holdings { get; set; } = { "HOLDING:ADEMI:284:-1", "HOLDING:BOT:238:-1", "HOLDING:BAM:251:-1", "HOLDING:DL524:232:1", "HOLDING:DL503:232:-1", "HOLDING:DL429:52:-1", "HOLDING:DL409:52:1", "HOLDING:DOMEG:170:-1", "HOLDING:ELDAR:356:-1", "HOLDING:HMM:257:-1", "HOLDING:LMA:240:1", "HOLDING:LEBTI:234:1", "HOLDING:MHV:359:-1", "HOLDING:RONAD:263:1" };
        public string[] DepartureAirstart { get; set; } = { "N051.16.52.419:E006.45.19.666", "N051.17.49.449:E006.47.19.702" };
        public string[] DepartureGroundstart { get; set; } = { "N051.16.42.803:E006.44.59.646", "N051.16.47.401:E006.45.09.066" };
        public List<string> ArrivalStartPoint { get; set; }
        public string Elevation { get; set; } = "150.0";
        public string[] RunwayNames { get; } = { "23L", "23R", "05R", "05L" };
        public string RunwayHeadingLastNumber { get; } = "2";
        public List<string>[] ILSDefinition { get; }
        #endregion
        internal EDDL(string _selectedRunwayDirection)
        {
            #region Sort
            SelectedRunwayDirection = _selectedRunwayDirection;
            if (_selectedRunwayDirection == RunwayDirections[0])
            {
                SelectedRunwayDirectionIndex = 0;
            }
            else
            {
                SelectedRunwayDirectionIndex = 1;
            }
            SIDS = new List<string>[RunwayDirections.Count()];
            for (int i = 0; i < RunwayDirections.Count(); i++)
            {
                SIDS[i] = new List<string>();
            }
            STARS = new List<string>[RunwayDirections.Count()];
            for (int i = 0; i < RunwayDirections.Count(); i++)
            {
                STARS[i] = new List<string>();
            }
            TRANSITION = new List<string>[RunwayDirections.Count()];
            for (int i = 0; i < RunwayDirections.Count(); i++)
            {
                TRANSITION[i] = new List<string>();
            }
            #endregion

            #region SIDS 23
            SIDS[0].Add(COL5T);
            SIDS[0].Add(DODEN9T);
            SIDS[0].Add(GMH9T);
            SIDS[0].Add(KUMIK6T);
            SIDS[0].Add(LMA2T);
            SIDS[0].Add(MEVEL3T);
            SIDS[0].Add(MODRU1T);
            SIDS[0].Add(NETEX5T);
            SIDS[0].Add(NVO1T);
            SIDS[0].Add(NUDGO5T);
            SIDS[0].Add(SONEB7T);
            #endregion
            #region SIDS 05
            SIDS[1].Add(COL4Z);
            SIDS[1].Add(DODEN6Z);
            SIDS[1].Add(GMH5Z);
            SIDS[1].Add(KUMIK4Z);
            SIDS[1].Add(LMA1Z);
            SIDS[1].Add(MEVEL9Z);
            SIDS[1].Add(MODRU6Z);
            SIDS[1].Add(NETEX3Z);
            SIDS[1].Add(NVO2Z);
            SIDS[1].Add(NUDGO4Z);
            SIDS[1].Add(SONEB5Z);
            #endregion

            #region STAR 23 
            STARS[0].Add(LMA8G);
            STARS[0].Add(PISAP1G);
            STARS[0].Add(DOMUX2G);
            STARS[0].Add(HALME1G);
            STARS[0].Add(BIKMU1G);
            #endregion
            #region STAR 05
            STARS[1].Add(LMA9X);
            STARS[1].Add(PISAP1X);
            STARS[1].Add(DOMUX2X);
            STARS[1].Add(HALME1X);
            STARS[1].Add(BIKMU1X);
            #endregion

            #region TRANS 23
            TRANSITION[0].Add(LMA23);
            TRANSITION[0].Add(PISAP23);
            TRANSITION[0].Add(DOMUX23);
            TRANSITION[0].Add(HALME23);
            TRANSITION[0].Add(BIKMU23);
            #endregion
            #region TRANS 05
            TRANSITION[1].Add(LMA05);
            TRANSITION[1].Add(PISAP05);
            TRANSITION[1].Add(DOMUX05);
            TRANSITION[1].Add(HALME05);
            TRANSITION[1].Add(BIKMU05);
            TRANSITION[1].Add(ELDAR05);
            #endregion

            #region ILS definition

            ILSDefinition = new List<string>[RunwayDirections.Count()];
            ILSDefinition[0] = new List<string> { "ILS23L:51.2939846:6.7821006:51.2798199:6.7524291", "ILS23R:51.2968503:6.7762140:51.2838245:6.7490896", "ILS05R:51.2808697:6.7546887:51.2950353:6.7842650", "ILS05L:51.2853858:6.7522646:51.2977799:6.7782330" };
            ILSDefinition[1] = ILSDefinition[0];
            #endregion
        }
    }
}
