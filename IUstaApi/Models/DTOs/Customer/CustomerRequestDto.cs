namespace IUstaApi.Models.DTOs.Customer
{
    public class CustomerRequestDto
    {
        public string Id { get; set; }
        public string WorkerEmail { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsCompleted { get; set; }
    }
}
