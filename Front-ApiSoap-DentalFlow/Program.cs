using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    // Establecemos que el esquema por defecto sea JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        // Usa la misma clave secreta con la que firmaste el JWT al crearlo
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Elimina el tiempo de gracia de 5 min por defecto
    };

    // Interceptamos la petición para decirle a .NET que busque el token en la cookie "jwt_token"
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("jwt_token"))
            {
                context.Token = context.Request.Cookies["jwt_token"];
            }
            return Task.CompletedTask;
        }
    };
});
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<moduloPaciente.PatientEndpoint, moduloPaciente.PatientEndpointClient>();

builder.Services.AddScoped<moduloMaterial.MaterialEndpoint, moduloMaterial.MaterialEndpointClient>();
builder.Services.AddScoped<moduloCitas.AppointmentEndpoint,moduloCitas.AppointmentEndpointClient>();
builder.Services.AddScoped<servicioTratamiento.TratamientoEndpoint, servicioTratamiento.TratamientoEndpointClient>();
builder.Services.AddScoped<moduloPago.PagoEndpoint,moduloPago.PagoEndpointClient>();
builder.Services.AddScoped<moduloDashboard.DashboardEndpoint,moduloDashboard.DashboardEndpointClient>();
builder.Services.AddScoped<moduloClinicalStaff.ClinicalStaffEndpoint,moduloClinicalStaff.ClinicalStaffEndpointClient>();
builder.Services.AddScoped<moduloEspecialidades.SpecialtyEndpoint,moduloEspecialidades.SpecialtyEndpointClient>();
builder.Services.AddScoped<moduloTipoCita.AppointmentTypeEndpoint, moduloTipoCita.AppointmentTypeEndpointClient>();

builder.Services.AddScoped<servicioSesionTratamiento.SesionTratamientoEndpoint, servicioSesionTratamiento.SesionTratamientoEndpointClient>();
builder.Services.AddScoped<servicioAuth.AuthEndpoint, servicioAuth.AuthEndpointClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
