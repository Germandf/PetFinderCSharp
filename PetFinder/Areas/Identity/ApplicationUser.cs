using Microsoft.AspNetCore.Identity;

namespace PetFinder.Areas.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        private bool Admin { get; set; }

        public bool IsAdmin()
        {
            return Admin;
        }

        public bool SetAdmin(bool admin)
        {
            return Admin = admin;
        }
    }
}