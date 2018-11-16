using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Serializable]
    public class Shift
    {
        public int date;
        public List<Doctor> doctors = new List<Doctor>();

        public Shift(int date)
        {
            this.date = date;
        }
        
        public override string ToString()
        {
            return string.Format("{0,-15}{1,-15}{2,-15}", "Shift", false, new Time(this));
        }

        public void Add(Doctor doctor)
        {
            if (doctors.Contains(doctor))
                return;
            doctors.Add(doctor);
            doctor.AddConstraint(this);
        }

        public void Remove(Doctor doctor)
        {
            doctors.Remove(doctor);
            doctor.RemoveConstraint(this);
        }
    }
}
