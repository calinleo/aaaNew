using System;

namespace aaaNew.Dtos
{
    public class UserForListDto  // the properties we actual want to return to the user
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public string Gender { get; set; }
        public int  Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        
    }
}