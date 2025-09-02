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
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService reviewService, IMapper mapper)
        {
            _reviewService = reviewService;
            _mapper = mapper;
        }

        // ✅ Add Review (only logged-in user)
        [HttpPost("AddReview")]
        public IActionResult AddReview(int pid, ReviewDTO reviewDto)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = GetUserIdFromToken(token);
                int uid = int.Parse(userId);

                _reviewService.AddReview(uid, pid, reviewDto);
                return Ok("Review added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding review. {ex.Message}");
            }
        }

        // ✅ Public - Get all reviews for a product
        [AllowAnonymous]
        [HttpGet("GetAllReviews")]
        public IActionResult GetAllReviews(
            [FromQuery] int productId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("PageNumber and PageSize must be greater than 0.");
                }

                var reviews = _reviewService.GetAllReviews(pageNumber, pageSize, productId);

                if (reviews == null || !reviews.Any())
                {
                    return NotFound("No reviews found for this product.");
                }

                // ✅ Map directly to DTOs
                var reviewDtos = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);

                return Ok(reviewDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving reviews. {ex.Message}");
            }
        }

        // ✅ Delete review (only by the owner)
        [HttpDelete("DeleteReview/{ReviewId}")]
        public IActionResult DeleteReview(int ReviewId)
        {
            try
            {
                var review = _reviewService.GetReviewById(ReviewId);
                if (review == null)
                    return NotFound($"Review with ID {ReviewId} not found.");

                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = GetUserIdFromToken(token);
                int uid = int.Parse(userId);

                if (review.UID == uid)
                {
                    _reviewService.DeleteReview(ReviewId);
                    return Ok($"Review with ID {ReviewId} deleted successfully.");
                }

                return BadRequest("You are not authorized to delete this review.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting review. {ex.Message}");
            }
        }

        // ✅ Update review (only by the owner)
        [HttpPut("UpdateReview/{ReviewId}")]
        public IActionResult UpdateReview(int ReviewId, ReviewDTO reviewDto)
        {
            try
            {
                if (reviewDto == null)
                    return BadRequest("Review data is required.");

                var review = _reviewService.GetReviewById(ReviewId);
                if (review == null)
                    return NotFound($"Review with ID {ReviewId} not found.");

                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = GetUserIdFromToken(token);
                int uid = int.Parse(userId);

                if (review.UID == uid)
                {
                    _reviewService.UpdateReview(ReviewId, reviewDto);
                    return Ok($"Review with ID {ReviewId} updated successfully.");
                }

                return BadRequest("You are not authorized to update this review.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating review. {ex.Message}");
            }
        }

        private string? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);

                // Extract the 'sub' claim
                var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");


                return (subClaim?.Value); // Return both values as a tuple
            }

            throw new UnauthorizedAccessException("Invalid or unreadable token.");
        }
    }

    
}
