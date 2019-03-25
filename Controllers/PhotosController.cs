using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using aaaNew.Data;
using aaaNew.Dtos;
using aaaNew.Helpers;
using aaaNew.Models;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace aaaNew.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);

        }
        [HttpGet("{id}", Name = "GetPhoto")]       // takes the id of the photo we need to give it a name
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }



        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, 
        [FromForm]PhotoForCreationDto photoForCreationDto) 
        {
/* we check if userid from token matches userId from route parameter {userid} and if they don't match we return an unauthorized request */
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                 // get cloudinary upload params 
                 var uploadParams = new ImageUploadParams()
                 {
                     File = new FileDescription(file.Name, stream),
                     Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                 };  

                 uploadResult = _cloudinary.Upload(uploadParams); // upload method from Cloudinary
                }
            }
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;
            
            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if(!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);

            

            if(await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id}, photoToReturn );
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMasin")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            // get user from repo
            var user = await _repo.GetUser(userId);
            // check if photo exist in photo user collection
            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();
            // get the photo from repo
            var photoFromRepo = await _repo.GetPhoto(id);
            // check if is the main photo
            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");
            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if(await _repo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to be main photo");
        }
    }
}