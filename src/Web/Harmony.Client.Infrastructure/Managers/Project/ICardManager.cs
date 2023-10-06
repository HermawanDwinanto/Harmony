﻿using Harmony.Application.DTO;
using Harmony.Application.Features.Cards.Commands.CreateCard;
using Harmony.Application.Features.Cards.Commands.MoveCard;
using Harmony.Application.Features.Cards.Commands.ToggleCardLabel;
using Harmony.Application.Features.Cards.Commands.UpdateCardDescription;
using Harmony.Application.Features.Cards.Commands.UpdateCardStatus;
using Harmony.Application.Features.Cards.Commands.UpdateCardTitle;
using Harmony.Application.Features.Cards.Queries.GetLabels;
using Harmony.Application.Features.Cards.Queries.LoadCard;
using Harmony.Shared.Wrapper;

namespace Harmony.Client.Infrastructure.Managers.Project
{
    public interface ICardManager : IManager
    {
        Task<IResult<LoadCardResponse>> LoadCardAsync(LoadCardQuery request);
        Task<IResult<CardDto>> CreateCardAsync(CreateCardCommand request);
        Task<IResult<CardDto>> MoveCardAsync(MoveCardCommand request);
        Task<IResult<bool>> UpdateDescriptionAsync(UpdateCardDescriptionCommand request);
        Task<IResult<bool>> UpdateTitleAsync(UpdateCardTitleCommand request);
        Task<IResult<bool>> UpdateStatusAsync(UpdateCardStatusCommand request);
        Task<IResult<List<LabelDto>>> GetCardLabelsAsync(GetCardLabelsQuery request);
        Task<IResult<LabelDto>> ToggleCardLabel(ToggleCardLabelCommand request);
    }
}
