
namespace Milefa_WebServer.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string[] Roles { get; set; }
    }
}