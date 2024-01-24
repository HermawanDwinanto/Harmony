using Grpc.Core;
using Harmony.Application.Contracts.Repositories;
using Harmony.Application.Specifications.Cards;
using Microsoft.EntityFrameworkCore;
using Harmony.Application.Extensions;
using Harmony.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Harmony.Api.Services.gRPC
{
    public class CardService : Protos.CardService.CardServiceBase
    {
        private readonly ILogger<CardService> _logger;
        private readonly ICardRepository _cardRepository;

        public CardService(ILogger<CardService> logger, ICardRepository cardRepository)
        {
            _logger = logger;
            _cardRepository = cardRepository;
        }

        public override async Task<Protos.Card> GetCard(Protos.CardFilterRequest request,
            ServerCallContext context)
        {
            var includes = new CardIncludes()
            {
                Attachments = request.Attachments,
                Board = request.Board,
                BoardList = request.BoardList,
                IssueType = request.IssueType,
                Children = request.Children
            };

            var cardId = Guid.Parse(request.CardId);

            var filter = new CardFilterSpecification(cardId, includes);

            var card = await _cardRepository
                .Entities.IgnoreQueryFilters().Specify(filter)
                .FirstOrDefaultAsync();

            return MapToProto(card);
        }

        public override async Task<Protos.MoveCardToListResponse> MoveCardToList(Protos.MoveCardToListRequest request, ServerCallContext context)
        {
            var includes = new CardIncludes() {};
            var cardId = Guid.Parse(request.CardId);

            var filter = new CardFilterSpecification(cardId, includes);
            var moveToBoardListId = Guid.Parse(request.BoardListId);

            var card = await _cardRepository
                .Entities.IgnoreQueryFilters().Specify(filter)
                .FirstOrDefaultAsync();

            card.BoardListId = moveToBoardListId;

            // make this the last card in the list
            var totalCards = await _cardRepository.CountCards(moveToBoardListId);
            card.Position = (short)totalCards;

            var updateResult = await _cardRepository.Update(card);

            var result = new Protos.MoveCardToListResponse()
            {
                Success = updateResult > 0,
                NewPosition  = card.Position
            };

            return result;
        }

        private Protos.Card MapToProto(Card card)
        {
            if (card == null)
            {
                return null;
            }

            var protoCard = new Protos.Card()
            {
                CardId = card.Id.ToString(),
                Position = card.Position,
                BoardListId = card.BoardListId?.ToString()
            };

            if (card.Children != null && card.Children.Any())
            {
                protoCard.Children.AddRange(card.Children.Select(MapToProto));
            }

            return protoCard;
        }
    }
}