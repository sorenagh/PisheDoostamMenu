namespace CafeMenuApi.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public int PlaceId { get; set; }
        public PlaceDto? Place { get; set; }
    }
    
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PlaceId { get; set; }
    }
    
    public class UpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PlaceId { get; set; }
    }
} 