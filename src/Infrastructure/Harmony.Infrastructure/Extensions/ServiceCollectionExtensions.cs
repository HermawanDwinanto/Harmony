﻿using Harmony.Application.Contracts.Repositories;
using Harmony.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Harmony.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureMappings(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        public static IServiceCollection AddSqlServerRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped<IWorkspaceRepository, WorkspaceRepository>()
                .AddScoped<IUserWorkspaceRepository, UserWorkspaceRepository>()
                .AddScoped<IBoardRepository, BoardRepository>()
                .AddScoped<IUserBoardRepository, UserBoardRepository>()
                .AddScoped<IBoardListRepository, BoardListRepository>()
				.AddScoped<ICardRepository, CardRepository>()
                .AddScoped<ICheckListRepository, CheckListRepository>()
                .AddScoped<ICheckListItemRepository, CheckListItemRepository>()
                .AddScoped<IBoardLabelRepository, BoardLabelRepository>()
                .AddScoped<ICardLabelRepository, CardLabelRepository>()
                .AddScoped<ICardActivityRepository, CardActivityRepository>()
                .AddScoped<IUserCardRepository, UserCardRepository>()
                .AddScoped<IIssueTypeRepository, IssueTypeRepository>()
                .AddScoped<ISprintRepository, SprintRepository>()
                .AddScoped<IUserNotificationRepository, UserNotificationRepository>()
                .AddScoped<ICommentRepository, CommentRepository>()
                .AddScoped<IAttachmentRepository, AttachmentRepository>()
                .AddScoped<IRetrospectiveRepository, RetrospectiveRepository>()
                .AddScoped<ILinkRepository, LinkRepository>();
        }
    }
}