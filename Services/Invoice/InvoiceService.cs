using E_CommerceSystem.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace E_CommerceSystem.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IOrderService _orders;
        private readonly IOrderProductsService _orderProducts;
        private readonly IProductService _products;
        private readonly IUserService _users;

        public InvoiceService(
            IOrderService orders,
            IOrderProductsService orderProducts,
            IProductService products,
            IUserService users)
        {
            _orders = orders;
            _orderProducts = orderProducts;
            _products = products;
            _users = users;
        }

        public InvoiceDTO BuildInvoice(int orderId)
        {
            // 1) Load order + user
            var order = _orders.GetOrderEntity(orderId)
              ?? throw new KeyNotFoundException($"Order {orderId} not found.");

            var user = _users.GetUserById(order.UID)
                       ?? throw new KeyNotFoundException($"User {order.UID} not found.");

            // 2) Load order lines
            var lines = _orderProducts.GetOrdersByOrderId(orderId);

            var dto = new InvoiceDTO
            {
                InvoiceNumber = order.OID,             // ✅ your entity uses OID
                InvoiceDate = order.OrderDate,       // ✅ you have OrderDate, not CreatedAt
                CustomerName = user.UName ?? "",      // ✅ only UName exists
                CustomerEmail = user.Email ?? "",
                CustomerPhone = "",                    // not in your model → leave blank
                BillingAddress = "",                    // not in your model → leave blank
                Shipping = 0m,                    // no field in your model → default
                Discount = 0m,                    // no field in your model → default
                Tax = 0m                     // no field in your model → default
            };


            foreach (var l in lines)
            {
                var p = _products.GetProductById(l.PID)
                        ?? throw new KeyNotFoundException($"Product {l.PID} not found.");

                dto.Lines.Add(new InvoiceLineDTO
                {
                    ProductId = p.PID,
                    ProductName = p.ProductName,
                    Quantity = l.Quantity,
                    UnitPrice = p.Price
                });
            }

            return dto;
        }

        public byte[] RenderInvoicePdf(InvoiceDTO dto)
        {
            // required by QuestPDF once per app start; safe to call more than once
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Header().Element(ComposeHeader(dto));
                    page.Content().Element(ComposeContent(dto));
                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.Span("Thank you for your purchase • ");
                        txt.CurrentPageNumber();
                        txt.Span(" / ");
                        txt.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static Action<IContainer> ComposeHeader(InvoiceDTO dto) => container =>
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Your Store Name").Bold().FontSize(18);
                    col.Item().Text("www.yourstore.example");
                    col.Item().Text("support@yourstore.example");
                    col.Item().Text("Phone: +968-0000-0000");
                });

                row.ConstantItem(220).Column(col =>
                {
                    col.Item().Background(Colors.Grey.Lighten3).Padding(8).Column(cc =>
                    {
                        cc.Item().Text("INVOICE").Bold().FontSize(14);
                        cc.Item().Text($"No: {dto.InvoiceNumber}");
                        cc.Item().Text($"Date: {dto.InvoiceDate:yyyy-MM-dd HH:mm}");
                    });
                });
            });
        };

        private static Action<IContainer> ComposeContent(InvoiceDTO dto) => container =>
        {
            container.PaddingVertical(10).Column(col =>
            {
                // Bill To
                col.Item().Row(r =>
                {
                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Bill To").Bold();
                        c.Item().Text(dto.CustomerName ?? string.Empty);
                        if (!string.IsNullOrWhiteSpace(dto.CustomerEmail))
                            c.Item().Text(dto.CustomerEmail);
                        if (!string.IsNullOrWhiteSpace(dto.CustomerPhone))
                            c.Item().Text(dto.CustomerPhone);
                        if (!string.IsNullOrWhiteSpace(dto.BillingAddress))
                            c.Item().Text(dto.BillingAddress);
                    });
                });

                col.Item().LineHorizontal(0.5f);

                // ------- Helpers in scope for entire table ----------
                Func<IContainer, IContainer> headerCell = c =>
                    c.DefaultTextStyle(x => x.SemiBold())
                     .Background(Colors.Grey.Lighten3)
                     .PaddingVertical(4).PaddingHorizontal(6);

                Func<IContainer, IContainer> dataCell = c =>
                    c.PaddingVertical(3).PaddingHorizontal(6);
                // ----------------------------------------------------

                // Items table
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(6);  // Product
                        cols.RelativeColumn(2);  // Qty
                        cols.RelativeColumn(2);  // Unit
                        cols.RelativeColumn(2);  // Line
                    });

                    table.Header(h =>
                    {
                        h.Cell().Element(headerCell).Text("Product");
                        h.Cell().Element(headerCell).AlignRight().Text("Qty");
                        h.Cell().Element(headerCell).AlignRight().Text("Unit Price");
                        h.Cell().Element(headerCell).AlignRight().Text("Line Total");
                    });

                    foreach (var l in dto.Lines)
                    {
                        table.Cell().Element(dataCell).Text(l.ProductName ?? string.Empty);
                        table.Cell().Element(dataCell).AlignRight().Text(l.Quantity.ToString());
                        table.Cell().Element(dataCell).AlignRight().Text($"{l.UnitPrice:N3}");
                        table.Cell().Element(dataCell).AlignRight().Text($"{l.LineTotal:N3}");
                    }

                    // Summary rows
                    table.Cell().ColumnSpan(3).Element(dataCell).AlignRight().Text("Subtotal");
                    table.Cell().Element(dataCell).AlignRight().Text($"{dto.Subtotal:N3}");

                    table.Cell().ColumnSpan(3).Element(dataCell).AlignRight().Text("Shipping");
                    table.Cell().Element(dataCell).AlignRight().Text($"{dto.Shipping:N3}");

                    table.Cell().ColumnSpan(3).Element(dataCell).AlignRight().Text("Tax");
                    table.Cell().Element(dataCell).AlignRight().Text($"{dto.Tax:N3}");

                    table.Cell().ColumnSpan(3).Element(dataCell).AlignRight().Text("Discount");
                    table.Cell().Element(dataCell).AlignRight().Text($"-{dto.Discount:N3}");

                    table.Cell().ColumnSpan(3).Element(dataCell).AlignRight().Text("Total").Bold();
                    table.Cell().Element(dataCell).AlignRight().Text($"{dto.Total:N3}").Bold();
                });
            });
        };

    }
}
