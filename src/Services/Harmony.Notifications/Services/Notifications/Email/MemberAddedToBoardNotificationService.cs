﻿using Hangfire;
using Harmony.Persistence.DbContext;
using Harmony.Domain.Enums;
using Harmony.Notifications.Contracts.Notifications.Email;
using Harmony.Application.Notifications.Email;
using Harmony.Application.Configurations;
using Microsoft.Extensions.Options;
using Grpc.Net.Client;
using Harmony.Api.Protos;
using Harmony.Domain.Entities;

namespace Harmony.Notifications.Services.Notifications.Email
{
    public class MemberAddedToBoardNotificationService : BaseNotificationService, IMemberAddedToBoardNotificationService
    {
        private readonly IEmailService _emailNotificationService;
        private readonly BoardService.BoardServiceClient _boardServiceClient;
        private readonly UserService.UserServiceClient _userServiceClient;
        private readonly UserNotificationService.UserNotificationServiceClient _userNotificationServiceClient;
        private readonly AppEndpointConfiguration _endpointConfiguration;

        public MemberAddedToBoardNotificationService(
            IEmailService emailNotificationService,
            NotificationContext notificationContext,
            BoardService.BoardServiceClient boardServiceClient,
            UserService.UserServiceClient userServiceClient,
            UserNotificationService.UserNotificationServiceClient userNotificationServiceClient,
            IOptions<AppEndpointConfiguration> endpointsConfiguration) : base(notificationContext)
        {
            _emailNotificationService = emailNotificationService;
            _boardServiceClient = boardServiceClient;
            _userServiceClient = userServiceClient;
            _userNotificationServiceClient = userNotificationServiceClient;
            _endpointConfiguration = endpointsConfiguration.Value;
        }

        public async Task Notify(MemberAddedToBoardNotification notification)
        {
            await RemovePendingCardJobs(notification.BoardId, EmailNotificationType.MemberAddedToBoard);

            var jobId = BackgroundJob.Enqueue(() => Notify(notification.BoardId, notification.UserId, notification.BoardUrl));

            if (string.IsNullOrEmpty(jobId))
            {
                return;
            }

            _notificationContext.Tasks.Add(new Notification()
            {
                BoardId = notification.BoardId,
                JobId = jobId,
                Type = (int)EmailNotificationType.MemberAddedToBoard,
                DateCreated = DateTime.Now,
            });

            await _notificationContext.SaveChangesAsync();
        }

        public async Task Notify(Guid boardId, string userId, string boardUrl)
        {
            var boardResponse = await _boardServiceClient.GetBoardAsync(new BoardFilterRequest()
            {
                BoardId = boardId.ToString(),
                Workspace = true
            });

            if (!boardResponse.Found)
            {
                return;
            }

            var board= boardResponse.Board;

            var userResponse = await _userServiceClient.GetUserAsync(
                              new UserFilterRequest
                              {
                                  UserId = userId
                              });

            if (!userResponse.Found)
            {
                return;
            }
            var user = userResponse.User;

            var userIsRegisteredResponse = await _userNotificationServiceClient.UserIsRegisterForNotificationAsync(
                              new UserIsRegisterForNotificationRequest()
                              {
                                  UserId = userId,
                                  Type = (int)EmailNotificationType.MemberAddedToBoard
                              });

            if (!userIsRegisteredResponse.IsRegistered)
            {
                return;
            }

            var userBoardAccessResponse = await _boardServiceClient
                .HasUserAccessToBoardAsync(new UserBoardAccessRequest()
                {
                    UserId= userId,
                    BoardId = boardId.ToString(),
                });

            if (!userBoardAccessResponse.HasAccess)
            {
                return;
            }

            var subject = $"Access {board.Title} in {board.Workspace.Name}";

            var content = EmailTemplates.EmailTemplates
                    .BuildFromGenericTemplate(_endpointConfiguration.FrontendUrl,
                    title: $"ACCESS TO BOARD GRANTED",
                    firstName: user.FirstName,
                    emailNotification: $"Accesss to <strong>{board.Title}</strong> granted.",
                    customerAction: $"You have permission to access {board.Title}.",
                buttonText: "VIEW BOARD",
                    buttonLink: boardUrl);

            await _emailNotificationService.SendEmailAsync(user.Email, subject, content);
        }
    }
}
