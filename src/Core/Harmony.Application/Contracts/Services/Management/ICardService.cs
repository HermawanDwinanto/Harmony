﻿using Harmony.Application.DTO;
using Harmony.Application.Features.Boards.Queries.GetArchivedItems;
using Harmony.Application.Features.Boards.Queries.GetBacklog;
using Harmony.Domain.Entities;
using Harmony.Domain.Enums;
using Harmony.Shared.Wrapper;

namespace Harmony.Application.Contracts.Services.Management
{
    /// <summary>
    /// Service for Cards
    /// </summary>
    public interface ICardService
	{
		Task<bool> PositionCard(Card card, Guid? listId, short position, CardStatus status);
        Task<List<GetBacklogItemResponse>> SearchBacklog(Guid boardId, string term, int pageNumber, int pageSize);
        Task<List<GetArchivedItemResponse>> SearchArchivedItems(Guid boardId, string term, int pageNumber, int pageSize);
        Task<IResult<List<Card>>> MoveCardsToSprint(Guid boardId, List<Guid> cardsToMove, Guid sprintId, Guid boardListId);
        Task<IResult<Card>> AddCardToSprint(Guid boardId, string userId, Guid sprintId, Guid issueTypeId, Guid boardListId, string title);
        Task<IResult<List<Card>>> MoveCardsToBacklog(Guid boardId, List<Guid> cardsToMove);
        Task<bool> CardCompleted(Guid cardId);
        Task<IResult<List<Card>>> ReactivateCards(Guid boardId, List<Guid> cardIds, Guid boardListId);
        Task ReorderCardsAfterArchive(Guid listId, short archivedCardPositionn);
        Task<IResult<short?>> SyncParentStoryPoints(Guid parentCardId);
        Task<bool> CanUpdateStoryPoints(Guid boardId, Guid cardId, Guid? issueTypeId);
        Task<bool> HasActiveChildren(Guid cardId);
        Task<int> CountWorkspaceCards(Guid workspaceId, string term, Guid? skipCardId = null);
        Task<List<CardDto>> SearchWorkspaceCards(Guid workspaceId, string term, int pageNumber, int pageSize, Guid? skipCardId = null);
    }
}
