//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace Auth.Api.Services;

//public class JwtService
//{
//    private readonly string _key;
//    private readonly string _issuer;
//    private readonly string _audience;

//    public JwtService(IConfiguration configuration)
//    {
//        _key = configuration["Jwt:Key"];
//        _issuer = configuration["Jwt:Issuer"];
//        _audience = configuration["Jwt:Audience"];
//    }

//    public string GenerateJwtToken(string username)
//    {
//        var claims = new[]
//        {
//            new Claim(ClaimTypes.Name, username),
//        };

//        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
//        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//        var token = new JwtSecurityToken(
//            issuer: _issuer,
//            audience: _audience,
//            claims: claims,
//            expires: DateTime.Now.AddMinutes(30), 
//            signingCredentials: creds
//        );

//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }
//}
