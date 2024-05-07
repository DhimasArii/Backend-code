using Language.Data;
using Language.DTOs.CourseShcedule;
using Language.DTOs.PaymentMethod;
using Language.DTOs.User;
using Language.Models;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        public IActionResult CreatePayment([FromBody] PaymentDTO paymentDto)
        {
            Payment payment = new Payment
            {
                id_payment_method = Guid.NewGuid(),
                payment_name = paymentDto.payment_name,
                payment_description = paymentDto.payment_description,
                payment_status = paymentDto.payment_status,
                payment_icon = paymentDto.payment_icon,
            };
            bool result = _payment.CreatePaymentMethod(payment);

            if (result)
            {
                return Ok("Course created successfully.");
            }
            else
            {
                return BadRequest("Failed to create course.");
            }
        }

        [HttpPut("{id_payment_method}")]
        [Authorize(Roles = "admin")]
        public IActionResult Put(Guid id_payment_method, [FromBody] PaymentDTO paymentDto)
        {

            if (paymentDto == null)
                return BadRequest("Data Should be Inputed");

            Payment payment = new Payment
            {
                id_payment_method = Guid.NewGuid(),
                payment_name = paymentDto.payment_name,
                payment_description = paymentDto.payment_description,
                payment_icon = paymentDto.payment_icon,
                payment_status = paymentDto.payment_status,

            };

            bool result = _payment.UpdatePaymentMethod(id_payment_method, payment);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, "error occur");
            }

        }

        [HttpDelete("{id_payment_method}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(Guid id_payment_method)
        {
            bool result = _payment.Delete(id_payment_method);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, "error occur");
            }
        }
    }
}
