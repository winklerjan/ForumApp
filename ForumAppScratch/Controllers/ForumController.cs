using ForumAppScratch.Dto;
using ForumAppScratch.Entities;
using ForumAppScratch.Services;
using ForumAppScratch.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using CoreIdentity = Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ForumAppScratch.Controllers
{
    //TODO add viewmodels and dtos for each view and endpoint

    [Authorize]
    public class ForumController : Controller
    {
        private readonly ForumService service;
        private readonly CoreIdentity.UserManager<User> userManager;


        public ForumController(ForumService service, CoreIdentity.UserManager<User> userManager)
        {
            this.service = service;
            this.userManager = userManager;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public IActionResult Index()
        {
            var topics = service.ReadAllTopics(); //TODO make it a single service, using Include
            var users = service.ReadAllUsers();
            var model = new ForumViewModel { Topics = topics, UserName = User.Identity.Name, Users = users };
            return View(model); 
        }

        [HttpGet("newTopic")]
        public IActionResult NewTopic()
        {
            return View();
        }

        [HttpPost("newTopic")]
        public IActionResult NewTopic(string name, string authorId)
        {
            //TODO dont pass authorId!! use service FindUserById
            service.CreateNewTopic(name, authorId);
            return RedirectToAction("index");
        }

        [HttpGet("topic/{topicId}")]
        public IActionResult Topic(int topicId)
        {
            var user = service.GetUserById(User.Identity.GetUserId());
            var posts = service.ReadAllPostsInTopic(topicId);
            var topic = service.GetTopicById(topicId); //TODO use Include
            var model = new TopicDto { Posts = posts, Topic = topic, User = user };

            service.NewTopicView(topicId);

            return View(model);
        }

        [HttpPost("newPost")]
        public IActionResult NewPost(Post post)
        {
            if (ModelState.IsValid)
            {
                service.CreateNewPost(post);
            }
            else
            {
                ModelState.AddModelError("", errorMessage: "Not meeting the input length criteria");
            }
            //TODO fix conditions for adding new post
            return Redirect($"topic/{post.TopicId}");
        }
    }
}
