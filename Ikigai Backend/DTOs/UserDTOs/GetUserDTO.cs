using System.ComponentModel.DataAnnotations;

namespace Ikigai_Backend.DTOs.UserDTOs
{
    public class GetUserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Created_at { get; set; }
    }
}