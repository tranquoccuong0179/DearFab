using System.Security.Claims;
using DearFab_Model.Entity;
using Microsoft.IdentityModel.Tokens;

namespace DearFab_Model.Utils;
using System.IdentityModel.Tokens.Jwt;

public class JwtUtil
{
    private JwtUtil()
    {
    }

    public static string GenerateJwtToken(Account account, Tuple<string, Guid> guidClaim)
    {
        JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

        byte[] keyBytes = Convert.FromHexString("0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F00");
        SymmetricSecurityKey secretKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

        List<Claim> claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.NameId, account.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, account.Phone.ToString()),
            new Claim("role", account.Role),
        };

        if (guidClaim != null)
        {
            claims.Add(new Claim(guidClaim.Item1, guidClaim.Item2.ToString()));
        }

        var expires = DateTime.Now.AddDays(30);


        var token = new JwtSecurityToken(
            issuer: "DEARFAB",
            audience: null,
            claims: claims,
            notBefore: DateTime.Now,
            expires: expires,
            signingCredentials: credentials
        );
        return jwtHandler.WriteToken(token);
    }
}