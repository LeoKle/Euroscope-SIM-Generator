using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroscopeSIMBib
{
    [Serializable]
    public class Airport
    {
        public string ICAO { get; set; }
        public string[] RunwayDirections { get; set; }
        public int[] RunwayHeadings { get; set; }
        public string Elevation { get; set; }
        public string[][] SIDSroute { get; set; }
        public string[][] SIDSflightplan { get; set; }
        public string[][] SIDSAltitude { get; set; }
        public string[][] STARSroute { get; set; }
        public string[][] STARSflightplan { get; set; }
        public string[][] STARSArrivalAltitude { get; set; }
        public string[][] TRANSITIONroute { get; set; }
        public string[][] TRANSITIONflightplan { get; set; }
        public string[][] TRANSITIONArrivalAltitude { get; set; }
        public string[] DeparturePositionGroundStart { get; set; }
        public string[] DeparturePositionAirStart { get; set; }
        public string[] ArrivalStartPosition { get; set; }
        public int[] ArrivalInitialHeading { get; set; }
        public string[] ILSDefinitions { get; set; }
        public string[] HoldingDefinitions { get; set; }
        public string[] Pseudopilots { get; set; }
    }
}
