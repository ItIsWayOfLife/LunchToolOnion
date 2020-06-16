using ApplicationCore.DTO;
using System.Collections.Generic;

namespace ApplicationCore.Interfaces
{
    public interface ICatalogService
    {
        void AddСatalog(СatalogDTO сatalogDTO);
        void DeleteСatalog(int? id);
        void EditСatalog(СatalogDTO сatalogDTO);
        СatalogDTO GetСatalog(int? id);
         IEnumerable<СatalogDTO> GetСatalogs();
        IEnumerable<СatalogDTO> GetСatalogs(int? providerId);
        void Dispose();
    }
}
