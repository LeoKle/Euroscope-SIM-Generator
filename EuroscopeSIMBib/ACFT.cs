using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroscopeSIMBib
{
    public class ACFT
    {
        /*
        PSEUDOPILOT:EDGG_P_CTR


        @<transponder flag>:<callsign>:<squawk code>:1:<latitude>:<longitude>:<altitude>:0:<heading>:0
        @N:DLH701:2277:1:51.9782979:6.4737925:17000:0:1592:0


        $FP<callsign>:*A:<flight plan type>:<aircraft type>:<true air speed>:<origin airport>:<departure time EST>:<departure time ACT>:<final cruising altitude>:<destination airport>:<HRS en route>:<MINS en route>:<HRS fuel>:<MINS fuel>:<alternate airport>:<remarks>:<route>
        $FPDLH701:*A:I:B744:420:KLAX:0000:0000:43000:EDDL:00:00:0:0::/v/:TEBRO PISAP PISAP23


        SIMDATA:<callsign>:<plane type>:<livery>:<maximum taxi speed>:<taxiway usage>:<object extent>
        SIMDATA:DLH701:*:*:25:1:0


        $ROUTE:<point by point route>
        $ROUTE:TEBRO PISAP AGEDA XAMOD DL422 DL426 DL429 DL430
        START:6
        DELAY:3:7
        REQALT:TEBRO:17000

        INITIALPSEUDOPILOT:EDGG_P_CTR
        */

        public string Callsign { get; set; }
        public string ADEP { get; set; }
        public string ADES { get; set; }
        public string TypeCat { get; set; }

        public string ENTRYorEXIT { get; set; }
    }
}
