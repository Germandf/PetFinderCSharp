using PetFinder.Areas.Identity;
using PetFinder.Models;

namespace PetFinder.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public int PetId { get; set; }
        public int Rate { get; set; }
        public virtual ApplicationUser User { get; set; }

        public Comment ConvertToComment()
        {
            var comment = new Comment
            {
                Id = Id,
                Message = Message,
                UserId = UserId,
                PetId = PetId,
                Rate = Rate,
                User = User
            };
            return comment;
        }

        public void ConvertFromComment(Comment comment)
        {
            Id = comment.Id;
            Message = comment.Message;
            UserId = comment.UserId;
            PetId = comment.PetId;
            Rate = comment.Rate;
            User = comment.User;
        }
    }
}