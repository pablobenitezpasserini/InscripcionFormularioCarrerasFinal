using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Interfaces.Repositories;
using Administradores.BR.Interfaces.Validators;
using Administradores.BR.Services;
using Administradores.Models.Exceptions;
using Administradores.Models.Formularios.DTOs;
using Administradores.Models.Formularios.Models;
using Administradores.Models.Results;
using Moq;

namespace AdministradoresTest.BR
{
    [TestClass]
    public class HabilitacionFormularioServiceTest
    {
        private Mock<IHabilitacionFormularioRepository> _repositoryMock = null!;
        private Mock<IHabilitacionFormularioValidator> _validadorMock = null!;
        private HabilitacionFormularioService _service = null!;

        #region Helpers
        [TestInitialize]
        public void Inicializar()
        {
            _repositoryMock = new Mock<IHabilitacionFormularioRepository>();
            _validadorMock = new Mock<IHabilitacionFormularioValidator>();

            _service = new HabilitacionFormularioService(
                _repositoryMock.Object,
                _validadorMock.Object);
        }

        private static CrearHabilitacionFormularioDto CrearDto()
        {
            return new CrearHabilitacionFormularioDto
            {
                Hab_Form_Año = 2026,
                Hab_Form_Fecha_Inicio = new DateTime(2026, 11, 1),
                Hab_Form_Fecha_Cierre = new DateTime(2026, 12, 31)
            };
        }
        private static EditarHabilitacionFormularioDto CrearEditarDto()
        {
            return new EditarHabilitacionFormularioDto
            {
                Hab_Form_Fecha_Inicio = new DateTime(2026, 11, 1),
                Hab_Form_Fecha_Cierre = new DateTime(2026, 12, 31),
                Hab_Form_Estado = "DESHABILITADO"
            };
        }
        #endregion
        #region Listar
        [TestMethod]
        public async Task GetAllAsync_CuandoExistenFormularios_RetornaResultadoExitoso()
        {
            // Arrange
            var formularios = new List<HabilitacionFormulario> { new HabilitacionFormulario(), new HabilitacionFormulario() };

            _repositoryMock
                .Setup(r => r.ListarAsync())
                .ReturnsAsync(formularios);

            // Act
            var resultado = await _service.GetAllAsync();

            // Assert
            Assert.IsTrue(resultado.Success);
            Assert.AreSame(formularios, resultado.Data);

            _repositoryMock.Verify(
                r => r.ListarAsync(),
                Times.Once);
        }

        [TestMethod]
        public async Task GetAllAsync_CuandoOcurreErrorDePersistencia_RetornaError()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.ListarAsync())
                .ThrowsAsync(new DataAccessException("Error de persistencia", new Exception("Error al obtener los formularios.")));

            // Act
            var resultado = await _service.GetAllAsync();

            // Assert
            Assert.IsFalse(resultado.Success);
            Assert.AreEqual("No se pudieron obtener los formularios por un error de persistencia.", resultado.Error);
            Assert.AreEqual(ErrorType.Unexpected, resultado.ErrorType);

