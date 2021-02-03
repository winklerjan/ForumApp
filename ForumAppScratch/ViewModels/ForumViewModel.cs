using ForumAppScratch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAppScratch.ViewModels
{
    public class ForumViewModel
    {
        public User User { get; set; }
        public string UserName { get; set; }
        public Topic Topic { get; set; }
        public List<Post> Posts { get; set; }
        public List<Topic> Topics { get; set; }
        public List<User> Users { get; set; }
        public string TopicName { get; set; }
    }
}
