using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES
{
    internal class Squawks
    {
        internal List<int> AvailableSquawks = new List<int>();

        internal Squawks()
        {
            for (int i = 2001; i <= 2577; i++)
            {
                AvailableSquawks.Add(i);
            }
        }
        internal static string AssignSquawk()
        {
            /*Random rnd = new Random();
            int r = rnd.Next(AvailableSquawks.Count);

            Squawk = AvailableSquawks[r].ToString();
            AvailableSquawks.RemoveAt(r); */ //aircraft count exceeds squawks count, fix needed

            return "1000";
        }
    }
}
