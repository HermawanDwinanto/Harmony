﻿using Harmony.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Domain.Entities
{
    public class Sprint : AuditableEntity<Guid>
    {
        public string Name { get; set; }
        public string Goal {  get; set; }
        public Board Board { get; set; }
        public Guid BoardId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SprintStatus Status { get; set; }
        public List<Card> Cards { get; set; }
    }
}