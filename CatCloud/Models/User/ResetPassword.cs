namespace CatCloud.Models.User;

public class ResetPassword
{
    public required string token { get; set; } 
    public required string newPassword { get; set; } 
}