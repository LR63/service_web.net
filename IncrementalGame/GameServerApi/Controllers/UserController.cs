using GameServerApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static readonly List<User> Users = new();
        
        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            if (Users.Exists(x => x.Id == id))
            {
                return Ok(new UserPublic(Users.Find(x => x.Id == id)));
            }
            return NotFound();
        }

        [HttpPost("register")]
        public ActionResult Register(UserPass userPass)
        {
            var u = new User(userPass);
            Users.Add(u);
            return Ok(new UserPublic(u));
        }
        
        [HttpPost("login")]
        public ActionResult Login(UserPass userPass)
        {
            if (Users.Exists(x => x.Pseudo == userPass.Pseudo && x.Password == userPass.Password))
            {
                return Ok(new UserPublic(Users.Find(x => x.Pseudo == userPass.Pseudo && x.Password == userPass.Password)));
            }
            return Unauthorized();
        }

        [HttpGet("All")]
        public ActionResult GetAll()
        {
            return Ok(Users.Select(user => new UserPublic(user)));
        }
    }
}
