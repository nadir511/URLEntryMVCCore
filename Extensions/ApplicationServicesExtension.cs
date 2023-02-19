﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Data;
using URLEntryMVC.Interfaces;
using URLEntryMVC.Models;
using URLEntryMVC.RepositoryClasses;

namespace URLEntryMVC.Extensions;

public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection _services, IConfiguration _config)
    {
        _services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        _services.AddScoped<IUrlRepository, UrlRepository>();
        _services.AddScoped<ICustomerRepository, CustomerRepository>();
        _services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
        });
        _services.AddIdentity<ApplicationUserExtension, IdentityRole>().AddEntityFrameworkStores<DataContext>();
        return _services;
    }
}

