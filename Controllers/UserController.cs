using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : Controller
    {
        private readonly DataContext _dbContext;

        public UserController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles ="manager")]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            var users = await _dbContext.Users.AsNoTracking().ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                user.Role = "employer";
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                user.Password = "";
                return Ok(user);
            }
            catch
            {
                return BadRequest(new { message = "Nao foi possivel criar seu usuário!" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]

        public async Task<ActionResult<User>> Put(int id,[FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                if (user.Id != id)
                    return NotFound(new { message = "User não encontrada" });

                _dbContext.Entry<User>(user).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(user);
            }

            catch
            {
                return BadRequest(new { message = "Não foi possivel atualizar seu usuário!" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            var user =await  _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound(new { message = " usuário não enctrado!" });

            try
            {
                _dbContext.Users.Remove(user);
               await  _dbContext.SaveChangesAsync();
                return Ok(new { message=$" Usuário {user.Username} removido com sucesso!"});
            }
            catch (Exception)
            {
                return NotFound(new { message = "Não foi possivel remover o usuário!" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(x => x.Username == model.Username && x.Password == model.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválido" });

            var token = TokenService.GenerateToken(user);
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }
    }
}
