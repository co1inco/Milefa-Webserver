using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Milefa_WebServer.Data;

namespace Milefa_WebServer.Models
{
    public class DbInitializer
    {
        public static void Initialize(CompanyContext context)
        {
//            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (context.Students.Any())
            {
                return;
            }

            var sk = new Skill[]
{
                new Skill{Name="skill1"},
                new Skill{Name="skill2"},
                new Skill{Name="skill3"}
};
            foreach (Skill s in sk)
            {
                context.Skills.Add(s);
            }
            context.SaveChanges();

            var dep = new Department[]
{
                new Department{Name="dep1", MaxEmployes=5},
                new Department{Name="dep2", MaxEmployes=10},
                new Department{Name="dep3", MaxEmployes=2}
};
            foreach (Department d in dep)
            {
                context.Departments.Add(d);
            }
            context.SaveChanges();

            var students = new Student[]
            {
                new Student{Name="Colin", School="LSBK", _Class="BS8ES1", Gender=Gender.Male, PersNr=-1, Breakfast=true, Lunch=true,
                    DeployedDepID = dep.Single(s => s.Name == "dep1").ID,
                    Choise1ID = dep.Single(s => s.Name == "dep1").ID,
                    Choise2ID = dep.Single(s => s.Name == "dep3").ID
                }
            };
            foreach (Student s in students) {
                context.Students.Add(s);
            }
            context.SaveChanges();

            var studentSkill = new StudentSkill[]
            {
                new StudentSkill
                {
                    StudentID = students.Single(s => s.Name == "Colin").ID,
                    SkillID = sk.Single(s => s.Name == "skill2").ID,
                },
                new StudentSkill
                {
                    StudentID = students.Single(s => s.Name == "Colin").ID,
                    SkillID = sk.Single(s => s.Name == "skill3").ID,
                }
            };
            foreach (var s in studentSkill)
            {
                context.StudentSkills.Add(s);
            }
            context.SaveChanges();

            var rsa = new RequiredSkill[]
            {
                new RequiredSkill
                {
                    SkillID = sk.Single(s => s.Name == "skill1").ID,
                    DepartmentID = dep.Single(s => s.Name == "dep1").ID
                },
                new RequiredSkill
                {
                    SkillID = sk.Single(s => s.Name == "skill3").ID,
                    DepartmentID = dep.Single(s => s.Name == "dep1").ID
                },
                new RequiredSkill
                {
                    SkillID = sk.Single(s => s.Name == "skill2").ID,
                    DepartmentID = dep.Single(s => s.Name == "dep2").ID
                },
                new RequiredSkill
                {
                    SkillID = sk.Single(s => s.Name == "skill3").ID,
                    DepartmentID = dep.Single(s => s.Name == "dep2").ID
                }
            };

            foreach (RequiredSkill s in rsa)   
            {
                context.RequiredSkills.Add(s);
            }
            context.SaveChanges();



        }
    }
}
