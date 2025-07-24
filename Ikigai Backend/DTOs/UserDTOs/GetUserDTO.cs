using System.ComponentModel.DataAnnotations;

namespace Ikigai_Backend.DTOs.UserDTOs
{
    public class GetUserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Created_at { get; set; }
    }
}