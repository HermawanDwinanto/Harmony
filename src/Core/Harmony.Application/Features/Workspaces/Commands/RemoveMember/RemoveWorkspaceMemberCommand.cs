﻿using Harmony.Shared.Wrapper;
using MediatR;

namespace Harmony.Application.Features.Workspaces.Commands.RemoveMember
{
    public class RemoveWorkspaceMemberCommand : IRequest<Result<bool>>
    {
        public string UserId { get; set; }
        public Guid WorkspaceId { get; set; }
    }
}
