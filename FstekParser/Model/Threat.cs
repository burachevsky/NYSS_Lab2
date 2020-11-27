using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FstekParser
{
    [Serializable]
    public class Threat : IComparable<Threat>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public bool ConfidenceViolation { get; set; }
        public bool IntegrityViolation { get; set; }
        public bool AccessViolation { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime LastModificationDate { get; set; }

        public bool Equals(Threat other)
        {
            return LastModificationDate.Equals(other.LastModificationDate) 
                   && Id == other.Id 
                   && Name.Equals(other.Name) 
                   && Description.Equals(other.Description) 
                   && Source.Equals(other.Source) 
                   && Target.Equals(other.Target) 
                   && ConfidenceViolation == other.ConfidenceViolation 
                   && IntegrityViolation == other.IntegrityViolation 
                   && AccessViolation == other.AccessViolation 
                   && UploadDate.Equals(other.UploadDate);
        }

        public static bool operator >(Threat ths, Threat oth)
        {
            return ths.Id > oth.Id;
        }

        public static bool operator <(Threat ths, Threat oth)
        {
            return ths.Id < oth.Id;
        }

        public static bool operator >=(Threat ths, Threat oth)
        {
            return ths.Id >= oth.Id;
        }

        public static bool operator <=(Threat ths, Threat oth)
        {
            return ths.Id <= oth.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() + 
                   Name.GetHashCode() +
                   Description.GetHashCode() + 
                   Source.GetHashCode() + 
                   Target.GetHashCode() + 
                   ConfidenceViolation.GetHashCode() + 
                   IntegrityViolation.GetHashCode() + 
                   AccessViolation.GetHashCode() + 
                   UploadDate.GetHashCode() + 
                   LastModificationDate.GetHashCode();
        }

        public int CompareTo(Threat other)
        {
            return this.Id - other.Id;
        }
    }
}