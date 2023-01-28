using URLEntryMVC.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/URL/checkRawUrl");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
//var EnteredPath ="";
//app.Use(async (context, next) =>
//{
//    EnteredPath = context.Request.HttpContext.Request.Path.ToString().Remove(0, 1);
//    await next();
//    if (context.Response.StatusCode == 404)
//    {
//        //context.Request.Path = "/URL/checkRawUrl" + EnteredPath;
//        context.Response.Redirect("URL/checkRawUrl?enterdPath=" + EnteredPath);
//        await next();
//    }
//});
app.UseStatusCodePagesWithReExecute("/URL/checkRawUrl", "?statusCode={0}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=URL}/{action=ListOfLinks}/{id?}");

app.Run();
