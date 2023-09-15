﻿using Harmony.Domain.Enums;

namespace Harmony.Domain.Entities
{
    /// <summary>
    /// Represents a user's board
    /// </summary>
    public class Board : AuditableEntity<Guid>
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public BoardVisibility Visibility { get; set; }
        public List<BoardList> Lists { get; set; }
        public List<UserBoard> Users { get; set; }
        public List<BoardLabel> Labels { get; set; }
    }
}
