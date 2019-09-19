using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Milefa_WebServer.Models
{
    public enum Gender
    {
        Male,
        Female,
        Diverse
    }

    public class Student
    {
        public Student() { }
        public Student(Student student)
        {
            Name = student.Name;
            School = student.School;
            _Class = student._Class;
            Gender = student.Gender;
            DateValide = student.DateValide;
            PersNr = student.PersNr;
            Breakfast = student.Breakfast;
            Lunch = student.Lunch;
            DeployedDepID = student.DeployedDepID;
            Choise1ID = student.Choise1ID;
            Choise2ID = student.Choise2ID;
        }

        public int ID { get;  set; }

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
        public HashSet<Skill> Skills { get; set; }

        [ForeignKey("User")]
        public int? UserID { get; set; }
        [Required]
        public User User { get; set; }
        /*
        public int? UserID { get; set; }
        public User User { get; set; }
        */
    }
}
