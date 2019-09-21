using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Options;
using Milefa_Webserver.Entities;
using Milefa_WebServer.Data;
using Milefa_WebServer.Entities;
using Milefa_WebServer.Helpers;
using Milefa_WebServer.Models;
using Milefa_Webserver.Services;
using Milefa_WebServer.Services;

namespace Milefa_WebServer.Controllers
{

    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        
        private readonly AppSettings _appSettings;
        private readonly CompanyContext _context;
        private readonly IUserService _userService;
        private readonly IRatingService _ratingService;
        private readonly IRolesService _roleService;

        public StudentsController(
            CompanyContext context,
            IUserService user,
            IOptions<AppSettings> appSettings,
            IRatingService ratingService,
            IRolesService rolesService
            )
        {
            _userService = user;
            _appSettings = appSettings.Value;
            _context = context;
            _ratingService = ratingService;
            _roleService = rolesService;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _context.Students
                .Include(i => i.DeployedDep)
                    .ThenInclude(i => i.RequiredSkills)
                .Include(i => i.Choise1)
                    .ThenInclude(i => i.RequiredSkills) 
                .Include(i => i.Choise2)
                    .ThenInclude(i => i.RequiredSkills)
                .Include(i => i.Skills)
                .ToListAsync();

            var currentUserId = int.Parse(User.Identity.Name);
            foreach (Student s in students)
            {
                s.User = null;
                if (s.Skills != null)
                {
                    s.Skills = await GetSkills(s.ID);
                }

                if (!User.IsInRole(RoleStrings.Admin))
                {
                    s.Name = null;
                    s.School = null;
                    s._Class = null;
                    s.Gender = null;
                }
                    
            }

            return students;
        }

