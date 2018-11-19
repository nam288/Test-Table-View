using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_Table_View;
using System.Drawing;

namespace Model
{
    class Schedule
    {
        public readonly int[][] amount = new int[7][];
        public bool[][][] isPrimary = new bool[7][][];
        public List<int>[][] _schedule = new List<int>[7][];
        public Data _data;
        public CustomForm _form;
        public int cntSuccess = 0;

        public Schedule(Data data, CustomForm form)
        {
            _form = form;
            _data = data;

            // init amount
            for (int date = 0; date <= 6; date++)
                amount[date] = new int[2];

            for (int date = 1; date <= 5; date++)
                for (int part = 0; part <= 1; part++)
                    amount[date][part] = 5 - part;
            amount[1][1] = 5;

            // init isPrimary
            for (int date = 0; date <= 6; date++)
            {
                var inDate = new bool[2][];
                for (int part = 0; part <= 1; part++)
                {
                    inDate[part] = new bool[amount[date][part]];
                    for (int i = 0; i < inDate[part].Length; i++)
                        inDate[part][i] = true;
                }
                isPrimary[date] = inDate;
            }

            // init real schedule
            for (int date = 0; date <= 6; date++)
            {
                _schedule[date] = new List<int>[2];
                for (int part = 0; part <= 1; part++)
                    _schedule[date][part] = new List<int>();
            }
        }

        public void Print()
        {
            Time.AllWorkingTime().ForEach(time =>
            {
                Console.WriteLine(time);
                foreach (var i in this[time])
                    Console.Write("{0} ", _data.GetDoctor(i).name);
                Console.WriteLine();
            });
        }

        public void SetSchedule(Time t, int index, int value)
        {
            if (isPrimary[t.date][t.part][index])
            {
                _schedule[t.date][t.part][index] = value;
                _form.Set(t, index, value);
            }
            else
            {
                _schedule[t.date][t.part][index] = value;
                isPrimary[t.date][t.part][index] = true;
            }
        }

        public List<int> this[Time t]
        {
            get => _schedule[t.date][t.part];
            set => _schedule[t.date][t.part] = value;
        }

        public List<int> this[int date, int part] => this[new Time(date, part)];

        public ComboBoxItem ColorComboBoxItem(Time time, int indexDoctor, int indexList)
        {
            Doctor doctor = _data.GetDoctor(indexDoctor);
            Color color = new Color();
            switch (doctor[time])
            {
                case -1:
                    color = Color.Blue;
                    break;
                case 0:
                    color = Color.Green;
                    break;
                case 1:
                    color = Color.OrangeRed;
                    break;
            }
            return new ComboBoxItem(doctor.name, indexDoctor.ToString(), color);
            
        }

        public void Refresh()
        {
            Time.AllWorkingTime().ForEach(t =>
            {
                for (int i = 0; i < this[t].Count; i++)
                    _form.Set(t, i, this[t][i]);

                for (int i = this[t].Count; i < amount[t.date][t.part]; i++)
                    _form.GetComboBox(t, i).Items.Clear();
            });

            Time.AllWorkingTime().ForEach(t =>
            {               
                List<ComboBoxItem> addItems = new List<ComboBoxItem>();
                AvailableMen(t).ForEach(i =>
                {
                    addItems.Add(ColorComboBoxItem(t, i, addItems.Count + 1));
                });

                for (int i = 0; i < this[t].Count; i++)
                    _form.GetComboBox(t, i).Items.AddRange(addItems.Cast<object>().ToArray());
            });
        }

        public void Main()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Time.AllWorkingTime().ForEach(time => this[time].Clear());
            RunTime(new Time(1, 0));

            for (int i = 0; i < 500; i++)
            {
                Move();
                Filling();
            }

            Print();
            Refresh();

            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            Console.WriteLine(cntSuccess);
        }

        public void RunTime(Time time)
        {
            Console.WriteLine("Start calculate {0}:", time);
            Console.WriteLine("List candidate:");
            foreach (var i in AvailableMen(time))
                Console.Write("{0} ", i);
            Console.WriteLine();

            bool finish = false;
            List<int> res = new List<int>();

            Run(AvailableMen(time), time, 0, 0, ref finish, ref res);

            Console.WriteLine("End calculate {0}:", time);
            Console.WriteLine("Result:");
            foreach (var i in res)
                Console.Write("{0} ", i);
            Console.WriteLine();

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
            if (res.Count < this[time].Count)
                res = new List<int>(this[time]);

            // fille enough doctor
            if (ithDoctor == amount[time.date][time.part])
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
                    this[time].Remove(candidate);
                    Console.WriteLine("{0} Unchoose {1}", new string(' ', ithDoctor), candidate);
                }
            }
        }

        public void Move()
        {
            var miss = Missing();
            Random rand = new Random();
            var time = Time.WorkingTimeRand();
            if (_schedule[time.date][time.part].Any())
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
            => Time.AllWorkingTime()
                   .Where(t => this[t].Count < amount[t.date][t.part])
                   .ToList();

        public List<int> AvailableMen(Time time)
            => _data.Freeman(time)
                    .Where(d => Available(d, time))
                    .ToList();

        public bool WorkedSameDay(int doctor, Time time)
            => this[time.Opposite()].Contains(doctor);

        public bool WorkedSamePart(int doctor, Time time)
            => Enumerable.Range(1, 5)
                         .Any(i => this[i, time.part]
                         .Contains(doctor));

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
            => _data.DepartmentOf(doctor).doctors
                    .Count(d => d[time] <= 1);

        public int WorkerDepTime(int doctor, Time time)
        {
            return this[time]
                .Where(d => d > -1)
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
                Console.WriteLine("{0,-2} SAME TIME", doctor);
                return false;
            }

            if (WorkedSameDay(doctor, time))
            {
                Console.WriteLine("{0,-2} SAME DAY", doctor);
                return false;
            }

            if (_data.doctors[doctor].maxWorking == 2)
            {
                if (WorkedSamePart(doctor, time))
                {
                    Console.WriteLine("{0,-2} SAME PART", doctor);
                    return false;
                }
                if (WorkedNearly(doctor, time))
                {
                    Console.WriteLine("={0,-2} NEARLY", doctor);
                    return false;
                }
            }

            if (WorkerInDepWeek(_data.DepartmentOf(doctor)) == _data.DepartmentOf(doctor).maxWorkingWeekly)
            {
                Console.WriteLine("{0,-2} DEP {1} ENOUGH", doctor, _data.DepartmentOf(doctor).name);
                return false;
            }

            if (FreemanDep(doctor, time) < 2)
            {
                Console.WriteLine("{0,-2} DEP {1} NOBODY ", doctor, _data.DepartmentOf(doctor).name);
                return false;
            }

            if (WorkerDepTime(doctor, time) >= 2 - time.part)
            {
                Console.WriteLine("{0,-2} DEP {1} ENOUGH", doctor, _data.DepartmentOf(doctor).name);
                return false;
            }
            return true;
        }
    }
}
