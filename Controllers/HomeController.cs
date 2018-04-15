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
                return RedirectToAction("Wall", "Wall");
            }
            else
                return View("Index");
        }
        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserModel user)
        {
            if(ModelState.IsValid)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password, 10);
                user.Password = hashedPassword;
                
                // create new user in database
                var newUser = new User();
                newUser.first_name = user.FirstName;
                newUser.last_name = user.LastName;
                newUser.email = user.Email;
                newUser.password = user.Password;
                //
                string sql = $"INSERT INTO users (first_name, last_name, email, password) ";
                sql += $"VALUES ('{newUser.first_name}', '{newUser.last_name}', '{newUser.email}', '{newUser.password}')";
                _dbConnector.Execute(sql);

                var test = _dbConnector.Query($"SELECT id FROM users WHERE email = '{newUser.email}'");
                var userId = test[0]["id"];
                HttpContext.Session.SetInt32("userId", (int)userId);
                //
                return RedirectToAction("Wall", "Wall");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginUserModel LoginUser)
        {
            if(ModelState.IsValid)
            {
                var user = _dbConnector.Query($"SELECT password, id FROM users WHERE email = '{LoginUser.Email}'");
                var userPassword = (string)user[0]["password"];
                var userId = (int)user[0]["id"];
                bool validPassword = BCrypt.Net.BCrypt.Verify(LoginUser.Password, userPassword);
                if(validPassword)
                {
                    HttpContext.Session.SetInt32("userId", userId);
                    return RedirectToAction("Wall", "Wall");
                }
            }
            return View("Login");
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
