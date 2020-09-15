using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GuessTheNumber.Models
{
    public class GuessedNumber
    {
        public Guid Id { get; set; }
        public Guid NumbersDataId { get; set; }
        public int Number { get; set; }
        [JsonIgnore]
        public NumbersData NumbersData { get; set; }
        
    }
}