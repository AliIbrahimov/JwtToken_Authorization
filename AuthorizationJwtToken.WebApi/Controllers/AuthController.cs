using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorizationJwtToken.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    string signInKey = "ThisIsMySigingKey";

    [HttpGet]
    public string Get(string userName, string password)
    {

        //payload - yukdaşıyıcı
        var claims = new[] {
            new Claim(ClaimTypes.Name, userName),
            new Claim(JwtRegisteredClaimNames.Email, userName)
        };

        //secure key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(signInKey));
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            // issures and audience must be written inside configuration file
            issuer: "example.com",
            audience: "ThisIsMyAudienceKey",
            //validate ederken
            claims: claims,
            expires: DateTime.Now.AddDays(5),
            notBefore: DateTime.Now,
            signingCredentials: credential

            );
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }
    [HttpGet("ValidateToken")]

    public bool ValidateToken(string token)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(signInKey));
        try
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false

            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var claims = jwtToken.Claims.ToList();
            return true;
        }
        catch (Exception)
        {

            return false;
        }
    }

}
