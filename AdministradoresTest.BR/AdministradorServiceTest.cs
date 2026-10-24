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
    public async Task CrearAsync_CuandoUsuarioEstaReservado_RetornaConflict(
    string usuario)
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
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(ErrorType.Conflict, resultado.ErrorType);
        Assert.AreEqual(
            "El nombre de usuario \"admin\" está reservado para el super administrador.",
            resultado.Error);

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
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(ErrorType.Conflict, resultado.ErrorType);
        Assert.AreEqual(
            "El DNI 00000000 está reservado para el super administrador.",
            resultado.Error);

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
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(ErrorType.Conflict, resultado.ErrorType);
        Assert.AreEqual(
            "Ya existe un administrador con ese DNI y tipo de DNI.",
            resultado.Error);

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
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(ErrorType.Conflict, resultado.ErrorType);
        Assert.AreEqual(
            "El nombre de usuario ingresado ya existe.",
            resultado.Error);

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
        Assert.IsTrue(resultado.Success);
        Assert.IsTrue(resultado.Data);

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
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(ErrorType.Unexpected, resultado.ErrorType);
        Assert.AreEqual(
            "No se pudo crear el administrador por un error de base de datos.",
            resultado.Error);

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
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(ErrorType.Unexpected, resultado.ErrorType);
        Assert.AreEqual(
            "No se pudo crear el administrador por un error de base de datos.",
            resultado.Error);

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
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(ErrorType.Unexpected, resultado.ErrorType);
        Assert.AreEqual(
            "No se pudo crear el administrador por un error de base de datos.",
            resultado.Error);

        _adminRepositoryMock.Verify(
            r => r.CrearAsync(It.IsAny<Administrador>()),
            Times.Once);
    }
    #endregion
}
