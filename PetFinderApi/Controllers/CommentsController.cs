using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetFinder.Models;
using Microsoft.EntityFrameworkCore;

namespace PetFinderApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly PetFinderContext _context;

        public CommentsController(ILogger<CommentsController> logger, PetFinderContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("comentario/{id}")]
        public string GetAsync()
        {
            return "Get";
        }

        [HttpGet("comentarios")]
        public string GetAllAsync()
        {
            return "GetAll";
        }

        [HttpPost("comentarios")]
        public string Add()
        {
            return "Add";
        }

        [HttpGet("comentarios/{id}")]
        public string GetAllFromPet()
        {
            return "GetAllFromPet";
        }

        [HttpDelete("comentarios/{id}")]
        public string Delete()
        {
            return "Delete";
        }

        [HttpPost("auth")]
        public string SignIn()
        {
            return "SignIn";
        }
    }
}
