using AutoMapper;
using E_CommerceSystem.Models;
using E_CommerceSystem.Models.DTOs;

namespace E_CommerceSystem.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Category
            CreateMap<Category, CategoryDTO>().ReverseMap();

            // Supplier
            CreateMap<Supplier, SupplierDTO>().ReverseMap();

            // Product
            CreateMap<Product, ProductDTO>().ReverseMap();

            // Review
            CreateMap<Review, ReviewDTO>().ReverseMap();

            // User
            CreateMap<User, UserDTO>().ReverseMap();

            // Order + extra DTOs
            CreateMap<Order, OrdersOutputDTO>().ReverseMap();
            CreateMap<OrderProducts, OrderItemDTO>().ReverseMap();
        }
    }
}
