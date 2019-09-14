


using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Milefa_WebServer.Data;
using Milefa_Webserver.Models;
using Milefa_WebServer.Models;
using Remotion.Linq.Clauses;

namespace Milefa_Webserver.Services
{
    public interface IRatingService
    {
        void RemoveRating(Student student);
        void RemoveRating(User user);
        void RemoveRating(Rating rating);
        void RemoveRatingSkills(Rating rating);
        void RemoveRatingSkills(Skill skill);
    }

    public class RatingService : IRatingService
    {
        private CompanyContext _context;

        public RatingService(CompanyContext context)
        {
            _context = context;
        }

        public async void RemoveRating(Student student)
        {
            var rating = await _context.Rating.FirstOrDefaultAsync(i => i.studentID == student.ID);

            if (rating == null)
            {
                return;
            }

            RemoveRatingSkills(rating);
            _context.Rating.Remove(rating);

        }
        public async void RemoveRating(User user)
        {
            var rating = await _context.Rating.FirstOrDefaultAsync(i => i.UserID == user.ID);

            if (rating == null)
            {
                return;
            }
            RemoveRatingSkills(rating);
            _context.Rating.Remove(rating);
        }

        public void RemoveRating(Rating rating)
        {
            if (rating == null)
            {
                return;
            }
            RemoveRatingSkills(rating);
            _context.Rating.Remove(rating);
        }

        public async void RemoveRatingSkills(Rating rating)
        {
            var r = await (from s in _context.RatingAssignments where s.RatingID == rating.ID select s).ToListAsync();
            _context.RatingAssignments.RemoveRange(r);
        }

        public async void RemoveRatingSkills(Skill skill)
        {
            var r = await (from s in _context.RatingAssignments where s.SkillID == skill.ID select s).ToListAsync();
            _context.RatingAssignments.RemoveRange(r);
        }
    }
}