using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Milefa_WebServer.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        //Store role in database?
        [NotMapped]
        public ICollection<Role> Roles { get; set; }

        [NotMapped]
        public string Token { get; set; }
    }
}