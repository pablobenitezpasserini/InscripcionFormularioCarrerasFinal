using Administradores.BR.Interfaces.Repositories;
using Administradores.BR.Interfaces.Validators;
using Administradores.BR.Services;
using Administradores.Models.Administradores.DTOs;
using Administradores.Models.Administradores.Models;
using Administradores.Models.Exceptions;
using Administradores.Models.Results;
using Moq;

namespace AdministradoresTest.BR;

/// <summary>
/// Contiene las pruebas unitarias de la clase <see cref="AdministradorService"/>.
/// </summary>
[TestClass]
public sealed class AdministradorServiceTest
{
    /// <summary>
    /// Simulación del validador de administradores utilizada para verificar las validaciones del servicio.
    /// </summary>
    private Mock<IAdministradorValidator> _adminValidadorMock = null!;

    /// <summary>
    /// Simulación del repositorio de administradores utilizada para verificar las operaciones de acceso a datos.
    /// </summary>
    private Mock<IAdministradorRepository> _adminRepositoryMock = null!;

    /// <summary>
    /// Instancia del servicio de administradores utilizada para ejecutar las pruebas unitarias.
    /// </summary>
    private AdministradorService _service = null!;

    #region Helpers

    /// <summary>
    /// Inicializa los mocks del validador y del repositorio, y crea una instancia del servicio
    /// antes de la ejecución de cada prueba.
    /// </summary>
    [TestInitialize]
    public void Inicializar()
    {
        _adminValidadorMock = new Mock<IAdministradorValidator>();
        _adminRepositoryMock = new Mock<IAdministradorRepository>();

        _service = new AdministradorService(
            _adminValidadorMock.Object,
            _adminRepositoryMock.Object
        );
    }

    /// <summary>
    /// Crea y devuelve un DTO de creación de administrador con datos válidos
    /// para utilizar en las pruebas unitarias.
    /// </summary>
    /// <returns>Un <see cref="CrearAdminDto"/> con datos de administrador válidos.</returns>
    private static CrearAdminDto CrearAdminDatosCorrectos()
    {
        return new CrearAdminDto
        {
            Admin_Nom_Ape = "Pepe",
            Admin_Contra = "12345",
            Admin_DNI = "12345678",
            Admin_Tipo_DNI = "DNI",
            Admin_Nom_Usuario = "PepeUser"
        };
    }

    /// <summary>
    /// Crea y devuelve un DTO de edición de administrador con datos válidos
    /// para utilizar en las pruebas unitarias.
    /// </summary>
    /// <returns>Un <see cref="EditarAdminDto"/> con datos de administrador válidos.</returns>
    private static EditarAdminDto EditarAdminDatosCorrectos()
    {
        return new EditarAdminDto
        {
            Admin_Nom_Ape = "Pepe",
            Admin_Contra = "12345",
            Admin_DNI = "12345678",
            Admin_Tipo_DNI = "DNI",
            Admin_Nom_Usuario = "PepeUser"
        };
    }

    /// <summary>
    /// Verifica que un resultado de tipo <see cref="Result{T}"/> indique un fallo,
    /// con el tipo de error y el mensaje esperados.
    /// </summary>
    /// <param name="resultado">Resultado de la operación que se desea verificar.</param>
    /// <param name="tipoError">Tipo de error esperado.</param>
    /// <param name="mensaje">Mensaje de error esperado.</param>
    private static void AssertResultadoError(Result<bool> resultado, ErrorType tipoError, string mensaje)
    {
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(tipoError, resultado.ErrorType);
        Assert.AreEqual(mensaje, resultado.Error);
    }

    /// <summary>
    /// Verifica que un resultado de tipo <see cref="Result{T}"/> indique una operación exitosa
    /// y que su dato devuelto sea verdadero.
    /// </summary>
    /// <param name="resultado">Resultado de la operación que se desea verificar.</param>
    private static void AssertResultadoExitoso(Result<bool> resultado)
    {
        Assert.IsTrue(resultado.Success);
        Assert.IsTrue(resultado.Data);
    }
    #endregion

    #region Test de listar
    /// <summary>
    /// Verifica que, cuando el repositorio devuelve correctamente la lista de administradores,
    /// el servicio retorne un resultado exitoso con los mismos datos y sin errores.
    /// También comprueba que el método del repositorio se invoque una sola vez.
    /// </summary>
    [TestMethod]
    public async Task ListarAsync_CuandoRepositorioRespondeCorrectamente_RetornaResultadoExitoso()
    {
        // Arrange: preparo los datos y configuro el mock
        IEnumerable<Administrador> administradores =
            new List<Administrador>();

        _adminRepositoryMock
            .Setup(repo => repo.ListarAsync())
            .ReturnsAsync(administradores);

        // Act: ejecuto el método que estoy probando
        var resultado = await _service.ListarAsync();

        // Assert: verifico el resultado
        Assert.IsTrue(resultado.Success);
        Assert.IsNotNull(resultado.Data);
        Assert.AreSame(administradores, resultado.Data);
        Assert.IsNull(resultado.Error);
        Assert.AreEqual(ErrorType.None, resultado.ErrorType);

        // Verifico que el repositorio se haya llamado una sola vez
        _adminRepositoryMock.Verify(
            repo => repo.ListarAsync(),
            Times.Once
        );
    }

