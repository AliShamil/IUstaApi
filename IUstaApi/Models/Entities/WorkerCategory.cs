
namespace IUstaApi.Models.Entities
{
    public class WorkerCategory : BaseEntity
    {
        public string WorkerId { get; set; }
        public Guid CategoryId { get; set; }
        public virtual AppUser Worker { get; set; }
        public virtual Category Category { get; set; }
    }
}
