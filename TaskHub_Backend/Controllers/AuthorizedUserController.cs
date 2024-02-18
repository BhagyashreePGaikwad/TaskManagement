﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using taskHub.Context;
using taskHub.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cors;

namespace taskHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthorizedUserController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly TestContext _userContext;

        public AuthorizedUserController(IConfiguration configuration, TestContext userContext)
        {
            _userContext = userContext;
            _configuration = configuration;
        }


        [HttpGet]
        [EnableCors("CorsPolicy")]
        public async Task<ActionResult<AuthorizedUser>> GetUser(int id)
        {
            if (_userContext.UserTable == null)
            {
                return NotFound();
            }
            var u = await _userContext.AuthorizedUser.FindAsync(id);


            if (u == null)
            {
                return null;
            }
            return u;
        }

        [HttpGet("ListofUser")]
        [EnableCors("CorsPolicy")]
        public async Task<ActionResult<IEnumerable<AuthorizedUser>>> GetUserList()
        {
            if (_userContext.UserTable == null)
            {
                return null;
            }
            return await _userContext.AuthorizedUser.ToListAsync();
        }




        [HttpPost]
        [EnableCors("CorsPolicy")]
        public async Task<ActionResult<AuthorizedUser>> PostUser(AuthorizedUser user)
        {
            if (user != null)
            {
                var userData = await GetUserByEmailAndPassword(user.Email, user.Username);
               // var authuser = await _userContext.UserTable.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (userData == null )
                {
                    _userContext.AuthorizedUser.Add(user);
                    await _userContext.SaveChangesAsync();
                }
                
                    var claims = new[]
           {

         new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]) ,
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),

                    new Claim("email", user.Email),
                    new Claim("username", user.Username)
    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.Now.AddMinutes(20),
                        SigningCredentials = signIn,
                        Issuer = _configuration["Jwt:Issuer"],
                        Audience = _configuration["Jwt:Audience"]
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);


                var u = await _userContext.AuthorizedUser.FirstOrDefaultAsync(u => u.Email == user.Email);
                



                return Ok(new { Token = tokenString, u });





            };

            return Ok();


        }

        [HttpGet("{email}/{username}")]
        [EnableCors("CorsPolicy")]
        public async Task<ActionResult<AuthorizedUser>> GetUserByEmailAndPassword(string email, string username)
        {
            var u = await _userContext.AuthorizedUser.FirstOrDefaultAsync(u => u.Email == email && u.Username == username);
            if (u == null)
            {
                return null;
            }
            else
            {
                return u;
            }
        }

        [HttpPut("{id}")]
        [EnableCors("CorsPolicy")]
        public async Task<ActionResult> PutEmployee(int id, AuthorizedUser user)
        {
            if (id != user.Id)
            {
                return null;
            }
            _userContext.Entry(user).State = EntityState.Modified;
            try
            {
                await _userContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [EnableCors("CorsPolicy")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            if (_userContext.AuthorizedUser == null)
            {
                return null;
            }
            var u = await _userContext.AuthorizedUser.FirstOrDefaultAsync(u => u.Id == id);

            if (u == null)
            {
                return null;
            }
            _userContext.AuthorizedUser.Remove(u);
            await _userContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("login")]
        [EnableCors("CorsPolicy")]
        public async Task<ActionResult> Login([FromBody] LoginRequestModel user)
        {
            var userFromDb = await GetUserByEmailAndPassword(user.Email, user.Password);

            if (userFromDb == null)
            {
                return Unauthorized(); // Invalid credentials
            }

            var claims = new[]
            {

         new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]) ,
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),

                    new Claim("email", user.Email),
                    new Claim("password", user.Password)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = signIn,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString, userFromDb });
        }

    }
}