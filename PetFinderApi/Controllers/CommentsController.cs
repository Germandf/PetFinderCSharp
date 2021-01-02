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

namespace PetFinderApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly PetFinderContext _context;
        private readonly ICommentService _commentService;

        public CommentsController(  ILogger<CommentsController> logger, 
                                    PetFinderContext context,
                                    ICommentService commentService)
        {
            _logger = logger;
            _context = context;
            _commentService = commentService;
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
        [Produces("application/json")]
        [HttpGet("comentarios/{id}")]
        public async Task<ActionResult<Comment>> Get(int id)
        {
            return await _commentService.Get(id);
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
        [HttpPost("comentarios")]
        public async Task<IActionResult> Insert([FromBody] Comment comment)
        {
            await _commentService.Insert(comment);
            return Accepted();
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
        [HttpPut("comentarios/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Comment comment)
        {
            comment.Id = id;
            await _commentService.Update(comment);
            return Accepted();
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
        [HttpDelete("comentarios/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _commentService.Delete(id);
            return Accepted();
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
        /// <response code="404">If the pet wasn't found</response>
        /// <response code="409">If the comment couldn't be gotten</response>
        [Produces("application/json")]
        [HttpGet("comentarios/mascota/{id}")]
        public async Task<IEnumerable<Comment>> GetAllFromPet(int id)
        {
            return await _commentService.GetAllFromPet(id);
        }
    }
}
