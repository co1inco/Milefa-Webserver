using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Milefa_WebServer.Data;
using Milefa_WebServer.Entities;
using Milefa_Webserver.Models;
using Milefa_WebServer.Models;
using Milefa_Webserver.Services;
using Remotion.Linq.Clauses;

namespace Milefa_Webserver.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly CompanyContext _context;
        private readonly IRatingService _ratingService;

        public RatingsController(
            CompanyContext context,
            RatingService ratingService
            )
        {
            _ratingService = ratingService;
            _context = context;
        }

        // GET: api/Ratings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRating()
        {
            var currentUserId = int.Parse(User.Identity.Name);
            return await (from r in _context.Rating where r.UserID == currentUserId select r).ToListAsync();
        }

        // GET: api/Ratings/5
        [Authorize(Roles = RoleStrings.AccessAdmin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRating(int id)
        {
            // Id => userId
            return await (from r in _context.Rating where r.UserID == id select r).ToListAsync();
        }


        // POST: api/Ratings
        [HttpPost]
        public async Task<ActionResult<Rating>> PostRating(Rating rating)
        {
            if (! (User.IsInRole(RoleStrings.Admin) || rating.UserID == int.Parse(User.Identity.Name)))
            {
                return Forbid();
            }

            if (RatingExists(rating))
            {
                return BadRequest();
            }

            _context.Rating.Add(rating);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRating", new { id = rating.ID }, rating);
        }

        // DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rating>> DeleteRating(int id)
        {
            var rating = await _context.Rating.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            if (!(User.IsInRole(RoleStrings.Admin) || rating.UserID == int.Parse(User.Identity.Name)))
            {
                return Forbid();
            }

            _ratingService.RemoveRating(rating);
            await _context.SaveChangesAsync();

            return rating;
        }

        private bool RatingExists(int id)
        {
            return _context.Rating.Any(e => e.ID == id);
        }

        private bool RatingExists(Rating rating)
        {
            return _context.Rating.Any(e => e.UserID == rating.UserID && e.studentID == rating.studentID);
        }
    }
}
