using API.Errors;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using System.IO;
using System.Threading.Tasks;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Order = Core.Entities.OrderAggregate.Order;

namespace API.Controllers;
public class PaymentsController : BaseApiController
{
    private readonly IPaymentService paymentService;
    private readonly ILogger<PaymentsController> logger;
    private readonly string whSecret;

    public PaymentsController(
        IPaymentService paymentService,
        ILogger<PaymentsController> logger,
        IConfiguration configuration)
    {
        this.paymentService = paymentService;
        this.logger = logger;
        whSecret = configuration["StripeSettings:WhSecret"] ?? "";
    }

    [Authorize]
    [HttpPost("{basketId}")]
    public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
    {
        CustomerBasket basket = await paymentService.CreateOrUpdatePaymentIntent(basketId);

        if (basket == null)
        {
            return BadRequest(new ApiResponse(400, "Problem with your basket. Can't create or update payment intent."));
        }

        return basket;
    }

    [HttpPost("webhook")]
    public async Task<ActionResult> StripeWebhook()
    {
        string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        Event stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], whSecret);
        PaymentIntent paymentIntent;
        Order order;

        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                logger.LogInformation("Payment succeeded: ", paymentIntent.Id);
                order = await paymentService.UpdateOrderPaymentSucceeded(paymentIntent.Id);
                logger.LogInformation("Order updated to payment received: ", order.Id);
                break;
            case "payment_intent.payment_failed":
                paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                logger.LogInformation("Payment failed: ", paymentIntent.Id);
                order = await paymentService.UpdateOrderPaymentFailed(paymentIntent.Id);
                logger.LogInformation("Payment failed: ", order.Id);
                break;
            default:
                break;
        }

        return new EmptyResult();
    }
}
