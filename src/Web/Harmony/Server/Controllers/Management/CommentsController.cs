﻿using Harmony.Application.Features.Comments.Commands.CreateComment;
using Harmony.Application.Features.Comments.Queries.GetCardComments;
using Microsoft.AspNetCore.Mvc;

namespace Harmony.Server.Controllers.Management
{
    /// <summary>
    /// Controller for comments operations
    /// </summary>
    public class CommentsController : BaseApiController<CommentsController>
    {

        [HttpPost]
        public async Task<IActionResult> Post(CreateCommentCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("{cardId:guid}")]
        public async Task<IActionResult> Get(Guid cardId)
        {
            return Ok(await _mediator.Send(new GetCardCommentsCommandQuery(cardId)));
        }
    }
}
