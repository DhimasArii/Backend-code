using Language.Data;
using Language.DTOs.DetailInvoice;
using Language.DTOs.Invoice;
using Language.DTOs.User;
using Language.Models;
using Microsoft.AspNetCore.Mvc;

namespace Language.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceData _invoice;

        public InvoiceController(InvoiceData invoiceData)
        {
            _invoice = invoiceData;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                List<Invoice> invoices = _invoice.GetAll();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetAllByUserId")]
        public IActionResult GetAllByUserId(Guid user_id)
        {
            try
            {
                List<Invoice> invoices = _invoice.GetAllByUserId(user_id);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetAllByInvoiceId")]
        public IActionResult Get(Guid invoice_id)
        {
            try
            {
                List<Invoice> invoices = _invoice.GetAllByInvoiceId(invoice_id);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("CreateInvoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceDTO invoiceDto)
        {
            try
            {
                

                if (invoiceDto == null || invoiceDto.user_id == Guid.Empty)
                {
                    return BadRequest("Invalid user data");
                }

                Invoice invoices = new Invoice
                {
                    invoice_id = Guid.NewGuid(),
                    user_id = invoiceDto.user_id,
                    //invoice_number = invoiceDto.invoice_number,
                    id_payment_method = invoiceDto.id_payment_method,
                    invoice_date = DateTime.Now,
                    total_price = 0
                };

                Detail_Invoice detail_Invoice = new Detail_Invoice
                {
                    //detail_invoice_id = Guid.NewGuid(),
                    invoice_id = invoices.invoice_id
                };

                bool result = _invoice.CreateInvoice(invoices, detail_Invoice,invoiceDto.checkout_id);

                if (result)
                {
                    return StatusCode(201, "Data successfully inserted");
                }
                else
                {
                    return StatusCode(500, "Data not inserted");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("CreateSingleInvoice")]
        public async Task<IActionResult> CreateSingleInvoice([FromBody] DetailInvoiceDTO detailInvoiceDto)
        {
            try
            {


                if (detailInvoiceDto == null)
                {
                    return BadRequest("Invalid invoice data");
                }

                Invoice invoices = new Invoice
                {
                    invoice_id = Guid.NewGuid(),
                    user_id = detailInvoiceDto.user_id,
                    id_payment_method = detailInvoiceDto.id_payment_method,
                    invoice_date = DateTime.Now,
                    total_price = 0
                };

                Detail_Invoice detail_Invoice = new Detail_Invoice
                {
                    detail_invoice_id = Guid.NewGuid(),
                    invoice_id = invoices.invoice_id,
                    schedule_id = detailInvoiceDto.schedule_id,
                };

                bool result = _invoice.CreateSingleInvoice(invoices, detail_Invoice);

                if (result)
                {
                    return StatusCode(201, "Data successfully inserted");
                }
                else
                {
                    return StatusCode(500, "Data not inserted");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
