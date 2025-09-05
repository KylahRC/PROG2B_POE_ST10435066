using System.ComponentModel.DataAnnotations;

namespace MonthlyClaimsSystem.Models
{
    public class User
    {
        [Key]
        public string EmployeeNumber { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public ICollection<Claim> Claims { get; set; }

    }

}
