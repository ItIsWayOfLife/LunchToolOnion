using ApplicationCore.DTO;
using System.Collections.Generic;

namespace ApplicationCore.Interfaces
{
    public interface ICatalogService
    {
        void AddСatalog(CatalogDTO сatalogDTO);
        void DeleteСatalog(int? id);
        void EditСatalog(CatalogDTO сatalogDTO);
        CatalogDTO GetСatalog(int? id);
         IEnumerable<CatalogDTO> GetСatalogs();
        IEnumerable<CatalogDTO> GetСatalogs(int? providerId);
        void Dispose();
    }
}
