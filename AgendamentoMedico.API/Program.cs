using DomainRole = AgendamentoMedico.Domain.Entities.IdentityRole;
using IdentityRole = Microsoft.AspNetCore.Identity.IdentityRole;
using AgendamentoMedico.Infra.Data;
using AgendamentoMedico.Infra.Repositories.Concrete;
using AgendamentoMedico.Infra.Repositories.Interfaces;
using AgendamentoMedico.Services.Services.Concrete;
using AgendamentoMedico.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Claim Autenticação
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie((options) =>
    {
        options.LoginPath = "/Auth/Login";
    });

//String de conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(connectionString));

//FluentEmail
builder.Services
    .AddFluentEmail("agendamentomedico43@gmail.com")
    .AddRazorRenderer()
    .AddSmtpSender(new SmtpClient
    {
        Host = "gmail.com",
        EnableSsl = true,
        Port = 587,
        Credentials = new NetworkCredential("agendamentomedico43@gmail.com", "AgendamentoMedico@123"),
        UseDefaultCredentials = false
    });

//Notificação Com Notify
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});

//AutoMapper
builder.Services.AddAutoMapper(typeof(FuncionarioProfile));

//Injeções de Dependencia
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IHorarioDisponivelRepository, HorarioDisponivelRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<ICargosRepository, CargosRepository>();
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();

builder.Services.AddScoped<IFuncionarioService, FuncionarioService>();
builder.Services.AddScoped<ICargosService, CargosServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IHorarioDisponivelService, HorarioDisponivelService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();

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

app.UseNotyf();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