    /// <summary>
    /// Verifica que, cuando el repositorio lanza una <see cref="DataAccessException"/>
    /// al intentar obtener los administradores, el servicio retorne un error inesperado
    /// con el mensaje correspondiente.
    /// También comprueba que el repositorio se invoque una sola vez.
    /// </summary>
    [TestMethod]
    public async Task ListarAsync_CuandoRepositorioLanzaDataAccessException_RetornaErrorUnexpected()
    {
        // Arrange: simulo un error de persistencia
        _adminRepositoryMock
            .Setup(repo => repo.ListarAsync())
            .ThrowsAsync(
                new DataAccessException("Error de persistencia",
                new Exception("Error interno de base de datos"))
            );

        // Act: ejecuto el método que estoy probando
        var resultado = await _service.ListarAsync();

        // Assert: verifico que el servicio gestione el error
        Assert.IsFalse(resultado.Success);
        Assert.IsNull(resultado.Data);

        Assert.AreEqual(
            "No se pudieron obtener los administradores por un error de persistencia.",
            resultado.Error
        );

        Assert.AreEqual(
            ErrorType.Unexpected,
            resultado.ErrorType
        );
        // Verifico que el repositorio se haya llamado una sola vez
        _adminRepositoryMock.Verify(
            repo => repo.ListarAsync(),
            Times.Once
        );
    }
    #endregion

    #region Test de crear
    /// <summary>
    /// Verifica que, cuando el validador devuelve un error al validar los datos del administrador,
    /// el servicio retorne un error de tipo Validation y no realice consultas ni operaciones
    /// de creación en el repositorio.
    /// </summary>
    [TestMethod]
    public async Task CrearAsync_CuandoValidadorDevuelveError_RetornaValidation()
    {
        // Arrange: preparo datos y configuro mock
        CrearAdminDto adminDto = CrearAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarCrear(adminDto))
            .Returns("Error en la validacion");

        // Act:
        var resultado = await _service.CrearAsync(adminDto);

