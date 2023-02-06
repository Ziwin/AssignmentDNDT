using Microsoft.AspNetCore.Mvc;

namespace DNDT.Models
{
    public class ImageModel
    {

        public string? Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public IFormFile Image { get; set; }

    }
}
