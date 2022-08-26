using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES
{
    public interface IAirport
    {
        string ICAO { get; }
        /// <summary>
        /// Determines the runway in use i.e. at EDDL 05/23
        /// </summary>
        string[] RunwayDirections { get; }
        int SelectedRunwayDirectionIndex { get; }
        string RunwayHeadingLastNumber { get; }
        /// <summary>
        /// List of SIDS corresponding to the index of the runway described in property "RunwayDirections"
        /// </summary>
        List<string>[] SIDS { get; }
        /// <summary>
        /// List of STARs responding to the index of the runway described in property "RunwayDirections"
        /// </summary>
        List<string>[] STARS { get; }
        /// <summary>
        /// List of TRANSITION corresponding to the index of the runway described in property "RunwayDirections"
        /// </summary>
        List<string>[] TRANSITION { get; }

        string[] DepartureAirstart { get; }
        string[] DepartureGroundstart { get; }
        string SelectedRunwayDirection { get; }

        #region details ES simfile requires

        List<string>[] ILSDefinition { get; }
        string[] Holdings { get; }

        List<string> Pseudopilots { get; }
        string Pseudopilot { get; set; }

        string Elevation { get; }
        string[] RunwayNames { get; }

        #endregion
    }
}
