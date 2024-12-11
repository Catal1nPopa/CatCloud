namespace Application.DTOs.Auth
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EmailConfirmed { get; set; }
        public double TotalStorage { get; set; }
        public double AvailableStorage { get; set; }
        public DateTime Added { get; set; }
    }
}
