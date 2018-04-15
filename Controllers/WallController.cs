using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using login_reg.Models;

namespace login_reg.Controllers
{
    public class WallController : Controller
    {
        private readonly DbConnector _dbConnector;
        public WallController(DbConnector connect)
        {
            _dbConnector = connect;
        }

        [HttpGet]
        [Route("Wall")]
        public IActionResult Wall()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId != null)
            {
                // get user data from db to send to template
                var user_data = _dbConnector.Query($"SELECT * from users WHERE id = {userId}");
                ViewBag.user_fn = (string)user_data[0]["first_name"];
                ViewBag.user_ln = (string)user_data[0]["last_name"];

                // get all posts in db from all users to send to template
                string query = @"
                SELECT posts.id, 
                    CONCAT_WS(' ', users.first_name, users.last_name) AS user_name, 
                    DATE_FORMAT(posts.created_at, '%M %D, %Y %H:%i:%s') AS time, 
                    posts.content 
                FROM 
                    posts 
                        JOIN 
                    users ON users.id = posts.user_id 
                ORDER BY posts.created_at DESC";
                
                var all_posts = _dbConnector.Query(query);
                ViewBag.all_posts = all_posts;

                // get all comments in db from all users to send to template
                query = @"
                SELECT 
                    comments.post_id AS parent_id,
                    CONCAT_WS(' ', users.first_name, users.last_name) AS user_name,
                    DATE_FORMAT(comments.created_at, '%M %D, %Y %H:%i:%s') AS time,
                    comments.content
                FROM
                    posts
                        JOIN
                    comments ON posts.id = comments.post_id
                        JOIN
                    users ON users.id = comments.user_id
                ORDER BY comments.created_at ASC";

                var all_comments = _dbConnector.Query(query);
                ViewBag.all_comments = all_comments;

                return View("Wall", "Wall");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [Route("post")]
        public IActionResult Post(string content)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return RedirectToAction("Index", "Home");
            if(content == null)
                return RedirectToAction("Wall", "Wall");
            else
            {
                string sql = "INSERT INTO posts (content, user_id) ";
                sql += $"VALUES ('{content.Replace("'","''")}', {userId})";
                _dbConnector.Execute(sql);
                return RedirectToAction("Wall", "Wall");
            }
        }
        [HttpPost]
        [Route("comment")]
        public IActionResult Comment(string content, string postId)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return RedirectToAction("Index", "Home");
            if(content == null)
                return RedirectToAction("Wall", "Wall");
            else
            {
                int post_Id = Int32.Parse(postId);
                string sql = "INSERT INTO comments (content, user_id, post_id) ";
                sql += $"VALUES ('{content.Replace("'","''")}', {userId}, {post_Id})";
                _dbConnector.Execute(sql);
                return RedirectToAction("Wall", "Wall");
            }
        }
    }
}
