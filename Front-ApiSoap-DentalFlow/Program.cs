var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<moduloPaciente.PatientService, moduloPaciente.PatientServiceClient>();

builder.Services.AddScoped<moduloMaterial.MaterialServiceImpl, moduloMaterial.MaterialServiceImplClient>();
builder.Services.AddScoped<moduloCitas.AppointmentEndpoint,moduloCitas.AppointmentEndpointClient>();
builder.Services.AddScoped<servicioTratamiento.TratamientoServiceImplClient>();
builder.Services.AddScoped<moduloPago.PagoServiceImplClient>();
builder.Services.AddScoped<moduloDashboard.DashboardServiceImplClient>();
builder.Services.AddScoped<moduloclinicalStaff.ClinicalStaffService,moduloclinicalStaff.ClinicalStaffServiceClient>();
builder.Services.AddScoped<moduloEspecialidades.SpecialtyService,moduloEspecialidades.SpecialtyServiceClient>();
builder.Services.AddScoped<moduloTipoCita.AppointmentTypeService, moduloTipoCita.AppointmentTypeServiceClient>();
builder.Services.AddScoped<moduloTipoCita.AppointmentTypeService, moduloTipoCita.AppointmentTypeServiceClient>();
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
