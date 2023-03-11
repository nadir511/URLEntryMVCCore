using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Data;
using URLEntryMVC.Interfaces;
using URLEntryMVC.RepositoryClasses;
using URLEntryMVC.Services;
using URLEntryMVC.ViewModel.EmailServiceVM;

namespace URLEntryMVC.Extensions;

public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection _services, IConfiguration _config)
    {
        _services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        _services.AddScoped<IUrlRepository, UrlRepository>();
        _services.AddScoped<ICustomerRepository, CustomerRepository>();
        _services.AddScoped<IEmailService, EmailService>();
        var emailConfig = _config.GetSection("EmailConfiguration").Get<EmailConfigurationVM>();
        _services.AddSingleton(emailConfig);
        _services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
        });
        _services.AddIdentity<ApplicationUserExtension, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
        
        _services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        _services.AddSession();
        return _services;
    }
}

