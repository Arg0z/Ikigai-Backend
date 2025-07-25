using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ikigai_Backend.DTOs.UserDTOs
{
    public class UpdateUserAdminDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        // Only include Roles if you want admins to update them
        public List<string>? Roles { get; set; }
    }
}