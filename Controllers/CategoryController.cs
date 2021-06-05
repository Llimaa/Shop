using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoryController(DataContext dataContext)
        {
            this._context = dataContext;
        }

        [HttpGet]

        // Cache por metodo.
        [ResponseCache(VaryByHeader ="User-Agent", Location =ResponseCacheLocation.Any, Duration =30)]

        //Nao Fazer Cache.
        // [ResponseCache(Duration =0, Location =ResponseCacheLocation.None,NoStore =true)]
        public async Task<ActionResult<List<Category>>> Get()
        {
            var categories = _context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Category>> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return Ok(category);
            }
            catch
            {

                return BadRequest(new { message = "Nao foi possivel salvar sua categoria" });
            }
        }

        [HttpPut]
        [Authorize(Roles = "manager")]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> Update(int id, [FromBody] Category category)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (category.Id != id)
                    return NotFound(new { message = "Categoria não encontrada" });

                _context.Entry<Category>(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(category);
            }
            catch (DbUpdateConcurrencyException)
            {

                return BadRequest(new { message = "Este registro ja foi atualizado" });
            }

            catch
            {

                return BadRequest(new { message = "Não foi possivel atualizar sua categoria" });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "manager")]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(category == null)
                return NotFound(new {  message=" Categoria não enctrada!"});

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Categoria {category.Title} removida" });
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover a categoria!" });
            }
        }
    }
}
