
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Milefa_Webserver.Models;
using Milefa_WebServer.Models;

namespace Milefa_WebServer.Data
{
    public class CompanyContext : DbContext
    {
        public CompanyContext(DbContextOptions<CompanyContext> options) : base (options) { }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            optionsBuilder.UseSqlServer("Data Source=test.db");
//        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<RatingAssignment> RatingAssignments { get; set; }

        // Link table for m:n relations
        public DbSet<RequiredSkill> RequiredSkills { get; set; }
        public DbSet<StudentSkill> StudentSkills { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Skill>().ToTable("Skill");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Rating>().ToTable("Rating");
            modelBuilder.Entity<RatingAssignment>().ToTable("RatingAssignment");

            //TODO: Change Tablename
            modelBuilder.Entity<RequiredSkill>().ToTable("RequiredSkills");
            modelBuilder.Entity<StudentSkill>().ToTable("StudentSkill");

            modelBuilder.Entity<RequiredSkill>()
                .HasKey(c => new { c.SkillID, c.DepartmentID });
            modelBuilder.Entity<StudentSkill>()
                .HasKey(c => new { c.StudentID, c.SkillID });

//            modelBuilder.Entity<Rating>()
//                .HasKey(c => new {c.ID});
            modelBuilder.Entity<RatingAssignment>()
                .HasKey(c => new {c.RatingID, c.SkillID });
        }
        

    }
}
