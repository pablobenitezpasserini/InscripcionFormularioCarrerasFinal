using Administradores.BR.Interfaces.Repositories;
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
    private Mock<IAdministradorRepository> _adminRepositoryMock = null!;
    private AdministradorService _service = null!;

    // Por lo que googlie con el decorador TestInitialize hace que el metodo
    // que lo tenga se ejecute siempre antes de cada test
    [TestInitialize]
    public void Inicializar()
    {
        _adminRepositoryMock = new Mock<IAdministradorRepository>();

        _service = new AdministradorService(
            _adminRepositoryMock.Object
        );
    }

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
    private static CrearAdminDto CrearAdminValido()
    {
        return new CrearAdminDto
        {
            Admin_Nom_Ape = "Juan Perez",
            Admin_DNI = "12345678",
            Admin_Tipo_DNI = "DNI",
            Admin_Nom_Usuario = "juanperez",
            Admin_Contra = "ClaveSegura123"
        };
    }

    [TestMethod]
    public async Task CrearAsync_CuandoDtoEsNulo_RetornaValidation()
    {
        // Arrange
        CrearAdminDto admin = null!;
        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.IsFalse(resultado.Data);
        Assert.AreEqual("Los datos del administrador son obligatorios.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoNombreEstaVacio_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_Nom_Ape = " ";

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual("El nombre y apellido no pueden estar vacíos.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoDniEstaVacio_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_DNI = " ";

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(
            "El DNI no puede estar vacío.",
            resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoTipoDniEstaVacio_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_Tipo_DNI = " ";

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual("El tipo de DNI no puede estar vacío.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoUsuarioEstaVacio_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_Nom_Usuario = " ";

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual("El nombre de usuario no puede estar vacío.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoContrasenaEstaVacia_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_Contra = " ";

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual("La contraseña no puede estar vacía.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoDniContieneLetras_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_DNI = "1234A678";

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual("El DNI solo puede contener números.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoNombreSupera100Caracteres_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_Nom_Ape = new string('A', 101);

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual("El nombre y apellido no pueden superar los 100 caracteres.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoDniSupera8Caracteres_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_DNI = "123456789";

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual("El DNI no puede superar los 8 caracteres.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoTipoDniSupera40Caracteres_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_Tipo_DNI = new string('A', 41);

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual("El tipo de DNI no puede superar los 40 caracteres.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoUsuarioSupera30Caracteres_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_Nom_Usuario = new string('A', 31);

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual("El nombre de usuario no puede superar los 30 caracteres.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoContrasenaSupera255Caracteres_RetornaValidation()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_Contra = new string('A', 256);

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual("La contraseña no puede superar los 255 caracteres.", resultado.Error);
        Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

        _adminRepositoryMock.Verify(repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.ExisteUsuarioAsync(It.IsAny<string>()), Times.Never);

        _adminRepositoryMock.Verify(repo => repo.CrearAsync(It.IsAny<Administrador>()), Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoUsuarioEsAdmin_RetornaConflict()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_Nom_Usuario = "admin";

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(
            "El nombre de usuario \"admin\" está reservado para el super administrador.",
            resultado.Error);
        Assert.AreEqual(ErrorType.Conflict, resultado.ErrorType);

        _adminRepositoryMock.Verify(
            repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        _adminRepositoryMock.Verify(
            repo => repo.ExisteUsuarioAsync(It.IsAny<string>()),
            Times.Never);
        _adminRepositoryMock.Verify(
            repo => repo.CrearAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoDniEstaReservado_RetornaConflict()
    {
        // Arrange
        var admin = CrearAdminValido();
        admin.Admin_DNI = "00000000";
        admin.Admin_Tipo_DNI = "DNI";

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(
            "El DNI 00000000 está reservado para el super administrador.",
            resultado.Error);
        Assert.AreEqual(ErrorType.Conflict, resultado.ErrorType);

        _adminRepositoryMock.Verify(
            repo => repo.ExisteDniAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        _adminRepositoryMock.Verify(
            repo => repo.ExisteUsuarioAsync(It.IsAny<string>()),
            Times.Never);
        _adminRepositoryMock.Verify(
            repo => repo.CrearAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoDniYaExiste_RetornaConflict()
    {
        // Arrange
        var admin = CrearAdminValido();

        _adminRepositoryMock
            .Setup(repo => repo.ExisteDniAsync("12345678", "DNI"))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(
            "Ya existe un administrador con ese DNI y tipo de DNI.",
            resultado.Error);
        Assert.AreEqual(ErrorType.Conflict, resultado.ErrorType);

        _adminRepositoryMock.Verify(
            repo => repo.ExisteDniAsync("12345678", "DNI"),
            Times.Once);
        _adminRepositoryMock.Verify(
            repo => repo.ExisteUsuarioAsync(It.IsAny<string>()),
            Times.Never);
        _adminRepositoryMock.Verify(
            repo => repo.CrearAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoUsuarioYaExiste_RetornaConflict()
    {
        // Arrange
        var admin = CrearAdminValido();

        _adminRepositoryMock
            .Setup(repo => repo.ExisteDniAsync("12345678", "DNI"))
            .ReturnsAsync(false);
        _adminRepositoryMock
            .Setup(repo => repo.ExisteUsuarioAsync("juanperez"))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsFalse(resultado.Success);
        Assert.AreEqual(
            "El nombre de usuario ingresado ya existe.",
            resultado.Error);
        Assert.AreEqual(ErrorType.Conflict, resultado.ErrorType);

        _adminRepositoryMock.Verify(
            repo => repo.ExisteDniAsync("12345678", "DNI"),
            Times.Once);
        _adminRepositoryMock.Verify(
            repo => repo.ExisteUsuarioAsync("juanperez"),
            Times.Once);
        _adminRepositoryMock.Verify(
            repo => repo.CrearAsync(It.IsAny<Administrador>()),
            Times.Never);
    }

    [TestMethod]
    public async Task CrearAsync_CuandoDatosSonValidos_CreaAdministrador()
    {
        // Arrange
        var admin = CrearAdminValido();

        _adminRepositoryMock
            .Setup(repo => repo.ExisteDniAsync("12345678", "DNI"))
            .ReturnsAsync(false);
        _adminRepositoryMock
            .Setup(repo => repo.ExisteUsuarioAsync("juanperez"))
            .ReturnsAsync(false);
        _adminRepositoryMock
            .Setup(repo => repo.CrearAsync(It.IsAny<Administrador>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.CrearAsync(admin);

        // Assert
        Assert.IsTrue(resultado.Success);
        Assert.IsTrue(resultado.Data);
        Assert.IsNull(resultado.Error);
        Assert.AreEqual(ErrorType.None, resultado.ErrorType);

        _adminRepositoryMock.Verify(
            repo => repo.ExisteDniAsync("12345678", "DNI"),
            Times.Once);
        _adminRepositoryMock.Verify(
            repo => repo.ExisteUsuarioAsync("juanperez"),
            Times.Once);
        _adminRepositoryMock.Verify(
            repo => repo.CrearAsync(It.Is<Administrador>(a =>
                a.Admin_Nom_Ape == "Juan Perez" &&
                a.Admin_DNI == "12345678" &&
                a.Admin_Tipo_DNI == "DNI" &&
                a.Admin_Nom_Usuario == "juanperez" &&
                a.Admin_Contra == "ClaveSegura123")),
            Times.Once);
    }

    #endregion
}
