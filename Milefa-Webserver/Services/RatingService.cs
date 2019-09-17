


using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly CompanyContext _context;

        public RatingService(CompanyContext context)
        {
            _context = context;
        }

        public void RemoveRating(Student student)
        {
            var rating = _context.Rating.AsNoTracking().FirstOrDefault(i => i.StudentID == student.ID);

            if (rating == null)
            {
                return;
            }

            RemoveRatingSkills(rating);
            _context.Rating.Remove(rating);
            _context.SaveChanges();

        }
        public  void RemoveRating(User user)
        {
            var rating = _context.Rating.AsNoTracking().FirstOrDefault(i => i.UserID == user.ID);

            if (rating == null)
            {
                return;
            }
            RemoveRatingSkills(rating);
            _context.Rating.Remove(rating);
            _context.SaveChanges();

        }

        public void RemoveRating(Rating rating)
        {
            if (rating == null)
            {
                return;
            }
            RemoveRatingSkills(rating);
            _context.Rating.Remove(rating);
            _context.SaveChanges();

        }

        public void RemoveRatingSkills(Rating rating)
        {
            var r = (from s in _context.RatingAssignments where s.RatingID == rating.ID select s).AsNoTracking().ToList();
            _context.RatingAssignments.RemoveRange(r);
        }

        public void RemoveRatingSkills(Skill skill)
        {
            var r = (from s in _context.RatingAssignments where s.SkillID == skill.ID select s).AsNoTracking().ToList();
            _context.RatingAssignments.RemoveRange(r);
        }
    }
}