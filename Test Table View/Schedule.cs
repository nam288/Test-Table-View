using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_Table_View;

namespace Model
{
    class Schedule
    {
        //public static readonly Dictionary<int, Dictionary<int, int>> nDoc = new Dictionary<int, Dictionary<int, int>>();

        public static readonly int[,] amount = new int[7, 2];
        public List<List<List<int>>> _schedule = new List<List<List<int>>>();
        public bool[,,] primary = new bool[7, 2, 6];
        public Data _data;
        public Form1 _form;
        public int cntSuccess = 0;

        public Schedule(Data data, Form1 form1)
        {
            _form = form1;
            _data = data;
            for (int i = 0; i <= 6; i++)
            {
                List<List<int>> tmp = new List<List<int>>();
                for (int j = 0; j <= 1; j++)
                    tmp.Add(new List<int>());
                _schedule.Add(tmp);
            }

           

            for (int i = 1; i <= 5; i++)
                for (int j = 0; j <= 1; j++)
                    amount[i, j] = 5 - j;
        }

        public void Set(Time t, int index, int value)
        {
            Console.WriteLine("In set func {0}", index);
            if (primary[t.date,t.part, index])
            {
                _schedule[t.date][t.part].Add(value);
                _form.Set(t, index, value);
            } else
            {
                _schedule[t.date][t.part][index] = value;
            }
            primary[t.date, t.part, index] = true;
        }

        public List<int> this[Time t]
        {
            get => _schedule[t.date][t.part];
            set
            {
                _schedule[t.date][t.part].Clear();
                Console.WriteLine(t);
                foreach (var i in value)
                    Console.WriteLine(i);
                Console.WriteLine();
                for (int i = 0; i < Math.Min(value.Count, amount[t.date,t.part]); i++)
                {
                    Console.Write("@@");
                    Console.WriteLine(i);

                    primary[t.date, t.part, i] = true;
                    if (i < value.Count)
                    {
                        Set(t, i, value[i]);
                    }
                    else
                        Set(t, i, -1);
                }
                
            }
        }

        public List<int> this[int date, int part]
        {
            get => this[new Time(date, part)];
            set => this[new Time(date, part)] = value;
        }

        public void Main()
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            RunTime(new Time(1, 0));

            for (int i = 1; i <= 5; i++)
                for (int j = 0; j <= 1; j++)
                {
                    Console.Write(j);
                    this[i, j].ForEach(d => Console.Write("@"));
                    Console.WriteLine();
                }
            Console.Read();
            for (int i = 0; i < 1000; i++)
            {
                Move();
                Filling();
            }
            for (int i = 1; i <= 5; i++)
                for (int j = 0; j <= 1; j++)
                {
                    Console.Write(j);
                    this[i, j].ForEach(d => Console.Write("@"));
                    Console.WriteLine();
                }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            Console.WriteLine(cntSuccess);
        }

        public void RunTime(Time time)
        {
            bool finish = false;
            List<int> res = new List<int>();

            Run(FreemanInTime(time), time, 1, 0, ref finish, ref res);

            this[time] = res;

            if (time.Next().date != -1)
                RunTime(time.Next());
        }

        public void Run(List<int> freeman, Time time,
            int ithDoctor, int from, ref bool isFinish, ref List<int> res)
        {
            // reach end freeman list
            if (from == freeman.Count)
                return;

            // update if possible
            if (res.Count < ithDoctor - 1)
                res = new List<int>(this[time]);

            // fille enough doctor
            if (ithDoctor > amount[time.date, time.part])
            {
                isFinish = true;
                return;
            }

            for (int k = from; k < freeman.Count; k++)
            {
                var candidate = freeman[k];
                if (Available(candidate, time))
                {
                    this[time].Add(candidate);
                    Console.WriteLine("{0} Choose {1}", new string(' ', ithDoctor), candidate);
                    Run(freeman, time, ithDoctor + 1, k + 1, ref isFinish, ref res);
                    if (isFinish) return;
                    this[time].RemoveAt(this[time].Count - 1);
                    Console.WriteLine("{0} Unchoose {1}", new string(' ', ithDoctor), candidate);
                }
            }
        }

