using Microsoft.EntityFrameworkCore;
using MinimalAPIPostgreSQLEF.Data;
using MinimalAPIPostgreSQLEF.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar dependencias de la BD leyendo la cadena de conexion: PostgreSQLConnection 
// de las variables de configuracion
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");

// agrega el contenedor de dependencias llamado DbContext "OfficeDb" usando postgres 
// y dentro enviamos la cadena de conexion "connectionString"
builder.Services.AddDbContext<OfficeDb>(options =>
    options.UseNpgsql(connectionString)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // app.UseOpenApi();
    // app.UseSwaggerUi(config =>
    // {
    //     config.DocumentTitle = "TodoAPI";
    //     config.Path = "/swagger";
    //     config.DocumentPath = "/swagger/{documentName}/swagger.json";
    //     config.DocExpansion = "list";
    // });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.MapGet("/", () => "Hello World!");


app.MapGet("/employees", async (OfficeDb db) => await db.Employees.ToListAsync());

app.MapPost("/employees/", async(Employee e, OfficeDb db)=> {
    db.Employees.Add(e);
    await db.SaveChangesAsync();

    return Results.Created($"/employee/{e.Id}", e);
});

app.MapGet("/employees/{id:int}", async(int id, OfficeDb db)=> 
{
    return await db.Employees.FindAsync(id)
            is Employee e
            ? Results.Ok(e)
            : Results.NotFound();
});

app.MapPut("/employees/{id:int}", async(int id, Employee e, OfficeDb db)=>
{
    if (e.Id != id)
    {
        return Results.BadRequest();
    }

    var emp = await db.Employees.FindAsync(id);
    
    if (emp is null) return Results.NotFound();

    //found, so update with incoming note n.
    emp.FirstName = e.FirstName;
    emp.LastName = e.LastName;
    emp.Branch = e.Branch;
    emp.Age = e.Age;
    await db.SaveChangesAsync();
    return Results.Ok(emp);
});


app.MapDelete("/employees/{id:int}", async(int id, OfficeDb db)=>{

    var employee = await db.Employees.FindAsync(id);

    if (employee is null) return Results.NotFound();
    
    db.Employees.Remove(employee);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
