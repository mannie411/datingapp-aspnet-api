using System.ComponentModel.DataAnnotations;

namespace api.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Username cannot be less than 3 characters")]
        public string Username { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Password cannot be less than 8 characters")]

        public string Password { get; set; }

    }
}