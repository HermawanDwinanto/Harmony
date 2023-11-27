﻿using Harmony.Application.Features.Boards.Commands.AddUserBoard;
using Harmony.Application.Features.Boards.Commands.Create;
using Harmony.Application.Features.Boards.Commands.CreateSprint;
using Harmony.Application.Features.Boards.Commands.RemoveUserBoard;
using Harmony.Application.Features.Boards.Commands.UpdateUserBoardAccess;
using Harmony.Application.Features.Boards.Queries.Get;
using Harmony.Application.Features.Boards.Queries.GetBoardUsers;
using Harmony.Application.Features.Boards.Queries.GetSprints;
using Harmony.Application.Features.Boards.Queries.SearchBoardUsers;
using Harmony.Application.Features.Cards.Commands.MoveToBacklog;
using Harmony.Application.Features.Cards.Commands.MoveToSprint;
using Harmony.Application.Features.Lists.Commands.UpdateListsPositions;
using Harmony.Application.Features.Lists.Queries.GetBoardLists;
using Harmony.Application.Features.Lists.Queries.LoadBoardList;
using Harmony.Application.Features.Workspaces.Queries.GetBacklog;
using Harmony.Application.Features.Workspaces.Queries.GetIssueTypes;
using Harmony.Application.Features.Workspaces.Queries.GetSprints;
using Microsoft.AspNetCore.Mvc;

namespace Harmony.Server.Controllers.Management
{
    /// <summary>
    /// Controller for Board operations
    /// </summary>
    public class BoardController : BaseApiController<BoardController>
    {
        [HttpPost]
        public async Task<IActionResult> Post(CreateBoardCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> LoadBoard(Guid id, int size)
        {
            return Ok(await _mediator.Send(new GetBoardQuery(id, size)));
        }

        [HttpGet("{id:guid}/members")]
        public async Task<IActionResult> GetMembers(Guid id)
        {
            return Ok(await _mediator.Send(new GetBoardUsersQuery(id)));
        }

        [HttpGet("{id:guid}/members/search")]
        public async Task<IActionResult> SearchMembers(Guid id, string term)
        {
            return Ok(await _mediator.Send(new SearchBoardUsersQuery(id, term)));
        }

        [HttpPost("{id:guid}/members")]
        public async Task<IActionResult> AddUserToBoard(Guid id, AddUserBoardCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("{id:guid}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(Guid id, string userId)
        {
            return Ok(await _mediator.Send(new RemoveUserBoardCommand(id, userId)));
        }

        [HttpPut("{id:guid}/members/{userId}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateUserBoardAccessCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id:guid}/positions")]
        public async Task<IActionResult> UpdateBoardListsPositions(Guid boardId, UpdateListsPositionsCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("{id:guid}/lists/{listId:guid}")]
        public async Task<IActionResult> LoadBoardListCards(Guid id, Guid listId, int page, int maxCards)
        {
            return Ok(await _mediator.Send(new LoadBoardListQuery(id, listId, page, maxCards)));
        }

        [HttpGet("{id:guid}/backlog")]
        public async Task<IActionResult> GetBacklog(Guid id, int pageNumber, int pageSize,
            string searchTerm = null, string orderBy = null, bool membersOnly = false)
        {
            return Ok(await _mediator.Send(new
                GetBacklogQuery(id, pageNumber, pageSize, searchTerm, orderBy)));
        }

        [HttpGet("{id:guid}/issuetypes")]
        public async Task<IActionResult> GetIssueTypes(Guid id)
        {
            return Ok(await _mediator.Send(new GetIssueTypesQuery(id)));
        }

        [HttpGet("{id:guid}/sprints/cards")]
        public async Task<IActionResult> GetSprintCards(Guid id, int pageNumber, int pageSize,
            string searchTerm = null, string orderBy = null)
        {
            return Ok(await _mediator.Send(new
                GetSprintCardsQuery(id, pageNumber, pageSize, searchTerm, orderBy)));
        }

        [HttpGet("{id:guid}/sprints/{sprintId:guid}/cards/pending")]
        public async Task<IActionResult> GetPendingItems(Guid id, Guid sprintId)
        {
            return Ok(await _mediator.Send(new GetPendingSprintCardsQuery(id, sprintId)));
        }

        [HttpGet("{id:guid}/sprints")]
        public async Task<IActionResult> GetSprints(Guid id, int pageNumber, int pageSize,
            string searchTerm = null, string orderBy = null)
        {
            return Ok(await _mediator.Send(new
                GetSprintsQuery(id, pageNumber, pageSize, searchTerm, orderBy)));
        }

        [HttpPost("{id:guid}/sprints")]
        public async Task<IActionResult> CreateSprint(Guid id, CreateEditSprintCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("{id:guid}/boardlists")]
        public async Task<IActionResult> GetBoardLists(Guid id)
        {
            return Ok(await _mediator.Send(new GetBoardListsQuery(id)));
        }

        [HttpPut("{id:guid}/movecardstosprint")]
        public async Task<IActionResult> MoveCardsToSprint(Guid id, MoveToSprintCommand request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPut("{id:guid}/movecardstobacklog")]
        public async Task<IActionResult> MoveCardsToBacklog(Guid id, MoveToBacklogCommand request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
