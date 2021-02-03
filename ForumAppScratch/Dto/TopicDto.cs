using ForumAppScratch.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAppScratch.Dto
{
    public class TopicDto
    {
        public User User { get; set; }
        public Topic Topic { get; set; }
        public List<Post> Posts { get; set; }
        public string AuthorId { get; set; }
        public int UserId { get; set; }

        public int TopicId { get; set; }

        public string Content { get; set; }

        public string Title { get; set; }

        

    }
}
