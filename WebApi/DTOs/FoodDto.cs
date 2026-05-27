namespace WebApi.DTOs
{
    public class FoodDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public string CategoryName { get; set; }
        public List<string> Allergens { get; set; }
    }
}