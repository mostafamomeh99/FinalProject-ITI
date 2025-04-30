using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IRepositoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using WebApplication1.Dtos.Book;
using WebApplication1.Dtos.Book.WebApplication1.Dtos.Book;
using WebApplication1.Dtos.Payment.WebApplication1.Dtos.Book;

namespace WebApplication1.Controllers
{

    [Route("api/[controller]")]

    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _stripeSecretKey;

        public PaymentsController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;

            // Get and validate Stripe key
            _stripeSecretKey = _configuration["Stripe:SecretKey"] ??
                throw new ApplicationException("Stripe:SecretKey is not configured in appsettings.json");

            StripeConfiguration.ApiKey = _stripeSecretKey;
        }
        [HttpPost("create-checkout-session")]
        public async Task<ActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionDto dto)
        {
            try
            {
                var book = await _unitOfWork.Books.GetByStringIdAsync(dto.BookingId);
                if (book == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Booking Not Found",
                        Detail = $"Booking with ID {dto.BookingId} doesn't exist"
                    });
                }

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(dto.Amount * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Booking #{dto.BookingId}",
                        },
                    },
                    Quantity = 1,
                },
            },
                    Mode = "payment",
                    SuccessUrl = $"{dto.BaseUrl}/booking-confirmation/{dto.BookingId}",
                    CancelUrl = $"{dto.BaseUrl}/booking-canceled",
                    Metadata = new Dictionary<string, string>
            {
                { "bookingId", dto.BookingId },
                { "tripId", dto.TripId }
            }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return Ok(new
                {
                    sessionId = session.Id,
                    url = session.Url
                });
            }
            catch (StripeException e)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Stripe Error",
                    Detail = e.Message
                });
            }
        }

        [HttpPost("create-payment-intent")]
        public async Task<ActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentDto dto)
        {
            try
            {
                var book = await _unitOfWork.Books.GetByStringIdAsync(dto.BookingId);
                if (book == null)
                {
                    return NotFound(new ProblemDetails { Title = "Booking Not Found", Detail = $"Booking with ID {dto.BookingId} doesn't exist" });
                }

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(dto.Amount * 100),
                    Currency = "usd",
                    ReceiptEmail = dto.CustomerEmail,
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string> { { "bookingId", dto.BookingId }, { "tripId", dto.TripId } },
                    Confirm = true, // Automatically confirm the PaymentIntent
                    PaymentMethod = "pm_card_visa", // Test card (only for testing!)
                };

                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(options);

                return Ok(new
                {
                    clientSecret = intent.ClientSecret,
                    paymentIntentId = intent.Id,
                    amount = dto.Amount,
                    currency = "usd",
                    status = intent.Status // "succeeded" if successful
                });
            }
            catch (StripeException e)
            {
                return BadRequest(new ProblemDetails { Title = "Stripe Error", Detail = e.Message });
            }
        }
        [HttpPost("webhook")]
        public async Task<ActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _configuration["Stripe:WebhookSecret"]
            );

            if (stripeEvent.Type == "payment_intent.succeeded") // Changed from Events.PaymentIntentSucceeded
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                // Update your booking status here
                var bookId = paymentIntent.Metadata["bookId"];
                var book = await _unitOfWork.Books.GetByStringIdAsync(bookId);

                if (book != null)
                {
                    // Update booking status or other properties
                     _unitOfWork.Compelet(); // Changed from Compelet
                }
            }

            return Ok();
        }
    }
}