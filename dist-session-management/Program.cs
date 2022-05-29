var builder = WebApplication.CreateBuilder(args);

// Add Distributed Redis Cache for Session
builder.Services.AddDistributedRedisCache(options =>
{
    options.Configuration = "localhost";
    options.InstanceName = "Session_";
});
builder.Services.AddSession(options =>
{
    // 20 minutes later from last access your session will be removed.
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});
// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();


// Adds session middleware to pipeline
app.UseSession();

app.Run();
