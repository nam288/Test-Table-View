using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Model
{
    [Serializable]
    public class Doctor
    {
        public string name, dep;
        public int maxWorking = 3, maxWorkingPref = 3, index;
        public List<Shift> shifts = new List<Shift>();
        public List<TimeConstraint> constraints = new List<TimeConstraint>();
        
        public List<Operation> operations = new List<Operation>();

        public int[,] Timeline { get; set; } = new int[7, 2];
        // -1:prefer, 0:can work, 1:not prefer, 2: cant work, 3: work on shift

        public Doctor(Random rand, int index)
        {
            this.index = index;
            string chars = "ABCDEFGH";
            name = chars.ElementAt(rand.Next(8)).ToString() + rand.Next(10).ToString();
            dep = name[0].ToString();
            AddConstraint(new MaxWorking(rand.Next(4)+1));
            AddConstraint(new PreferMaxWorking(rand.Next(3)+1));
            for (int i = 0; i<6; i++)
                AddConstraint(new TimeConstraint(rand));

        }

        public Doctor(string name, string dept)
        {
            this.name = name;
            dep = dept;
        }

        public int this[Time t]
        {
            get => Timeline[t.date, t.part];
            set
            {
                if (value == -1 && this[t] == 0)
                {
                    Timeline[t.date, t.part] = -1;
                }
                else
                    Timeline[t.date, t.part] = Max(Timeline[t.date, t.part], value);
            }
        }

        public int this[int date, int part]
        {
            get => this[new Time(date, part)];
            set => this[new Time(date, part)] = value;
        }

        public void AddConstraint(TimeConstraint c)
        {
            constraints.Add(c);
            CalculateTimeline();
        }

        public void AddConstraint(MaxWorking c)
        {
            maxWorking = Min(maxWorking, c.value);
        }

        public void AddConstraint(PreferMaxWorking p)
        {
            maxWorkingPref = Min(Min(maxWorkingPref, p.value),maxWorking);
        }

        public void AddConstraint(Shift s)
        {
            shifts.Add(s);
            CalculateTimeline();
        }

        public void RemoveConstraint(Shift s)
        {
            shifts.Remove(s);
            CalculateTimeline();
        }

        public void AddConstraint(Operation o)
        {
            operations.Add(o);
            CalculateTimeline();
        }

        public void RemoveConstraint(Operation o)
        {
            operations.Remove(o);
            CalculateTimeline();
        }

        public void CalculateTimeline()
        {
            Timeline = new int[7, 2];

            foreach (var cons in constraints)
                if (cons.isBusy == false)
                    this[cons.time] = cons.isBusy == false ? -1 : 2;

            foreach (var shiftDate in shifts)
            {
                this[shiftDate.date, 0] = 2;
                this[shiftDate.date, 1] = 2;
                if (shiftDate.date + 1 < 7)
                {
                    this[shiftDate.date + 1, 0] = 2;
                    this[shiftDate.date + 1, 1] = 2;
                }
                if (shiftDate.date - 1 >= 0)
                    this[shiftDate.date - 1, 1] = 1;
            }

            foreach (var op in operations)
            {
                this[op.time] = 2;
                this[op.time.Opposite()] = 1;
            }
        }

        public override string ToString()
        {
            string res = "";
            res += "----Description Doctor----\n";
            res += string.Format("{0,-15}: {1}\n", "Index", index);
            res += string.Format("{0,-15}: {1}\n", "Name", name);
            res += string.Format("{0,-15}: {1}\n", "Department", dep);

            res += "List constraint:\n";
            res += string.Format("{0,-15}: {1}\n", "Max working", maxWorking);
            res += string.Format("{0,-15}: {1}\n", "Max working Refer", maxWorkingPref);
            res += string.Format("{0,-15}{1,-15}{2,-15}\n", "TYPE", "PERMANENT", "VALUE");

            foreach (var shift in shifts)
                res += shift.ToString() + "\n";

            foreach (var cons in constraints)
                res += cons.ToString() + "\n";

            foreach (var op in operations)
                res += op.ToString() + "\n";

            res += "Timeline:\n";
            for (int i = 0; i < 7; i++)
                res += string.Format("{0,3} ", Time.weekdays[i]);
            res += "\n";
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 7; j++)
                    res += string.Format("{0,-4}", Timeline[j, i]);
                res += "\n";
            }
            return res;
        }
    }
}
