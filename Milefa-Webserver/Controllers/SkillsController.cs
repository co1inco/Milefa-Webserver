using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Milefa_Webserver.Data;
using Milefa_Webserver.Models;

namespace Milefa_Webserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly CompanyContext _context;

        public SkillsController(CompanyContext context)
        {
            _context = context;
        }

        // GET: api/Skills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            if (GetUserAccsessLevel() < AccsessLevel.Normal)
                return StatusCode(403);

            return await _context.Skills.ToListAsync();
        }

        // GET: api/Skills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
            if (GetUserAccsessLevel() < AccsessLevel.Normal)
                return StatusCode(403);

            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            return skill;
        }

        // PUT: api/Skills/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkill(int id, Skill skill)
        {
            if (GetUserAccsessLevel() < AccsessLevel.Admin)
                return StatusCode(403);

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
        [HttpPost]
        public async Task<ActionResult<Skill>> PostSkill(Skill skill)
        {
            if (GetUserAccsessLevel() < AccsessLevel.Admin)
                return StatusCode(403);

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSkill", new { id = skill.ID }, skill);
        }

        // DELETE: api/Skills/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Skill>> DeleteSkill(int id)
        {
            if (GetUserAccsessLevel() < AccsessLevel.Admin)
                return StatusCode(403);

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
        //TODO: Bether autentification (asp.net Accsess controll)?
        /// <summary>
        /// Get the user accsess Level from query
        /// </summary>
        /// <returns></returns>
        private AccsessLevel GetUserAccsessLevel()
        {
            string user = Request.Query["user"];
            string password = Request.Query["password"];

            if (user == "Colin" && password == "q")
                return AccsessLevel.Sysadmin;
            if (user == "User" && password == "x")
                return AccsessLevel.Normal;
            if (user == "x" && password == "x")
                return AccsessLevel.Normal;

            return AccsessLevel.None;
        }
    }
}
