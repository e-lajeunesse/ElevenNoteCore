using System.Collections.Generic;
using System.Threading.Tasks;
using ElevenNote.Models.User;

namespace ElevenNote.Services.User
{
    public interface IUserService
    {
         Task<bool> RegisterUserAsync(UserRegister model);
         Task<UserDetail> GetUserById(int id);
         Task<IEnumerable<UserDetail>> GetUsers();
    }
}