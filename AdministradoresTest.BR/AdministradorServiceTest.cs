using Administradores.BR.Interfaces.Repositories;
using Administradores.BR.Interfaces.Validators;
using Administradores.BR.Services;
using Administradores.Models.Administradores.DTOs;
using Administradores.Models.Administradores.Models;
using Administradores.Models.Exceptions;
using Administradores.Models.Results;
using Moq;

namespace AdministradoresTest.BR;

[TestClass]
public sealed class AdministradorServiceTest
{
    private Mock<IAdministradorValidator> _adminValidadorMock = null!;
    private Mock<IAdministradorRepository> _adminRepositoryMock = null!;
    private AdministradorService _service = null!;

    #region Helpers
    // Por lo que googlie con el decorador TestInitialize hace que el metodo
    // que lo tenga se ejecute siempre antes de cada test
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

    private static void AssertResultadoError(
        Result<bool> resultado,
        ErrorType tipoError,
        string mensaje)
    {
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(tipoError, resultado.ErrorType);
        Assert.AreEqual(mensaje, resultado.Error);
    }

    private static void AssertResultadoExitoso(
    Result<bool> resultado)
    {
        Assert.IsTrue(resultado.Success);
        Assert.IsTrue(resultado.Data);
    }
    #endregion
    #region Test de listar
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
}
