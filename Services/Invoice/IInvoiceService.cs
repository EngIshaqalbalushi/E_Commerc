using E_CommerceSystem.DTOs;

namespace E_CommerceSystem.Services
{
    public interface IInvoiceService
    {
        // Builds the InvoiceDTO from DB for a given order id
        InvoiceDTO BuildInvoice(int orderId);

        // Renders the PDF as bytes
        byte[] RenderInvoicePdf(InvoiceDTO dto);
    }
}
