using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudineryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudineryConfig)
        {
            this._cloudineryConfig = cloudineryConfig;
            this._mapper = mapper;
            this._repo = repo;
            Account acc = new Account(_cloudineryConfig.Value.CloudName, _cloudineryConfig.Value.ApiKey, _cloudineryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromRepo = await this._repo.GetUser(userId);
            var file = photoForCreationDto.File;
            var uploadResults = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResults = _cloudinary.Upload(uploadParams);
                }
            };
            photoForCreationDto.Url = uploadResults.Uri.ToString();
            photoForCreationDto.PublicId = uploadResults.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);
            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }
            userFromRepo.Photos.Add(photo);
            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }
            return BadRequest("Could not add the photo");
        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromRepo = await this._repo.GetUser(userId);
            if (!userFromRepo.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }
            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("This was already the main photo");
            }
            var currentMainPhoto = await _repo.GetMainPhoto(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
            {
                return NoContent();
            }
            return BadRequest("Could not set photo to main");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromRepo = await this._repo.GetUser(userId);
            if (!userFromRepo.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }
            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("Could not remove main photo");
            }
            if (!string.IsNullOrEmpty(photoFromRepo.PublicId))
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = this._cloudinary.Destroy(deleteParams);

                if (result.Result.ToLower() == "ok")
                {
                    this._repo.Delete(photoFromRepo);
                }
            }
            else
            {
                this._repo.Delete(photoFromRepo);
            }

            if (await this._repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Could not remove photo");
        }
    }
}

