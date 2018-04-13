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
    public class HomeController : Controller
    {
        private readonly DbConnector _dbConnector;
        public HomeController(DbConnector connect)
        {
            _dbConnector = connect;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId != null)
            {
                return RedirectToAction("Wall");
            }
            else
                return View("Index");
        }
        [HttpPost]
        [Route("create/newuser")]
        public IActionResult CreateUser(User newUser)
        {
            if(ModelState.IsValid)
            {
                //
                string query = $"INSERT INTO users (first_name, last_name, email, password) ";
                query += $"VALUES ('{newUser.FirstName}', '{newUser.LastName}', '{newUser.Email}', '{newUser.Password}')";
                _dbConnector.Execute(query);

                var test = _dbConnector.Query($"SELECT id FROM users WHERE email = '{newUser.Email}'");
                var userId = test[0]["id"];
                HttpContext.Session.SetInt32("userId", (int)userId);
                //
                return RedirectToAction("Wall");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(string email, string password)
        {
                if(email == null)
                {
                    ViewBag.errors = "Please enter an email.";
                    return View("Index");
                }
                if(password == null)
                {
                    ViewBag.errors = "Incorrect password.";
                    return View("Index");
                }
                var login_user = _dbConnector.Query($"SELECT * FROM users WHERE email = '{email}'");
                if(login_user.Count != 0)
                {
                    if(password == (string)login_user[0]["password"])
                    {
                        var userId = login_user[0]["id"];
                        HttpContext.Session.SetInt32("userId", (int)userId);
                        return RedirectToAction("Wall");
                    }
                    else
                    {
                        ViewBag.errors = "Incorrect password.";
                        return View("Index");
                    }
                }
                else
                {
                    ViewBag.errors = "Email not found.";
                    return View("Index");
                }
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
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

                return View("Wall");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [Route("post")]
        public IActionResult Post(string content)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return RedirectToAction("Index");
            if(content == null)
                return RedirectToAction("Wall");
            else
            {
                string query = "INSERT INTO posts (content, user_id) ";
                query += $"VALUES ('{content}', {userId})";
                _dbConnector.Execute(query);
                return RedirectToAction("Wall");
            }
        }
        [HttpPost]
        [Route("comment")]
        public IActionResult Comment(string content, string postId)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return RedirectToAction("Index");
            if(content == null)
                return RedirectToAction("Wall");
            else
            {
                int post_Id = Int32.Parse(postId);
                string query = "INSERT INTO comments (content, user_id, post_id) ";
                query += $"VALUES ('{content}', {userId}, {post_Id})";
                _dbConnector.Execute(query);
                return RedirectToAction("Wall");
            }
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
