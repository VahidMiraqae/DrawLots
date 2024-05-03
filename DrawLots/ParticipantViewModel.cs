using System;

namespace DrawLots
{
    internal class ParticipantViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateWon { get; internal set; }
    }
}