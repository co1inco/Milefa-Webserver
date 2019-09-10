using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Milefa_WebServer.Data;
using Milefa_WebServer.Models;

namespace Milefa_WebServer.Controllers
{
    enum AccsessLevel
    {
        None    = 0,
        Normal  = 1,
        Admin   = 2,
        Sysadmin= 3
    }

    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        //TODO: Use ASP.NET Autentication Logic (Replace GetUserAccsessLevel)

        private readonly CompanyContext _context;

        public StudentsController(CompanyContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var al = GetUserAccsessLevel();
            if (al < AccsessLevel.Normal)
                return StatusCode(403);

            var students = await _context.Students
                .Include(i => i.DeployedDep)
                    .ThenInclude(i => i.RequiredSkills)
                .Include(i => i.Choise1)
                    .ThenInclude(i => i.RequiredSkills) 
                .Include(i => i.Choise2)
                    .ThenInclude(i => i.RequiredSkills)
                .Include(i => i.Skills)
                .ToListAsync();

            foreach (Student s in students) {

                if (s.Skills != null)
                {
                    s.Skills = GetSkills(s.ID);
                }

                if (al < AccsessLevel.Admin)
                {
                    s.Name = null;
                    s.School = null;
                    s._Class = null;
                    s.Gender = null;
                }
                    
            }

            return students;
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var al = GetUserAccsessLevel();

            if (al < AccsessLevel.Normal)
                return StatusCode(403);

            var student = await _context.Students
                .Include(i => i.DeployedDep)
                    .ThenInclude(i => i.RequiredSkills)
                .Include(i => i.Choise1)
                    .ThenInclude(i => i.RequiredSkills)
                .Include(i => i.Choise2)
                    .ThenInclude(i => i.RequiredSkills)
                .Include(i => i.Skills)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (student == null)
            {
                return NotFound();
            }

            if (student.Skills != null)
            {
                student.Skills = GetSkills(student.ID);
            }

            if (al < AccsessLevel.Admin)
            {
                student.Name = null;
                student.School = null;
                student._Class = null;
                student.Gender = null;
            }

            return student;
        }

        /// <summary>
        /// Update Student Entry
        /// </summary>
        /// <param name="id"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (GetUserAccsessLevel() < AccsessLevel.Normal)
                StatusCode(403);

            if (id != student.ID)
            {
                return BadRequest();
            }
            student.DateValide = student.DateValide.Date;

            _context.Entry(student).State = EntityState.Modified;
            ModifySkills(student, student.Skills);
//            if (await TryUpdateModelAsync<Student>)
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/Students
        /// <summary>
        /// Add a new Student to the Database
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            //Autenticate
            if (GetUserAccsessLevel() < AccsessLevel.Normal)
            {
                return StatusCode(403);
            }

            if (StudentExists(student))
            {
                return BadRequest();
            }

            var skills = student.Skills;
            student.Skills = null; // Skills has to be null to avoid IDENTITY_INSERT Error

            student.DateValide = student.DateValide.Date;

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            if (skills != null)
            {
                //after SaveChanges() to get new Student id
                ModifySkills(student, skills);
                await _context.SaveChangesAsync();
                student.Skills = skills;
            }

            return CreatedAtAction("GetStudent", new { id = student.ID }, student);
        }


        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> DeleteStudent(int id)
        {
            //Autenticate
            if (GetUserAccsessLevel() < AccsessLevel.Normal)
            {
                return StatusCode(403);
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            ModifySkills(student, new List<Skill>());
            await _context.SaveChangesAsync();

            return student;
        }

        [HttpDelete("all/{date}")]
        public async Task<ActionResult<Student[]>> DeleteAllStudents(DateTime date)
        {
            //Autenticate
            if (GetUserAccsessLevel() < AccsessLevel.Admin)
            {
                return StatusCode(403);
            }

            date = date.Date;

            var del = new List<Student>();
            await _context.Students.ForEachAsync(s =>
                {
                    if (s.DateValide.Date == date)
                    {
                        del.Add(s);
                        ModifySkills(s, new List<Skill>());
                    }
                });

            _context.Students.RemoveRange(del);
            await _context.SaveChangesAsync();

            return del.ToArray();
        }


        // --- Utility ---

        /// <summary>
        /// Check if the studentID already exists in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }

        /// <summary>
        /// Check if the student allready exists by PersNr and DateValide
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        private bool StudentExists(Student student)
        {
            return _context.Students.Any(e => (e.PersNr == student.PersNr && e.DateValide == student.DateValide));
        }

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


        private Skill[] GetSkills(int studentID)
        {
            var skills = new List<Skill>();

            var skillIDs = _context.StudentSkills.Where(
                x => x.StudentID == studentID);

            foreach (var sk in skillIDs)
            {
                var skill = _context.Skills.Single(i => i.ID == sk.SkillID);
                if (skill != null)
                    skills.Add(skill);
            }
            return skills.ToArray();
        }

        private void ModifySkills(Student student, ICollection<Skill> skills)
        {
            if (skills == null)
                return;

            var linkedSkills = _context.StudentSkills.Where(i => i.StudentID == student.ID).ToList();

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
                    _context.StudentSkills.Add(new StudentSkill { StudentID = student.ID, SkillID = skill.ID });
                }
            }

            _context.StudentSkills.RemoveRange(linkedSkills);
        }
    }
}
