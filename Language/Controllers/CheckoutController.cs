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
                List<Checkout> checkouts = _checkout.GetAll();
                return Ok(checkouts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetAllByUserId")]
        public IActionResult GetAllByCheckoutId(Guid user_id,string sortOrder)
        {
            try
            {
                List<Checkout> checkouts = _checkout.GetAllByUserId(user_id,sortOrder);
                return Ok(checkouts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("AddCheckout")]
        public IActionResult AddCheckout([FromBody] CheckoutDTO checkoutDto)
        {
            if (checkoutDto == null)
            {
                return BadRequest("Data Should be Inputed");
            }

            Checkout checkout = new Checkout
            {
                checkout_id = Guid.NewGuid(),
                user_id = checkoutDto.user_id,
                id_payment_method = checkoutDto.id_payment_method,

            };
            

            try
            {
                bool result = _checkout.InsertCheckout(checkout);
                if (result)
                {
                    return StatusCode(201, checkout.checkout_id);
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
                schedule_id = detailCheckoutDto.schedule_id,
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

        [HttpPost("BuyNow")]
        public IActionResult BuyNow(BuyNowDTO request)
        {
            try
            {
                Checkout checkout = new Checkout
                {
                    checkout_id = Guid.NewGuid(),
                    user_id = request.user_id,
                    id_payment_method = request.id_payment_method,
                    create_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                };

                Detail_Checkout detailCheckout = new Detail_Checkout
                {
                    detail_checkout_id = Guid.NewGuid(),
                    checkout_id = checkout.checkout_id,
                    schedule_id = request.schedule_id,
                    checklist = true 
                };

                bool success = _checkout.CreateBuyNow(checkout, detailCheckout);

                if (success)
                {
                    return Ok(new { message = "Buy now successful" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to create buy now" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpPut("UpdateDetailCheckout")]
        public IActionResult Put(Guid detail_checkout_id, [FromBody] UpdateDetailCheckoutDTO updateDetailCheckoutDto)
        {
            if (updateDetailCheckoutDto == null)
            {
                return BadRequest("Data Should be Inputed");
            }

            Detail_Checkout details = new Detail_Checkout
            {
                checklist = updateDetailCheckoutDto.checklist,

            };

            try
            {
                bool result = _checkout.UpdateDetailCheckout(detail_checkout_id, details);
                if (result)
                {
                    return Ok("Update successful");
                }
                else
                {
                    return StatusCode(500, "error occur");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("DeleteDetailCheckout")]
        public IActionResult Delete(Guid detail_checkout_id)
        {
            bool result = _checkout.DeleteDetailCheckout(detail_checkout_id);

            if (result)
            {
                return Ok("Delete successful");
            }
            else
            {
                return StatusCode(500, "error occur");
            }
        }
    }
}
