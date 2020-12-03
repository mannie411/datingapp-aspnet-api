using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Data;
using AutoMapper;
using Microsoft.Extensions.Options;
using api.Infrastructure;
using CloudinaryDotNet;
using System.Threading.Tasks;
using api.Dtos;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using api.Models;
using System.Linq;
using System.IO;

namespace api.Controllers
{
    [Authorize]
    [Route("api/users/")]
    public class PhotosController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo,
            IMapper mapper,
            IOptions<CloudinarySettings> cloudinaryConfig)
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

        [HttpGet("{id}/photo", Name = "photo")]
        public async Task<IActionResult> GetPhoto(int id)
        {

            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost("{id}/photos")]
        public async Task<IActionResult> uploadPhoto(int id, PhotoForUploadDto photoDto)
        {
            var user = await _repo.GetUser(id);

            if (user == null)
                return BadRequest("User not found");

            var currUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (currUser != user.Id)
                return Unauthorized();

            var file = photoDto.File;

            var uploadParams = new ImageUploadParams();

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {


                using (var stream = file.OpenReadStream())
                {

                    uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        UploadPreset = "tfzxyrgw"
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);

                }

            }

            photoDto.Url = uploadResult.Url.ToString();
            photoDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoDto);
            photo.User = user;

            if (!user.Photos.Any(m => m.isMain))
                photo.isMain = true;

            user.Photos.Add(photo);


            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);

                return CreatedAtRoute("photo", new { id = photo.Id }, photoToReturn);
            }


            return BadRequest("Unable to upload photo");

        }

        [HttpPost("{userId}/photo/setmain/{photoId}/")]
        public async Task<IActionResult> SetMainPhoto(int userId, int photoId)
        {
            var user = await _repo.GetUser(userId);

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(photoId);

            if (photoFromRepo == null)
                return NotFound();

            if (photoFromRepo.isMain)
                return BadRequest("This is already the main photo");

            var currPhoto = await _repo.GetMainPhoto(userId);

            if (currPhoto != null)
                currPhoto.isMain = false;

            photoFromRepo.isMain = true;

            if (await _repo.SaveAll())
                return NoContent();


            return BadRequest("Unable to set main photo");
        }

        [HttpDelete("{userId}/photo/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int userId, int photoId)
        {
            var user = await _repo.GetUser(userId);

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(photoId);

            if (photoFromRepo == null)
                return NotFound(new { message = "Photo not found" });

            if (photoFromRepo.isMain)
                return BadRequest(new { message = "You cannot delete main photo" });

            if (photoFromRepo.PublicId != null)
            {
                var delete = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(delete);

                // if (result.Result == "ok")
                //     _repo.Delete(photoFromRepo);

                switch (result.Result)
                {
                    case "ok":
                        _repo.Delete(photoFromRepo);
                        break;
                    case "not found":
                        _repo.Delete(photoFromRepo);
                        await _repo.SaveAll();
                        return NotFound(new { message = "Photo Not Found" });
                    default:
                        break;
                }

            }
            else
            {
                _repo.Delete(photoFromRepo);

            }

            if (await _repo.SaveAll())
                return Ok(new { message = "Photo deleted" });

            return BadRequest("Unable to delete photo");

        }

    }
}