            _repositoryMock.Verify(
                r => r.ListarAsync(),
                Times.Once);
        }
        #endregion
        #region Crear

        [TestMethod]
        [DataRow("El año del formulario no es válido.")]
        [DataRow("La fecha de inicio es obligatoria.")]
        [DataRow("La fecha de cierre es obligatoria.")]
        public async Task CrearAsync_CuandoFallaLaValidacion_RetornaErrorDeValidacion(string mensajeError)
        {
            // Arrange
            var data = CrearDto();

            _validadorMock
                .Setup(v => v.ValidarCrear(data))
                .Returns(mensajeError);

            // Act
            var resultado = await _service.CrearAsync(data);

            // Assert
            Assert.IsFalse(resultado.Success);
            Assert.AreEqual(mensajeError, resultado.Error);
            Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

            _repositoryMock.Verify(
                r => r.ListarAsync(),
                Times.Never);

            _repositoryMock.Verify(
                r => r.CrearAsync(It.IsAny<HabilitacionFormulario>()),
                Times.Never);
        }

        [TestMethod]
        [DataRow(false, "Ya existe un formulario para el año ingresado.")]
        [DataRow(true, "No pueden existir más de dos formularios.")]
        public async Task CrearAsync_CuandoExisteUnConflicto_RetornaError(bool cantidadMaxima, string mensajeError)
        {
            // Arrange
            var data = CrearDto();

            var formularios = new List<HabilitacionFormulario>();

            if (cantidadMaxima)
            {
                formularios.Add(new HabilitacionFormulario { Hab_Form_Año = 2024 });
                formularios.Add(new HabilitacionFormulario { Hab_Form_Año = 2025 });
            }
            else
            {
                formularios.Add(new HabilitacionFormulario
                {
                    Hab_Form_Año = data.Hab_Form_Año
                });
            }

            _validadorMock
                .Setup(v => v.ValidarCrear(data))
                .Returns((string?)null);

            _repositoryMock
                .Setup(r => r.ListarAsync())
                .ReturnsAsync(formularios);

            // Act
            var resultado = await _service.CrearAsync(data);

            // Assert
            Assert.IsFalse(resultado.Success);
            Assert.AreEqual(mensajeError, resultado.Error);
            Assert.AreEqual(ErrorType.Conflict, resultado.ErrorType);

            _repositoryMock.Verify(
                r => r.CrearAsync(It.IsAny<HabilitacionFormulario>()),
                Times.Never);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task CrearAsync_CuandoNoExistenConflictos_CreaFormularioExitosamente(bool existeOtroFormulario)
        {
            // Arrange
            var data = CrearDto();

            var formularios = new List<HabilitacionFormulario>();

            if (existeOtroFormulario)
            {
                formularios.Add(new HabilitacionFormulario
                {
                    Hab_Form_Año = 2025
                });
            }

            _validadorMock
                .Setup(v => v.ValidarCrear(data))
                .Returns((string?)null);

            _repositoryMock
                .Setup(r => r.ListarAsync())
                .ReturnsAsync(formularios);

            _repositoryMock
                .Setup(r => r.CrearAsync(It.IsAny<HabilitacionFormulario>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.CrearAsync(data);

            // Assert
            Assert.IsTrue(resultado.Success);
            Assert.IsTrue(resultado.Data);

            _repositoryMock.Verify(
                r => r.CrearAsync(It.Is<HabilitacionFormulario>(f =>
                    f.Hab_Form_Año == data.Hab_Form_Año &&
                    f.Hab_Form_Fecha_Inicio == data.Hab_Form_Fecha_Inicio &&
                    f.Hab_Form_Fecha_Cierre == data.Hab_Form_Fecha_Cierre &&
                    f.Hab_Form_Estado == "DESHABILITADO")),
                Times.Once);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task CrearAsync_CuandoOcurreErrorDePersistencia_RetornaError(bool errorAlListar)
        {
            // Arrange
            var data = CrearDto();

            _validadorMock
                .Setup(v => v.ValidarCrear(data))
                .Returns((string?)null);

            _repositoryMock
                .Setup(r => r.ListarAsync())
                .ReturnsAsync(new List<HabilitacionFormulario>());

            if (errorAlListar)
            {
                _repositoryMock
                    .Setup(r => r.ListarAsync())
                    .ThrowsAsync(new DataAccessException(
                        "Error de persistencia",
                        new Exception("Error al obtener los formularios.")));
            }
            else
            {
                _repositoryMock
                    .Setup(r => r.CrearAsync(It.IsAny<HabilitacionFormulario>()))
                    .ThrowsAsync(new DataAccessException(
                        "Error de persistencia",
                        new Exception("Error al crear el formulario.")));
            }

            // Act
            var resultado = await _service.CrearAsync(data);

            // Assert
            Assert.IsFalse(resultado.Success);
            Assert.AreEqual("No se pudo crear el formulario por un error de persistencia.", resultado.Error);
            Assert.AreEqual(ErrorType.Unexpected, resultado.ErrorType);

            _repositoryMock.Verify(
                r => r.CrearAsync(It.IsAny<HabilitacionFormulario>()),
                errorAlListar ? Times.Never : Times.Once);
        }
        #endregion
        #region Editar
        [TestMethod]
        public async Task EditarAsync_CuandoElIdNoEsValido_RetornaErrorDeValidacion()
        {
            // Arrange
            var data = CrearEditarDto();

            // Act
            var resultado = await _service.EditarAsync(0, data);

            // Assert
            Assert.IsFalse(resultado.Success);
            Assert.AreEqual("El ID del formulario no es válido.", resultado.Error);
            Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

            _validadorMock.Verify(
                v => v.ValidarEditar(It.IsAny<EditarHabilitacionFormularioDto>()),
                Times.Never);

            _repositoryMock.Verify(
                r => r.ListarAsync(),
                Times.Never);
        }

        [TestMethod]
        [DataRow("La fecha de inicio es obligatoria.")]
        [DataRow("La fecha de cierre es obligatoria.")]
        [DataRow("El estado del formulario no es válido.")]
        public async Task EditarAsync_CuandoFallaLaValidacion_RetornaErrorDeValidacion(string mensajeError)
        {
            // Arrange
            var data = CrearEditarDto();

            _validadorMock
                .Setup(v => v.ValidarEditar(data))
                .Returns(mensajeError);

            // Act
            var resultado = await _service.EditarAsync(1, data);

            // Assert
            Assert.IsFalse(resultado.Success);
            Assert.AreEqual(mensajeError, resultado.Error);
            Assert.AreEqual(ErrorType.Validation, resultado.ErrorType);

            _repositoryMock.Verify(
                r => r.ListarAsync(),
                Times.Never);
        }

        [TestMethod]
        public async Task EditarAsync_CuandoNoExisteElFormulario_RetornaNotFound()
        {
            // Arrange
            var data = CrearEditarDto();

            _validadorMock
                .Setup(v => v.ValidarEditar(data))
                .Returns((string?)null);

            _repositoryMock
                .Setup(r => r.ListarAsync())
                .ReturnsAsync(new List<HabilitacionFormulario>());

            // Act
            var resultado = await _service.EditarAsync(1, data);

            // Assert
            Assert.IsFalse(resultado.Success);
            Assert.AreEqual("No existe un formulario con el ID indicado.", resultado.Error);
            Assert.AreEqual(ErrorType.NotFound, resultado.ErrorType);

            _repositoryMock.Verify(
                r => r.EditarAsync(It.IsAny<HabilitacionFormulario>()),
                Times.Never);
        }

        [TestMethod]
        public async Task EditarAsync_CuandoExisteOtroFormularioHabilitado_RetornaConflict()
        {
            // Arrange
            var data = CrearEditarDto();
            data.Hab_Form_Estado = "HABILITADO";

            var formularios = new List<HabilitacionFormulario>
                {
                    new HabilitacionFormulario
                    {
                        ID_Hab_Form = 1,
                        Hab_Form_Año = 2025,
                        Hab_Form_Estado = "DESHABILITADO"
                    },
                    new HabilitacionFormulario
                    {
                        ID_Hab_Form = 2,
                        Hab_Form_Año = 2026,
                        Hab_Form_Estado = "HABILITADO"
                    }
                };

            _validadorMock
                .Setup(v => v.ValidarEditar(data))
                .Returns((string?)null);

            _repositoryMock
                .Setup(r => r.ListarAsync())
                .ReturnsAsync(formularios);

            // Act
            var resultado = await _service.EditarAsync(1, data);

            // Assert
            Assert.IsFalse(resultado.Success);
            Assert.AreEqual(
                "Ya existe otro formulario habilitado.",
                resultado.Error);
            Assert.AreEqual(ErrorType.Conflict, resultado.ErrorType);

            _repositoryMock.Verify(
                r => r.EditarAsync(It.IsAny<HabilitacionFormulario>()),
                Times.Never);
        }

        [TestMethod]
        [DataRow("HABILITADO")]
        [DataRow("DESHABILITADO")]
        public async Task EditarAsync_CuandoNoExistenConflictos_EditaFormularioExitosamente(string estado)
        {
            // Arrange
            var data = CrearEditarDto();
            data.Hab_Form_Estado = estado;

            var formularios = new List<HabilitacionFormulario>
                {
                    new HabilitacionFormulario
                    {
                        ID_Hab_Form = 1,
                        Hab_Form_Año = 2026,
                        Hab_Form_Estado = "DESHABILITADO"
                    }
                };

            _validadorMock
                .Setup(v => v.ValidarEditar(data))
                .Returns((string?)null);

            _repositoryMock
                .Setup(r => r.ListarAsync())
                .ReturnsAsync(formularios);

            _repositoryMock
                .Setup(r => r.EditarAsync(It.IsAny<HabilitacionFormulario>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.EditarAsync(1, data);

            // Assert
            Assert.IsTrue(resultado.Success);
            Assert.IsTrue(resultado.Data);

            _repositoryMock.Verify(
                r => r.EditarAsync(It.Is<HabilitacionFormulario>(f =>
                    f.ID_Hab_Form == 1 &&
                    f.Hab_Form_Año == 2026 &&
                    f.Hab_Form_Fecha_Inicio == data.Hab_Form_Fecha_Inicio &&
                    f.Hab_Form_Fecha_Cierre == data.Hab_Form_Fecha_Cierre &&
                    f.Hab_Form_Estado == estado)),
                Times.Once);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task EditarAsync_CuandoOcurreErrorDePersistencia_RetornaError(bool errorAlListar)
        {
            // Arrange
            var data = CrearEditarDto();

            _validadorMock
                .Setup(v => v.ValidarEditar(data))
                .Returns((string?)null);

            _repositoryMock
                .Setup(r => r.ListarAsync())
                .ReturnsAsync(new List<HabilitacionFormulario>
                {
            new HabilitacionFormulario
            {
                ID_Hab_Form = 1,
                Hab_Form_Año = 2026,
                Hab_Form_Estado = "DESHABILITADO"
            }
                });

            if (errorAlListar)
            {
                _repositoryMock
                    .Setup(r => r.ListarAsync())
                    .ThrowsAsync(new DataAccessException(
                        "Error de persistencia",
                        new Exception("Error al obtener los formularios.")));
            }
            else
            {
                _repositoryMock
                    .Setup(r => r.EditarAsync(It.IsAny<HabilitacionFormulario>()))
                    .ThrowsAsync(new DataAccessException(
                        "Error de persistencia",
                        new Exception("Error al modificar el formulario.")));
            }

            // Act
            var resultado = await _service.EditarAsync(1, data);

            // Assert
            Assert.IsFalse(resultado.Success);
            Assert.AreEqual(
                "No se pudo modificar el formulario por un error de persistencia.",
                resultado.Error);
            Assert.AreEqual(ErrorType.Unexpected, resultado.ErrorType);

            _repositoryMock.Verify(
                r => r.EditarAsync(It.IsAny<HabilitacionFormulario>()),
                errorAlListar ? Times.Never : Times.Once);
        }
        #endregion 
    }
}