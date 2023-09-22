﻿using Harmony.Application.Features.Boards.Commands.Create;
using Harmony.Application.Features.Workspaces.Queries.GetAllForUser;
using Harmony.Application.Requests.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Harmony.Client.Shared.Modals
{
    public partial class CreateBoardModal
    {
        private readonly CreateBoardCommand _createBoardModel = new();
        private bool _processing;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        List<GetAllForUserWorkspaceResponse> _ownedWorkspaces = new List<GetAllForUserWorkspaceResponse>();
        GetAllForUserWorkspaceResponse? _selectedWorkspace;

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        protected async override Task OnInitializedAsync()
        {
            var ownedWorkspacesResult = await _workspaceManager.GetAllAsync();

            if (ownedWorkspacesResult.Succeeded)
            {
                _ownedWorkspaces = ownedWorkspacesResult.Data;
            }
        }

        private async Task SubmitAsync()
        {
            _processing = true;
            var response = await _boardManager.CreateAsync(_createBoardModel);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            _processing = false;
        }

        Func<GetAllForUserWorkspaceResponse, string> converter = p => p?.Name;
    }
}