var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
