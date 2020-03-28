﻿using System;

namespace ApplicationCore.DTO
{
    public class MenuDTO : BaseEntityDTO
    {
        public string Info { get; set; }
        public DateTime Date { get; set; }
        public int ProviderId { get; set; }
    }
}
