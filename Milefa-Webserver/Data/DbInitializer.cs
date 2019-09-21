using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Milefa_WebServer.Data;
using Milefa_WebServer.Entities;

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
                new Department{Name="Geschäftsleitung", MaxEmployes=2, AssignmentRole = RoleStrings.HumanResource},
                new Department{Name="Personalabteilung", MaxEmployes=10, AssignmentRole = RoleStrings.HumanResource},
                new Department{Name="IT-Abteilung", MaxEmployes=9, AssignmentRole = RoleStrings.It},
                new Department{Name="dep4", MaxEmployes=2},
                new Department{Name="dep5", MaxEmployes=2},

            };
            foreach (Department d in dep)
            {
                context.Departments.Add(d);
            }
            context.SaveChanges();

            /*
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
            */

            /*
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
            */

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


            /*
            var newRoles = new Role[]
            {
                new Role {ID = 1, RoleName = RoleStrings.Sysadmin, UserID = 1},
                new Role {ID = 2, RoleName = RoleStrings.Admin, UserID = 1},
                new Role {ID = 3, RoleName = RoleStrings.User, UserID = 1},
                new Role {ID = 4, RoleName = RoleStrings.HumanResource, UserID = 1},
            };
            foreach (var r in newRoles)
            {
                context.Roles.Add(r);
            }
            context.SaveChanges();
            */

            /* //Not setting the correct password
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            var newUsers = new User[]
            {
                new User
                {
                    ID = 1,
                    PasswordHash =
                        enc.GetBytes(
                            "PjarYd+u9GZe7ujUQAX7whlU8hSFVTbIZ8Blh/jd52J2kQZeH8sKVoTRvFSr5m9Ot8bvF0n3UV5nkm8qxsxHvg=="),
                    PasswordSalt =
                        enc.GetBytes(
                            "oh6mD6RgaeBDT/NQ6lp3sMZ+vPLq2hBlb0aewKgs2MH2DuD6Bu90RDPU2NrGzBnqAz0ScwR8qFCdze5QVlMlPVoEnO9/vFpvbhkTBGeZKqcsAUbFrvlXglo9Cr1lwO7MqP5dVoKWjo4CB2hjCH1pjpi3719kK3GNOs3DpeP6MSs="),
                    Username = "Colin",

                },
            };
            foreach (var u in newUsers)
            {
                context.User.Add(u);
            }
            context.SaveChanges();
            */
            

        }
    }
}
