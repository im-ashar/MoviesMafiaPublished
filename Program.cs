using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoviesMafia.Models.Repo;
using MoviesMafia.Models.GenericRepo;

var builder = WebApplication.CreateBuilder(args);
//Getting Connection string
string PGHOST = Environment.GetEnvironmentVariable("PGHOST");
string PGPORT = Environment.GetEnvironmentVariable("PGPORT");
string PGDATABASE = Environment.GetEnvironmentVariable("PGDATABASE");
string PGUSER = Environment.GetEnvironmentVariable("PGUSER");
string PGPASSWORD = Environment.GetEnvironmentVariable("PGPASSWORD");

string connString = $"Server={PGHOST};Port={PGPORT};Database={PGDATABASE};User Id={PGUSER};Password={PGPASSWORD}";

//string connString = $"Server=containers-us-west-125.railway.app;Port=6479;Database=railway;User Id=postgres;Password=2Y6lNRRNyVye5VhTRIFa";

//Getting Assembly Name
var migrationAssembly = typeof(Program).Assembly.GetName().Name;

// Add services to the container.

builder.Services.AddDbContext<UserContext>(options =>
options.UseNpgsql(connString, sql => sql.MigrationsAssembly(migrationAssembly)));

builder.Services.AddDbContext<RecordsDBContext>(options=>options.UseNpgsql(connString, sql => sql.MigrationsAssembly(migrationAssembly)));

builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = true);
builder.Services.AddIdentity<ExtendedIdentityUser, IdentityRole>().AddEntityFrameworkStores<UserContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped(typeof(IGenericRecordsDB<>), typeof(GenericRecordsDB<>));
builder.Services.AddServerSideBlazor();

// Add additional services, etc.
builder.Services.AddControllersWithViews();



var app = builder.Build();

var scope = app.Services.CreateScope();

var migUserContext = scope.ServiceProvider.GetRequiredService<UserContext>();
migUserContext.Database.MigrateAsync().Wait();


var migRecordsContext = scope.ServiceProvider.GetRequiredService<RecordsDBContext>();
migRecordsContext.Database.MigrateAsync().Wait();

var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
if (!await roleManager.RoleExistsAsync("Admin"))
{
    var adminRole = new IdentityRole("Admin");
    await roleManager.CreateAsync(adminRole);
}

// Check if the "User" role exists and create it if it doesn't
if (!await roleManager.RoleExistsAsync("User"))
{
    var userRole = new IdentityRole("User");
    await roleManager.CreateAsync(userRole);
}


var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ExtendedIdentityUser>>();
var adminUser = await userManager.FindByNameAsync("admin");
if (adminUser == null)
{
    adminUser = new ExtendedIdentityUser
    {
        UserName = "admin",
        Email = "admin@moviesmafia.com",
        EmailConfirmed = true,
        LockoutEnabled = false,
        ProfilePicturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfilePictures", "admin.jpg")
    };
    var adminPassword= Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
    var result = await userManager.CreateAsync(adminUser, adminPassword);
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LandingPage}/{action=LandingPage}/{id?}");
app.MapBlazorHub();
app.Run();



