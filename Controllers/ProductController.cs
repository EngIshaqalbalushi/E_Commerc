using AutoMapper;
using E_CommerceSystem.Models;
using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace E_CommerceSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        // ✅ Add Product (Admin only)
        [HttpPost("AddProduct")]
        [AllowAnonymous]
        public IActionResult AddNewProduct([FromBody] ProductDTO productInput)
        {
            try
            {
                if (productInput == null)
                    return BadRequest("Product data is required.");

                // Get user role from JWT
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userRole = GetUserRoleFromToken(token);

                if (userRole != "admin")
                    return Unauthorized("You are not authorized to perform this action.");

                var product = _mapper.Map<Product>(productInput);
                product.OverallRating = 0;

                _productService.AddProduct(product);

                return Ok(_mapper.Map<ProductDTO>(product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the product: {ex.Message}");
            }
        }

        // ✅ Update Product (Admin only)
        [HttpPut("UpdateProduct/{productId}")]
        public IActionResult UpdateProduct(int productId, [FromBody] ProductDTO productInput)
        {
            try
            {
                if (productInput == null)
                    return BadRequest("Product data is required.");

                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userRole = GetUserRoleFromToken(token);

                if (userRole != "admin")
                    return Unauthorized("You are not authorized to perform this action.");

                var product = _productService.GetProductById(productId);
                if (product == null)
                    return NotFound($"Product with ID {productId} not found.");

                _mapper.Map(productInput, product); // AutoMapper updates fields
                _productService.UpdateProduct(product);

                return Ok(_mapper.Map<ProductDTO>(product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the product: {ex.Message}");
            }
        }

        // ✅ Get All Products (Pagination + Filtering)
        [AllowAnonymous]
        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts(
            [FromQuery] string? name,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                    return BadRequest("PageNumber and PageSize must be greater than 0.");

                var products = _productService.GetAllProducts(pageNumber, pageSize, name, minPrice, maxPrice);

                var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products);

                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = productDtos.Count(),
                    Products = productDtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving products: {ex.Message}");
            }
        }

        // ✅ Get Product By ID
        [AllowAnonymous]
        [HttpGet("GetProductByID/{productId}")]
        public IActionResult GetProductById(int productId)
        {
            try
            {
                var product = _productService.GetProductById(productId);
                if (product == null)
                    return NotFound("No product found.");

                return Ok(_mapper.Map<ProductDTO>(product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the product: {ex.Message}");
            }
        }

       
        [HttpPost("UploadImage/{productId}")]
        public async Task<IActionResult> UploadImage(int productId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var product = _productService.GetProductById(productId);
                if (product == null)
                    return NotFound("Product not found.");

                // Save file to wwwroot/images
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Save the image path in DB
                product.ImageUrl = $"/images/{fileName}";
                _productService.UpdateProduct(product);

                return Ok(new { message = "Image uploaded successfully", imageUrl = product.ImageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        // ✅ Helper Method – Extract Role from JWT
        private string? GetUserRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == "unique_name");
                return roleClaim?.Value;
            }

            throw new UnauthorizedAccessException("Invalid or unreadable token.");
        }
    }
}
