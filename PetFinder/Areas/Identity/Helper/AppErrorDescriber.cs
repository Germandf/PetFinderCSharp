using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Areas.Identity.Helper
{
    public class AppErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            var error = base.DuplicateUserName(userName);
            error.Description = $"El email {userName} ya está registrado";
            return error;
        }

        public override IdentityError PasswordTooShort(int length)
        {
            var error = base.PasswordTooShort(length);
            error.Description = $"La contraseña es demasiado corta, se requiere almenos una contraseña de {length} caracteres";
            return error;

        }
    }
}
