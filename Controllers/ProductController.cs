using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        private DataContext _dataContext;

        public ProductController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAll()
        {
            var products = await _dataContext.Products.
                Include(x => x.Category).AsNoTracking().ToListAsync();
            return Ok(products);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _dataContext.Products.
                Include(x => x.Category).
                AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            return Ok(product);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("categories/{id:int}")]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id)
        {
            var products = await _dataContext.Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.CategoryId == id).ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles ="employer")]
        public async Task<ActionResult<Product>> Post([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _dataContext.Products.Add(product);
                await _dataContext.SaveChangesAsync();
                return Ok(product);
            }
            catch 
            {
                return BadRequest(new { message = "Nao foi possivel salvar seu produto" });
            }
        }

        [HttpPut]
        [Authorize(Roles = "manager, employer")]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> Put(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (product.Id != id)
                    return NotFound(new { message = "Produto não encontrada" });

                _dataContext.Entry<Product>(product).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
                return Ok(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro ja foi atualizado" });
            }

            catch
            {
                return BadRequest(new { message = "Não foi possivel atualizar seu produto" });
            }
        }


        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            var product = await _dataContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if(product == null)
                return NotFound(new { message = " Produto não enctrada!" });

            try
            {
                _dataContext.Products.Remove(product);
                await _dataContext.SaveChangesAsync();
                return Ok(new { message = $"Produto {product.Title} removida" });
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover o produto!" });
            }
        }
    }
}
