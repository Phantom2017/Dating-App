namespace DatingApp.API.Dtos
{
    public class LikeDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string KnownAs { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        public string PhotoUrl { get; set; }
    }
}