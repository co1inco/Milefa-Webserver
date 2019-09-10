using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Milefa_WebServer.Models;
using Milefa_WebServer.Entities;

namespace Milefa_WebServer.Data
{
    public class CompanyContext : DbContext
    {
        public CompanyContext(DbContextOptions<CompanyContext> options) : base (options)
        {
        }
        
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Skill> Skills { get; set; }

        // Link table for m:n relations
        public DbSet<RequiredSkill> RequiredSkills { get; set; }
        public DbSet<StudentSkill> StudentSkills { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Skill>().ToTable("Skill");

            //TODO: Change Tablename
            modelBuilder.Entity<RequiredSkill>().ToTable("RequredSkillAssignment");
            modelBuilder.Entity<StudentSkill>().ToTable("StudentSkill");

            modelBuilder.Entity<RequiredSkill>()
                .HasKey(c => new { c.SkillID, c.DepartmentID });
            modelBuilder.Entity<StudentSkill>()
                .HasKey(c => new { c.StudentID, c.SkillID });
        }
        
        public DbSet<Milefa_WebServer.Entities.User> User { get; set; }

    }
}
