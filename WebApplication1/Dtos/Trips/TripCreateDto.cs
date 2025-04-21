namespace WebApplication1.Dtos.Trips
{
    public class TripCreateDto
    {
       
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public decimal Money { get; set; }
        public int AvailablePeople { get; set; }
        public int MaxPeople { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? OutOfDate { get; set; }
        public List<string>? IncludedItems { get; set; }
        public List<string>? ExcludedItems { get; set; }
        public List<int>? Sites { get; set; }

    }
}
