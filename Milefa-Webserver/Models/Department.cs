using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milefa_Webserver.Models
{
    public class Department
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public int MaxEmployes { get; set; }
        public ICollection<Skill> RequiredSkills { get; set; }

    }
}
