using Administradores.BR.Interfaces.Repositories;
using Administradores.BR.Services;
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
}
