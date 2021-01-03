using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetFinder.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using PetFinder.Areas.Identity;
using Microsoft.AspNetCore.Identity;
using PetFinder.Data;
using PetFinderApi.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace PetFinderApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly PetFinderContext _context;
        private readonly ICommentService _commentService;
        private readonly IJWTService _JWTService;

        public CommentsController(ILogger<CommentsController> logger,
                                    PetFinderContext context,
                                    ICommentService commentService,
                                    IJWTService jwtService)
        {
            _logger = logger;
            _context = context;
            _commentService = commentService;
            _JWTService = jwtService;
        }

        // Obtiene un comentario segun su ID
        /// <summary>
        /// Gets a comment
        /// </summary>
        /// <remarks>
        /// Gets a specific comment by its ID.
        /// </remarks>
        /// <param name="id">Comment's id</param>
        /// <response code="200">Returns the comment</response>
        /// <response code="404">If the comment wasn't found</response>
        /// <response code="401">If the user isn't loged or can't delete</response>

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("comentarios/{id}")]
        public async Task<ActionResult<Comment>> Get(int id)
        {
            Comment comment = await _commentService.Get(id);
            if (comment != null)
            {
                return Ok(comment);
            }
            else
            {
                return NotFound();
            }
        }

        // Crea un comentario
        /// <summary>
        /// Creates a comment
        /// </summary>
        /// <remarks>
        /// Creates a comment by sending its complete content in http body with JSON format
        /// </remarks>
        /// <param name="comment">Comment's content</param>
        /// <response code="201">The comment was created</response>
        /// <response code="409">If the comment couldn't be created</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost("comentarios")]
        [Authorize]
        public async Task<ActionResult<Comment>> Insert([FromBody] Comment comment)
        {
            bool wasCreated = await _commentService.Insert(comment);
            if (wasCreated)
            {
                return Created(new Uri($"{Request.Path}/{comment.Id}", UriKind.Relative), comment);
                //return StatusCode(201, comment);
            }
            else
            {
                return Conflict();
            }
        }

        // Modifica un comentario segun su ID
        /// <summary>
        /// Modifies a comment
        /// </summary>
        /// <remarks>
        /// Modifies a specific comment by its ID.
        /// </remarks>
        /// <param name="id">Comment's id</param>
        /// <param name="comment">Comment's content</param>
        /// <response code="200">The comment was modified</response>
        /// <response code="404">If the comment wasn't found</response>
        /// <response code="409">If the comment couldn't be modified</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("comentarios/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Comment comment)
        {
            string userEmail = _JWTService.GetUserEmail(HttpContext);
            if (!await _commentService.userCanEdit(userEmail, id))
            {
                return Unauthorized();
            }
            bool commentExists = await _commentService.Exists(id);
            if (commentExists)
            {
                comment.Id = id;
                bool wasUpdated = await _commentService.Update(comment);
                if (wasUpdated)
                {
                    return Ok(comment);
                }
                else
                {
                    return Conflict();
                }
            }
            else
            {
                return NotFound();
            }
        }

        // Elimina un comentario segun su ID
        /// <summary>
        /// Deletes a comment
        /// </summary>
        /// <remarks>
        /// Deletes a specific comment by its ID.
        /// </remarks>
        /// <param name="id">Comment's id</param>
        /// <response code="200">The comment was deleted</response>
        /// <response code="404">If the comment wasn't found</response>
        /// <response code="409">If the comment couldn't be deleted</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)] 
        [HttpDelete("comentarios/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            bool commentExists = await _commentService.Exists(id);
            if (commentExists)
            {
                string userEmail = _JWTService.GetUserEmail(HttpContext);
                if (!await _commentService.userCanEdit(userEmail, id))
                {
                    return Unauthorized();
                }

                bool wasDeleted = await _commentService.Delete(id);
                if (wasDeleted)
                {
                    return Ok();
                }
                else
                {
                    return Conflict();
                }
            }
            else
            {
                return NotFound();
            }
        }

        // Obtiene todos los comentarios de una mascota segun su ID
        /// <summary>
        /// Gets all comments from a pet
        /// </summary>
        /// <remarks>
        /// Gets all comments from a pet by its ID
        /// </remarks>
        /// <param name="id">Pet's id</param>
        /// <response code="200">Returns the comments</response>
        /// <response code="404">If comments weren't found</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("comentarios/mascota/{id}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllFromPet(int id)
        {
            IEnumerable<Comment> comments = await _commentService.GetAllFromPet(id);
            if (comments.Any())
            {
                return Ok(comments);
            }
            else
            {
                return NotFound(comments);
            }
        }
    }
}
