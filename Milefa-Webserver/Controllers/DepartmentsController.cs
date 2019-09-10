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
    public class DepartmentsController : ControllerBase
    {
        private readonly CompanyContext _context;

        public DepartmentsController(CompanyContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            if (GetUserAccsessLevel() < AccsessLevel.Normal)
                return StatusCode(403);

            var departments = await _context.Departments
                .Include(i => i.RequiredSkills)
                .ToListAsync();

            foreach (var d in departments)
            {
                if (d.RequiredSkills != null)
                {
                    d.RequiredSkills = GetSkills(d.ID);
                }
            }

            return departments;
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            if (GetUserAccsessLevel() < AccsessLevel.Normal)
                StatusCode(403);

            var department = await _context.Departments
                .Include(i => i.RequiredSkills)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (department.RequiredSkills != null)
            {
                department.RequiredSkills = GetSkills(department.ID);
            }

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (GetUserAccsessLevel() < AccsessLevel.Admin)
                return StatusCode(403);

            if (id != department.ID)
            {
                return BadRequest();
            }

            _context.Entry(department).State = EntityState.Modified;
            ModifySkills(department, department.RequiredSkills);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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

        /// <summary>
        /// Add new Department
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            if (GetUserAccsessLevel() < AccsessLevel.Admin)
                return StatusCode(403);

            var skills = department.RequiredSkills;
            department.RequiredSkills = null;

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            ModifySkills(department, skills);
            await _context.SaveChangesAsync();

            department.RequiredSkills = skills;
            return CreatedAtAction("GetDepartment", new { id = department.ID }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            if (GetUserAccsessLevel() < AccsessLevel.Admin)
                return StatusCode(403);

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            ModifySkills(department, new List<Skill>());
            await _context.SaveChangesAsync();

            return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.ID == id);
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


        private Skill[] GetSkills(int DepartmentID)
        {
            var skills = new List<Skill>();

            var skillIDs = _context.RequiredSkills.Where(
                x => x.DepartmentID== DepartmentID);

            foreach (var sk in skillIDs)
            {
                var skill = _context.Skills.Single(i => i.ID == sk.SkillID);
                if (skill != null)
                    skills.Add(skill);
            }
            return skills.ToArray();
        }

        private void ModifySkills(Department department, ICollection<Skill> skills)
        {
            if (skills == null)
                return;

            var linkedSkills = _context.RequiredSkills.Where(i => i.DepartmentID == department.ID).ToList();

            foreach (Skill skill in skills)
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
                    _context.RequiredSkills.Add(new RequiredSkill { DepartmentID = department.ID, SkillID = skill.ID });
                }
            }

            _context.RequiredSkills.RemoveRange(linkedSkills);
        }
    }
}