        [Authorize(Roles = RoleStrings.HumanResource)]
        [HttpGet("assign")]
        public async Task<ActionResult<IEnumerable<Student>>> AssignAllStudents()
        {
            var students = await _context.Students
                .Include(i => i.DeployedDep)
                .ThenInclude(i => i.RequiredSkills)
                .Include(i => i.Choise1)
                .ThenInclude(i => i.RequiredSkills)
                .Include(i => i.Choise2)
                .ThenInclude(i => i.RequiredSkills)
                .Include(i => i.Skills)
                .ToArrayAsync();

            var currentUserId = int.Parse(User.Identity.Name);
            foreach (Student s in students)
            {
                s.User = null;
                if (s.Skills != null)
                {
                    s.Skills = await GetSkills(s.ID);
                }

                if (!User.IsInRole(RoleStrings.Admin))
                {
                    s.Name = null;
                    s.School = null;
                    s._Class = null;
                    s.Gender = null;
                }
            }

            var departments = await _context.Departments
                .Include(i => i.RequiredSkills)
                .ToArrayAsync();

            foreach (Department department in departments)
            {
                department.RequiredSkills = GetDepartmentReq(department.ID);
            }

            AutoDepartmentAssignment.GenerateUserAssignments(students, departments, out students);

            foreach (Student student in students)
            {
                await TryUpdateModelAsync(
                    student,
                    "",
                    s => s.DeployedDepID);
            }

            await _context.SaveChangesAsync();

            return students;
        }


        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
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
                student.Skills = await GetSkills(student.ID);
            }

            if (!User.IsInRole(RoleStrings.Admin))
            {
                student.Name = null;
                student.School = null;
                student._Class = null;
                student.Gender = null;
            }

            //We don't have to return the Userdata here, also the Json response becomes invalid if send to non admin users?!
            student.User = null;

            return student;
        }

        /// <summary>
        /// Update Student Entry
        /// </summary>
        /// <param name="id"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleStrings.HumanResource)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.ID)
            {
                return BadRequest();
            }
            student.DateValide = student.DateValide.Date;

            var studentToUpdate = await _context.Students.FirstOrDefaultAsync(i => i.ID == student.ID);
            if (studentToUpdate == null)
            {
                return BadRequest("Student not found");
            }

            if (student.UserID != null)
                studentToUpdate.UserID = student.UserID;

            if (User.IsInRole(RoleStrings.Admin))
            {
                if (student.Name != null)
                    studentToUpdate.Name = student.Name;
                if (student.School != null)
                    studentToUpdate.School = student.School;
                if (student._Class != null)
                    studentToUpdate._Class = student._Class;
                if (student.Gender != null)
                    studentToUpdate.Gender = student.Gender;
            }
            if (student.DeployedDepID != null)
                studentToUpdate.DeployedDepID = student.DeployedDepID;
            if (student.DateValide != null)
                studentToUpdate.DateValide = student.DateValide;
            studentToUpdate.Breakfast = student.Breakfast;
            studentToUpdate.Lunch = student.Lunch;
            studentToUpdate.Choise1ID = student.Choise1ID;
            studentToUpdate.Choise2ID = student.Choise2ID;
            studentToUpdate.PersNr = student.PersNr;

            await ModifySkills(student, student.Skills);

            await AssignUserRole(student);

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
        [Authorize(Roles = RoleStrings.HumanResource)]
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent([Bind(
            "Name,School,_Class,Gender,DateValide,PersNr,Breakfast,Lunch,DeployedDepID,Choise1ID,Choise2ID,Skills"
            )] Student student)
        {

            if (StudentExists(student))
            {
                return BadRequest("Student already exists");
            }

            var skills = student.Skills;
            student.DateValide = student.DateValide.Date;

            // Not copying to another object results in EF6 trying to Identity_Insert (copying does not copy ID) 
            var newStudent = new Student(student);

            var newUser = new User { Username = GenerateStudentUsername(student), Type = Usertypes.Student, StudentID = newStudent.ID };
            var u = _userService.CreateOrReset(newUser, _appSettings.NewUserPass);

            newStudent.UserID = u.ID;
            _context.Students.Add(newStudent);

            await ModifySkills(newStudent, skills);
            await _context.SaveChangesAsync();

            newUser.StudentID = newStudent.ID;
            _context.Entry(newUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            newStudent.Skills = skills;
            newStudent.User = null;
            return CreatedAtAction("GetStudent", new { id = newStudent.ID }, newStudent);
        }


        // DELETE: api/Students/5
        [Authorize(Roles = RoleStrings.HumanResource)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> DeleteStudent(int id)
        {
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(i => i.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            await RemoveStudent(student);
            return student;
        }

        [Authorize(Roles = RoleStrings.Admin)]
        [HttpDelete("all/{date}")]
        public async Task<ActionResult<Student[]>> DeleteAllStudents(DateTime date)
        {
            date = date.Date;

            var del = await (from s in _context.Students where s.DateValide == date select s).AsNoTracking().ToListAsync();
            foreach (Student student in del)
            {
                await RemoveStudent(student);
            }

            return del.ToArray();
        }


        // --- Utility ---

        /// <summary>
        /// Check if the StudentID already exists in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }

        /// <summary>
        /// Check if the student already exists by PersNr and DateValide
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        private bool StudentExists(Student student)
        {
            return _context.Students.Any(e => (e.PersNr == student.PersNr && e.DateValide == student.DateValide));
        }

        private HashSet<Skill> GetDepartmentReq(int DepartmentID)
        {
            var skills = new HashSet<Skill>();

            var skillIDs = _context.RequiredSkills.Where(
                x => x.DepartmentID == DepartmentID);

            foreach (var sk in skillIDs)
            {
                var skill = _context.Skills.Single(i => i.ID == sk.SkillID);
                if (skill != null)
                    skills.Add(skill);
            }
            return skills;
        }

        private async Task<HashSet<Skill>> GetSkills(int studentId)
        {
            var skills = new HashSet<Skill>();

            var skillIDs = await _context.StudentSkills.Where(x => x.StudentID == studentId).ToArrayAsync();

            foreach (var sk in skillIDs)
            {
                var skill = await _context.Skills.SingleAsync(i => i.ID == sk.SkillID);
                if (skill != null)
                    skills.Add(skill);
            }
            return skills;
        }

        private async Task ModifySkills(Student student, ICollection<Skill> skills)
        {
            if (skills == null)
                return;

            var linkedSkills = await _context.StudentSkills.AsNoTracking().Where(i => i.StudentID == student.ID).ToListAsync();

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

        private string GenerateStudentUsername(Student student)
        {
            return student.DateValide.Year.ToString()
                + student.DateValide.Month.ToString()
                + student.DateValide.Day.ToString()
                + student.PersNr.ToString();
        }

        private async Task RemoveStudent(Student student)
        {
            if (student.UserID == null)
            {
                return;
            }
            _userService.Delete(student.UserID.Value);
            await _context.SaveChangesAsync();

            await ModifySkills(student, new List<Skill>());
            await _context.SaveChangesAsync();

 //           _ratingService.RemoveRating(student);
 //           _context.SaveChanges();

            var delStudent = await _context.Students.AsNoTracking().FirstOrDefaultAsync(i => i.ID == student.ID);
            if (delStudent != null)
            {
                _context.Students.Remove(delStudent);
            }
            await _context.SaveChangesAsync();
        }

        private async Task AssignUserRole(Student student)
        {
            if (student.DeployedDepID != null &&
                student.UserID != null)
            {
                // don't want to remove Admin Rights here
                foreach (string availableRole in RoleStrings.AvailableRoles)
                {
                    await _roleService.RemoveUserRole(student.UserID.Value, availableRole);
                }

                //await _context.SaveChangesAsync();


                var d = _context.Departments.FindAsync(student.DeployedDepID);
                if (d != null)
                {
                    var roleToAssign = d.Result.AssignmentRole;

                    var found = RoleStrings.AvailableRoles.Any(availableRole => availableRole == roleToAssign);

                    if (found)
                    {
                        _roleService.AddUserRoles(student.UserID.Value, roleToAssign); // saves changes here!!!
                    }

                }
            }
        }
    }
}
