using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using E_CommerceSystem.Models;
using E_CommerceSystem.Models.DTOs;
using E_CommerceSystem.Services;

namespace E_CommerceSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;

        public SupplierController(ISupplierService supplierService, IMapper mapper)
        {
            _supplierService = supplierService;
            _mapper = mapper;
        }

        // GET: api/supplier
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDTO>>> GetSuppliers()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<SupplierDTO>>(suppliers));
        }

        // GET: api/supplier/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDTO>> GetSupplier(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null) return NotFound();

            return Ok(_mapper.Map<SupplierDTO>(supplier));
        }

        // POST: api/supplier
        [HttpPost]
        public async Task<ActionResult<SupplierDTO>> CreateSupplier(SupplierDTO supplierDto)
        {
            var supplier = _mapper.Map<Supplier>(supplierDto);
            await _supplierService.AddAsync(supplier);

            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.SID }, _mapper.Map<SupplierDTO>(supplier));
        }

        // PUT: api/supplier/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, SupplierDTO supplierDto)
        {
            if (id != supplierDto.SID) return BadRequest();

            var supplier = _mapper.Map<Supplier>(supplierDto);
            var updated = await _supplierService.UpdateAsync(supplier);

            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE: api/supplier/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var deleted = await _supplierService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
