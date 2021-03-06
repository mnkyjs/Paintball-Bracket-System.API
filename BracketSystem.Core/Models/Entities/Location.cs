﻿using System.Collections.Generic;

namespace BracketSystem.Core.Models.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Paintballfield> Paintballfields { get; set; } = new List<Paintballfield>();
    }
}