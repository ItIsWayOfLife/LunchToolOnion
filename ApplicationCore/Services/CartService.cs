using ApplicationCore.DTO;
using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Services
{
    public class CartService : ICartService
    {
        private IUnitOfWork Database { get; set; }

        public CartService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public Cart Create(string applicationUserId)
        {
            if (applicationUserId == null)
                throw new ValidationException("Не установлен id пользователя", "");

            Database.Cart.Create(new Cart() { ApplicationUserId = applicationUserId });
            Database.Save();

            return Database.Cart.Find(p => p.ApplicationUserId == applicationUserId).FirstOrDefault();
        }

        public CartDTO GetCart(string applicationUserId)
        {
            if (applicationUserId == null)
                throw new ValidationException("Не установлен id пользователя", "");

            var cart = Database.Cart.Find(p => p.ApplicationUserId == applicationUserId).FirstOrDefault();

            if (cart == null)
            {
                cart = Create(applicationUserId);
            }

            CartDTO cartDTO = new CartDTO()
            {
                ApplicationUserId = cart.ApplicationUserId,
                Id = cart.Id
            };

            return cartDTO;
        }

        public IEnumerable<CartDishesDTO> GetCartDishes(string applicationUserId)
        {
            if (applicationUserId == null)
                throw new ValidationException("Не установлен id пользователя", "");

            var cart = GetCart(applicationUserId);

            if (cart == null)
                throw new ValidationException("Корзина не найдена", "");

            var cartDishes = Database.CartDishes.Find(p => p.CartId == cart.Id);

            var cartDishesDTO = new List<CartDishesDTO>();

            foreach (var cartD in cartDishes)
            {
                cartDishesDTO.Add(new CartDishesDTO()
                {
                    CartId = cart.Id,
                    Id = cartD.Id,
                    Count = cartD.Count,
                    DishId = cartD.DishId,
                    Info = cartD.Dish.Info,
                    Name = cartD.Dish.Name,
                    Path = cartD.Dish.Path,
                    Price = cartD.Dish.Price,
                    Weight = cartD.Dish.Weight
                });
            }

            return cartDishesDTO;
        }

        public void DeleteCartDish(int? id, string applicationUserId)
        {
            if (applicationUserId == null)
                throw new ValidationException("Не установлен id пользователя", "");

            var cart = GetCart(applicationUserId);

            if (cart == null)
                throw new ValidationException("Корзина не найдена", "");

            if (id == null)
                throw new ValidationException("Не установлено id удаляемого блюда в корзине", "");

            var cartDish = Database.CartDishes.Get(id.Value);

            if (cartDish.CartId != cart.Id)
                throw new ValidationException("Блюдо в корзене не найдено", "");

            Database.CartDishes.Delete(id.Value);
            Database.Save();
        }

        public void AddDishToCart(int? dishId, string applicationUserId)
        {
            if (applicationUserId == null)
                throw new ValidationException("Не установлен id пользователя", "");

            var cart = GetCart(applicationUserId);

            if (cart == null)
                throw new ValidationException("Корзина не найдена", "");

            if (dishId == null)
                throw new ValidationException("Не установлен id добавляемого блюда в корзине", "");

            // если уже существует в корзине, то увеличивается на 1 (если нет, то созд нов с количеством 1)
            if (GetCartDishes(applicationUserId).Where(p => p.DishId == dishId.Value).Count() > 0)
            {
                var cartDish = Database.CartDishes.Find(p => p.CartId == cart.Id).Where(p => p.DishId == dishId.Value).FirstOrDefault();
                cartDish.Count++;
                Database.CartDishes.Update(cartDish);
                Database.Save();
            }
            else
            {
                Dish dish = Database.Dish.Get(dishId.Value);

                if (dish == null)
                    throw new ValidationException("Блюдо не найдено", "");

                Database.CartDishes.Create(new CartDishes()
                {
                    CartId = cart.Id,
                    Count = 1,
                    DishId = dishId.Value
                });
                Database.Save();
            }
        }

        public void AllDeleteDishesToCart(string applicationUserId)
        {
            if (applicationUserId == null)
                throw new ValidationException("Не установлен id пользователя", "");

            var cart = GetCart(applicationUserId);

            if (cart == null)
                throw new ValidationException("Корзина не найдена", "");

            var cartDish = GetCartDishes(applicationUserId);

            if (cartDish.Count() < 1)
                throw new ValidationException("Корзина пуста", "");

            foreach (var cartD in cartDish)
            {
                Database.CartDishes.Delete(cartD.Id);
            }

            Database.Save();
        }

        public void UpdateCountDishInCart(string applicationUserId, int? dishCartId, int count)
        {
            if (applicationUserId == null)
                throw new ValidationException("Не установлен id пользователя", "");

            var cart = GetCart(applicationUserId);

            if (cart == null)
                throw new ValidationException("Корзина не найдена", "");

            var cartDishes = GetCartDishes(applicationUserId);

            if (cartDishes.Count() < 1)
                throw new ValidationException("Корзина пуста", "");

            if (cartDishes.Where(p => p.Id == dishCartId).Count() < 1)
                throw new ValidationException("В корзине нету указанного блюда", "");

            CartDishes cartDishe = Database.CartDishes.Find(p => p.Id == dishCartId).FirstOrDefault();

            if (count <= 0)
                throw new ValidationException("Количество должно быть положительныим целым числом", "");

            cartDishe.Count = count;

            Database.CartDishes.Update(cartDishe);
            Database.Save();
        }

        public decimal FullPriceCart(string applicationUserId)
        {
            decimal fullPrice = 0;

            var cartDishes = GetCartDishes(applicationUserId);

            foreach (var cartDish in cartDishes)
            {
                fullPrice += cartDish.Count * cartDish.Price;
            }

            return fullPrice;
        }
    }
}
