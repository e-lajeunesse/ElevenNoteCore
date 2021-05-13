using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevenNote.Models.User;
using ElevenNote.Models.Token;
using ElevenNote.Services.Token;
using ElevenNote.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElevenNote.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public UserController(IUserService service, ITokenService tokerService)
        {
            _tokenService = tokerService;
            _userService = service;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegister model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool registerResult = await _userService.RegisterUserAsync(model);
            if (registerResult)
            {
                return Ok("User was registered");
            }
            return BadRequest("User could not be registered");
        }

        [Authorize]
        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetUserById([FromRoute] int userId)
        {
            var userDetail = await _userService.GetUserById(userId);
            if (userDetail is null)
            {
                return NotFound();
            }
            return Ok(userDetail);
        }

        [HttpPost("~/api/Token")]
        public async Task<IActionResult> Token([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenResponse = await _tokenService.GetTokenAsync(request);
            if (tokenResponse is null)
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(tokenResponse);
        }
    }
}