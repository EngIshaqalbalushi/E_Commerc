using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoice;

        public InvoiceController(IInvoiceService invoice)
        {
            _invoice = invoice;
        }

        // GET: /api/Invoice/123/pdf
        [HttpGet("{orderId:int}/pdf")]
        public IActionResult GetPdf(int orderId)
        {
            var dto = _invoice.BuildInvoice(orderId);
            var pdf = _invoice.RenderInvoicePdf(dto);
            var fileName = $"invoice_{orderId}.pdf";
            return File(pdf, "application/pdf", fileName);
        }
    }
}
