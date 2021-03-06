﻿using BracketSystem.Core.Models.Entities;
using System.Collections.Generic;

namespace BracketSystem.Core.Models.Dtos
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