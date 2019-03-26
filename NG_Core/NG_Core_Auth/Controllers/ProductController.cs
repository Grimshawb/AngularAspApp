using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NG_Core_Auth.Data;
using NG_Core_Auth.Models;


namespace NG_Core_Auth.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _db;


        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }



        [HttpGet("[action]")]
        public IActionResult GetProducts()
        {
            return Ok(_db.Products.ToList());
        }



        [HttpPost("[action]")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel FormData)
        {
            var NewProduct = new ProductModel
            {
                Name = FormData.Name,
                ImageUrl = FormData.ImageUrl,
                Description = FormData.Description,
                OutOfStock = FormData.OutOfStock,
                Price = FormData.Price
            };

            await _db.Products.AddAsync(NewProduct);
            await _db.SaveChangesAsync();

            return Ok();
        }



        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductModel FormData)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var findProd = _db.Products.FirstOrDefault(p => p.ProductID == id);
            if(findProd == null)
            {
                return NotFound();
            }

            findProd.Name = FormData.Name;
            findProd.ImageUrl = FormData.ImageUrl;
            findProd.Description = FormData.Description;
            findProd.OutOfStock = FormData.OutOfStock;
            findProd.Price = FormData.Price;

            _db.Entry(findProd).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            await _db.SaveChangesAsync();

            return Ok(new JsonResult("The product: " + findProd.Name + " with id: " + id + " has been updated successfully."));
        }



        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var findProd = await _db.Products.FindAsync(id);
            if (findProd == null)
            {
                return NotFound();
            }

            _db.Products.Remove(findProd);

            await _db.SaveChangesAsync();

            return Ok(new JsonResult("The product with id: " + id + " has been removed successfully."));
        }
    }
}
