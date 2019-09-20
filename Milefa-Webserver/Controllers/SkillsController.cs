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
using Milefa_WebServer.Models;
using Milefa_Webserver.Services;

namespace Milefa_WebServer.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly CompanyContext _context;
        private readonly IRatingService _ratingService;

        public SkillsController(
            CompanyContext context,
            IRatingService ratingService
            )
        {
            _context = context;
            _ratingService = ratingService;
        }

        // GET: api/Skills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            return await _context.Skills.ToListAsync();
        }

        // GET: api/Skills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            return skill;
        }

        // PUT: api/Skills/5
        [Authorize(Roles = RoleStrings.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkill(int id, Skill skill)
        {
            if (id != skill.ID)
            {
                return BadRequest();
            }

            _context.Entry(skill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Skills
        [Authorize(Roles = RoleStrings.Admin)]
        [HttpPost]
        public async Task<ActionResult<Skill>> PostSkill(Skill skill)
        {

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSkill", new { id = skill.ID }, skill);
        }

        // DELETE: api/Skills/5
        [Authorize(Roles = RoleStrings.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Skill>> DeleteSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound();
            }

            _context.Skills.Remove(skill);
            DeleteSkillLinks(skill);
            await _context.SaveChangesAsync();

            return skill;
        }

        private bool SkillExists(int id)
        {
            return _context.Skills.Any(e => e.ID == id);
        }

        private void DeleteSkillLinks(Skill skill)
        {
            var toDeleteStudents = _context.StudentSkills.Where(f => f.SkillID == skill.ID);
            _context.StudentSkills.RemoveRange(toDeleteStudents);
            var toDeleteDepartments = _context.RequiredSkills.Where(f => f.SkillID == skill.ID);
            _context.RequiredSkills.RemoveRange(toDeleteDepartments);
            _ratingService.RemoveRatingSkills(skill);
        }

    }
}
