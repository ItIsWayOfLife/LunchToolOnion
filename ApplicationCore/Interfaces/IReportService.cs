using ApplicationCore.DTO;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Interfaces
{
    public interface IReportService
    {
        IEnumerable<ReportProviderDTO> GetReportProvider(int? providerId);
        IEnumerable<ReportProviderDTO> GetReportProvider(int? providerId, DateTime? date);
        IEnumerable<ReportProviderDTO> GetReportProvider(int? providerId, DateTime? dateWith, DateTime? dateTo);

        IEnumerable<ReportProvidersDTO> GetReportProviders();
        IEnumerable<ReportProvidersDTO> GetReportProviders(DateTime? date);
        IEnumerable<ReportProvidersDTO> GetReportProviders(DateTime? dateWith, DateTime? dateTo);

        IEnumerable<ReportUserDTO> GetReportUser(string userId);
        IEnumerable<ReportUserDTO> GetReportUser(string userId, DateTime? date);
        IEnumerable<ReportUserDTO> GetReportUser(string userId, DateTime? dateWith, DateTime? dateTo);

        IEnumerable<ReportUsersDTO> GetReportUsers();
        IEnumerable<ReportUsersDTO> GetReportUsers(DateTime? date);
        IEnumerable<ReportUsersDTO> GetReportUsers(DateTime? dateWith, DateTime? dateTo);
    }
}
