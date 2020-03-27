//using ApplicationCore.DTO;
//using ApplicationCore.Exceptions;
//using ApplicationCore.Identity;
//using ApplicationCore.Interfaces;
//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace ApplicationCore.Services
//{
//    public class ReportService : IReportService
//    {
//        private IUnitOfWork Database { get; set; }

//        private readonly UserManager<ApplicationUser> _userManager;

//        public ReportService(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
//        {
//            Database = uow;
//            _userManager = userManager;
//        }

//        #region for provider

//        public IEnumerable<ReportProviderDTO> GetReportProvider(int? providerId)
//        {
//            if (providerId == null)
//                throw new ValidationException("Не установлено id поставщика", "");

//            return GetGetReportProviderList(providerId.Value).ToList();
//        }

//        public IEnumerable<ReportProviderDTO> GetReportProvider(int? providerId, DateTime? date)
//        {
//            if (providerId == null)
//                throw new ValidationException("Не установлено id поставщика", "");

//            if (date == null)
//                throw new ValidationException("Не указана дата", "");

//            return GetGetReportProviderList(providerId.Value).Where(p => p.DateOrder.ToString("dd.MM.yyyy") == date.Value.ToString("dd.MM.yyyy"));
//        }

//        public IEnumerable<ReportProviderDTO> GetReportProvider(int? providerId, DateTime? dateWith, DateTime? dateTo)
//        {
//            if (providerId == null)
//                throw new ValidationException("Не установлено id поставщика", "");

//            if (dateWith == null || dateTo == null)
//                throw new ValidationException("Не указана дата", "");

//            return GetGetReportProviderList(providerId.Value).Where(p => p.DateOrder.Date >= dateWith.Value.Date &&
//                p.DateOrder.Date <= dateTo.Value.Date);
//        }


//        public IEnumerable<ReportProvidersDTO> GetReportProviders()
//        {
//            var providers = Database.Provider.GetAll().ToList();

//            var providersDTOs = new List<ReportProvidersDTO>();

//            foreach (var pr in providers)
//            {
//                var prov = GetReportProvider(pr.Id);

//                providersDTOs.Add(
//                    new ReportProvidersDTO()
//                    {
//                        Id = pr.Id,
//                        Name = pr.Name,
//                        CountOrder = prov.Count(),
//                        CountOrderDishes = prov.Sum(p => p.CountOrderDishes),
//                        FullPrice = prov.Sum(p => p.FullPrice)
//                    });
//            }

//            return providersDTOs;
//        }

//        public IEnumerable<ReportProvidersDTO> GetReportProviders(DateTime? date)
//        {
//            if (date == null)
//                throw new ValidationException("Не указана дата", "");

//            var providers = Database.Provider.GetAll().ToList();

//            var providersDTOs = new List<ReportProvidersDTO>();

//            foreach (var pr in providers)
//            {
//                var prov = GetReportProvider(pr.Id).Where(p => p.DateOrder.Date == date.Value.Date);

//                providersDTOs.Add(
//                    new ReportProvidersDTO()
//                    {
//                        Id = pr.Id,
//                        Name = pr.Name,
//                        CountOrder = prov.Count(),
//                        CountOrderDishes = prov.Sum(p => p.CountOrderDishes),
//                        FullPrice = prov.Sum(p => p.FullPrice)
//                    });
//            }

//            return providersDTOs;
//        }

//        public IEnumerable<ReportProvidersDTO> GetReportProviders(DateTime? dateWith, DateTime? dateTo)
//        {
//            if (dateWith == null || dateTo == null)
//                throw new ValidationException("Не указана дата", "");

//            var providers = Database.Provider.GetAll().ToList();

//            var providersDTOs = new List<ReportProvidersDTO>();

//            foreach (var pr in providers)
//            {
//                var prov = GetReportProvider(pr.Id).Where(p => p.DateOrder.Date >= dateWith.Value.Date &&
//                    p.DateOrder.Date <= dateTo.Value.Date);

//                providersDTOs.Add(
//                    new ReportProvidersDTO()
//                    {
//                        Id = pr.Id,
//                        Name = pr.Name,
//                        CountOrder = prov.Count(),
//                        CountOrderDishes = prov.Sum(p => p.CountOrderDishes),
//                        FullPrice = prov.Sum(p => p.FullPrice)
//                    });
//            }

//            return providersDTOs;
//        }


//        private IEnumerable<ReportProviderDTO> GetGetReportProviderList(int providerId)
//        {
//            var menus = Database.Menu.Find(p => p.ProviderId == providerId).ToList();

