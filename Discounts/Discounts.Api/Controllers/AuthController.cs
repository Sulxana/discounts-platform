using Discounts.Application.Auth.Commands.Login;
using Discounts.Application.Auth.Commands.RefreshTokens;
using Discounts.Application.Auth.Commands.Register;
using Discounts.Application.Auth.Commands.Revoke;
using Discounts.Application.Auth.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RegisterHandler _registerHandler;
        private readonly LoginHandler _loginHandler;
        private readonly RefreshTokenHandler _refreshTokenHandler;
        private readonly RevokeHandler _revokeHandler;

        public AuthController(RegisterHandler registerHandler, LoginHandler loginHandler, RefreshTokenHandler refreshTokenHandler, RevokeHandler revokeHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
            _refreshTokenHandler = refreshTokenHandler;
            _revokeHandler = revokeHandler;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(CancellationToken token, [FromBody] RegisterCommand command)
        {
            var result = await _registerHandler.Register(command, token);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(CancellationToken token, [FromBody] LoginCommand command)
        {
            var result = await _loginHandler.Login(command, token);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> RefreshToken(CancellationToken token, [FromBody] RefreshTokenCommand command)
        {
            var result = await _refreshTokenHandler.CreateRefreshToken(command, token);
            return Ok(result);
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke(CancellationToken token, [FromBody] RevokeCommand command)
        {
            await _revokeHandler.RevokeToken(command, token);
            return NoContent();
        }
    }
}
