using System;

namespace WebAPI.Models.Provider
{
    public class ProviderModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime TimeWorkWith { get; set; }
        public DateTime TimeWorkTo { get; set; }
        public bool IsActive { get; set; }
        public string Path { get; set; }
        public string WorkingDays { get; set; }
        public string Info { get; set; }
        public string Img { get; set; }
    }
}
