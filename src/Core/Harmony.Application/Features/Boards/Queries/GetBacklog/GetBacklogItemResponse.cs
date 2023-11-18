﻿using Harmony.Application.DTO;
using Harmony.Application.Responses;
using Harmony.Domain.Entities;

namespace Harmony.Application.Features.Boards.Queries.GetBacklog
{
    public class GetBacklogItemResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string SerialKey { get; set; }
        public IssueTypeDto IssueType { get; set; }
        public short Position { get; set; }
    }
}
