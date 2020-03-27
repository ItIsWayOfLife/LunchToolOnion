using ApplicationCore.DTO;
using System.Collections.Generic;

namespace ApplicationCore.Interfaces
{
    public interface IСatalogService
    {
        void AddСatalog(СatalogDTO сatalogDTO);
        void DeleteСatalog(int? id);
        void EditСatalog(СatalogDTO сatalogDTO);
        СatalogDTO GetСatalog(int? id);
        IEnumerable<СatalogDTO> GetСatalogs(int? providerId);
        void Dispose();
    }
}
