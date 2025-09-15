using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OfficeService.Business.IServices;
using OfficeService.DAL.DTOs.Requests;
using OfficeService.DAL.DTOs.Responses;
using OfficeService.DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OfficeService.Business.Services
{
    public class JWTContext : IJWTContext
    {
        private readonly AppSetting _setting;

        public JWTContext(IOptions<AppSetting> options)
        {
            _setting = options.Value;
        }

        public ResponseToken GenerateToken(RequestToken request, Dictionary<string, string> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_setting.DocumentServerSecret);
            List<Claim> tokenClaims = new List<Claim>();
            foreach (var item in claims)
            {
                if(IsValidJson(item.Value))
                    tokenClaims.Add(new Claim(item.Key, item.Value, JsonClaimValueTypes.Json));
                else
                    tokenClaims.Add(new Claim(item.Key, item.Value));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(tokenClaims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            if (request.ExpiresIn.HasValue)
                tokenDescriptor.Expires = DateTime.UtcNow.AddSeconds(request.ExpiresIn.Value);

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            return new ResponseToken
            {
                AccessToken = accessToken,
                ExpiresIn = request.ExpiresIn.HasValue ? (int)DateTimeOffset.UtcNow.AddSeconds(request.ExpiresIn.Value).ToUnixTimeSeconds() : null
            };
        }

        private bool IsValidJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            // JSON object hoặc array phải bắt đầu bằng { hoặc [
            if (!(input.StartsWith("{") && input.EndsWith("}")) &&
                !(input.StartsWith("[") && input.EndsWith("]")))
            {
                return false;
            }

            try
            {
                JsonDocument.Parse(input);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
