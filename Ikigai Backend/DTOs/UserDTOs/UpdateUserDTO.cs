using System.ComponentModel.DataAnnotations;

namespace Ikigai_Backend.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }
    }
}