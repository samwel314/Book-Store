using Microsoft.EntityFrameworkCore;
using Samuel_Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Samuel_Web.DataAccess.Data
{
    // why we use IdentityDbContext instead of DbContext ?
    // because we need to use Identity features like Users , Roles ... etc
    public class AppDbContext : IdentityDbContext
    {

        // How can pass connecion string to base class constructor ? by di 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        // this property represent Categories table in database 
        // if you need to change table name you can use fluent api in OnModelCreating method
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }    
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; } 
        public DbSet<Company> Companies { get; set; }  
        
        public DbSet<OrderHeader> OrderHeaders { get; set; }    

        public DbSet<OrderDetail> OrderDetails { get; set; }        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // whey this line is important ?
            // because it will configure the Identity tables and relationships it is mandatory to call it
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData (
                new Category { Id = 1, Name = "Action" , DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi"  , DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
            );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Fortune of Time",
                    Author = "Billy Spark",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                    ISBN = "SWD9999001",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    categoryId = 1,
                    ImageUrl = ""

                },
                new Product
                {
                    Id = 2,
                    Title = "Dark Skies",
                    Author = "Nancy Hoover",
                    Description = "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                    ISBN = "SWD9999002",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    categoryId = 1,
                    ImageUrl = ""

                }
                ,
                new Product
                {
                    Id = 3,
                    Title = "Vanish in the Sunset",
                    Author = "Julian Button",
                    Description = "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                    ISBN = "SWD9999003",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    categoryId = 2,
                    ImageUrl = ""

                }
                ,
                new Product
                {
                    Id = 4,
                    Title = "Cotton Candy",
                    Author = "Abby Muscles",
                    Description = "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                    ISBN = "SWD9999004",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    categoryId = 2,
                    ImageUrl = ""

                }
                , new Product
                {
                    Id = 5,
                    Title = "Rock in the Ocean",
                    Author = "Ron Parker",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20 , 
                    categoryId = 3,
                    ImageUrl = ""

                },
                new Product
                {
                    Id = 6,
                    Title = "Leaves and Wonders",
                    Author = "Laura Phantom",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    categoryId = 3,
                    ImageUrl = ""

                }

                );

            modelBuilder.Entity<Company>().HasData(
  new Company
  {
      Id = 1,
      Name = "Tech Solutions Ltd",
      StreetAddress = "12 El Tahrir St",
      City = "Cairo",
      State = "Cairo",
      PostalCode = "11511",
      PhoneNumber = "+20 100 123 4567"
  },
  new Company
  {
      Id = 2,
      Name = "Nile Soft",
      StreetAddress = "45 Corniche El Nile",
      City = "Giza",
      State = "Giza",
      PostalCode = "12611",
      PhoneNumber = "+20 109 555 8821"
  },
  new Company
  {
      Id = 3,
      Name = "Delta Systems",
      StreetAddress = "10 Saad Zaghloul St",
      City = "Mansoura",
      State = "Dakahlia",
      PostalCode = "35511",
      PhoneNumber = "+20 122 334 9988"
  },
  new Company
  {
      Id = 4,
      Name = "Alexandria Trading",
      StreetAddress = "22 El Horreya Rd",
      City = "Alexandria",
      State = "Alexandria",
      PostalCode = "21500",
      PhoneNumber = "+20 111 777 6633"
  }
);



        }

    }

}
