using System.ComponentModel.DataAnnotations;

namespace MonthlyClaimsSystem.Models
{
    public class User
    {
        public string EmployeeNumber { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

}
