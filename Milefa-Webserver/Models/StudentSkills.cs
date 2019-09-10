using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milefa_Webserver.Models
{
    public class StudentSkill
    {
        public int StudentID { get; set; }
        public int SkillID { get; set; }
        public Student Student { get; set; }
        public Skill Skill { get; set; }
    }
}