//            var dishes = Database.Dish.GetAll().ToList();
//            var orderDishes = Database.OrderDishes.GetAll().ToList();
//            var order = Database.Orders.GetAll().ToList();

//            var result = from m in menus
//                         join d in dishes on m.Id equals d.MenuId
//                         join oD in orderDishes on d.Id equals oD.DishId
//                         join o in order on oD.OrderId equals o.Id
//                         select new { Id = o.Id, Count = oD.Count, OrderId = oD.OrderId, DateOrder = o.DateOrder };


//            List<ReportProviderDTO> reportProviderDTOs = new List<ReportProviderDTO>();

//            foreach (var rs in result)
//            {
//                if (reportProviderDTOs.Find(p => p.Id == rs.Id) == null)
//                {
//                    reportProviderDTOs.Add(
//                        new ReportProviderDTO()
//                        {
//                            Id = rs.Id,
//                            CountOrderDishes = GetCountForProvider(rs.Id, providerId),
//                            DateOrder = rs.DateOrder,
//                            FullPrice = GetPriceForProvider(rs.Id, providerId)
//                        });
//                }
//            }

//            return reportProviderDTOs.OrderBy(p => p.DateOrder);
//        }

//        private decimal GetPriceForProvider(int orderId, int providerId)
//        {
//            var menus = Database.Menu.Find(p => p.ProviderId == providerId).ToList();
//            var dishes = Database.Dish.GetAll().ToList();
//            var orderDishes = Database.OrderDishes.Find(p => p.OrderId == orderId).ToList();

//            var result = from m in menus
//                         join d in dishes on m.Id equals d.MenuId
//                         join oD in orderDishes on d.Id equals oD.DishId
//                         select new { Count = oD.Count, d.Price };

//            decimal fullPrice = 0;

//            foreach (var pr in result)
//            {
//                fullPrice += pr.Count * pr.Price;
//            }

//            return fullPrice;
//        }

//        private int GetCountForProvider(int orderId, int providerId)
//        {
//            var menus = Database.Menu.Find(p => p.ProviderId == providerId).ToList();
//            var dishes = Database.Dish.GetAll().ToList();
//            var orderDishes = Database.OrderDishes.Find(p => p.OrderId == orderId).ToList();

//            var result = from m in menus
//                         join d in dishes on m.Id equals d.MenuId
//                         join oD in orderDishes on d.Id equals oD.DishId
//                         select new { Count = oD.Count };

//            int count = 0;

//            foreach (var pr in result)
//            {
//                count += pr.Count;
//            }

//            return count;
//        }

//        #endregion

//        #region for user

//        public IEnumerable<ReportUserDTO> GetReportUser(string userId)
//        {
//            if (userId == null)
//                throw new ValidationException("Не установлено id пользователя", "");

//            return GetGetReportUserList(userId).ToList();
//        }

//        public IEnumerable<ReportUserDTO> GetReportUser(string userId, DateTime? date)
//        {
//            if (userId == null)
//                throw new ValidationException("Не установлено id пользователя", "");

//            if (date == null)
//                throw new ValidationException("Не указана дата", "");

//            return GetGetReportUserList(userId).ToList().Where(p => p.DateOrder.ToString("dd.MM.yyyy") == date.Value.ToString("dd.MM.yyyy")); ;
//        }

//        public IEnumerable<ReportUserDTO> GetReportUser(string userId, DateTime? dateWith, DateTime? dateTo)
//        {
//            if (userId == null)
//                throw new ValidationException("Не установлено id пользователя", "");

//            if (dateWith == null || dateTo == null)
//                throw new ValidationException("Не указана дата", "");

//            return GetGetReportUserList(userId).ToList().Where(p => p.DateOrder.Date >= dateWith.Value.Date &&
//              p.DateOrder.Date <= dateTo.Value.Date);
//        }


//        public IEnumerable<ReportUsersDTO> GetReportUsers()
//        {
//            var users = _userManager.Users.ToList();

//            var usersDTOs = new List<ReportUsersDTO>();

//            foreach (var u in users)
//            {
//                var prov = GetReportUser(u.Id);

//                usersDTOs.Add(
//                    new ReportUsersDTO()
//                    {
//                        Id = u.Id,
//                        Email = u.Email,
//                        LFP = $"{u.Lastname} {u.Firstname} {u.Patronomic}",
//                        CountOrder = prov.Count(),
//                        CountOrderDishes = prov.Sum(p => p.CountOrderDishes),
//                        FullPrice = prov.Sum(p => p.FullPrice)
//                    }); ;
//            }

