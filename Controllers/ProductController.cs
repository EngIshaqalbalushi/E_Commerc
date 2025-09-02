using E_CommerceSystem.Models;
using E_CommerceSystem.Models.DTOs;
using E_CommerceSystem.Services;
using AutoMapper;
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

        // ✅ Only admins can add products
        [HttpPost("AddProduct")]
        public IActionResult AddNewProduct(ProductDTO productDto)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userRole = GetUserRoleFromToken(token);

                if (userRole != "admin")
                {
                    return BadRequest("You are not authorized to perform this action.");
                }

                if (productDto == null)
                {
                    return BadRequest("Product data is required.");
                }

                var product = _mapper.Map<Product>(productDto);
                product.OverallRating = 0; // default rating

                _productService.AddProduct(product);

                return Ok(_mapper.Map<ProductDTO>(product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the product. {ex.Message}");
            }
        }

        // ✅ Only admins can update products
        [HttpPut("UpdateProduct/{productId}")]
        public IActionResult UpdateProduct(int productId, ProductDTO productDto)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userRole = GetUserRoleFromToken(token);

                if (userRole != "admin")
                {
                    return BadRequest("You are not authorized to perform this action.");
                }

                if (productDto == null)
                {
                    return BadRequest("Product data is required.");
                }

                var product = _productService.GetProductById(productId);
                if (product == null) return NotFound("Product not found.");

                // Map updated values from DTO
                _mapper.Map(productDto, product);

                _productService.UpdateProduct(product);

                return Ok(_mapper.Map<ProductDTO>(product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the product. {ex.Message}");
            }
        }

        // ✅ Public - get paginated & filtered products
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
                {
                    return BadRequest("PageNumber and PageSize must be greater than 0.");
                }

                var products = _productService.GetAllProducts(pageNumber, pageSize, name, minPrice, maxPrice);
                if (products == null || !products.Any())
                {
                    return NotFound("No products found matching the given criteria.");
                }

                return Ok(_mapper.Map<IEnumerable<ProductDTO>>(products));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving products. {ex.Message}");
            }
        }

        // ✅ Public - get product by ID
        [AllowAnonymous]
        [HttpGet("GetProductByID/{ProductId}")]
        public IActionResult GetProductById(int ProductId)
        {
            try
            {
                var product = _productService.GetProductById(ProductId);
                if (product == null) return NotFound("No product found.");

                return Ok(_mapper.Map<ProductDTO>(product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving product. {ex.Message}");
            }
        }
        private string? GetUserRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);

                // Extract the 'role' claim
                var roleClaim = jwtToken.Claims.FirstOrDefault (c => c.Type == "role" || c.Type == "unique_name" );
                

                return roleClaim?.Value; // Return the role or null if not found
            }

            throw new UnauthorizedAccessException("Invalid or unreadable token.");
        }
    }
}
