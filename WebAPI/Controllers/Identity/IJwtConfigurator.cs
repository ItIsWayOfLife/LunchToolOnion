
namespace WebAPI.Identity.Controllers
{
    public interface IJwtConfigurator
    {
        public string GetToken(string userName);
    }
}
