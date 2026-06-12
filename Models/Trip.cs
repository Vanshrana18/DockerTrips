namespace DockerTrips.Api.Models
{
    public class Trip
    {
        public int Id { get; set; }

        public string Destination { get; set; } = "";

        public decimal Price { get; set; }

        public int Days { get; set; }
    }
}
