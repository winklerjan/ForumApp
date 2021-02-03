using ForumAppScratch.Dto;
using ForumAppScratch.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAppScratch.Controllers
{
    public class ApiController : Controller
    {
        private readonly ForumService service;
        public ApiController(ForumService service)
        {
            this.service = service;
        }

        [HttpGet("topics")]
        public IActionResult Topics()
        {
            var topics = service.ReadAllTopicNames();
            if (topics.Count == 0)
            {
                return BadRequest(new { error = "There are no topics." });
            }
            return Ok(new { topics });
        }

        [HttpGet("topics/{id}")]
        public IActionResult Topics([FromRoute] int id)
        {
            var posts = service.ReadAllPostsInTopic(id);
            if (posts.Count == 0)
            {
                return BadRequest(new { error = "Topic doesnt exist." });
            }
            return Ok(new { posts });
        }

        [HttpGet("users")]
        public IActionResult Users()
        {
            var users = service.ReadAllUsers();
            if (users.Count == 0)
            {
                return BadRequest(new { error = "There are no users." });
            }
            return Ok(new { users });
        }

        [HttpGet("users/{id}")]
        public IActionResult Users([FromRoute]string id)
        {
            var user = service.GetUserById(id);
            if (user == null)
            {
                return BadRequest(new { error = "User not found." });
            }
            return Ok(new { user });
        }
    }
}
