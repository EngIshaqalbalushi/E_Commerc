using E_CommerceSystem.Models;

namespace E_CommerceSystem.Services
{
    public interface IOrderService
    {
        IEnumerable<Order> GetOrderByUserId(int uid);
        IEnumerable<OrdersOutputDTO> GetOrderById(int oid, int uid);
        IEnumerable<OrderSummaryDTO> GetOrderSummaries(int userId);
        List<OrderProducts> GetAllOrders(int uid);


        void UpdateOrderStatus(int orderId, int userId, OrderStatus status);

        



        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int oid);
        void PlaceOrder(List<OrderItemDTO> items, int uid);

        void CancelOrder(int orderId, int userId);

    }
}
