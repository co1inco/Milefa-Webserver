using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Milefa_WebServer.Data;
using Milefa_WebServer.Entities;
using Milefa_Webserver.Models;
using Milefa_WebServer.Models;
using Milefa_Webserver.Services;
using Remotion.Linq.Clauses;

namespace Milefa_Webserver.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly CompanyContext _context;
        private readonly IRatingService _ratingService;

        public RatingsController(
            CompanyContext context,
            IRatingService ratingService
            )
        {
            _ratingService = ratingService;
            _context = context;
        }

        // GET: api/Ratings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRating()
        {
            var userId = int.Parse(User.Identity.Name);
            var ratings = await (
                from r in _context.Rating
                where r.UserID == userId
                select r).ToListAsync();

            foreach (var rating in ratings)
            {
                rating.Skills = await GetLinkedSkills(rating.ID);
            }

            return ratings;
        }

        // GET: api/Ratings/5
        //[Authorize(Roles = RoleStrings.AccessAdmin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRating(int id)
        {
            // Id => userId
            return await (from r in _context.Rating where r.UserID == id select r).ToListAsync();
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRating(int id, Rating rating)
        {
            if (id != rating.ID)
            {
                return BadRequest();
            }

            if (rating.UserID == 0)
            {
                rating.UserID = int.Parse(User.Identity.Name);
            }

            _context.Entry(rating).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(rating) || !RatingExists(rating.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            await ModifySkills(rating);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Ratings
        [HttpPost]
        public async Task<ActionResult<Rating>> PostRating(Rating rating)
        {
            //if (! (User.IsInRole(RoleStrings.Admin) || rating.UserID == int.Parse(User.Identity.Name)))
            //{
            //    return Forbid();
            //}

            if (rating.UserID == 0)
            {
                rating.UserID = int.Parse(User.Identity.Name);
            }

            if (RatingExists(rating))
            {
                return BadRequest();
            }

            Rating newRating = new Rating()
            {
                Skills = rating.Skills,
                UserID = rating.UserID,
                StudentID = rating.StudentID,
            };


            _context.Rating.Add(newRating);
            await _context.SaveChangesAsync();

            await ModifySkills(newRating);
            await _context.SaveChangesAsync();

            newRating.User = null;
            return CreatedAtAction("GetRating", new { id = newRating.ID }, newRating);
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
            return _context.Rating.Any(e => (e.UserID == rating.UserID && e.StudentID == rating.StudentID));
        }

        private async Task<HashSet<Skill>> GetLinkedSkills(int ratingId)
        {
            var skillIds =
                await (from r in _context.RatingAssignments where r.RatingID == ratingId select r.SkillID).ToListAsync();

            HashSet<Skill> skills = new HashSet<Skill>();
            foreach (int skillId in skillIds)
            {
                var skill = _context.Skills.FirstOrDefault(s => s.ID == skillId);
                if (skill != null)
                    skills.Add(skill);
            }
            return skills;
        }

        private async Task ModifySkills(Rating rating)
        {

            var linkedSkills = await _context.RatingAssignments.AsNoTracking().Where(i => i.RatingID == rating.ID).ToListAsync();

            foreach (Skill skill in rating.Skills)
            {
                bool found = false;
                foreach (var f in linkedSkills)
                {
                    if (f.SkillID == skill.ID)
                    {
                        found = true;
                        linkedSkills.Remove(f);
                        break;
                    }
                }

                if (!found)
                {
                    await _context.RatingAssignments.AddAsync(new RatingAssignment { RatingID = rating.ID, SkillID = skill.ID });
                }
            }
            // Remove all elements that got not removed from linkedSkills
            _context.RatingAssignments.RemoveRange(linkedSkills);
        }
    }
}
