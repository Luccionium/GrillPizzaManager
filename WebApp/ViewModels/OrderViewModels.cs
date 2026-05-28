namespace WebApp.ViewModels
{
    public class PlaceOrderDto
    {
        public List<OrderItemDto> Items { get; set; } = new();
        public string? Note { get; set; }

        public string? DeliveryAddress { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}