//            return usersDTOs;
//        }

//        public IEnumerable<ReportUsersDTO> GetReportUsers(DateTime? date)
//        {
//            if (date == null)
//                throw new ValidationException("Не указана дата", "");

//            var users = _userManager.Users.ToList();

//            var usersDTOs = new List<ReportUsersDTO>();

//            foreach (var u in users)
//            {
//                var prov = GetReportUser(u.Id, date);

//                usersDTOs.Add(
//                    new ReportUsersDTO()
//                    {
//                        Id = u.Id,
//                        Email = u.Email,
//                        LFP = $"{u.Lastname} {u.Firstname} {u.Patronomic}",
//                        CountOrder = prov.Count(),
//                        CountOrderDishes = prov.Sum(p => p.CountOrderDishes),
//                        FullPrice = prov.Sum(p => p.FullPrice)
//                    }); ;
//            }

//            return usersDTOs;
//        }

//        public IEnumerable<ReportUsersDTO> GetReportUsers(DateTime? dateWith, DateTime? dateTo)
//        {
//            if (dateWith == null || dateTo == null)
//                throw new ValidationException("Не указана дата", "");

//            var users = _userManager.Users.ToList();

//            var usersDTOs = new List<ReportUsersDTO>();

//            foreach (var u in users)
//            {
//                var prov = GetReportUser(u.Id, dateWith, dateTo);

//                usersDTOs.Add(
//                    new ReportUsersDTO()
//                    {
//                        Id = u.Id,
//                        Email = u.Email,
//                        LFP = $"{u.Lastname} {u.Firstname} {u.Patronomic}",
//                        CountOrder = prov.Count(),
//                        CountOrderDishes = prov.Sum(p => p.CountOrderDishes),
//                        FullPrice = prov.Sum(p => p.FullPrice)
//                    }); ;
//            }

//            return usersDTOs;
//        }


//        private IEnumerable<ReportUserDTO> GetGetReportUserList(string userId)
//        {
//            var orders = Database.Orders.Find(p => p.ApplicationUserId == userId).ToList();

//            var ordersDishes = Database.OrderDishes.GetAll().ToList();
//            var dishes = Database.Dish.GetAll().ToList();

//            var result = from o in orders
//                         join oD in ordersDishes on o.Id equals oD.OrderId
//                         join d in dishes on oD.DishId equals d.Id
//                         select new { Id = o.Id, Count = oD.Count, OrderId = oD.OrderId, DateOrder = o.DateOrder };

//            List<ReportUserDTO> reportUserDTOs = new List<ReportUserDTO>();

//            foreach (var rs in result)
//            {
//                if (reportUserDTOs.Find(p => p.Id == rs.Id) == null)
//                {
//                    reportUserDTOs.Add(
//                        new ReportUserDTO()
//                        {
//                            Id = rs.Id,
//                            CountOrderDishes = GetCountForUser(rs.Id, userId),
//                            DateOrder = rs.DateOrder,
//                            FullPrice = GetPriceForUser(rs.Id, userId)
//                        });
//                }
//            }

//            return reportUserDTOs.OrderBy(p => p.DateOrder);
//        }

//        private decimal GetPriceForUser(int orderId, string userId)
//        {
//            var order = Database.Orders.Find(p => p.ApplicationUserId == userId).ToList();
//            var orderDishes = Database.OrderDishes.Find(p => p.OrderId == orderId).ToList();
//            var dishes = Database.Dish.GetAll().ToList();

//            var result = from o in order
//                         join oD in orderDishes on o.Id equals oD.OrderId
//                         join d in dishes on oD.DishId equals d.Id
//                         select new { Count = oD.Count, d.Price };

//            decimal fullPrice = 0;

//            foreach (var pr in result)
//            {
//                fullPrice += pr.Count * pr.Price;
//            }

//            return fullPrice;
//        }

//        private int GetCountForUser(int orderId, string userId)
//        {
//            var order = Database.Orders.Find(p => p.ApplicationUserId == userId).ToList();
//            var orderDishes = Database.OrderDishes.Find(p => p.OrderId == orderId).ToList();
//            var dishes = Database.Dish.GetAll().ToList();

//            var result = from o in order
//                         join oD in orderDishes on o.Id equals oD.OrderId
//                         join d in dishes on oD.DishId equals d.Id
//                         select new { Count = oD.Count };

//            int count = 0;

//            foreach (var pr in result)
//            {
//                count += pr.Count;
//            }

//            return count;
//        }

//        #endregion
//    }
//}
