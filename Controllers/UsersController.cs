using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Data;
using api.Dtos;
using api.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;

        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currUserId);

            userParams.UserId = currUserId;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "Male" ? "Female" : "Male";

            }
            var users = await _repo.GetUsers(userParams);

            var mapUsers = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(mapUsers);
        }

        [HttpGet("{id}", Name = "getuser")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var mapUser = _mapper.Map<UserForDetailDto>(user);
            return Ok(mapUser);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto userForUpdateDto)
        {

            if (!ModelState.IsValid)
                return BadRequest();

            var currUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(id);

            if (userFromRepo == null)
                return NotFound($"User with the ID {id} not found");

            if (currUserId != userFromRepo.Id)
                return Unauthorized();

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Failed to update user {id}");
        }
    }
}