using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace ES
{
    internal class Airport
    {
        internal virtual string ICAO { get; set; }
        /// <summary>
        /// Determines the runway in use i.e. at EDDL 05/23
        /// </summary>
        internal virtual List<string> RunwayDirections { get; set; } = new List<string>();
        internal virtual int SelectedRunwayDirectionIndex { get; set; }
        internal virtual string RunwayHeadingLastNumber { get; set; }
        /// <summary>
        /// List of SIDSRoute corresponding to the index of the runway described in property "RunwayDirections"
        /// </summary>
        internal virtual List<string>[] SIDSRoute { get; set; }
        internal virtual List<string>[] SIDSFpln { get; set; }
        /// <summary>
        /// List of STARs responding to the index of the runway described in property "RunwayDirections"
        /// </summary>
        internal virtual List<string>[] STARSRoute { get; set; }
        internal virtual List<string>[] STARSFpln { get; set; }
        /// <summary>
        /// List of TRANSITION corresponding to the index of the runway described in property "RunwayDirections"
        /// </summary>
        internal virtual List<string>[] TRANSITIONRoute { get; set; } 
        internal virtual List<string>[] TRANSITIONFpln { get; set; }

        internal virtual string[] DepartureAirstart { get; set; }
        internal virtual string[] DepartureGroundstart { get; set; }
        internal virtual List<string> ArrivalStartPoint { get; set; } = new List<string>();




        /// <summary>
        /// Should be obsolete and property should be in method
        /// </summary>
        internal virtual string SelectedRunwayDirection { get; set; }
        internal virtual int InboundHeading { get; set; }

        #region details ES simfile requires

        internal virtual List<string>[] ILSDefinition { get; set; }
        internal virtual string[] Holdings { get; set; }

        internal virtual List<string> Pseudopilots { get; set; } = new List<string>();

        internal virtual string Elevation { get; set; }
        internal virtual string[] RunwayNames { get; set; }
        internal virtual List<string> InboundStartPosition { get; set; } = new List<string>();
        internal virtual List<string> InboundAltitude { get; set; } = new List<string>();

        #endregion
    }
}
