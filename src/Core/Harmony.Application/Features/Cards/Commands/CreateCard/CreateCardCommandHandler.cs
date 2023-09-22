﻿using Harmony.Application.Contracts.Repositories;
using Harmony.Shared.Wrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony.Domain.Entities;
using Microsoft.Extensions.Localization;
using Harmony.Application.Contracts.Services;
using Harmony.Application.Features.Boards.Commands.CreateList;
using Harmony.Application.DTO;
using AutoMapper;
using Harmony.Application.Features.Boards.Commands.Create;

namespace Harmony.Application.Features.Cards.Commands.CreateCard
{
    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, Result<CardDto>>
    {
        private readonly ICardRepository _cardRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<CreateCardCommandHandler> _localizer;
        private readonly IMapper _mapper;

        public CreateCardCommandHandler(ICardRepository cardRepository,
            ICurrentUserService currentUserService,
            IStringLocalizer<CreateCardCommandHandler> localizer,
            IMapper mapper)
        {
            _cardRepository = cardRepository;
            _currentUserService = currentUserService;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<CardDto>> Handle(CreateCardCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                return await Result<CardDto>.FailAsync(_localizer["Login required to complete this operator"]);
            }

            var totalCards = await _cardRepository.CountCards(request.ListId);

            var card = new Card()
            {
                Name = request.Name,
                UserId = userId,
                BoardListId = request.ListId,
                Position = (byte)totalCards
            };

            var dbResult = await _cardRepository.Add(card);

            if (dbResult > 0)
            {
                var result = _mapper.Map<CardDto>(card);
                return await Result<CardDto>.SuccessAsync(result, _localizer["Card Created"]);
            }

            return await Result<CardDto>.FailAsync(_localizer["Operation failed"]);
        }
    }
}