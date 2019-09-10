namespace Milefa_WebServer.Entities
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        //public string HashValue { get; set; }
        //public string PasswordHashed { get; set; }

        //Store role in database?
        public string Role { get; set; }

        public string Token { get; set; }
    }
}