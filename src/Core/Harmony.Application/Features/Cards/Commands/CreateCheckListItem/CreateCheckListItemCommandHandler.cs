﻿using Harmony.Application.Contracts.Repositories;
using Harmony.Shared.Wrapper;
using MediatR;
using Harmony.Domain.Entities;
using Microsoft.Extensions.Localization;
using Harmony.Application.Contracts.Services;
using Harmony.Application.DTO;
using AutoMapper;
using Harmony.Application.Features.Cards.Commands.CreateCheckListItem;

namespace Harmony.Application.Features.Cards.Commands.CreateChecklist
{
    public class CreateCheckListItemCommandHandler : IRequestHandler<CreateCheckListItemCommand, Result<CheckListItemDto>>
    {
        private readonly ICheckListItemRepository _checkListItemRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<CreateChecklistCommandHandler> _localizer;
        private readonly IMapper _mapper;

        public CreateCheckListItemCommandHandler(ICheckListItemRepository checkListItemRepository,
            ICurrentUserService currentUserService,
            IStringLocalizer<CreateChecklistCommandHandler> localizer,
            IMapper mapper)
        {
            _checkListItemRepository = checkListItemRepository;
            _currentUserService = currentUserService;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<CheckListItemDto>> Handle(CreateCheckListItemCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                return await Result<CheckListItemDto>.FailAsync(_localizer["Login required to complete this operator"]);
            }

            var items = await _checkListItemRepository.GetItems(request.CheckListId);

            var newItem = new CheckListItem()
            {
                CheckListId = request.CheckListId,
                Description = request.Description,
                Position = (byte)items.Count,
                DueDate = request.DueDate,
            };

            var dbResult = await _checkListItemRepository.Add(newItem);

            if (dbResult > 0)
            {
                var result = _mapper.Map<CheckListItemDto>(newItem);

                return await Result<CheckListItemDto>.SuccessAsync(result, _localizer["Item Created"]);
            }

            return await Result<CheckListItemDto>.FailAsync(_localizer["Operation failed"]);
        }
    }
}