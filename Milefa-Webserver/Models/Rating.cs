using Microsoft.EntityFrameworkCore.Query;
using Milefa_WebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milefa_Webserver.Models
{
    public class Rating
    {
        public int studentID { get; set; }
        public Student Student { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public AsyncEnumerable<Skill> Skills { get; set; }
    }
}
