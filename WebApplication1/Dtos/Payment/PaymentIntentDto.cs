namespace WebApplication1.Dtos.Payment
{
    public class PaymentIntentDto
    {
        public string Name { get; set; }
        public long Price { get; set; } // Price in cents
        public int Quantity { get; set; }
    }
}
