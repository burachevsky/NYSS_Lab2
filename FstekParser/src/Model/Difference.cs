using System;

namespace FstekParser.Model
{
    public class Difference
    {
        public int Id { get; }
        public Threat OldThreat { get; }
        public Threat NewThreat { get; }

        public String Message =>
            OldThreat == null ? "Отсутствует старая вервия" :
            NewThreat == null ? "Отсутствует новая версия" : "Произошли изменения в содержании";

        public Difference(int id, Threat oldThreat, Threat newThreat)
        {
            Id = id;
            OldThreat = oldThreat;
            NewThreat = newThreat;
        }
    }
}
