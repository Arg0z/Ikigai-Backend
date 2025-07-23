namespace Ikigai_Backend.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Created_at { get; set; }
    }
}