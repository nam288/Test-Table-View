using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Serializable]
    class Data
    {
        public const string fileBinaryPath = @"C:\Users\WIN\Desktop\data.bin";
        public const string fileJsonPath = @"C:\Users\WIN\Desktop\data.txt";
        public int cnt = 0;
        public List<Shift> shifts = new List<Shift>();
        public List<Operation> operations = new List<Operation>();
        public List<Doctor> doctors = new List<Doctor>();
        public List<Department> depts = new List<Department>();
        public Dictionary<string, int> indexDoctors = new Dictionary<string, int>();
        public Dictionary<string, int> indexDepts = new Dictionary<string, int>();
        public Dictionary<Tuple<int, int>, int> indexShifts = new Dictionary<Tuple<int, int>, int>();

        public Data() { }

        public Data(int numberDoctor, Random rand)
        {
            for (int date = 0; date < 7; date++)
                shifts.Add(new Shift(date));

            for (; cnt < numberDoctor;)
            {
                var d = new Doctor(rand, cnt++);
                AddDoctor(d);
            }

            foreach (var dep in depts)
                Console.WriteLine(dep);
        }

        public void AddDoctor(Doctor d)
        {
            if (indexDoctors.ContainsKey(d.name))
            {
                Console.WriteLine("Exist doctor {0}!\n", d.name);
                cnt--;
                return;
            }
            doctors.Add(d);
            indexDoctors.Add(d.name, doctors.Count - 1);
            if (indexDepts.ContainsKey(d.dep))
            {
                depts[indexDepts[d.dep]].Add(d);
            }
            else
            {
                depts.Add(new Department(d.dep, 10));
                indexDepts.Add(d.dep, depts.Count - 1);
                depts.Last().Add(d);
            }
        }

        public void GenerateShift(Random rand)
        {
            foreach (Doctor doc in doctors)
            {
                int randPeriod = rand.Next(2) + 4;
                int randDate = rand.Next(randPeriod);

                shifts[randDate].Add(doc);

                if (randDate + randPeriod < 7)
                    shifts[randDate + randPeriod].Add(doc);
            }
        } 

        public void GenerateOperation(Random rand)
        {
            foreach (var time in Time.AllWorkingTime())
            {
                var op = new Operation(time);
                for (int i = 1; i<= rand.Next(5); i++)
                {
                    var doc = doctors[rand.Next(doctors.Count)];
                    op.Add(doc);
                    
                    Console.WriteLine("Add {0}",doc.index);
                }
                operations.Add(op);
            }
        }

        public void Print()
        {
            foreach (var doctor in doctors)
                Console.WriteLine(doctor);
        }

        public Doctor GetDoctor(string name)
        {
            return doctors[indexDoctors[name]];
        }

        public Doctor GetDoctor(int indexDoctor) => doctors[indexDoctor];

        public Department DepartmentOf(int indexDoctor) => depts[indexDepts[GetDoctor(indexDoctor).dep]];

        public Department GetDepartment(string nameDept) => depts[indexDepts[nameDept]];

        public Department GetDepartment(Doctor doctor) => GetDepartment(doctor.dep);

        public bool SameDep(int d1, int d2) => doctors[d1].dep == doctors[d2].dep;
        
        public List<int> Freeman(Time t)
        {
            var d = from doctor in doctors
                    where doctor[t] <= 1
                    orderby doctor[t]
                    select doctor.index;
            return d.ToList();
        }

        public void Write()
        {
            BinarySerialization.WriteToBinaryFile(fileBinaryPath, this);
        }

    }
}
