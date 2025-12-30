namespace SportsGearStore.Models
{
    public class User
    {
        public int Id { get; set; } // primary key
        public string Username { get; set; }
        public string Useremail { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Boolean IsAdmin { get; set; }
    }
}
