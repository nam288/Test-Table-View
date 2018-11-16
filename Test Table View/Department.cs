using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Serializable]
    class Department
    {
        public readonly string name;
        public readonly int maxWorkingWeekly;
        public List<Doctor> doctors = new List<Doctor>();

        public Department(string name, int maxWorking = 10)
        {
            this.name = name;
            maxWorkingWeekly = maxWorking;
        }

        public void Add(Doctor doctor)
        {
            doctors.Add(doctor);
        }
        public override string ToString()
        {
            string res = "";
            res += "----Description Doctor----\n";
            res += string.Format("{0,-15}: {1}\n", "Name", name);
            res += "List doctor: ";
            foreach (var doctor in doctors)
            {
                res += doctor.index + " ";
            }
            res += "\n";
            return res;
        }
    }
}
