using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFinder.Models;
using PetFinderApi.Data;

namespace PetFinderApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IJwtService _jwtService;

        public CommentsController(ICommentService commentService, IJwtService jwtService)
        {
            _commentService = commentService;
            _jwtService = jwtService;
        }

        /// <summary>
        ///     Gets a comment
        /// </summary>
        /// <remarks>
        ///     Gets a specific comment by its ID.
        /// </remarks>
        /// <param name="id">Comment's id</param>
        /// <response code="200">Returns the comment</response>
        /// <response code="404">If the comment wasn't found</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("comentarios/{id}")]
        public async Task<ActionResult<Comment>> Get(int id)
        {
            var comment = await _commentService.Get(id);
            if (comment != null)
                return Ok(comment);
            return NotFound();
        }

        /// <summary>
        ///     Creates a comment
        /// </summary>
        /// <remarks>
        ///     Creates a comment by sending its complete content in http body with JSON format
        /// </remarks>
        /// <param name="comment">Comment's content</param>
        /// <response code="201">The comment was created</response>
        /// <response code="400">If the comment has incorrect data like a wrong petId or userId</response>
        /// <response code="401">If the user isn't logged in</response>
        /// <response code="409">If the comment couldn't be created</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost("comentarios")]
        [Authorize]
        public async Task<ActionResult<Comment>> Insert([FromBody] Comment comment)
        {
            var wasCreated = await _commentService.Insert(comment);
            if (wasCreated.Success)
                return Created(new Uri($"{Request.Path}/{comment.Id}", UriKind.Relative), comment);
            if (wasCreated.Errors.Contains(CommentService.ERROR_SAVING_COMMENT)) return Conflict();
            return BadRequest();
        }

        /// <summary>
        ///     Modifies a comment
        /// </summary>
        /// <remarks>
        ///     Modifies a specific comment by its ID.
        /// </remarks>
        /// <param name="id">Comment's id</param>
        /// <param name="comment">Comment's content</param>
        /// <response code="200">The comment was modified</response>
        /// <response code="400">If the comment has incorrect data like a wrong petId or userId</response>
        /// <response code="401">If the user isn't logged in</response>
        /// <response code="404">If the comment wasn't found</response>
        /// <response code="409">If the comment couldn't be modified</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("comentarios/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Comment comment)
        {
            var userEmail = _jwtService.GetUserEmail(HttpContext);
            if (!await _commentService.UserCanEdit(userEmail, id)) return Unauthorized();
            comment.Id = id;
            var wasUpdated = await _commentService.Update(comment);
            if (wasUpdated.Success)
                return Ok(comment);
            if (wasUpdated.Errors.Contains(CommentService.ERROR_COMMENT_NOT_FOUND))
                return NotFound();
            if (wasUpdated.Errors.Contains(CommentService.ERROR_SAVING_COMMENT)) return Conflict();
            return BadRequest();
        }

        /// <summary>
        ///     Deletes a comment
        /// </summary>
        /// <remarks>
        ///     Deletes a specific comment by its ID.
        /// </remarks>
        /// <param name="id">Comment's id</param>
        /// <response code="200">The comment was deleted</response>
        /// <response code="401">If the user isn't logged in</response>
        /// <response code="404">If the comment wasn't found</response>
        /// <response code="409">If the comment couldn't be deleted</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpDelete("comentarios/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var commentExists = await _commentService.Exists(id);
            if (commentExists)
            {
                var userEmail = _jwtService.GetUserEmail(HttpContext);
                if (!await _commentService.UserCanEdit(userEmail, id)) return Unauthorized();
                var wasDeleted = await _commentService.Delete(id);
                if (wasDeleted)
                    return Ok();
                return Conflict();
            }

            return NotFound();
        }

        /// <summary>
        ///     Gets all comments from a pet
        /// </summary>
        /// <remarks>
        ///     Gets all comments from a pet by its ID
        /// </remarks>
        /// <param name="id">Pet's id</param>
        /// <response code="200">Returns the comments</response>
        /// <response code="404">If comments weren't found</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("comentarios/mascota/{id}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllFromPet(int id)
        {
            var comments = await _commentService.GetAllFromPet(id);
            if (comments.Any())
                return Ok(comments);
            return NotFound(comments);
        }
    }
}