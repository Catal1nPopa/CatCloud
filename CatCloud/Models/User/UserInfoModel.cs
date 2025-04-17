namespace CatCloud.Models.User
{
    public class UserInfoModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public double TotalStorage { get; set; }
        public double AvailableStorage { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool Enabled { get; set; }
        public DateTime Added { get; set; }
        public List<string> Role { get; set; }
    }
}
