﻿using Harmony.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Application.Events
{
    public class CardMemberAddedEvent
    {
        public CardMemberAddedEvent(Guid cardId, CardMemberDto member)
        {
            CardId = cardId;
            Member = member;
        }

        public Guid CardId { get; set; }
        public CardMemberDto Member { get; set; }
    }
}