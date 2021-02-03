using ForumAppScratch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAppScratch.Dto
{
    public class JsonDto
    {
        public List<Topic> Topics { get; set; }
        public List<Post> Posts { get; set; }
        public List<User> Users { get; set; }
        public User User { get; set; }
    }
}
