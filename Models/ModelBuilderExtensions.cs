using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public static class ModelBuilderExtensions
    {
         public static void Seed(this ModelBuilder modelBuilder)
        {

            

            // Örnek: Identity kullanıcılarını ekleyelim
            var hasher = new PasswordHasher<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "omerapaydin",
                Email = "info@gmail.com",
                FullName = "Ömer Apaydın",
                EmailConfirmed = true // E-posta doğrulamasını sağlıyoruz
            };

            user.PasswordHash = hasher.HashPassword(user, "User123!");

            var user2 = new ApplicationUser
            {
                Id = "2",
                UserName = "ahmettambuga",
                Email = "info2@gmail.com",
                FullName = "Ahmet Tamboğa",
                EmailConfirmed = true // E-posta doğrulamasını sağlıyoruz
            };
            user2.PasswordHash = hasher.HashPassword(user2, "User2Password!");

            modelBuilder.Entity<ApplicationUser>().HasData(user, user2);

            // Seed işleminin diğer adımları (örneğin, ürün ve sipariş verileri) burada yer alabilir.
            modelBuilder.Entity<Post>().HasData(
                new Post { PostId = 1, Title = "First Post", Description = "This is the first post", PublishedOn = DateTime.Now.AddDays(-50), Image = "1.jpg", UserId = "1" },
                new Post { PostId = 2, Title = "Second Post", Description = "This is the second post", PublishedOn = DateTime.Now.AddDays(-20), Image = "2.jpg", UserId = "1" },
                new Post { PostId = 3, Title = "Another Post", Description = "Another user's post", PublishedOn = DateTime.Now.AddDays(-60), Image = "3.jpg", UserId = "2" }
            );
        }
    }
}