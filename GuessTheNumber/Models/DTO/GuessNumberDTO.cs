using System;

namespace GuessTheNumber.Models.DTO
{
    public class GuessNumberDTO
    {
        public Guid NumberRecordId { get; set; }
        public int GuessNumber { get; set; }
    }
}