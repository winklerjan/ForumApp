using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAppScratch.Entities
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 40 characters long.")]
        public string Title { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 255 characters long.")]
        public string Content { get; set; }
        public User Author { get; set; }
        public string AuthorId { get; set; }
        public Topic Topic { get; set; }
        public int TopicId { get; set; }
        public List<Post> Posts { get; set; }
        public DateTime CreatedAt { get; set; }

        public Post()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
