namespace BuyMyHouse.Domain.DTO
{
    public class HouseDTO
    {
        public string HouseId { get; set; }
        public string Title { get; set; }
        public string Decsription { get; set; }
        public double Mortgage { get; set; }
        public string ImageURL { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string Addition { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }


        public HouseDTO()
        {

        }

        public HouseDTO(string houseId, string title, string decsription, double mortgage, string imageURL, string street, string houseNumber, string addition, string zipCode, string country)
        {
            HouseId = houseId;
            Title = title;
            Decsription = decsription;
            Mortgage = mortgage;
            ImageURL = imageURL;
            Street = street;
            HouseNumber = houseNumber;
            Addition = addition;
            ZipCode = zipCode;
            Country = country;
        }
    }
}
