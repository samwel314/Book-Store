using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samuel_Web.DataAccess.Data;
using Samuel_Web.Models;
using Samuel_Web.Utility;
using static Samuel_Web.DataAccess.DBInitializer.DBInitializer;

namespace Samuel_Web.DataAccess.DBInitializer
{

    public class DBInitializer : IDBInitializer
    {
        UserManager<IdentityUser> _usermanager;
        RoleManager<IdentityRole> _rolemanager;
        AppDbContext _db;
        public DBInitializer(UserManager<IdentityUser> _usermanager
        , RoleManager<IdentityRole> _rolemanager,
        AppDbContext _db)
        {
            this._usermanager = _usermanager;
            this._rolemanager = _rolemanager;
            this._db = _db;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

            }
            catch (Exception e)
            {

            }

            if (!_rolemanager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _rolemanager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();


                _usermanager.CreateAsync(new ApplicationUser
                {
                    UserName = "zoka123@gmail.com",
                    Email = "zoka123@gmail.com",
                    Name = "marzouk rezq",
                    PhoneNumber = "1234567890",
                    StreetAddress = "almo3lmen 23",
                    City = "Assuit",
                    State = "active",
                    PostalCode = "1234567890",
                }, "*123Zoka").GetAwaiter().GetResult();

                ApplicationUser appUser =
                    _db.ApplicationUsers.FirstOrDefault(u => u.Email ==
                    "zoka123@gmail.com")!;
                _usermanager.AddToRoleAsync(appUser, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;
        }
    }

}
