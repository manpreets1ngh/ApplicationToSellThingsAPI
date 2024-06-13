namespace ApplicationToSellThings.APIs.Models
{
    public class OrderApiResponseModel
    {
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Tax { get; set; }
        public string OrderStatus { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; }
        public DateTime? OrderCreatedAt { get; set; }

    }
}
