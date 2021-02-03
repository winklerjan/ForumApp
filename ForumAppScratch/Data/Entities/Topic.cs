using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAppScratch.Entities
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public User Author { get; set; }
        public string AuthorId { get; set; }
        public List<Post> Posts { get; set; }
        public double TopicViews { get; set; }

        public Topic()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
