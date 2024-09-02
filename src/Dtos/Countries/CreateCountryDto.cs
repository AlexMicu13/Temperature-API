namespace Backend.Dtos.Countries
{
    public class CreateCountryDto
    {
        public required string Nume { get; set; }
        public required double Lat { get; set; }
        public required double Lon { get; set; }
    }
}
