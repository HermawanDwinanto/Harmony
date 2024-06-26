﻿using Harmony.Application.Contracts.Repositories;
using Harmony.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using Harmony.Application.Contracts.Services;
using Harmony.Application.DTO;
using AutoMapper;
using Harmony.Application.Contracts.Services.Management;

namespace Harmony.Application.Features.Cards.Commands.UpdateBacklog
{
    public class UpdateBacklogCommandHandler : IRequestHandler<UpdateBacklogCommand, Result<bool>>
    {
        private readonly ICardRepository _cardRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICardService _cardService;
        private readonly IStringLocalizer<UpdateBacklogCommandHandler> _localizer;
        private readonly IMapper _mapper;

        public UpdateBacklogCommandHandler(ICardRepository cardRepository,
            ICurrentUserService currentUserService,
            ICardService cardService,
            IStringLocalizer<UpdateBacklogCommandHandler> localizer,
            IMapper mapper)
        {
            _cardRepository = cardRepository;
            _currentUserService = currentUserService;
            _cardService = cardService;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<bool>> Handle(UpdateBacklogCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                return await Result<bool>.FailAsync(_localizer["Login required to complete this operator"]);
            }

            var card = await _cardRepository.Get(request.CardId);

            if (card == null)
            {
                return await Result<bool>.FailAsync(_localizer["Card doesn't exist"]);
            }

            var canUpdateStoryPoints = await _cardService
                .CanUpdateStoryPoints(request.BoardId, request.CardId, card.IssueTypeId);

            if (!canUpdateStoryPoints)
            {
                return await Result<bool>
                    .FailAsync($"There's an active 'Sum up story points' automation: {Environment.NewLine}" +
                    $"Set the story points at the subtask level to update the parent story points or " +
                    $"remove the specific issue type from the automation");
            }

            card.IssueTypeId = request.IssueType.Id;
            card.Title = request.Title;
            card.StoryPoints = request.StoryPoints;

            var dbResult = await _cardRepository.Update(card);

            if (dbResult > 0)
            {
                var result = _mapper.Map<CardDto>(card);
                return await Result<bool>.SuccessAsync(true, _localizer["Issue Updated"]);
            }

            return await Result<bool>.FailAsync(_localizer["Operation failed"]);
        }
    }
}
