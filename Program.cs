using IdentityApp.Entity;
using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IEmailSender,SmtpEmailSender>( i => 
new SmtpEmailSender(
    builder.Configuration["EmailSender:Host"],
    builder.Configuration.GetValue<int>("EmailSender:Port"),
    builder.Configuration.GetValue<bool>("EmailSender:EnableSSL"),
    builder.Configuration["EmailSender:Username"],
    builder.Configuration["EmailSender:Password"]

));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<IdentityContext>(options =>{
    options.UseSqlite(builder.Configuration.GetConnectionString("connection_"));
});

builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>{

    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;  // Değişik karakter zorunluluğu

    // options.User.RequireUniqueEmail = true;            // Bir e-posta bir kez kullanılabilir
    options.SignIn.RequireConfirmedEmail = true;       // E-posta doğrulaması gereksinimi

      options.User.RequireUniqueEmail = true;  //bir e-posta bir kez
    // options.User.AllowedUserNameCharacters = ""; bu karakterler harici şifre oluştur

    //   options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); 
    //   options.Lockout.MaxFailedAccessAttempts = 5;
    //   hatalı parola girişinde 5 dk engelle

});

builder.Services.ConfigureApplicationCookie(options =>{
    options.LoginPath = "/Account/Login"; 

    //  options.SlidingExpiration = true;
    // options.ExpireTimeSpan = TimeSpan.FromDays(30);      // 30 gün boyunca hesap açık kalır
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
