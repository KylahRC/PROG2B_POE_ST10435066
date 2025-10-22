using System.ComponentModel.DataAnnotations;

namespace MonthlyClaimsSystem.Models
{
    public class User
    {
        [Key] // This tells EF that this is the primary key
        public string EmployeeNumber { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

}