using IUstaApi.Models.Entities;

namespace IUstaApi.Models.DTOs.Worker
{
    public class ProfileDto
    {
        public string Email { get; set; }
        public int TotalRequestsCount { get; set; }
        public int InactiveRequestsCount { get; set; }
        public int ActiveRequestsCount { get; set; }
        public int CompletedRequestsCount { get; set; }
        public double Rating { get; set; }
    }
}
