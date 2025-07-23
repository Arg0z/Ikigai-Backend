using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikigai_Backend.Constants;

namespace Ikigai_Backend.DbModels
{
    public class UserRole
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }

        [Key, Column(Order = 1)]
        public Roles RoleName { get; set; }

        public User User { get; set; } = null!;
    }
}