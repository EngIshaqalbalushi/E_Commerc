using E_CommerceSystem.Models;
using E_CommerceSystem.Repositories;


namespace E_CommerceSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IProductService _productService;
        private readonly IOrderProductsService _orderProductsService;
        private readonly IUserRepo _userRepo;
        private readonly IEmailService _emailService;
        public OrderService(IOrderRepo orderRepo, IProductService productService, IOrderProductsService orderProductsService, IEmailService emailService)
        {
            _orderRepo = orderRepo;
            _productService = productService;
            _orderProductsService = orderProductsService;
            
            _emailService = emailService;
        }

        // ✅ Get all orders for a user
        public IEnumerable<Order> GetOrderByUserId(int uid)
        {
            var orders = _orderRepo.GetOrderByUserId(uid);
            if (orders == null || !orders.Any())
                throw new KeyNotFoundException($"No orders found for user ID {uid}.");
            return orders;
        }

        // ✅ Get order details (products inside a specific order)
        public IEnumerable<OrdersOutputDTO> GetOrderById(int oid, int uid)
        {
            var items = new List<OrdersOutputDTO>();
            var order = _orderRepo.GetOrderById(oid);

            if (order == null || order.UID != uid)
                throw new InvalidOperationException($"No order found with ID {oid} for user {uid}.");

            var products = _orderProductsService.GetOrdersByOrderId(oid);
            foreach (var p in products)
            {
                var product = _productService.GetProductById(p.PID);
                items.Add(new OrdersOutputDTO
                {
                    ProductName = product.ProductName,
                    Quantity = p.Quantity,
                    OrderDate = order.OrderDate,
                    TotalAmount = p.Quantity * product.Price,
                });
            }

            return items;
        }

        // ✅ Order Summary DTOs (new feature)
        public IEnumerable<OrderSummaryDTO> GetOrderSummaries(int uid)
        {
            var orders = _orderRepo.GetOrderByUserId(uid);

            return orders.Select(order => new OrderSummaryDTO
            {
                OrderId = order.OID,
                UserName = order.User?.UName,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                Products = order.OrderProducts.Select(op => new OrderProductDTO
                {
                    ProductName = op.Product.ProductName,
                    Quantity = op.Quantity,
                    Price = op.Product.Price
                }).ToList()
            });
        }

        // ✅ Place order
        public void PlaceOrder(List<OrderItemDTO> items, int uid)
{
    decimal totalOrderPrice = 0;

    // Validate stock first
    foreach (var item in items)
    {
        var product = _productService.GetProductByName(item.ProductName);
        if (product == null)
            throw new Exception($"{item.ProductName} not found.");
        if (product.Stock < item.Quantity)
            throw new Exception($"{item.ProductName} is out of stock.");
    }

    // Create order
    var order = new Order 
    { 
        UID = uid, 
        OrderDate = DateTime.Now, 
        TotalAmount = 0, 
        Status = OrderStatus.Pending // 👈 set initial status
    };

    _orderRepo.AddOrder(order);

    foreach (var item in items)
    {
        var product = _productService.GetProductByName(item.ProductName);
        var totalPrice = item.Quantity * product.Price;

        product.Stock -= item.Quantity;
        totalOrderPrice += totalPrice;

        var orderProducts = new OrderProducts
        {
            OID = order.OID,
            PID = product.PID,
            Quantity = item.Quantity
        };
        _orderProductsService.AddOrderProducts(orderProducts);
        _productService.UpdateProduct(product);
    }

    order.TotalAmount = totalOrderPrice;
    _orderRepo.UpdateOrder(order);

            // ✅ Send email confirmation
            // Send email confirmation
            var user = _userRepo.GetUserById(uid);
            if (user != null)
            {
                _emailService.SendEmail(
                    user.Email,
                    "Order Confirmation",
                    $"Hello {user.UName},<br/>" +
                    $"Your order #{order.OID} has been placed successfully.<br/>" +
                    $"Total Amount: {order.TotalAmount:C}"
                );
            }


        }


        public List<OrderProducts> GetAllOrders(int uid)
        {
            var orders = _orderRepo.GetOrderByUserId(uid);

            if (orders == null || !orders.Any())
                throw new InvalidOperationException($"No orders found for user ID {uid}.");

            var allOrderProducts = new List<OrderProducts>();

            foreach (var order in orders)
            {
                var orderProducts = _orderProductsService.GetOrdersByOrderId(order.OID);
                if (orderProducts != null)
                    allOrderProducts.AddRange(orderProducts);
            }

            return allOrderProducts;
        }

        public void CancelOrder(int orderId, int userId)
        {
            var order = _orderRepo.GetOrderById(orderId);
            if (order == null || order.UID != userId)
                throw new Exception("Order not found or unauthorized.");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Order already cancelled.");

            // Update status
            order.Status = OrderStatus.Cancelled;
            _orderRepo.UpdateOrder(order);

            // Restore stock
            foreach (var item in order.OrderProducts)
            {
                var product = _productService.GetProductById(item.PID);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                    _productService.UpdateProduct(product);
                }
            }

            // ✅ Send cancellation email
            var user = _userRepo.GetUserById(userId);
            if (user != null)
            {
                _emailService.SendEmail(
                    user.Email,
                    "Order Cancellation",
                    $"Hello {user.UName},<br/>" +
                    $"Your order #{order.OID} has been <b>cancelled</b>.<br/>" +
                    $"If you have questions, please contact support."
                );
            }
        }


        public void UpdateOrderStatus(int orderId, int userId, OrderStatus status)
        {
            var order = _orderRepo.GetOrderById(orderId);

            if (order == null || order.UID != userId)
                throw new Exception("Order not found or unauthorized.");

            order.Status = status;  // ✅ uses enum instead of string
            _orderRepo.UpdateOrder(order);
        }



        // ✅ Add, Update, Delete
        public void AddOrder(Order order) => _orderRepo.AddOrder(order);

        public void UpdateOrder(Order order) => _orderRepo.UpdateOrder(order);

        public void DeleteOrder(int oid)
        {
            var order = _orderRepo.GetOrderById(oid);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {oid} not found.");

            _orderRepo.DeleteOrder(oid);
        }
    }
}
