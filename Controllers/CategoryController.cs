using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using E_CommerceSystem.Models;
using E_CommerceSystem.Models.DTOs;
using E_CommerceSystem.Services;

namespace E_CommerceSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        // GET: api/category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryDTO>>(categories));
        }

        // GET: api/category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();

            return Ok(_mapper.Map<CategoryDTO>(category));
        }

        // POST: api/category
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            await _categoryService.AddAsync(category);

            return CreatedAtAction(nameof(GetCategory), new { id = category.CID }, _mapper.Map<CategoryDTO>(category));
        }

        // PUT: api/category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDTO categoryDto)
        {
            if (id != categoryDto.CID) return BadRequest();

            var category = _mapper.Map<Category>(categoryDto);
            var updated = await _categoryService.UpdateAsync(category);

            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE: api/category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
