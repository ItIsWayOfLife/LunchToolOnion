
namespace WebAPI.Controllers
{
    public interface IJwtConfigurator
    {
        public string GetToken(string userName);
    }
}
