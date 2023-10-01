

namespace IUstaApi.Models.Entities
{
    public class WorkRequest : BaseEntity
    {
        public string WorkerEmail { get; set; }
        public string ClientEmail { get; set; }
        public string Message { get; set; }
        public double? Rating { get; set; }
        public bool? IsAccepted { get; set; }
        public bool IsCompleted { get; set; }
    }
}
