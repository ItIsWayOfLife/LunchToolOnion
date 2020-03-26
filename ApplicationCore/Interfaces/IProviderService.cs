using ApplicationCore.DTO;
using System.Collections.Generic;

namespace ApplicationCore.Interfaces
{
    public interface IProviderService
    {
        void AddProvider(ProviderDTO providerDTO);
        void DeleteProvider(int? id);
        void EditProvider(ProviderDTO providerDTO);
        ProviderDTO GetProvider(int? id);
        IEnumerable<ProviderDTO> GetProviders();
        IEnumerable<ProviderDTO> GetFavoriteProviders();
        void Dispose();
    }
}
