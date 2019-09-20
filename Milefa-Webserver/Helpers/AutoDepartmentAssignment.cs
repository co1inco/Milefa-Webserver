using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Milefa_WebServer.Models;

namespace Milefa_WebServer.Helpers
{
    public class AutoDepartmentAssignment
    {
        private static bool HasSkill(HashSet<Skill> skills, Skill skill)
        {
            foreach (Skill s in skills)
            {
                if (s.ID == skill.ID)
                    return true;
            }
            return false;
        }


        private static int GetUserDepAffinity(Student student, Department department)
        {
            int depReqCount = department.RequiredSkills.Count;
            if (depReqCount == 0) return 50;

            int matches = 0;

            foreach (Skill s in department.RequiredSkills)
            {
                if (HasSkill(student.Skills, s))
                    matches++;
            }

            if (depReqCount > 6) depReqCount = 6;

            decimal ratio = Decimal.Divide(matches, depReqCount);

            return decimal.ToInt32(ratio * 100);
        }


        private static int? GetUserChoise(Student student, int choise)
        {
            switch (choise)
            {
                case 2:
                    return student.Choise2ID;
                default:
                    return student.Choise1ID;
            }
        }


        private static int TotalDepartmentSpace(Department[] departments)
        {

            int space = 0;

            foreach (Department dep in departments)
                space += dep.MaxEmployes;

            return space;
        }


        public static string GenerateUserAssignments(Student[] students, Department[] departments, out Student[] processedStudents)
        {
            int weigthingPerChoise = 10;
            int choiseTreshhold = 0; //Min user -> department affinity for choise to be valid




            int totalSpace = TotalDepartmentSpace(departments);
            if (totalSpace < students.Length)
            {
                processedStudents = new Student[0];
                return "Not enough space";
            }

            Student[] studentsToProcess = students;
            List<Student> leftUser;
            Dictionary<Department, List<Student>> depAssignment = new Dictionary<Department, List<Student>>();

            //Assign to users to their choise
            for (int i = 0; i < 3; i++)
            {
                //Repeat 2. choise for users that got removed from first choise by a second choise
                int choise = i + 1;
                if (choise > 2) choise = 2;

                leftUser = new List<Student>();

                foreach (Department department in departments)
                {

                    List<Student> addedUser;
                    if (depAssignment.ContainsKey(department))
                        addedUser = depAssignment[department];
                    else
                        addedUser = new List<Student>();
                    var userAffinity = new Dictionary<Student, int>();

                    foreach (Student student in studentsToProcess)
                    {
                        if (department.ID == GetUserChoise(student, choise))
                        {
                            int currentWeighting = (weigthingPerChoise * (choise - 1));
                            int tmpUserAf = GetUserDepAffinity(student, department) - currentWeighting;
                            userAffinity.Add(student, tmpUserAf);
                        }
                    }

                    foreach (var user in userAffinity.OrderByDescending(x => x.Value))
                    {
                        if (addedUser.Count < department.MaxEmployes && user.Value >= choiseTreshhold)
                            addedUser.Add(user.Key);
                        else
                            leftUser.Add(user.Key);
                    }

                    depAssignment[department] = addedUser;
                }

                studentsToProcess = leftUser.ToArray();
            }
            leftUser = studentsToProcess.ToList();


            // Left user assign to what they fit to
            foreach (Student student in leftUser)
            {
                Dictionary<Department, int> depAffinity = new Dictionary<Department, int>();


                foreach (Department department in departments)
                {
                    int tmpAf = GetUserDepAffinity(student, department);
                    depAffinity.Add(department, tmpAf);
                }

                foreach (KeyValuePair<Department, int> depAf in depAffinity.OrderByDescending(key => key.Value))
                {
                    int curDepCount = depAssignment[depAf.Key].Count;
                    if (curDepCount < depAf.Key.MaxEmployes)
                    {
                        depAssignment[depAf.Key].Add(student);
                        break;
                    }
                }

            }

            var studentOutput = new List<Student>();
            foreach (KeyValuePair<Department, List<Student>> pair in depAssignment)
            {
                foreach (Student student in pair.Value)
                {
                    student.DeployedDepID = pair.Key.ID;
                    student.DeployedDep = pair.Key; 
                    studentOutput.Add(student);
                }
            }

            processedStudents = studentOutput.ToArray();
            return "";
        }
    }
}