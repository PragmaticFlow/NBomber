using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookstoreSimulator.Infra
{
    public static class JwtToken
    {
        public static string GenerateJwtToken(string userId, JwtSetings jwtSetings)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSetings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId) }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtSetings.Issuer,
                Audience = jwtSetings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string DecodeJwtToken(string jwtToken)
        {
            var tokenHendler = new JwtSecurityTokenHandler();
            var userIdString = "";
            try
            {
                var jwtSecurityToken = tokenHendler.ReadJwtToken(jwtToken);
                var claims = jwtSecurityToken.Claims;
                foreach (var claim in claims)
                {
                    if (claim.Type == "id")
                        userIdString = claim.Value;
                }
                return userIdString;
            }
            catch
            {
                return userIdString;
            }
        }
    }
}
