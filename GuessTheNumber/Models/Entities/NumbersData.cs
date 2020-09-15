using System;
using System.Collections.Generic;

namespace GuessTheNumber.Models
{
    public class NumbersData
    {
        public Guid Id { get; set; }
        public int PredefinedNumber { get; set; }
        public int UsedAttempts { get; set; }
        public ICollection<GuessedNumber> GuessedNumbers { get; set; } = new HashSet<GuessedNumber>();
    }
}