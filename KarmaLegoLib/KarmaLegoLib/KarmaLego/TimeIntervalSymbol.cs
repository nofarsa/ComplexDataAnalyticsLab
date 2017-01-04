using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarmaLegoLib.KarmaLego
{
    public class TimeIntervalSymbol
    {
        public int startTime;
        public int endTime;
        public int symbol;

        public TimeIntervalSymbol(int setStartTime, int setEndTime, int setSymbol)
        {
            startTime = setStartTime;
            endTime = setEndTime;
            symbol = setSymbol;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + symbol.GetHashCode();
            hash = (hash * 7) + startTime.GetHashCode();
            hash = (hash * 7) + endTime.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            TimeIntervalSymbol other = obj as TimeIntervalSymbol;
            return (other != null && startTime == other.startTime && endTime == other.endTime);
        }

        public static bool operator ==(TimeIntervalSymbol a, TimeIntervalSymbol b)
        {
            if (a.startTime == b.startTime && a.endTime == b.endTime && a.symbol == b.symbol)
                return true;
            else
                return false;
        }

        public static bool operator !=(TimeIntervalSymbol a, TimeIntervalSymbol b)
        {
            if (a.startTime != b.startTime || a.endTime != b.endTime || a.symbol != b.symbol)
                return true;
            else
                return false;
        }

        public static int compareTIS(TimeIntervalSymbol A, TimeIntervalSymbol B) // A<B?
        {
            if (A.startTime < B.startTime)
                return -1;
            else if (A.startTime == B.startTime && A.endTime < B.endTime)
                return -1;
            else if (A.startTime == B.startTime && A.endTime == B.endTime && A.symbol < B.symbol)
                return -1;
            else
                return 1;
        }
    }

}
