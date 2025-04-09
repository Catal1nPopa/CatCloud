namespace Domain.Entities.Auth
{
    public class UserInfoEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public double TotalStorage { get; set; }
        public double AvailableStorage { get; set; }
        public DateTime Added { get; set; }
        public bool Enabled { get; set; }
    }
}
