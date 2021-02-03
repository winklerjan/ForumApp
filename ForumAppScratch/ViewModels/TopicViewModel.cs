using ForumAppScratch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAppScratch.ViewModels
{
    public class TopicViewModel
    {
        public User User { get; set; }
        public Topic Topic { get; set; }
        public List<Post> Posts { get; set; }
    }
}
