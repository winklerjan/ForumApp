
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ForumAppScratch.Entities
{
    public class User : IdentityUser
    {
        public DateTime RegisteredAt { get; set; }
        public string AvatarUrl { get; set; }
        public List<Post> Posts { get; set; }
        public List<Topic> Topics { get; set; }
        public User()
        {
            RegisteredAt = DateTime.Now;
        }
    }
}
