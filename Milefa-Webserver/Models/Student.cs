using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Milefa_Webserver.Models
{
    public enum Gender
    {
        Male,
        Female,
        Diverse
    }

    public class Student
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string School { get; set; }
        [StringLength(50)]
        public string _Class { get; set; }
        public Gender? Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateValide { get; set; }
        public int PersNr { get; set; }
        public bool Breakfast { get; set; }
        public bool Lunch { get; set; }

        // Relation ID always optional?
        public int? DeployedDepID { get; set; }
        public Department DeployedDep { get; set; }

        public int? Choise1ID { get; set; }
        public Department Choise1 { get; set; }

        public int? Choise2ID { get; set; }
        public Department Choise2 { get; set; }
        public ICollection<Skill> Skills { get; set; }

    }
}
