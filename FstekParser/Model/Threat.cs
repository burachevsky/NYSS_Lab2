using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FstekParser
{
    [Serializable]
    public class Threat
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
                   && Id == other.Id && Name.Equals(other.Name) 
                   && Description.Equals(other.Description) 
                   && Source.Equals(other.Source) 
                   && Target.Equals(other.Target) 
                   && ConfidenceViolation == other.ConfidenceViolation 
                   && IntegrityViolation == other.IntegrityViolation 
                   && AccessViolation == other.AccessViolation 
                   && UploadDate.Equals(other.UploadDate);
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
    }
}