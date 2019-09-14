using DatingApp.API.Helpers;
using DatingApp.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        // This is a generic method
        // In our case, T might be a type of User class or type of Photo class
        // And we contraint the method to accept only type of 'class' 
        // by using the where clause
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        Task<bool> SaveAll();

        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int userId);
        Task<Photo> GetPhoto(int Id);
        Task<Photo> GetMainPhoto(int userId);
    }
}