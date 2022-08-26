using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES
{
    internal class ACFT
    {
        internal string Callsign { get; set; }
        internal string DEST { get; set; }
        internal string DEP { get; set; }
        internal string ATYP { get; set; }
        internal string Squawk { get; set; }

        internal ACFT(string callsign, string dep, string dest, string atyp, string squawk)
        {
            Callsign = callsign;
            DEP = dep;
            DEST = dest;
            ATYP = atyp;
            Squawk = squawk;
        }
    }
}
