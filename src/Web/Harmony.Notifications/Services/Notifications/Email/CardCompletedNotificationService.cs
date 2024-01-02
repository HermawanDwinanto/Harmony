﻿using Hangfire;
using Harmony.Application.Contracts.Repositories;
using Harmony.Application.Specifications.Cards;
using Harmony.Notifications.Persistence;
using Harmony.Application.Extensions;
using Microsoft.EntityFrameworkCore;
using Harmony.Application.Contracts.Services.Identity;
using Harmony.Domain.Enums;
using Harmony.Notifications.Contracts.Notifications.Email;
using Harmony.Application.Notifications.Email;

namespace Harmony.Notifications.Services.Notifications.Email
{
    public class CardCompletedNotificationService : BaseNotificationService, ICardCompletedNotificationService
    {
        private readonly IEmailService _emailNotificationService;
        private readonly IUserService _userService;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly ICardRepository _cardRepository;

        public CardCompletedNotificationService(IEmailService emailNotificationService,
            IUserService userService,
            IUserNotificationRepository userNotificationRepository,
            NotificationContext notificationContext,
            ICardRepository cardRepository) : base(notificationContext)
        {
            _emailNotificationService = emailNotificationService;
            _userService = userService;
            _userNotificationRepository = userNotificationRepository;
            _cardRepository = cardRepository;
        }

        public async Task Notify(CardCompletedNotification notification)
        {
            var cardId = notification.Id;

            await RemovePendingCardJobs(cardId, EmailNotificationType.CardCompleted);

            var card = await _cardRepository.Get(cardId);

            if (card == null)
            {
                return;
            }

            var jobId = BackgroundJob.Schedule(() => Notify(cardId), TimeSpan.FromSeconds(10));

            if (string.IsNullOrEmpty(jobId))
            {
                return;
            }

            _notificationContext.Notifications.Add(new Notification()
            {
                CardId = cardId,
                JobId = jobId,
                Type = EmailNotificationType.CardCompleted,
                DateCreated = DateTime.Now,
            });

            await _notificationContext.SaveChangesAsync();
        }


        public async Task Notify(Guid cardId)
        {
            var filter = new CardNotificationSpecification(cardId);

            var card = await _cardRepository
                .Entities.Specify(filter)
                .FirstOrDefaultAsync();

            if (card == null || card.BoardList.CardStatus != BoardListCardStatus.DONE)
            {
                return;
            }

            var subject = $"{card.Title} in {card.BoardList.Board.Title} completed";
            var cardMembers = (await _userService.GetAllAsync(card.Members.Select(m => m.UserId))).Data;

            var registeredUsers = await _userNotificationRepository
                .GetUsersForType(cardMembers.Select(m => m.Id).ToList(), EmailNotificationType.CardCompleted);

            foreach (var member in cardMembers.Where(m => registeredUsers.Contains(m.Id)))
            {
                var content = $"Dear {member.FirstName} {member.LastName}, {card.Title} has been completed. ";

                await _emailNotificationService.SendEmailAsync(member.Email, subject, content);
            }
        }
    }
}
