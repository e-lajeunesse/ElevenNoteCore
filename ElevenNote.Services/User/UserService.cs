using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElevenNote.Services.User
{
    public class UserService : IUserService
    {

        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> RegisterUserAsync(UserRegister model)
        {
            if (await GetUserByEmail(model.Email) != null || await GetUserByUserName(model.UserName) != null)
            {
                return false;
            }

            UserEntity user = new UserEntity
            {
                Email = model.Email,
                UserName = model.UserName,
                DateCreated = DateTime.Now
            };

            PasswordHasher<UserEntity> passwordHasher = new PasswordHasher<UserEntity>();
            user.Password = passwordHasher.HashPassword(user, model.Password);


            _context.Users.Add(user);
            int changes = await _context.SaveChangesAsync();
            return changes == 1;
        }

        public async Task<UserDetail> GetUserById(int id)
        {
            UserEntity user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }
            return new UserDetail
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<IEnumerable<UserDetail>> GetUsers()
        {
            var users = _context.Users.Select(u => new UserDetail
            {
                Email = u.Email,
                UserName = u.UserName,
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName
            });
            return await users.ToListAsync();
        }

        private async Task<UserEntity> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        private async Task<UserEntity> GetUserByUserName(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
    }
}