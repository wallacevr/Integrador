using HelpIn; // use o namespace correto do seu DbContext
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);

// Conexão com MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 34)) // ajuste para sua versão do MySQL
    ));

// MVC
builder.Services.AddControllersWithViews();



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OngPolicy", policy => policy.RequireRole("Ong"));
    options.AddPolicy("VoluntarioPolicy", policy => policy.RequireRole("Voluntario"));
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";  // Caminho para a tela de login
        options.AccessDeniedPath = "/login"; // Opcional
    });
var app = builder.Build();
// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // importante para servir arquivos estáticos (img, css, js)

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 🗂️ Rotas amigáveis
app.MapControllerRoute(
    name: "cadastroEscolha",
    pattern: "cadastro",
    defaults: new { controller = "Home", action = "EscolhaCadastro" });



// Rota padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

