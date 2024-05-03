using System;

namespace DrawLots
{
    public class Participant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateWon { get; set; }
    }
}