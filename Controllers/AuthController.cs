using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Text;
using api.Data;
using api.Dtos;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System;
using api.Infrastructure;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly byte[] _secret;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, JwtTokenConfig jwtTokenConfig, IMapper mapper)
        {
            _mapper = mapper;
            _jwtTokenConfig = jwtTokenConfig;
            _repo = repo;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
        {
            if (!string.IsNullOrEmpty(userForRegisterDto.Username))
                userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExist(userForRegisterDto.Username))
                // return BadRequest("Username is already taken");
                ModelState.AddModelError("Username", "Username is already taken");

            // Validate credentials
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var createUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            var returnUser = _mapper.Map<UserForDetailDto>(createUser);

            return CreatedAtRoute("getuser", new { controller = "Users", id = createUser.Id }, returnUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
        {
            // throw new Exception("Error");

            var userForLogin = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userForLogin == null)
                return Unauthorized();

            // Generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.ASCII.GetBytes("datingapp secret key");
            // var tokenDescriptor = new SecurityTokenDescriptor
            // {
            //     Subject = new ClaimsIdentity(new Claim[] {
            //         new Claim(ClaimTypes.NameIdentifier, userForLogin.Id.ToString()),
            //         new Claim(ClaimTypes.Name, userForLogin.Username) }),
            //     Expires = DateTime.UtcNow.AddDays(7),
            //     SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
            //         SecurityAlgorithms.HmacSha512Signature)
            // };

            var claims = new Claim[] {
                    new Claim(JwtRegisteredClaimNames.NameId, userForLogin.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userForLogin.Username) };

            var jwtToken = new JwtSecurityToken(
                    _jwtTokenConfig.Issuer,
                    _jwtTokenConfig.Audience,
                    claims,
                    expires: DateTime.UtcNow.AddDays(_jwtTokenConfig.AccessTokenExpiration),
                    signingCredentials: new SigningCredentials(
                                        new SymmetricSecurityKey(_secret),
                                                SecurityAlgorithms.HmacSha512)
            );

            // var token = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(jwtToken);
            var user = _mapper.Map<UserForListDto>(userForLogin);

            return Ok(new { token, user });


        }


    }
}