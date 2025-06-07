using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinScope.Interfaces;

namespace FinScope.Services
{
    public class AuthService : IAuthService
    {
        //        private readonly WorklyDBContext _dbContext;
        //        private User? _currentUser;

        //        public AuthService(WorklyDBContext dbContext)
        //        {
        //            _dbContext = dbContext;
        //        }

        //        public User? CurrentUser => _currentUser;
        //        public bool IsAuthenticated => _currentUser != null;

        //        public  bool LoginAsync(string email, string password)
        //        {
        //            var user = User.GetUserByEmail(email);

        //            if (user == null || !VerifyPassword(password, user.PasswordHash))
        //            {
        //                return false;
        //            }

        //            _currentUser = user;
        //            return true;
        //        }

        //        public void Logout()
        //        {
        //            _currentUser = null;
        //        }

        //        private bool VerifyPassword(string password, string storedHash)
        //        {
        //            return password== storedHash;
        //        }
    }
}
