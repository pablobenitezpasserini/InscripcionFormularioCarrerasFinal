using Administradores.BR.Interfaces.Repositories;
using Administradores.BR.Interfaces.Services;
using Administradores.BR.Interfaces.Validators;
using Administradores.BR.Services;
using Administradores.BR.Validadores;
using Administradores.DA.Repositories;
using Administradores.Models.Administradores.DTOs;
using Administradores.Models.Administradores.Models;
using Administradores.Models.Formularios.DTOs;
using Administradores.Models.Formularios.Models;
using Administradores.Models.Results;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
// Generación del documento OpenAPI
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});

// Configuración de la conexión
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión DefaultConnection.");

// Inyección de dependencias
builder.Services.AddSingleton<IAdministradorRepository>(
    _ => new AdministradorRepository(connectionString));

builder.Services.AddSingleton<IAdministradorService, AdministradorService>();

builder.Services.AddSingleton<IAdministradorValidator, AdministradorValidador>();

builder.Services.AddSingleton<
    IHabilitacionFormularioValidator,
    HabilitacionFormularioValidador>();

builder.Services.AddSingleton<IHabilitacionFormularioRepository>(
    _ => new HabilitacionFormularioRepository(connectionString));

builder.Services.AddSingleton<IHabilitacionFormularioService, HabilitacionFormularioService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// tuve que poner esto porque si hay un error de deseralizacion la excepcion no es controlada
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features
            .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()
            ?.Error;

        if (exception is Microsoft.AspNetCore.Http.BadHttpRequestException)
        {
            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            await Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Solicitud inválida",
                detail: "El cuerpo de la solicitud contiene datos inválidos o incompatibles con los tipos esperados."
            ).ExecuteAsync(context);

            return;
        }

        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Error interno del servidor",
            detail: "Ocurrió un error inesperado al procesar la solicitud."
        ).ExecuteAsync(context);
    });
});

app.UseStatusCodePages();

app.UseCors("FrontendPolicy");

// OpenAPI y Swagger UI solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Administradores API v1");
    });
}

app.UseHttpsRedirection();

