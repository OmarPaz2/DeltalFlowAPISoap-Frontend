var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<moduloPaciente.PatientService, moduloPaciente.PatientServiceClient>();

builder.Services.AddScoped<moduloMaterial.MaterialServiceImpl, moduloMaterial.MaterialServiceImplClient>();
builder.Services.AddScoped<moduloCitas.AppointmentServiceClient>();
builder.Services.AddScoped<servicioTratamiento.TratamientoServiceImplClient>();
builder.Services.AddScoped<moduloPago.PagoServiceImplClient>();
builder.Services.AddScoped<moduloDashboard.DashboardServiceImplClient>();


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
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
