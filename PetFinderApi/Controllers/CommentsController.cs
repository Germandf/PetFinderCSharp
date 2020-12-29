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
        [HttpGet("comentarios/{id}")]
        public async Task<ActionResult<Comment>> Get(int id)
        {
            return await _commentService.Get(id);
        } 

        // Crea un comentario
        [HttpPost("comentarios")]
        public async Task<IActionResult> Insert([FromBody] Comment comment)
        {
            await _commentService.Insert(comment);
            return Accepted();
        }

        // Modifica un comentario segun su ID
        [HttpPut("comentarios/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Comment comment)
        {
            comment.Id = id;
            await _commentService.Update(comment);
            return Accepted();
        }

        // Elimina un comentario segun su ID
        [HttpDelete("comentarios/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _commentService.Delete(id);
            return Accepted();
        }

        // Obtiene todos los comentarios de una mascota segun su ID
        [HttpGet("comentarios/mascota/{id}")]
        public async Task<IEnumerable<Comment>> GetAllFromPet(int id)
        {
            return await _commentService.GetAllFromPet(id);
        }

        // Logea al usuario
        [HttpPost("auth")]
        public string SignIn()
        {
            return "SignIn";
        }
    }
}
