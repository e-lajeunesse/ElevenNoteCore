using System.Threading.Tasks;
using ElevenNote.Data.Entities;
using ElevenNote.Models.Token;

namespace ElevenNote.Services.Token
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync(TokenRequest model);
        //Task<UserEntity> GetValidUserAsync(TokenRequest model);
        //TokenResponse GenerateToken(UserEntity user);
    }
}