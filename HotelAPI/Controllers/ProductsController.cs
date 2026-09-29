using HotelAPI.DTOs;
using HotelAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly HotelContext _context;

        public ProductsController(HotelContext context)
        {
            _context = context;
        }

        private static ProductDTO ToDto(Product product) => new ProductDTO
        {
            IdProduct = product.IdProduct,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            ImagePath = product.ImagePath,
            CreatedAt = product.CreatedAt
        };

        [HttpGet]
        public IActionResult GetAll(int page = 1, int pageSize = 10)
        {
            var products = _context.Products
                .OrderBy(p => p.IdProduct)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(ToDto)
                .ToList();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.IdProduct == id);
            if (product is null) return NotFound();
            return Ok(ToDto(product));
        }

        [HttpPost]
        public IActionResult Create(CreateProductDTO dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                ImagePath = dto.ImagePath,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = product.IdProduct }, ToDto(product));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateProductDTO dto)
        {
            var product = _context.Products.FirstOrDefault(p => p.IdProduct == id);
            if (product is null) return NotFound();

            product.ProductName = dto.ProductName;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.ImagePath = dto.ImagePath;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, PatchProductDTO dto)
        {
            var product = _context.Products.FirstOrDefault(p => p.IdProduct == id);
            if (product is null) return NotFound();

            if (dto.ProductName is not null)
                product.ProductName = dto.ProductName;

            if (dto.Description is not null)
                product.Description = dto.Description;

            if (dto.Price is not null)
                product.Price = dto.Price.Value;

            if (dto.CategoryId is not null)
                product.CategoryId = dto.CategoryId.Value;

            if (dto.ImagePath is not null)
                product.ImagePath = dto.ImagePath;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.IdProduct == id);
            if (product is null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }
}