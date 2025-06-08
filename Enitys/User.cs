using FinScope.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinScope.Enitys;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<PortfolioAsset> PortfolioAssets { get; set; } = new List<PortfolioAsset>();

    public virtual ICollection<SectorAllocation> SectorAllocations { get; set; } = new List<SectorAllocation>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public static bool CreateOrUpdate(User profile)
    {
        try
        {
            using (var context = new FinScopeDbContext())
            {
                var updateProfile = context.Users.FirstOrDefault(p => p.Id == profile.Id);

                if (updateProfile == null)
                    context.Users.Add(profile);
                else
                {
                    updateProfile.Username = profile.Username;
                    updateProfile.Email = profile.Email;
                    updateProfile.PasswordHash = profile.PasswordHash;

                }
                context.SaveChanges();
            }
            return true;

        }
        catch (Exception ex)
        {
            return false;
        }
    }


    public static User GetUserByEmail(string email)
    {
        return GetUsers().Where(u => u.Email == email).FirstOrDefault();
    }
    public static User GetById(int userId)
    {
        return GetUsers()
            .Where(u => u.Id == userId)
            .FirstOrDefault();
    }
    public static List<User> GetUsers()
    {
        using (var context = new FinScopeDbContext())  // Используйте ваш контекст данных
        {
            return context.Users
             .ToList();
        }
    }
}
