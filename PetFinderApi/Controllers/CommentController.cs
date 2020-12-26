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
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {

        private readonly ILogger<CommentController> _logger;
        private readonly PetFinderContext _context;

        public CommentController(ILogger<CommentController> logger, PetFinderContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Comments>> GetAsync()
        {
            return await _context.Comments.ToListAsync();
        }
    }
}
