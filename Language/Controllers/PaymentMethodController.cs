using Language.Data;
using Language.Models;
using Microsoft.AspNetCore.Mvc;

namespace Language.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly PaymentMethodData _payment;

        public PaymentMethodController(PaymentMethodData paymentData)
        {
            _payment = paymentData;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                List<Payment> payments = _payment.GetPayments();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
