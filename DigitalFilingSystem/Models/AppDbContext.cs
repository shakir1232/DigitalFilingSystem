using Microsoft.AspNet.Identity.EntityFramework;

namespace DigitalFilingSystem.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext():base("AppDbContext", throwIfV1Schema: false)
        {
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }
    }
}