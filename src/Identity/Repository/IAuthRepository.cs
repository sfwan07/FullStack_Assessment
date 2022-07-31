using Identity.Model;

namespace Identity.Repository
{
    public interface IAuthRepository
    {
        Task<LoginResponse> Login(LoginModel model);
        IAsyncEnumerable<UserViewModel> GetUsers();
    }
}