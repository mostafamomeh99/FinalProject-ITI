   namespace WebApplication1.Dtos.Payment
   {
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
        public string ClientUrl { get; set; }
        public string CancelUrl { get; set; }
    }
   }
   