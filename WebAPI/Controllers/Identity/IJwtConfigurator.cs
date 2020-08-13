
namespace WebAPI.Controllers.Identity
{
    public interface IJwtConfigurator
    {
        public string GetToken(string userName);
    }
}
