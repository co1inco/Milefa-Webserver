using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milefa_Webserver.Models
{
    /// <summary>
    /// Required skills for a department
    /// </summary>
    public class RequiredSkill
    { 
        public int SkillID { get; set; }
        public int DepartmentID { get; set; }
        public Skill Skill { get; set; }
        public Department Department { get; set; }
    }
}
