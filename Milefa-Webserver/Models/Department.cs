using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milefa_WebServer.Models
{
    public class Department
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public int MaxEmployes { get; set; }
        public HashSet<Skill> RequiredSkills { get; set; }

        public string AssignmentRole { get; set; }
    }
}
