using Language.Data;
using Language.DTOs.Checkout;
using Language.DTOs.DetailCheckout;
using Language.Models;
using Microsoft.AspNetCore.Mvc;

namespace Language.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutData _checkout;

        public CheckoutController(CheckoutData checkoutData)
        { 
            _checkout = checkoutData;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                List<CheckoutDTO> checkouts = _checkout.GetAll();
                return Ok(checkouts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetAllByCheckoutId")]
        public IActionResult GetAllByCheckoutId(Guid checkout_id)
        {
            try
            {
                List<CheckoutDTO> checkouts = _checkout.GetAllByCheckoutId(checkout_id);
                return Ok(checkouts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        
        [HttpPost("AddDetailCheckout")]
        public IActionResult AddDetailCheckout([FromBody] DetailCheckoutDTO detailCheckoutDto)
        {
            if (detailCheckoutDto == null)
            {
                return BadRequest("Data Should be Inputed");
            }

            Detail_Checkout details = new Detail_Checkout
            {
                detail_checkout_id = Guid.NewGuid(),
                checkout_id = detailCheckoutDto.checkout_id,
                course_id = detailCheckoutDto.course_id,
                checklist = detailCheckoutDto.checklist,

            };

            try
            {
                bool result = _checkout.InsertDetailCheckout(details);
                if (result)
                {
                    return StatusCode(201, details.detail_checkout_id);
                }
                else
                {
                    return StatusCode(500, "Failed to add detail checkout.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
