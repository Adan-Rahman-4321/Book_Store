using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Website.Models
{
    public class DefaultUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        [PersonalData]
        public string? Address { get; set; } = string.Empty;

        [PersonalData]
        public string? City { get; set; } = string.Empty;

        [PersonalData]
        [DataType(DataType.Date)]
        public DateTime UserCreatedDate { get; set; } = DateTime.Now;
    }
}
