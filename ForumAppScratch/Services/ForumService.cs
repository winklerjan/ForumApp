
using ForumAppScratch.Database;
using ForumAppScratch.Dto;
using ForumAppScratch.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;

namespace ForumAppScratch.Services
{
    public class ForumService
    {
        private readonly ApplicationDbContext dbContext;
        public ForumService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<Post> ReadAllPostsInTopic(int topicId) 
            => dbContext.Posts.Where(p=>p.TopicId == topicId).ToList();

        public List<Topic> ReadAllTopics() 
            => dbContext.Topics
            .Include(t=>t.Posts)
            .Include(t=>t.Author)
            .ToList();

        public List<string> ReadAllTopicNames()
           => dbContext.Topics
            .Select(t=>t.Name)
           .ToList();

        public List<User> ReadAllUsers()
            => dbContext.Users.ToList();

        public Topic GetTopicById(int id) 
            => dbContext.Topics.FirstOrDefault(t => t.Id == id);

        public void CreateNewTopic(string name, string authorId)
        {
            var topic = new Topic { Name = name, AuthorId = authorId};
            dbContext.Topics.Add(topic);
            dbContext.SaveChanges();
        }

        public bool CreateNewPost(Post post)
        {
            dbContext.Posts.Add(post);
            dbContext.SaveChanges();
            return true;
        }

        public List<User> GetAllUsersPostingInTopic(int topicId) 
            => dbContext.Topics
            .Where(t => t.Id == topicId)
            .Select(t => t.Author)
            .ToList();

        public User GetUserById(string userId)
            => dbContext.Users.FirstOrDefault(u => u.Id == userId);
        //TODO rewrite to the native userManager service userManager.FindByIdAsync()

        public User GetCurrentUserByName(string name)
            => dbContext.Users.FirstOrDefault(u => u.Email == name || u.UserName == name);

        public List<Post> GetPostsByUserId(string userId)
        => dbContext.Posts.Where(p => p.AuthorId == userId).Include(p=>p.Topic).ToList();

        public List<Topic> GetTopicsByUserId(string userId)
            => dbContext.Topics.Where(t => t.AuthorId == userId).Include(t=>t.Posts).ToList();

        public List<Post> GetRecentPosts(string userId)
            => dbContext.Posts.Where(p => p.AuthorId == userId).ToList();

        public void NewTopicView(int topicId)
        {
            var topic = dbContext.Topics.FirstOrDefault(t => t.Id == topicId);
            topic.TopicViews += 1;
            dbContext.Update(topic);
            dbContext.SaveChanges();
        }

        public void ChangeUsername(string username, User user)
        {
            user.UserName = username;
            dbContext.Update(user);
            dbContext.SaveChanges();
        }

        public void ChangeAvatar(string avatarUrl, User user)
        {
            user.AvatarUrl = avatarUrl;
            dbContext.Update(user);
            dbContext.SaveChanges();
        }
    }
}