using System;
using System.Collections.Generic;

namespace Model
{
    [Serializable]
    public class PreferMaxWorking
    {
        public static readonly string name = "MSP";
        public bool isPermanent;
        public int value;
        public PreferMaxWorking(int n = 1, bool per = true)
        {
            value = n;
            isPermanent = per;
        }
        public override string ToString()
        {
            return string.Format("{0,-15}{1,-15}{2,-15}", name, isPermanent, value);
        }
    }
    [Serializable]
    public class PreferTime
    {
        public static readonly string name = "Prefer";
        public bool isPermanent;
        public Time time;
        public PreferTime(Time s, bool per = true)
        {
            time = s;
            isPermanent = per;
        }
        public PreferTime(Random rand)
        {
            isPermanent = Convert.ToBoolean(rand.Next(2));
            time = new Time(rand);
        }
        public override string ToString()
        {
            return string.Format("{0,-15}{1,-15}{2,-15}", name, isPermanent, time);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PreferTime);
        }

        public bool Equals(PreferTime t)
        {
            if (ReferenceEquals(t, null))
                return false;

            if (ReferenceEquals(this, t))
                return true;

            return (t.isPermanent == isPermanent) && t.time.Equals(time);
        }

        public override int GetHashCode()
        {
            return time.GetHashCode() * 2 + Convert.ToInt16(isPermanent);
        }
    }
    [Serializable]
    public class MaxWorking
    {
        public static readonly string name = "MSC";
        public bool isPermanent = true;
        public int value;
        public MaxWorking(int n = 2, bool per = true)
        {
            value = n;
            isPermanent = per;
        }
        public override string ToString()
        {
            return string.Format("{0,15}{1,15}{2,15}", name, isPermanent, value);
        }
    }

    [Serializable]
    public class BusyTime: IEquatable<BusyTime>
    {
        public static readonly string name = "Busy";
        public bool isPermanent;
        public Time time;

        public BusyTime(Time s, bool p = true)
        {
            isPermanent = p;
            time = s;
        }

        public BusyTime(Random rand)
        {
            isPermanent = Convert.ToBoolean(rand.Next(2));
            time = new Time(rand);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BusyTime);
        }
        
        public bool Equals(BusyTime t)
        {
            if (ReferenceEquals(t, null))
                return false;

            if (ReferenceEquals(this, t))
                return true;

            return (t.isPermanent == isPermanent) && t.time.Equals(time);
        }

        public override int GetHashCode()
        {
            return time.GetHashCode() * 2 + Convert.ToInt16(isPermanent);
        }

        public override string ToString()
        {
            return string.Format("{0,-15}{1,-15}{2,-15}", name, isPermanent, time);
        }
    }

    [Serializable]
    public class Time: IEquatable<Time>
    {
        public static readonly string[] weekdays = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        public int date, part; //0- morning, 1-afternoon;

        public Time(int d, int p)
        {
            date = d;
            part = p;
        }

        public Time(Shift s)
        {
            date = s.date;
            part = 2;
        }

        public Time(Random rand)
        {
            date = rand.Next(5) + 1;
            part = rand.Next(2);
        }

        public static Time WorkingTimeRand()
        {
            Random rand = new Random();
            return new Time(rand.Next(5) + 1, rand.Next(2));
        }

        public static List<Time> AllTime()
        {
            List<Time> res = new List<Time>();
            for (int i = 1; i <= 5; i++)
                for (int j = 0; j <= 1; j++)
                    res.Add(new Time(i, j));
            return res;
        }

        public override string ToString()
        {
            string tmp = "";
            if (part == 0)
            {
                tmp += "Morning ";
            }
            else if (part == 1)
            {
                tmp += "Afternoon ";
            }
            else
            {
                tmp += "Whole ";
            }
            tmp += weekdays[date];
            return tmp;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Time);
        }

        public bool Equals(Time t)
        {
            if (ReferenceEquals(t, null))
                return false;
            if (ReferenceEquals(this, t))
                return true;
            return (date == t.date) && (part == t.part);
        }

        public override int GetHashCode()
        {
            return date * 3 + part;
        }
        public Time Opposite()
        {
            return new Time(date, 1 - part);
        }

        public Time Next()
        {
            if (date == 5 && part == 1)
                return new Time(-1, -1);
            return new Time(date + part, 1 - part);
        }

        public Time Prev()
        {
            if (date == 1 && part == 0)
                return new Time(-1, -1);
            return new Time(date + 1 - part, 1 - part);
        }
    }
}

