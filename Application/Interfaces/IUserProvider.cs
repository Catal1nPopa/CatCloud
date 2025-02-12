namespace Application.Interfaces
{
    public interface IUserProvider
    {
        Guid GetUserId();
        string GetName();
        string GetEmail();
    }
}
