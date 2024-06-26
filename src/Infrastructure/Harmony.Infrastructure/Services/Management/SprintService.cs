﻿using AutoMapper;
using Harmony.Application.Contracts.Repositories;
using Harmony.Application.Contracts.Services.Management;
using Harmony.Application.DTO;
using Harmony.Application.Features.Sprints.Queries.GetSprintReports;
using Harmony.Domain.Entities;
using Harmony.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Harmony.Infrastructure.Services.Management
{
    public class SprintService : ISprintService
    {
        private readonly ISprintRepository _sprintRepository;
        private readonly IMapper _mapper;
        private readonly ICardRepository _cardRepository;

        public SprintService(ISprintRepository sprintRepository, IMapper mapper,
                                ICardRepository cardRepository)
        {
            _sprintRepository = sprintRepository;
            _mapper = mapper;
            _cardRepository = cardRepository;
        }

        public async Task<GetSprintReportsResponse?> GetSprintReports(Guid sprintId)
        {
            var sprint = await _sprintRepository.GetSprint(sprintId);

            if (sprint == null || !sprint.StartDate.HasValue || !sprint.EndDate.HasValue)
            {
                return null;
            }

            var sprintCards = await (from card in _cardRepository.Entities.Include(c => c.IssueType)
                                     where card.SprintId == sprintId
                                     select card).ToListAsync();

            var completedCards = sprintCards.Where(c => c.DateCompleted.HasValue);

            var result = new GetSprintReportsResponse();

            var totalStoryPoints = sprintCards.Sum(c => c.StoryPoints) ?? 0;
            var remainingStoryPoints = totalStoryPoints;
            double guideLineRemainingStoryPoints = totalStoryPoints;
            var burnDownReportDates = new List<string>();
            var remainingStoryPointsSeries = new List<double>();
            var guideLineStoryPointsSeries = new List<double>();
            var totalWorkDays = EachDay(sprint.StartDate.Value, sprint.EndDate.Value)
                .Where(date => date.DayOfWeek != DayOfWeek.Saturday
                && date.DayOfWeek != DayOfWeek.Sunday).Count();

            var averageStoryPointsPerDay = (double)totalStoryPoints / (double)(totalWorkDays - 1);

            if (totalStoryPoints == 0)
            {
                return null;
            }

            var sprintDays = EachDay(sprint.StartDate.Value, sprint.EndDate.Value).ToList();
            var firstDay = true;

            foreach (var day in sprintDays.OrderBy(date => date))
            {
                burnDownReportDates.Add(day.Date.ToString("MMM dd"));

                foreach (var completedCard in completedCards
                    .Where(c => c.DateCompleted.Value.Date == day.Date))
                {
                    remainingStoryPoints -= (int)(completedCard.StoryPoints ?? 0);
                }

                remainingStoryPointsSeries.Add(remainingStoryPoints);

                if (firstDay)
                {
                    guideLineStoryPointsSeries.Add(guideLineRemainingStoryPoints);
                    firstDay = false;
                    continue;
                }

                if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
                {
                    guideLineRemainingStoryPoints -= averageStoryPointsPerDay;
                }

                guideLineStoryPointsSeries.Add(guideLineRemainingStoryPoints);
            }


            var burnDownReport = new BurnDownReportDto()
            {
                Name = "BurnDown chart",
                Dates = burnDownReportDates,
                RemainingStoryPoints = remainingStoryPointsSeries,
                GuideLineStoryPoints = guideLineStoryPointsSeries
            };

            var issuesOverviewReport = new IssuesOverviewReportDto()
            {
                TotalIssues = sprintCards.Count
            };

            foreach (var issueType in sprintCards.Select(c => c.IssueType).Distinct())
            {
                var cardsInIssue = sprintCards.Count(c => c.IssueType == issueType);
                issuesOverviewReport.TotalIssuesPerType.Add(cardsInIssue);
                issuesOverviewReport.IssueTypes.Add(issueType.Summary);
            }

            result.BurnDownReport = burnDownReport;
            result.IssuesOverviewReport = issuesOverviewReport;
            result.TotalStoryPoints = totalStoryPoints;
            result.RemainingStoryPoints = remainingStoryPoints;
            result.Sprint = _mapper.Map<SprintDto>(sprint);

            return result;
        }

        public async Task<List<Card>> GetSprintsCards(Guid sprintId, string term,
            int pageNumber, int pageSize, CardStatus? status)
        {
            var query = _cardRepository.Entities
                .Where(card => status == null ? true : card.Status == status.Value &&
                card.SprintId == sprintId &&
                        (string.IsNullOrEmpty(term) ? true : card.Title
                        .Contains(term, StringComparison.InvariantCultureIgnoreCase)));

            var result = await query.Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize).ToListAsync();

            return result;
        }


        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}
