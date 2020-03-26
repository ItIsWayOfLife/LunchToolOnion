﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Cart
{
    public class CartDishesViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Количество")]
        [Range(1, 1000, ErrorMessage = "Количество должно быть положительным, но не более 1000")]
        public int Count { get; set; }
        public int CartId { get; set; }
        public int DishId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public double Weight { get; set; }
        public decimal Price { get; set; }
        public string Path { get; set; }
    }
}
