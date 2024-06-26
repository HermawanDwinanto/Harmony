﻿using Harmony.Application.Contracts.Repositories;
using Harmony.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using Harmony.Application.Contracts.Services;
using Harmony.Application.Constants;
using Harmony.Application.Contracts.Messaging;
using Harmony.Application.Notifications;
using Harmony.Domain.Enums;
using Harmony.Application.Contracts.Services.Caching;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Harmony.Application.Features.Lists.Commands.UpdateListItemChecked
{
    public class UpdateListItemCheckedCommandHandler : IRequestHandler<UpdateListItemCheckedCommand, Result<bool>>
    {
        private readonly ICheckListItemRepository _checkListItemRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICardRepository _cardRepository;
        private readonly INotificationsPublisher _notificationsPublisher;
        private readonly ICardSummaryService _cardSummaryService;
        private readonly IStringLocalizer<UpdateListItemCheckedCommandHandler> _localizer;

        public UpdateListItemCheckedCommandHandler(ICheckListItemRepository checkListItemRepository,
            ICurrentUserService currentUserService,
            ICardRepository cardRepository,
            INotificationsPublisher notificationsPublisher,
            ICardSummaryService cardSummaryService,
            IStringLocalizer<UpdateListItemCheckedCommandHandler> localizer)
        {
            _checkListItemRepository = checkListItemRepository;
            _currentUserService = currentUserService;
            _cardRepository = cardRepository;
            _notificationsPublisher = notificationsPublisher;
            _cardSummaryService = cardSummaryService;
            _localizer = localizer;
        }
        public async Task<Result<bool>> Handle(UpdateListItemCheckedCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                return await Result<bool>.FailAsync(_localizer["Login required to complete this operator"]);
            }

            var listItem = await _checkListItemRepository.Get(request.ListItemId);

            listItem.IsChecked = request.IsChecked;
            var dbResult = await _checkListItemRepository.Update(listItem);
            if (dbResult > 0)
            {
                await _cardSummaryService.UpdateCardSummary(request.BoardId, request.CardId,
                (summary) =>
                {
                    var checkList = summary.CheckLists.FirstOrDefault(c => c.CheckListId == listItem.CheckListId);

                    if (checkList != null)
                    {
                        if(!request.IsChecked && checkList.TotalItemsChecked == 0)
                        {
                            return;
                        }
                        checkList.TotalItemsChecked += request.IsChecked ? 1 : -1;
                    }
                });

                var message = new CardItemCheckedChangedMessage(request.BoardId, request.CardId, 
                    listItem.Id, listItem.IsChecked);

                _notificationsPublisher.PublishMessage(message,
                    NotificationType.CardItemCheckedChanged, routingKey: BrokerConstants.RoutingKeys.SignalR);

                return await Result<bool>.SuccessAsync(true, _localizer["List item Checked updated"]);
            }

            return await Result<bool>.FailAsync(_localizer["Operation failed"]);
        }
    }
}
