using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroscopeSIMBib
{
    public class Squawks
    {
        public  List<int> AvailableSquawks = new List<int>();

        public Squawks()
        {
            for (int i = 2001; i <= 2577; i++)
            {
                AvailableSquawks.Add(i);
            }
        }
        internal string GetSquawk()
        {
            string Squawk = "";
            Random rnd = new Random();
            int r = rnd.Next(AvailableSquawks.Count);

            Squawk = AvailableSquawks[r].ToString();
            AvailableSquawks.RemoveAt(r);
            return Squawk;
        }
    }
}
