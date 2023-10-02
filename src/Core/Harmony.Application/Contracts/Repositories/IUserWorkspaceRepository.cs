﻿using Harmony.Application.Features.Workspaces.Queries.GetWorkspaceUsers;
using Harmony.Domain.Entities;

namespace Harmony.Application.Contracts.Repositories
{
    public interface IUserWorkspaceRepository
    {
        /// <summary>
        /// Create a user workspace
        /// </summary>
        /// <param name="workspace"></param>
        /// <returns></returns>
        Task<int> CreateAsync(UserWorkspace userWorkspace);
        Task<int> CountWorkspaceUsers(Guid workspaceId);
        Task<List<string>> GetWorkspaceUsers(Guid workspaceId, int pageNumber, int pageSize);
        Task<List<string>> FindWorkspaceUsers(Guid workspaceId, List<string> userIds);
        Task<int> RemoveAsync(UserWorkspace userWorkspace);
        Task<UserWorkspace?> GetUserWorkspace(Guid workspaceId, string userId);
        Task<List<UserWorkspaceResponse>> SearchWorkspaceUsers(Guid workspaceId, string term, int pageNumber, int pageSize);
        Task<int> CountWorkspaceUsers(Guid workspaceId, string term, int pageNumber, int pageSize);
    }
}