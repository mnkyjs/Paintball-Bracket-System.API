using System.Collections.Generic;
using il_y.BracketSystem.Core.Models.Entities;

namespace il_y.BracketSystem.Core.Models.Dtos
{
    public class CreateScheduleDto
    {
        public bool AddClashToAnExistingOne { get; set; }
        public string Date { get; set; }
        public List<Team> Teams { get; set; }
        public int PaintballfieldId { get; set; }
        public string Name { get; set; }
    }
}