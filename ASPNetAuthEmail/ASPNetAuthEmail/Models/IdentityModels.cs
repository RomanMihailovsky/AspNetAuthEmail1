using Microsoft.AspNet.Identity.EntityFramework;

namespace ASPNetAuthEmail.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Email { get; set; }
        public bool ConfirmedEmail { get; set; }
        public bool ConfirmedCodeSMS { get; set; }
        public int id_user { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Ikur_newSQL")
        {
        }
    }
}
