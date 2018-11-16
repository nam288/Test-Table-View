using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Serializable]
    public class Operation: IEquatable<Operation>
    {
        public Time time;
        public List<Doctor> doctors = new List<Doctor>();
        public Operation(Time time)
        {
            this.time = time;
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

        public override string ToString() => string.Format("{0,-15}{1,-15}{2,-15}", "Operation", false, time);

        public override bool Equals(object obj)
        {
            return Equals(obj as Operation);
        }

        public bool Equals(Operation t)
        {
            if (ReferenceEquals(t, null))
                return false;
            if (ReferenceEquals(this, t))
                return true;
            return time.Equals(t.time);
        }

        public override int GetHashCode()
        {
            return time.GetHashCode();
        }
    }
}
