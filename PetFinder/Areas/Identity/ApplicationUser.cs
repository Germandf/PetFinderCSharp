using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace PetFinder.Areas.Identity
{
    public class ApplicationUser : IdentityUser
    {

        public string Name { get; set; }

        public string Surname { get; set; }

        private bool admin { get; set; }

        public bool isAdmin()
        {
            return this.admin; 
        }

        public bool setAdmin(bool admin)
        {
            return this.admin = admin;

        }

    }

}
