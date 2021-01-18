using PetFinder.Areas.Identity;

#nullable disable

namespace PetFinder.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public int PetId { get; set; }
        public int Rate { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}