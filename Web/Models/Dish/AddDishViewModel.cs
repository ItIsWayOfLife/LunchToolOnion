using System.ComponentModel.DataAnnotations;

namespace Web.Models.Dish
{
    public class AddDishViewModel
    {
        public int MenuId { get; set; }

        [Required(ErrorMessage = "Не указано название")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указана информация")]
        [Display(Name = "Информация")]
        public string Info { get; set; }

        [Required(ErrorMessage = "Не указан вес")]
        [Display(Name = "Вес")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Не указана цена")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }
        public string Path { get; set; }
    }
}
