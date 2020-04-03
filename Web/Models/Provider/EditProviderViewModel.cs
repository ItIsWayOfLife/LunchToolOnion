﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Provider
{
    public class EditProviderViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "NameNotSpecified")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "EmailNotSpecified")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "OpeningTimeNotSpecified")]
        [Display(Name = "TimeWorkWith")]
        public DateTime TimeWorkWith { get; set; }
        [Required(ErrorMessage = "ClosingTimeNotSpecified")]
        [Display(Name = "TimeWorkTo")]
        public DateTime TimeWorkTo { get; set; }
        public bool IsActive { get; set; }
        public string Path { get; set; }
        public string WorkingDays { get; set; }
        public bool IsFavorite { get; set; }
        public string Info { get; set; }
    }
}
