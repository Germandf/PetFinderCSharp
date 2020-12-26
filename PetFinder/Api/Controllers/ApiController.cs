using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFinder.Models;

namespace PetFinder.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ApiController> _logger;
        private readonly PetFinderContext _context;

        public ApiController(ILogger<ApiController> logger, PetFinderContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet ("comments")]
        public async Task<IEnumerable<Comments>> GetAsync()
        {
            return await _context.Comments.ToListAsync();
        }
    }
}
