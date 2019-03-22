using System;
using Microsoft.AspNetCore.Http;

namespace aaaNew.Dtos
{
    public class PhotoForCreationDto
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }  // we get this from cloudinary
        public PhotoForCreationDto()
        {
            DateAdded = DateTime.Now;
        }
    }
}