        // Assert:
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);
        Assert.AreEqual("Error en la validacion", resultado.Error);

        _adminRepositoryMock.Verify(
            r => r.ExisteDniAsync(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.ExisteUsuarioAsync(It.IsAny<string>()),
            Times.Never);

        _adminRepositoryMock.Verify(
        r => r.CrearAsync(It.IsAny<Administrador>()),
        Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando se intenta registrar un administrador con el nombre de usuario
    /// reservado para el super administrador, el servicio retorne un error de tipo Conflict.
    /// Contempla distintas combinaciones de mayúsculas y minúsculas.
    /// </summary>
    [TestMethod]
    [DataRow("admin")]
    [DataRow("ADMIN")]
    [DataRow("Admin")]
    [DataRow("AdMiN")]
    public async Task CrearAsync_CuandoUsuarioEstaReservado_RetornaConflict(string usuario)
    {
        // Arrange
        CrearAdminDto adminDto = CrearAdminDatosCorrectos();
        adminDto.Admin_Nom_Usuario = usuario;

        _adminValidadorMock
            .Setup(v => v.ValidarCrear(adminDto))
            .Returns((string?)null);

        // Act
        var resultado = await _service.CrearAsync(adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "El nombre de usuario \"admin\" está reservado para el super administrador.");

        _adminRepositoryMock.Verify(
            r => r.ExisteDniAsync(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.ExisteUsuarioAsync(It.IsAny<string>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.CrearAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando se intenta registrar un administrador con el DNI reservado
    /// para el super administrador y el tipo de documento correspondiente,
    /// el servicio retorne un error de tipo Conflict sin realizar consultas ni crear el administrador.
    /// </summary>
    [TestMethod]
    public async Task CrearAsync_CuandoDniEstaReservado_RetornaConflict()
    {
        // Arrange
        CrearAdminDto adminDto = CrearAdminDatosCorrectos();
        adminDto.Admin_DNI = "00000000";
        adminDto.Admin_Tipo_DNI = "DNI";

        _adminValidadorMock
            .Setup(v => v.ValidarCrear(adminDto))
            .Returns((string?)null);

        // Act
        var resultado = await _service.CrearAsync(adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "El DNI 00000000 está reservado para el super administrador.");

        _adminRepositoryMock.Verify(
            r => r.ExisteDniAsync(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.ExisteUsuarioAsync(It.IsAny<string>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.CrearAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando se utiliza el DNI reservado con un tipo de documento distinto
    /// al DNI, el servicio permita continuar con el registro si no existen otros conflictos.
    /// </summary>
    [TestMethod]
    public async Task CrearAsync_CuandoDniReservadoPeroTipoDistinto_RetornaResultadoDeCreacion()
    {
        // Arrange
        CrearAdminDto adminDto = CrearAdminDatosCorrectos();
        adminDto.Admin_DNI = "00000000";
        adminDto.Admin_Tipo_DNI = "Pasaporte";

        _adminValidadorMock
            .Setup(v => v.ValidarCrear(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync("00000000", "Pasaporte"))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync(adminDto.Admin_Nom_Usuario!.Trim()))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.CrearAsync(It.IsAny<Administrador>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.CrearAsync(adminDto);

        // Assert
        Assert.IsTrue(resultado.Success);

        _adminRepositoryMock.Verify(
            r => r.CrearAsync(It.IsAny<Administrador>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, cuando ya existe un administrador con el mismo DNI y tipo de documento,
    /// el servicio retorne un error de tipo Conflict y no continúe con la verificación del usuario
    /// ni con la creación del administrador.
    /// </summary>
    [TestMethod]
    public async Task CrearAsync_CuandoDniYaExiste_RetornaConflict()
    {
        // Arrange
        CrearAdminDto adminDto = CrearAdminDatosCorrectos();
        string dni = adminDto.Admin_DNI!.Trim();
        string tipoDni = adminDto.Admin_Tipo_DNI!.Trim();

        _adminValidadorMock
            .Setup(v => v.ValidarCrear(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync(dni, tipoDni))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.CrearAsync(adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "Ya existe un administrador con ese DNI y tipo de DNI.");

        _adminRepositoryMock.Verify(
            r => r.ExisteDniAsync(dni, tipoDni),
            Times.Once);

        _adminRepositoryMock.Verify(
            r => r.ExisteUsuarioAsync(It.IsAny<string>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.CrearAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando ya existe un administrador con el nombre de usuario ingresado,
    /// el servicio retorne un error de tipo Conflict y no realice la creación.
    /// También comprueba que se hayan realizado las consultas de existencia del DNI y del usuario.
    /// </summary>
    [TestMethod]
    public async Task CrearAsync_CuandoUsuarioYaExiste_RetornaConflict()
    {
        // Arrange
        CrearAdminDto adminDto = CrearAdminDatosCorrectos();
        string dni = adminDto.Admin_DNI!.Trim();
        string tipoDni = adminDto.Admin_Tipo_DNI!.Trim();
        string usuario = adminDto.Admin_Nom_Usuario!.Trim();

        _adminValidadorMock
            .Setup(v => v.ValidarCrear(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync(dni, tipoDni))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync(usuario))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.CrearAsync(adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "El nombre de usuario ingresado ya existe.");

        _adminRepositoryMock.Verify(
            r => r.ExisteDniAsync(dni, tipoDni),
            Times.Once);

        _adminRepositoryMock.Verify(
            r => r.ExisteUsuarioAsync(usuario),
            Times.Once);

        _adminRepositoryMock.Verify(
            r => r.CrearAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando los datos son válidos y no existen conflictos de DNI ni de usuario,
    /// el servicio cree correctamente el administrador con los datos proporcionados,
    /// aplicando el recorte de espacios en los campos correspondientes.
    /// </summary>
    [TestMethod]
    public async Task CrearAsync_CuandoDatosSonCorrectos_CreaAdministrador()
    {
        // Arrange
        CrearAdminDto adminDto = CrearAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarCrear(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync(
                adminDto.Admin_DNI!.Trim(),
                adminDto.Admin_Tipo_DNI!.Trim()))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync(
                adminDto.Admin_Nom_Usuario!.Trim()))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.CrearAsync(It.IsAny<Administrador>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.CrearAsync(adminDto);

        // Assert
        AssertResultadoExitoso(resultado);

        _adminRepositoryMock.Verify(
            r => r.CrearAsync(It.Is<Administrador>(a =>
                a.Admin_Nom_Ape == adminDto.Admin_Nom_Ape!.Trim() &&
                a.Admin_DNI == adminDto.Admin_DNI!.Trim() &&
                a.Admin_Tipo_DNI == adminDto.Admin_Tipo_DNI!.Trim() &&
                a.Admin_Nom_Usuario == adminDto.Admin_Nom_Usuario!.Trim() &&
                a.Admin_Contra == adminDto.Admin_Contra)),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia al consultar si el DNI ya existe,
    /// el servicio retorne un error de tipo Unexpected y no continúe con las demás operaciones.
    /// </summary>
    [TestMethod]
    public async Task CrearAsync_CuandoFallaConsultaDni_RetornaUnexpected()
    {
        // Arrange
        CrearAdminDto adminDto = CrearAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarCrear(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync(
                adminDto.Admin_DNI!.Trim(),
                adminDto.Admin_Tipo_DNI!.Trim()))
            .ThrowsAsync(new DataAccessException(
                "Error de persistencia.",
                new Exception("Error interno de prueba.")));

        // Act
        var resultado = await _service.CrearAsync(adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Unexpected, "No se pudo crear el administrador por un error de base de datos.");

        _adminRepositoryMock.Verify(
            r => r.ExisteUsuarioAsync(It.IsAny<string>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.CrearAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia al consultar si el nombre de usuario
    /// ya existe, el servicio retorne un error de tipo Unexpected y no intente crear el administrador.
    /// </summary>
    [TestMethod]
    public async Task CrearAsync_CuandoFallaConsultaUsuario_RetornaUnexpected()
    {
        // Arrange
        CrearAdminDto adminDto = CrearAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarCrear(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync(
                adminDto.Admin_DNI!.Trim(),
                adminDto.Admin_Tipo_DNI!.Trim()))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync(
                adminDto.Admin_Nom_Usuario!.Trim()))
            .ThrowsAsync(new DataAccessException(
                "Error de persistencia.",
                new Exception("Error interno de prueba.")));

        // Act
        var resultado = await _service.CrearAsync(adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Unexpected, "No se pudo crear el administrador por un error de base de datos.");

        _adminRepositoryMock.Verify(
            r => r.CrearAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia durante la creación del administrador
    /// en el repositorio, el servicio retorne un error de tipo Unexpected.
    /// También comprueba que se haya intentado ejecutar la operación de creación una sola vez.
    /// </summary>
    [TestMethod]
    public async Task CrearAsync_CuandoFallaCreacionEnRepositorio_RetornaUnexpected()
    {
        // Arrange
        CrearAdminDto adminDto = CrearAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarCrear(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync(
                adminDto.Admin_DNI!.Trim(),
                adminDto.Admin_Tipo_DNI!.Trim()))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync(
                adminDto.Admin_Nom_Usuario!.Trim()))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.CrearAsync(It.IsAny<Administrador>()))
            .ThrowsAsync(new DataAccessException(
                "Error de persistencia.",
                new Exception("Error interno de prueba.")));

        // Act
        var resultado = await _service.CrearAsync(adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Unexpected, "No se pudo crear el administrador por un error de base de datos.");

        _adminRepositoryMock.Verify(
            r => r.CrearAsync(It.IsAny<Administrador>()),
            Times.Once);
    }
    #endregion

    #region Test de editar
    /// <summary>
    /// Verifica que, cuando se intenta editar un administrador con un ID menor o igual a cero,
    /// el servicio retorne un error de tipo Validation y no ejecute las validaciones
    /// ni consulte la existencia del administrador.
    /// </summary>
    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(-10)]
    public async Task EditarAsync_CuandoIdNoEsValido_RetornaValidation(int id)
    {
        // Arrange
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Validation, "El ID del administrador no es válido.");

        _adminValidadorMock.Verify(
            v => v.ValidarEditar(It.IsAny<EditarAdminDto>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.ExistePorIdAsync(It.IsAny<int>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando el validador devuelve un error al validar los datos de edición,
    /// el servicio retorne un error de tipo Validation y no consulte la existencia
    /// del administrador ni intente modificarlo.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoValidadorDevuelveError_RetornaValidation()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns("Error en la validacion");

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Validation, "Error en la validacion");

        _adminRepositoryMock.Verify(
            r => r.ExistePorIdAsync(It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando no existe un administrador con el ID indicado,
    /// el servicio retorne un error de tipo NotFound y no continúe con las demás
    /// verificaciones ni intente modificarlo.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoAdministradorNoExiste_RetornaNotFound()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(false);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.NotFound, "No existe un administrador con el ID indicado.");

        _adminRepositoryMock.Verify(
            r => r.EsSuperAdminAsync(It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.ExisteDniAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia al consultar si el administrador
    /// existe, el servicio retorne un error de tipo Unexpected y no continúe con las
    /// verificaciones ni intente realizar la edición.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoFallaConsultaExistencia_RetornaUnexpected()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ThrowsAsync(new DataAccessException(
                "Error de persistencia.",
                new Exception("Error interno de prueba.")));

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Unexpected, "No se pudo modificar el administrador por un error de persistencia.");

        _adminRepositoryMock.Verify(
            r => r.EsSuperAdminAsync(It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando un administrador común intenta utilizar el nombre de usuario
    /// reservado para el super administrador, el servicio retorne un error de tipo Conflict.
    /// Contempla distintas combinaciones de mayúsculas y minúsculas.
    /// </summary>
    [TestMethod]
    [DataRow("admin")]
    [DataRow("ADMIN")]
    [DataRow("Admin")]
    [DataRow("AdMiN")]
    public async Task EditarAsync_CuandoAdminNormalUsaUsuarioReservado_RetornaConflict(string usuario)
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();
        adminDto.Admin_Nom_Usuario = usuario;

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "El nombre de usuario admin está reservado.");

        _adminRepositoryMock.Verify(
            r => r.ExisteDniAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando un administrador común intenta utilizar el DNI reservado
    /// para el super administrador junto con el tipo de documento DNI,
    /// el servicio retorne un error de tipo Conflict y no realice la edición.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoAdminNormalUsaDniReservado_RetornaConflict()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();
        adminDto.Admin_DNI = "00000000";
        adminDto.Admin_Tipo_DNI = "DNI";

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "El DNI 00000000 está reservado.");

        _adminRepositoryMock.Verify(
            r => r.ExisteDniAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando un administrador común utiliza el DNI reservado
    /// con un tipo de documento distinto de DNI, el servicio permita continuar
    /// con la edición si no existen otros conflictos.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoDniReservadoTieneOtroTipo_ContinuaEdicion()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();
        adminDto.Admin_DNI = "00000000";
        adminDto.Admin_Tipo_DNI = "Pasaporte";

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync("00000000", "Pasaporte", id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync("PepeUser", id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.EditarAsync(It.IsAny<Administrador>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        Assert.IsTrue(resultado.Success);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, cuando el super administrador intenta modificar su DNI,
    /// el servicio retorne un error de tipo Conflict y no consulte la existencia
    /// de otros DNI ni ejecute la edición.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoSuperAdminIntentaCambiarDni_RetornaConflict()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();
        adminDto.Admin_DNI = "12345678";
        adminDto.Admin_Tipo_DNI = "DNI";
        adminDto.Admin_Nom_Usuario = "admin";

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "No se puede modificar el DNI ni el tipo de DNI del super administrador.");

        _adminRepositoryMock.Verify(
            r => r.ExisteDniAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando el super administrador intenta modificar su tipo de DNI,
    /// el servicio retorne un error de tipo Conflict y no ejecute la edición.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoSuperAdminIntentaCambiarTipoDni_RetornaConflict()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();
        adminDto.Admin_DNI = "00000000";
        adminDto.Admin_Tipo_DNI = "Pasaporte";
        adminDto.Admin_Nom_Usuario = "admin";

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "No se puede modificar el DNI ni el tipo de DNI del super administrador.");

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando el super administrador intenta modificar su nombre de usuario,
    /// el servicio retorne un error de tipo Conflict y no continúe con las verificaciones
    /// de DNI ni ejecute la edición.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoSuperAdminIntentaCambiarUsuario_RetornaConflict()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();
        adminDto.Admin_DNI = "00000000";
        adminDto.Admin_Tipo_DNI = "DNI";
        adminDto.Admin_Nom_Usuario = "OtroUsuario";

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "No se puede modificar el nombre de usuario del super administrador.");

        _adminRepositoryMock.Verify(
            r => r.ExisteDniAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que el super administrador pueda editar sus datos cuando conserva
    /// el DNI, el tipo de documento y el nombre de usuario reservados,
    /// independientemente de las mayúsculas y minúsculas utilizadas.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoSuperAdminConservaDatosReservados_PermiteEditar()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();
        adminDto.Admin_DNI = "00000000";
        adminDto.Admin_Tipo_DNI = "dni";
        adminDto.Admin_Nom_Usuario = "ADMIN";

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync("00000000", "dni", id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync("ADMIN", id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.EditarAsync(It.IsAny<Administrador>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        Assert.IsTrue(resultado.Success);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.Is<Administrador>(a =>
                a.ID_Admin == id &&
                a.Admin_DNI == "00000000" &&
                a.Admin_Tipo_DNI == "dni" &&
                a.Admin_Nom_Usuario == "ADMIN")),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, cuando el DNI y el tipo de documento ingresados ya pertenecen
    /// a otro administrador, el servicio retorne un error de tipo Conflict
    /// y no continúe con la verificación del nombre de usuario ni realice la edición.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoDniYaPerteneceAOtroAdmin_RetornaConflict()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync("12345678", "DNI", id))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "Ya existe otro administrador con ese DNI y tipo de DNI.");

        _adminRepositoryMock.Verify(
            r => r.ExisteUsuarioAsync(It.IsAny<string>(), It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando el nombre de usuario ingresado ya pertenece a otro administrador,
    /// el servicio retorne un error de tipo Conflict y no realice la modificación.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoUsuarioYaPerteneceAOtroAdmin_RetornaConflict()
    {
        // Arrange
        int id = 1;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync("12345678", "DNI", id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync("PepeUser", id))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "El nombre de usuario ya pertenece a otro administrador.");

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando los datos son válidos y no existen conflictos de DNI ni de usuario,
    /// el servicio modifique correctamente el administrador con los datos proporcionados.
    /// También comprueba que la entidad enviada al repositorio contenga los valores esperados.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoDatosSonCorrectos_EditaAdministrador()
    {
        // Arrange
        int id = 5;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync("12345678", "DNI", id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync("PepeUser", id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.EditarAsync(It.IsAny<Administrador>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoExitoso(resultado);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.Is<Administrador>(a =>
                a.ID_Admin == id &&
                a.Admin_Nom_Ape == "Pepe" &&
                a.Admin_DNI == "12345678" &&
                a.Admin_Tipo_DNI == "DNI" &&
                a.Admin_Nom_Usuario == "PepeUser" &&
                a.Admin_Contra == "12345")),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia al consultar si el DNI
    /// ya pertenece a otro administrador, el servicio retorne un error de tipo Unexpected
    /// y no continúe con la consulta del usuario ni con la edición.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoFallaConsultaDni_RetornaUnexpected()
    {
        // Arrange
        int id = 5;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync("12345678", "DNI", id))
            .ThrowsAsync(new DataAccessException(
                "Error de persistencia.",
                new Exception("Error interno de prueba.")));

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Unexpected, "No se pudo modificar el administrador por un error de persistencia.");

        _adminRepositoryMock.Verify(
            r => r.ExisteUsuarioAsync(It.IsAny<string>(), It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia al consultar si el nombre
    /// de usuario ya pertenece a otro administrador, el servicio retorne un error
    /// de tipo Unexpected y no intente realizar la edición.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoFallaConsultaUsuario_RetornaUnexpected()
    {
        // Arrange
        int id = 5;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync("12345678", "DNI", id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync("PepeUser", id))
            .ThrowsAsync(new DataAccessException(
                "Error de persistencia.",
                new Exception("Error interno de prueba.")));

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Unexpected, "No se pudo modificar el administrador por un error de persistencia.");

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia durante la modificación
    /// del administrador en el repositorio, el servicio retorne un error de tipo Unexpected.
    /// También comprueba que se haya intentado ejecutar la operación de edición una sola vez.
    /// </summary>
    [TestMethod]
    public async Task EditarAsync_CuandoFallaEdicionEnRepositorio_RetornaUnexpected()
    {
        // Arrange
        int id = 5;
        EditarAdminDto adminDto = EditarAdminDatosCorrectos();

        _adminValidadorMock
            .Setup(v => v.ValidarEditar(adminDto))
            .Returns((string?)null);

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteDniAsync("12345678", "DNI", id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.ExisteUsuarioAsync("PepeUser", id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.EditarAsync(It.IsAny<Administrador>()))
            .ThrowsAsync(new DataAccessException(
                "Error de persistencia.",
                new Exception("Error interno de prueba.")));

        // Act
        var resultado = await _service.EditarAsync(id, adminDto);

        // Assert
        AssertResultadoError(resultado, ErrorType.Unexpected, "No se pudo modificar el administrador por un error de persistencia.");

        _adminRepositoryMock.Verify(
            r => r.EditarAsync(It.IsAny<Administrador>()),
            Times.Once);
    }
    #endregion

    #region Test de eliminar
    /// <summary>
    /// Verifica que, al intentar eliminar un administrador con un ID inválido (cero o negativo),
    /// el servicio retorne un error de validación y no realice consultas ni operaciones de eliminación en el repositorio.
    /// </summary>
    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(-10)]
    public async Task EliminarAsync_CuandoIdNoEsValido_RetornaValidation(int id)
    {
        // Arrange

        // Act
        var resultado = await _service.EliminarAsync(id);

        // Assert
        AssertResultadoError(resultado, ErrorType.Validation, "El ID del administrador no es válido.");

        _adminRepositoryMock.Verify(
            r => r.ExistePorIdAsync(It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EliminarAsync(It.IsAny<int>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando el administrador indicado no existe,
    /// el servicio retorne un error de tipo <see cref="ErrorType.NotFound"/>
    /// y no intente consultar si es superadministrador ni eliminarlo.
    /// </summary>
    [TestMethod]
    public async Task EliminarAsync_CuandoAdministradorNoExiste_RetornaNotFound()
    {
        // Arrange
        int id = 5;

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(false);

        // Act
        var resultado = await _service.EliminarAsync(id);

        // Assert
        AssertResultadoError(resultado, ErrorType.NotFound, "No existe un administrador con el ID indicado.");

        _adminRepositoryMock.Verify(
            r => r.EsSuperAdminAsync(It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EliminarAsync(It.IsAny<int>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia al consultar la existencia del administrador,
    /// el servicio retorne un error inesperado y no continúe con las demás operaciones del repositorio.
    /// </summary>
    [TestMethod]
    public async Task EliminarAsync_CuandoFallaConsultaExistencia_RetornaUnexpected()
    {
        // Arrange
        int id = 5;

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ThrowsAsync(new DataAccessException(
                "Error de persistencia.",
                new Exception("Error interno de prueba.")));

        // Act
        var resultado = await _service.EliminarAsync(id);

        // Assert
        AssertResultadoError(resultado, ErrorType.Unexpected, "No se pudo eliminar el administrador por un error de persistencia.");

        _adminRepositoryMock.Verify(
            r => r.EsSuperAdminAsync(It.IsAny<int>()),
            Times.Never);

        _adminRepositoryMock.Verify(
            r => r.EliminarAsync(It.IsAny<int>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que el servicio impida eliminar al superadministrador,
    /// retornando un error de tipo <see cref="ErrorType.Conflict"/>
    /// sin ejecutar la operación de eliminación en el repositorio.
    /// </summary>
    [TestMethod]
    public async Task EliminarAsync_CuandoEsSuperAdmin_RetornaConflict()
    {
        // Arrange
        int id = 1;

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.EliminarAsync(id);

        // Assert
        AssertResultadoError(resultado, ErrorType.Conflict, "El superadministrador no puede ser eliminado.");

        _adminRepositoryMock.Verify(
            r => r.EliminarAsync(It.IsAny<int>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia al consultar si el administrador
    /// es superadministrador, el servicio retorne un error inesperado y no intente eliminarlo.
    /// </summary>
    [TestMethod]
    public async Task EliminarAsync_CuandoFallaConsultaSuperAdmin_RetornaUnexpected()
    {
        // Arrange
        int id = 5;

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ThrowsAsync(new DataAccessException(
                "Error de persistencia.",
                new Exception("Error interno de prueba.")));

        // Act
        var resultado = await _service.EliminarAsync(id);

        // Assert
        AssertResultadoError(resultado, ErrorType.Unexpected, "No se pudo eliminar el administrador por un error de persistencia.");

        _adminRepositoryMock.Verify(
            r => r.EliminarAsync(It.IsAny<int>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando el administrador existe y no es superadministrador,
    /// el servicio realice la eliminación correctamente y confirme las llamadas esperadas al repositorio.
    /// </summary>
    [TestMethod]
    public async Task EliminarAsync_CuandoAdministradorExisteYNoEsSuperAdmin_EliminaAdministrador()
    {
        // Arrange
        int id = 5;

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.EliminarAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.EliminarAsync(id);

        // Assert
        AssertResultadoExitoso(resultado);

        _adminRepositoryMock.Verify(
            r => r.ExistePorIdAsync(id),
            Times.Once);

        _adminRepositoryMock.Verify(
            r => r.EsSuperAdminAsync(id),
            Times.Once);

        _adminRepositoryMock.Verify(
            r => r.EliminarAsync(id),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia durante la eliminación del administrador,
    /// el servicio retorne un error inesperado y confirme que la operación de eliminación fue intentada.
    /// </summary>
    [TestMethod]
    public async Task EliminarAsync_CuandoFallaEliminacionEnRepositorio_RetornaUnexpected()
    {
        // Arrange
        int id = 5;

        _adminRepositoryMock
            .Setup(r => r.ExistePorIdAsync(id))
            .ReturnsAsync(true);

        _adminRepositoryMock
            .Setup(r => r.EsSuperAdminAsync(id))
            .ReturnsAsync(false);

        _adminRepositoryMock
            .Setup(r => r.EliminarAsync(id))
            .ThrowsAsync(new DataAccessException(
                "Error de persistencia.",
                new Exception("Error interno de prueba.")));

        // Act
        var resultado = await _service.EliminarAsync(id);

        // Assert
        AssertResultadoError(resultado, ErrorType.Unexpected, "No se pudo eliminar el administrador por un error de persistencia.");

        _adminRepositoryMock.Verify(
            r => r.EliminarAsync(id),
            Times.Once);
    }
    #endregion

    #region Test de login

    /// <summary>
    /// Verifica que, cuando el nombre de usuario está vacío o contiene únicamente espacios,
    /// el servicio retorne un error de validación y no intente realizar el inicio de sesión en el repositorio.
    /// </summary>
    [TestMethod]
    public async Task LoguearAsync_UsuarioVacio_RetornaErrorDeValidacion()
    {
        // Arrange
        var usuario = " ";
        var contrasena = "Clave123";

        // Act
        var resultado = await _service.LoguearAsync(usuario, contrasena);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(
            "El nombre de usuario es obligatorio.",
            resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(
            r => r.LoguearAsync(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando la contraseña está vacía,
    /// el servicio retorne un error de validación y no intente realizar el inicio de sesión en el repositorio.
    /// </summary>
    [TestMethod]
    public async Task LoguearAsync_ContrasenaVacia_RetornaErrorDeValidacion()
    {
        // Arrange
        var usuario = "juanperez";
        var contrasena = "";

        // Act
        var resultado = await _service.LoguearAsync(usuario, contrasena);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(
            "La contraseña es obligatoria.",
            resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(
            r => r.LoguearAsync(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica que, cuando las credenciales son correctas,
    /// el servicio retorne un resultado exitoso con el mensaje devuelto por el repositorio
    /// y confirme que la operación de inicio de sesión se ejecutó una vez.
    /// </summary>
    [TestMethod]
    public async Task LoguearAsync_CredencialesCorrectas_RetornaResultadoExitoso()
    {
        // Arrange
        var usuario = "juanperez";
        var contrasena = "Clave123";
        var mensaje = "Logueo exitoso";

        _adminRepositoryMock
            .Setup(r => r.LoguearAsync(usuario, contrasena))
            .ReturnsAsync(mensaje);

        // Act
        var resultado = await _service.LoguearAsync(usuario, contrasena);

        // Assert
        Assert.IsTrue(resultado.Success);
        Assert.AreEqual(mensaje, resultado.Data);
        Assert.IsNull(resultado.Error);
        Assert.AreEqual(ErrorType.None, resultado.ErrorType);

        _adminRepositoryMock.Verify(
            r => r.LoguearAsync(usuario, contrasena),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, cuando el repositorio indica que las credenciales son incorrectas,
    /// el servicio retorne un error de tipo <see cref="ErrorType.Unauthorized"/>
    /// con el mensaje correspondiente.
    /// </summary>
    [TestMethod]
    public async Task LoguearAsync_CredencialesIncorrectas_RetornaErrorNoAutorizado()
    {
        // Arrange
        var usuario = "juanperez";
        var contrasena = "Incorrecta";
        var mensaje = "Usuario o contraseña inválidos";

        _adminRepositoryMock
            .Setup(r => r.LoguearAsync(usuario, contrasena))
            .ReturnsAsync(mensaje);

        // Act
        var resultado = await _service.LoguearAsync(usuario, contrasena);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(
            "Nombre de usuario o contraseña incorrectos.",
            resultado.Error);
        Assert.AreEqual(ErrorType.Unauthorized, resultado.ErrorType);
    }

    /// <summary>
    /// Verifica que, cuando ocurre un error de persistencia durante el inicio de sesión,
    /// el servicio retorne un error inesperado con el mensaje correspondiente.
    /// </summary>
    [TestMethod]
    public async Task LoguearAsync_ErrorDePersistencia_RetornaErrorInesperado()
    {
        // Arrange
        var usuario = "juanperez";
        var contrasena = "Clave123";

        _adminRepositoryMock
            .Setup(r => r.LoguearAsync(usuario, contrasena))
            .ThrowsAsync(new DataAccessException("Error de persistencia", new Exception()));

        // Act
        var resultado = await _service.LoguearAsync(usuario, contrasena);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(
            "No se pudo verificar el acceso por un error de persistencia.",
            resultado.Error);
        Assert.AreEqual(ErrorType.Unexpected, resultado.ErrorType);
    }

    /// <summary>
    /// Verifica que, cuando el nombre de usuario contiene espacios al inicio o al final,
    /// el servicio recorte esos espacios antes de enviar el usuario al repositorio
    /// y procese correctamente el inicio de sesión.
    /// </summary>
    [TestMethod]
    public async Task LoguearAsync_UsuarioConEspacios_EnviaUsuarioRecortadoAlRepositorio()
    {
        // Arrange
        var usuario = "  juanperez  ";
        var contrasena = "Clave123";
        var usuarioEsperado = "juanperez";
        var mensaje = "Logueo exitoso";

        _adminRepositoryMock
            .Setup(r => r.LoguearAsync(usuarioEsperado, contrasena))
            .ReturnsAsync(mensaje);

        // Act
        var resultado = await _service.LoguearAsync(usuario, contrasena);

        // Assert
        Assert.IsTrue(resultado.Success);

        _adminRepositoryMock.Verify(
            r => r.LoguearAsync(usuarioEsperado, contrasena),
            Times.Once);
    }
    #endregion

}