// Endpoints administradores
app.MapGet("/api/admin", async (IAdministradorService service) =>
{
    var result = await service.ListarAsync();

    if (result.Success)
    {
        return Results.Ok(result.Data);
    }

    return Results.Problem(
        title: "Error al listar administradores",
        detail: result.Error,
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Administradores")
.WithSummary("Listado de administradores")
.WithDescription("Devuelve un listado de todos los administradores registrados en el sistema")
.Produces<IEnumerable<Administrador>>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status500InternalServerError);


app.MapPost("/api/admin", async (CrearAdminDto admin, IAdministradorService service) =>
{
    var result = await service.CrearAsync(admin);

    if (result.Success)
    {
        return Results.Created("/api/admin", new
        {
            mensaje = "Administrador creado correctamente."
        });
    }

    if (result.ErrorType == ErrorType.Validation)
    {
        return Results.Problem(
            title: "Error de validación",
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (result.ErrorType == ErrorType.Conflict)
    {
        return Results.Problem(
            title: "Conflicto al crear el administrador",
            detail: result.Error,
            statusCode: StatusCodes.Status409Conflict);
    }

    return Results.Problem(
        title: "Error interno del servidor",
        detail: "No se pudo crear el administrador por un error interno.",
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Administradores")
.WithSummary("Alta de administrador")
.WithDescription("Registra un nuevo administrador en el sistema.")
.Produces(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status409Conflict)
.ProducesProblem(StatusCodes.Status500InternalServerError);


app.MapPut("/api/admin/{id}", async (int id, EditarAdminDto admin, IAdministradorService service) =>
{
    var result = await service.EditarAsync(id, admin);

    if (result.Success)
    {
        return Results.Ok(new
        {
            mensaje = "Administrador modificado correctamente."
        });
    }

    if (result.ErrorType == ErrorType.Validation)
    {
        return Results.Problem(
            title: "Error de validación",
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (result.ErrorType == ErrorType.NotFound)
    {
        return Results.Problem(
            title: "Administrador no encontrado",
            detail: result.Error,
            statusCode: StatusCodes.Status404NotFound);
    }

    if (result.ErrorType == ErrorType.Conflict)
    {
        return Results.Problem(
            title: "Conflicto al modificar el administrador",
            detail: result.Error,
            statusCode: StatusCodes.Status409Conflict);
    }

    return Results.Problem(
        title: "Error interno del servidor",
        detail: "No se pudo modificar el administrador por un error interno.",
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Administradores")
.WithSummary("Editar administrador")
.WithDescription("Actualiza los datos de un administrador mediante su ID. Valida los campos obligatorios, las longitudes, la existencia del administrador y la unicidad del DNI y del nombre de usuario.")
.Produces(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status404NotFound)
.ProducesProblem(StatusCodes.Status409Conflict)
.ProducesProblem(StatusCodes.Status500InternalServerError);


app.MapDelete("/api/admin/{id}", async (int id, IAdministradorService service) =>
{
    var result = await service.EliminarAsync(id);

    if (result.Success)
    {
        return Results.Ok(new
        {
            mensaje = "Administrador eliminado correctamente."
        });
    }

    if (result.ErrorType == ErrorType.Validation)
    {
        return Results.Problem(
            title: "Error de validación",
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (result.ErrorType == ErrorType.NotFound)
    {
        return Results.Problem(
            title: "Administrador no encontrado",
            detail: result.Error,
            statusCode: StatusCodes.Status404NotFound);
    }

    if (result.ErrorType == ErrorType.Conflict)
    {
        return Results.Problem(
            title: "Conflicto al eliminar el administrador",
            detail: result.Error,
            statusCode: StatusCodes.Status409Conflict);
    }

    return Results.Problem(
        title: "Error interno del servidor",
        detail: "No se pudo eliminar el administrador por un error interno.",
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Administradores")
.WithSummary("Eliminar administrador")
.WithDescription("Elimina un administrador mediante su ID. No permite eliminar al superadministrador.")
.Produces(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status404NotFound)
.ProducesProblem(StatusCodes.Status409Conflict)
.ProducesProblem(StatusCodes.Status500InternalServerError);



app.MapPost("/api/admin/login", async (AdminLoginRequestDto data, IAdministradorService service) =>
{
    var result = await service.LoguearAsync(
        data.Usuario!,
        data.Contrasena!);

    if (result.Success)
    {
        return Results.Ok(new
        {
            mensaje = result.Data
        });
    }

    if (result.ErrorType == ErrorType.Validation)
    {
        return Results.Problem(
            title: "Error de validación",
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (result.ErrorType == ErrorType.Unauthorized)
    {
        return Results.Problem(
            title: "No autorizado",
            detail: result.Error,
            statusCode: StatusCodes.Status401Unauthorized);
    }

    return Results.Problem(
        title: "Error interno del servidor",
        detail: "No se pudo iniciar sesión por un error interno.",
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Administradores")
.WithSummary("Inicio de sesión del administrador")
.WithDescription("Verifica las credenciales del administrador y devuelve el resultado del inicio de sesión.")
.Produces(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status401Unauthorized)
.ProducesProblem(StatusCodes.Status500InternalServerError);



app.MapGet("/api/admin/exportar-estudiantes", async (IAdministradorService service) =>
{
    var result = await service.ExportarEstudiantesExcelAsync();

    if (result.Success)
    {
        return Results.Ok(result.Data);
    }

    if (result.ErrorType == ErrorType.NotFound)
    {
        return Results.Problem(
            title: "Inscripciones no encontradas",
            detail: result.Error,
            statusCode: StatusCodes.Status404NotFound);
    }

    return Results.Problem(
        title: "Error interno del servidor",
        detail: "No se pudieron obtener los estudiantes para la exportación por un error interno.",
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Administradores")
.WithSummary("Exportar estudiantes para Excel")
.WithDescription("Devuelve un listado de estudiantes inscritos con su información académica.")
.Produces<IEnumerable<EstudianteExportacionDto>>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status404NotFound)
.ProducesProblem(StatusCodes.Status500InternalServerError);



// Endpoints formularios
app.MapGet("/api/formularios", async (IHabilitacionFormularioService service) =>
{
    var result = await service.GetAllAsync();

    if (result.Success)
    {
        return Results.Ok(result.Data);
    }

    return Results.Problem(
        title: "Error al listar los formularios",
        detail: "No se pudieron obtener los formularios por un error interno.",
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Formularios")
.WithSummary("Listado de formularios")
.WithDescription("Devuelve un listado de todos los formularios registrados en el sistema.")
.Produces<IEnumerable<HabilitacionFormulario>>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status500InternalServerError);



app.MapPost("/api/formularios", async (CrearHabilitacionFormularioDto data, IHabilitacionFormularioService service) =>
{
    var result = await service.CrearAsync(data);

    if (result.Success)
    {
        return Results.Created("/api/formularios", new
        {
            mensaje = "Formulario creado correctamente."
        });
    }

    if (result.ErrorType == ErrorType.Validation)
    {
        return Results.Problem(
            title: "Error de validación",
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (result.ErrorType == ErrorType.Conflict)
    {
        return Results.Problem(
            title: "Conflicto al crear el formulario",
            detail: result.Error,
            statusCode: StatusCodes.Status409Conflict);
    }

    return Results.Problem(
        title: "Error interno del servidor",
        detail: "No se pudo crear el formulario por un error interno.",
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Formularios")
.WithSummary("Alta de formulario")
.WithDescription("Crea un nuevo formulario en el sistema.")
.Produces(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status409Conflict)
.ProducesProblem(StatusCodes.Status500InternalServerError);



app.MapPut("/api/formularios/{id}", async (int id, EditarHabilitacionFormularioDto data, IHabilitacionFormularioService service) =>
{
    var result = await service.EditarAsync(id, data);

    if (result.Success)
    {
        return Results.Ok(new
        {
            mensaje = "Formulario actualizado correctamente."
        });
    }

    if (result.ErrorType == ErrorType.Validation)
    {
        return Results.Problem(
            title: "Error de validación",
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (result.ErrorType == ErrorType.NotFound)
    {
        return Results.Problem(
            title: "Formulario no encontrado",
            detail: result.Error,
            statusCode: StatusCodes.Status404NotFound);
    }

    if (result.ErrorType == ErrorType.Conflict)
    {
        return Results.Problem(
            title: "Conflicto al actualizar el formulario",
            detail: result.Error,
            statusCode: StatusCodes.Status409Conflict);
    }

    return Results.Problem(
        title: "Error interno del servidor",
        detail: "No se pudo actualizar el formulario por un error interno.",
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Formularios")
.WithSummary("Editar formulario")
.WithDescription("Actualiza las fechas y el estado de un formulario mediante su ID.")
.Produces(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status404NotFound)
.ProducesProblem(StatusCodes.Status409Conflict)
.ProducesProblem(StatusCodes.Status500InternalServerError);



app.MapDelete("/api/formularios/{id}", async (int id, IHabilitacionFormularioService service) =>
{
    var result = await service.EliminarAsync(id);

    if (result.Success)
    {
        return Results.Ok(new
        {
            mensaje = "Formulario eliminado correctamente."
        });
    }

    if (result.ErrorType == ErrorType.Validation)
    {
        return Results.Problem(
            title: "Error de validación",
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (result.ErrorType == ErrorType.NotFound)
    {
        return Results.Problem(
            title: "Formulario no encontrado",
            detail: result.Error,
            statusCode: StatusCodes.Status404NotFound);
    }

    return Results.Problem(
        title: "Error interno del servidor",
        detail: "No se pudo eliminar el formulario por un error interno.",
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Formularios")
.WithSummary("Eliminar formulario")
.WithDescription("Elimina un formulario del sistema mediante su ID.")
.Produces(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status404NotFound)
.ProducesProblem(StatusCodes.Status500InternalServerError);


app.MapGet("/api/formulario/disponibilidad", async (IHabilitacionFormularioService service) =>
{
    var result = await service.EstaDisponibleAsync();

    if (result.Success)
    {
        return Results.Ok(result.Data);
    }

    return Results.Problem(
        title: "Error al consultar la disponibilidad del formulario",
        detail: "No se pudo consultar la disponibilidad por un error interno.",
        statusCode: StatusCodes.Status500InternalServerError);
})
.WithTags("Formularios")
.WithSummary("Obtener formulario disponible")
.WithDescription("Devuelve el formulario habilitado o cuyo ciclo de inscripción está vigente. Si no hay ninguno disponible, devuelve null.")
.Produces<HabilitacionFormulario>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status500InternalServerError);


app.Run();
