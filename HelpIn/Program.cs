var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// 👉 ROTA AMIGÁVEL
app.MapControllerRoute(
    name: "cadastroEscolha",
    pattern: "cadastro",
    defaults: new { controller = "Home", action = "EscolhaCadastro" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "cadastroOng",
    pattern: "cadastro-ong",
    defaults: new { controller = "Ong", action = "Cadastro" });

app.Run();
