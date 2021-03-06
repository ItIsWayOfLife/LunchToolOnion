﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class Catalog : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public string Info { get; set; }
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }

        public ICollection<Dish> Dishes { get; set; }
    }
}
