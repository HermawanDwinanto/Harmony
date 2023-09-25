﻿using Harmony.Application.Contracts.Repositories;
using Harmony.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using Harmony.Application.Contracts.Services;
using Harmony.Application.DTO;
using AutoMapper;
using Harmony.Application.Contracts.Services.Management;

namespace Harmony.Application.Features.Cards.Commands.UpdateCardTitle;

public class UpdateCardTitleCommandHandler : IRequestHandler<UpdateCardTitleCommand, Result<bool>>
{
	private readonly ICardRepository _cardRepository;
	private readonly ICurrentUserService _currentUserService;
	private readonly IStringLocalizer<UpdateCardTitleCommandHandler> _localizer;

	public UpdateCardTitleCommandHandler(ICardRepository cardRepository,
		ICurrentUserService currentUserService,
		IStringLocalizer<UpdateCardTitleCommandHandler> localizer)
	{
		_cardRepository = cardRepository;
		_currentUserService = currentUserService;
		_localizer = localizer;
	}
	public async Task<Result<bool>> Handle(UpdateCardTitleCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserService.UserId;

		if (string.IsNullOrEmpty(userId))
		{
			return await Result<bool>.FailAsync(_localizer["Login required to complete this operator"]);
		}

		var card = await _cardRepository.Get(request.CardId);

		card.Title = request.Title;

		// commit all the changes
		var updateResult = await _cardRepository.Update(card);

		if (updateResult > 0)
		{
			
			return await Result<bool>.SuccessAsync(true, _localizer["Title updated"]);
		}

		return await Result<bool>.FailAsync(_localizer["Operation failed"]);
	}
}
