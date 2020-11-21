using System.Collections.Generic;
using System.Threading.Tasks;
using api.Data;
using api.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace api.Controllers
{
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var mapUsers = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(mapUsers);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var mapUser = _mapper.Map<UserForDetailDto>(user);
            return Ok(mapUser);
        }
    }
}