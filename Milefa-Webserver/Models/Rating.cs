using Microsoft.EntityFrameworkCore.Query;
using Milefa_WebServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Milefa_Webserver.Models
{
    public class Rating
    {
        public int ID { get; set; }
        public int StudentID { get; set; }
        public Student Student { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        [NotMapped]
        public ICollection<Skill> Skills { get; set; }
    }
}
