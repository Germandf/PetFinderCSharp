using PetFinder.Areas.Identity;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var comment = new Comment();
            comment.Id = Id;
            comment.Message = Message;
            comment.UserId = UserId;
            comment.PetId = PetId;
            comment.Rate = Rate;
            comment.User = User;
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
