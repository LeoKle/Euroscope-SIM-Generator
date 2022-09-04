using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroscopeSIMBib
{
    /// <summary>
    /// Attempt at defining an airport using json
    /// </summary>
    internal class AirportJson
    {
        #region ICAO, runway directions

        internal string ICAO { get; set; }
        internal string[] RunwayDirections { get; set; }
        internal int[] RunwayHeadings { get; set; }
        internal string Elevation { get; set; }

        #endregion

        #region SID / STAR / TRANSITION : route / flightplan and altitude

        internal string[,] SIDSroute { get; set; }
        internal string[,] SIDSflightplan { get; set; }
        internal string[,] SIDSAltitude { get; set; }

        internal string[,] STARSroute { get; set; }
        internal string[,] STARSflightplan { get; set; }
        internal string[,] STARSArrivalAltitude { get; set; }

        internal string[,] TRANSITIONroute { get; set; }
        internal string[,] TRANSITIONflightplan { get; set; }
        internal string[,] TRANSITIONArrivalAltitude { get; set; }

        #endregion

        #region ES SIM SETTINGS : ground/air start position, Arrivals start positions

        internal string[] DeparturePositionGroundStart { get; set; }
        internal string[] DeparturePositionAirStart { get; set; }
        internal string[] ArrivalStartPosition { get; set; }
        internal int[] ArrivalInitialHeading { get; set; }

        internal string[,] ILSDefinitions { get; set; }
        internal string[] HoldingDefinitions { get; set; }
        internal string[] Pseudopilots { get; set; }

        #endregion
    }
}
