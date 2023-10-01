
namespace IUstaApi.Models.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<WorkerCategory> WorkerCategories { get; set; } = new List<WorkerCategory>();
    }
}
