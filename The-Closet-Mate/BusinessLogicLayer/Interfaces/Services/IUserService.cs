namespace BusinessLogicLayer.Interfaces.Services
{
    public interface IUserService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(string email, string password);

        Task<bool> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
    }
}
