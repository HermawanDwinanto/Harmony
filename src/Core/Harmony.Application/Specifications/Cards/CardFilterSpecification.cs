﻿using BlazorHero.CleanArchitecture.Application.Specifications.Base;
using Harmony.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Application.Specifications.Cards
{
    public class CardFilterSpecification : HarmonySpecification<Card>
    {
        public CardFilterSpecification(Guid? cardId, CardIncludes includes)
        {
            if(includes.Attachments)
            {
                Includes.Add(card => card.Attachments);
            }

            if(cardId.HasValue)
            {
                Criteria = card => card.Id == cardId;
            }
        }
    }
}