        public void Move()
        {
            var miss = Missing();
            Random rand = new Random();
            var time = Time.WorkingTimeRand();
            if (this[time].Any())
            {
                int index = rand.Next(this[time].Count);
                int candidate = this[time][index];
                this[time].RemoveAt(index);
                Console.WriteLine("Choose {0}", candidate);
                foreach (var missTime in miss)
                {
                    if (Available(candidate, missTime))
                    {
                        this[missTime].Add(candidate);
                        return;
                    }
                }
                this[time].Add(candidate);
            }
        }

        public void Filling()
        {
            foreach (var missTime in Missing())
                foreach (var candidate in _data.Freeman(missTime))
                    if (Available(candidate, missTime))
                    {
                        this[missTime].Add(candidate);
                        cntSuccess++;
                        return;
                    }
        }

        public List<Time> Missing()
        {
            return Time.AllTime().Where(t => this[t].Count < amount[t.date, t.part]).ToList();
        }

        public List<int> FreemanInTime(Time time)
        {
            return _data.Freeman(time)
                .Where(d => Available(d, time))
                .ToList();
        }

        public bool WorkedSameDay(int doctor, Time time)
        {
            return this[time.Opposite()].Contains(doctor);
        }

        public bool WorkedSamePart(int doctor, Time time)
        {
            return Enumerable.Range(1, 5).Any(i => this[i, time.part].Contains(doctor));
        }

        public bool WorkedNearly(int doctor, Time time)
        {
            Time prev = time.Prev(), next = time.Next();
            bool checkPrev = false, checkNext = false;
            if (prev.date != -1)
                checkPrev = this[prev].Contains(doctor);
            if (next.date != -1)
                checkNext = this[next].Contains(doctor);
            return checkPrev || checkNext;
        }

        public int FreemanDep(int doctor, Time time)
        {
            return _data.DepartmentOf(doctor).doctors
                .Count(d => d[time] <= 1);
        }

        public int WorkerDepTime(int doctor, Time time)
        {
            return this[time]
                .Where(d => _data.SameDep(d, doctor))
                .Count();
        }

        public int WorkerInDepWeek(Department department)
        {
            var all = from a in _schedule
                      from b in a
                      from doctor in b
                      where _data.doctors[doctor].dep == department.name
                      select doctor;
            return all.Count();
        }

        public bool Available(int doctor, Time time)
        {
            if (this[time].Contains(doctor))
            {
                Console.WriteLine("Cant choose doctor {0,-2}. Reason: SAME TIME", doctor);
                return false;
            }

            if (WorkedSameDay(doctor, time))
            {
                Console.WriteLine("Cant choose doctor {0,-2}. Reason: SAME DAY", doctor);
                return false;
            }

            if (_data.doctors[doctor].maxWorking == 2)
            {
                if (WorkedSamePart(doctor, time))
                {
                    Console.WriteLine("Cant choose doctor {0,-2}. Reason: SAME PART", doctor);
                    return false;
                }
                if (WorkedNearly(doctor, time))
                {
                    Console.WriteLine("Cant choose doctor {0,-2}. Reason: NEARLY", doctor);
                    return false;
                }
            }

            if (WorkerInDepWeek(_data.DepartmentOf(doctor)) == _data.DepartmentOf(doctor).maxWorkingWeekly)
            {
                Console.WriteLine("Cant choose doctor {0,-2}. Reason: DEP {1} ENOUGH", doctor, _data.DepartmentOf(doctor).name);
                return false;
            }

            if (FreemanDep(doctor, time) < 2)
            {
                Console.WriteLine("Cant choose doctor {0,-2}. Reason: DEP {1} NOBODY ", doctor, _data.DepartmentOf(doctor).name);
                return false;
            }

            if (WorkerDepTime(doctor, time) >= 2 - time.part)
            {
                Console.WriteLine("Cant choose doctor {0,-2}. Reason: DEP {1} ENOUGH", doctor, _data.DepartmentOf(doctor).name);
                return false;
            }
            return true;
        }
    }
}
