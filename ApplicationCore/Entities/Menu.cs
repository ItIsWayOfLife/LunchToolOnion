using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
   public class Menu: BaseEntity
    {
        public string Info { get; set; }
        public DateTime Date { get; set; }
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        public List<MenuDishes> MenuDishes { get; set; }

        public Menu()
        {
            MenuDishes = new List<MenuDishes>();
        }
    }
}
