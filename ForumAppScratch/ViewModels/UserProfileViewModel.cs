using ForumAppScratch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAppScratch.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Post> RecentPosts { get; set; }
    }
}
