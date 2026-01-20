using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using H4G_Project.DAL;
using H4G_Project.Services;


var builder = WebApplication.CreateBuilder(args);

// Firebase initialization for auth - handle missing credentials gracefully
try
{
    if (FirebaseApp.DefaultInstance == null)
    {
        var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "DAL", "config", "squad-60b0b-firebase-adminsdk-fbsvc-cff3f594d5.json");
        
        if (File.Exists(credentialPath))
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(credentialPath)
            });
            Console.WriteLine("Firebase initialized successfully");
        }
        else
        {
            Console.WriteLine("Firebase credential file not found. Firebase features will be disabled.");
            // Continue without Firebase for now
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Firebase initialization failed: {ex.Message}");
    // Continue without Firebase for now
}

builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<EmailService>();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// MVC
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<UserDAL>();
builder.Services.AddScoped<StaffDAL>();


var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Configure for Render deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");
