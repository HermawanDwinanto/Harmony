﻿namespace Harmony.Application.DTO
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public UserPublicInfo User { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
