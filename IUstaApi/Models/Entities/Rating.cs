using System.Diagnostics;

namespace IUstaApi.Models.Entities
{
    public class Rating
    {
        public string Id { get; set; }
        public double Value { get; set; }

        public string WorkerId { get; set; }
        public string CustomerId { get; set; }
        public virtual AppUser Customer { get; set; }
        public virtual AppUser Worker { get; set; }
    }
}
