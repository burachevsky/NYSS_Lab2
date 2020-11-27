using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FstekParser
{
    public class Difference
    {
        public int Id { get; }
        public Threat OldThreat { get; }
        public Threat NewThreat { get; }

        public String Message =>
            OldThreat == null ? "Старая версия отсутствует" :
            NewThreat == null ? "Новая версия отсутствует" : "";

        public Difference(int id, Threat oldThreat, Threat newThreat)
        {
            Id = id;
            OldThreat = oldThreat;
            NewThreat = newThreat;
        }
    }
}
