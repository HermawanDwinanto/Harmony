﻿using Harmony.Application.DTO;
using Harmony.Application.Features.Cards.Commands.CreateSprintIssue;
using Harmony.Application.Features.Sprints.Commands.CompleteSprint;
using Harmony.Application.Features.Sprints.Commands.StartSprint;
using Harmony.Application.Features.Sprints.Queries.GetSprintCards;
using Harmony.Application.Features.Sprints.Queries.GetSprintReports;
using Harmony.Client.Infrastructure.Extensions;
using Harmony.Shared.Wrapper;
using System.Net.Http.Json;

namespace Harmony.Client.Infrastructure.Managers.Project
{
    /// <summary>
    /// Client manager for Sprints
    /// </summary>
    public class SprintManager : ISprintManager
    {
        private readonly HttpClient _httpClient;

        public SprintManager(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<IResult> StartSprint(StartSprintCommand request)
        {
            var response = await _httpClient
                .PutAsJsonAsync(Routes.SprintEndpoints.Start(request.SprintId), request);

            return await response.ToResult<bool>();
        }

        public async Task<IResult<bool>> CompleteSprint(CompleteSprintCommand request)
        {
            var response = await _httpClient
                .PostAsJsonAsync(Routes.SprintEndpoints.Complete(request.SprintId), request);

            return await response.ToResult<bool>();
        }

        public async Task<IResult<CardDto>> CreateSprintCardAsync(CreateSprintIssueCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.SprintEndpoints
                .CreateIssue(request.SprintId), request);

            return await response.ToResult<CardDto>();
        }

        public async Task<IResult<SprintDto>> GetSprint(Guid sprintId)
        {
            var response = await _httpClient
                .GetAsync(Routes.SprintEndpoints.Sprint(sprintId));

            return await response.ToResult<SprintDto>();
        }

        public async Task<IResult<GetSprintReportsResponse>> GetSprintReports(Guid sprintId)
        {
            var response = await _httpClient
                .GetAsync(Routes.SprintEndpoints.Reports(sprintId));

            return await response.ToResult<GetSprintReportsResponse>();
        }

        public async Task<PaginatedResult<CardDto>> GetSprintCards(GetSprintCardsQuery query)
        {
            var response = await _httpClient
                .GetAsync(Routes.SprintEndpoints.Cards(query.SprintId, query.PageNumber, 
                query.PageSize, query.SearchTerm, query.OrderBy, query.Status));

            return await response.ToPaginatedResult<CardDto>();
        }
    }
}
