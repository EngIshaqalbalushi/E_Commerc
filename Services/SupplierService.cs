using E_CommerceSystem.Models;
using E_CommerceSystem.Repositories;

namespace E_CommerceSystem.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _supplierRepository.GetAllAsync();
        }

        public async Task<Supplier> GetByIdAsync(int id)
        {
            return await _supplierRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Supplier supplier)
        {
            await _supplierRepository.AddAsync(supplier);
        }

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            return await _supplierRepository.UpdateAsync(supplier);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _supplierRepository.DeleteAsync(id);
        }
    }
}
