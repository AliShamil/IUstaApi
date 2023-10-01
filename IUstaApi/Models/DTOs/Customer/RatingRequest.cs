namespace IUstaApi.Models.DTOs.Customer
{
    public class RatingRequest
    {
        public string WorkerId { get; set; } = string.Empty;
        public double Value { get; set; } = default;
    }
}
