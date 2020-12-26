using PetFinder.Areas.Identity;
using System;
using System.Collections.Generic;

#nullable disable

namespace PetFinder.Models
{
    public partial class Comments
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public int PetId { get; set; }
    }
}