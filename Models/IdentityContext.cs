using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class IdentityContext:IdentityDbContext<IdentityUser>
    {

        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
            
        }

         public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed verilerini ekle
            builder.Seed();

            // User ve Post arasındaki bire-çok ilişkiyi tanımlama
            builder.Entity<Post>()
                .HasOne(p => p.User)         // Her post bir kullanıcıya aittir
                .WithMany(u => u.Posts)      // Bir kullanıcının birden fazla postu olabilir
                .HasForeignKey(p => p.UserId);  // Foreign key olarak UserId kullanılır
        }






        
    }
}