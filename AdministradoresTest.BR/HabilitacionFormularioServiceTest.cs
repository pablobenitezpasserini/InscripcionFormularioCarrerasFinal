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
    /// <summary>
    /// Contiene las pruebas unitarias de la clase <see cref="HabilitacionFormularioService"/>.
    /// </summary>
    [TestClass]
    public class HabilitacionFormularioServiceTest
    {
        /// <summary>
        /// Simulación del repositorio utilizada para verificar las operaciones de acceso a datos.
        /// </summary>
        private Mock<IHabilitacionFormularioRepository> _repositoryMock = null!;
        /// <summary>
        /// Simulación del validador utilizada para verificar las validaciones de los datos.
        /// </summary>
        private Mock<IHabilitacionFormularioValidator> _validadorMock = null!;
        /// <summary>
        /// Instancia del servicio que se utiliza para ejecutar las pruebas unitarias.
        /// </summary>
        private HabilitacionFormularioService _service = null!;

        #region Helpers
        /// <summary>
        /// Inicializa los mocks de las dependencias y crea una nueva instancia del servicio
        /// antes de la ejecución de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _repositoryMock = new Mock<IHabilitacionFormularioRepository>();
            _validadorMock = new Mock<IHabilitacionFormularioValidator>();

            _service = new HabilitacionFormularioService(
                _repositoryMock.Object,
                _validadorMock.Object);
        }

        /// <summary>
        /// Crea un objeto de transferencia de datos con valores predefinidos para las pruebas
        /// de creación de una habilitación de formulario.
        /// </summary>
        /// <returns>Una instancia de <see cref="CrearHabilitacionFormularioDto"/> con datos de prueba.</returns>
        private static CrearHabilitacionFormularioDto CrearDto()
        {
            return new CrearHabilitacionFormularioDto
            {
                Hab_Form_Año = 2026,
                Hab_Form_Fecha_Inicio = new DateTime(2026, 11, 1),
                Hab_Form_Fecha_Cierre = new DateTime(2026, 12, 31)
            };
        }

        /// <summary>
        /// Crea un objeto de transferencia de datos con valores predefinidos para las pruebas
        /// de edición de una habilitación de formulario.
        /// </summary>
        /// <returns>Una instancia de <see cref="EditarHabilitacionFormularioDto"/> con datos de prueba.</returns>
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

        /// <summary>
        /// Verifica que el servicio retorne un resultado exitoso con la lista de formularios
        /// cuando existen registros disponibles en el repositorio.
        /// </summary>
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

        /// <summary>
        /// Verifica que el servicio retorne un error de tipo inesperado cuando ocurre
        /// una excepción de acceso a datos al intentar obtener los formularios.
        /// </summary>
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

        /// <summary>
        /// Verifica que el servicio retorne un error de validación cuando los datos proporcionados
        /// no cumplen con alguna de las reglas establecidas para crear una habilitación de formulario.
        /// </summary>
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

        /// <summary>
        /// Verifica que el servicio retorne un error de conflicto cuando ya existe un formulario
        /// para el mismo año o se ha alcanzado la cantidad máxima de formularios permitidos.
        /// </summary>
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

        /// <summary>
        /// Verifica que el servicio cree correctamente una habilitación de formulario cuando no existen
        /// conflictos, tanto si no hay formularios registrados como si ya existe uno para otro año.
        /// También comprueba que la entidad creada contenga los datos esperados y comience deshabilitada.
        /// </summary>
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

        /// <summary>
        /// Verifica que el servicio retorne un error inesperado cuando ocurre una excepción de acceso
        /// a datos al consultar los formularios existentes o al intentar crear uno nuevo.
        /// </summary>
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
        /// <summary>
        /// Verifica que, al intentar editar un formulario con un ID igual o menor a cero,
        /// se retorne un error de validación y no se ejecuten las validaciones del DTO
        /// ni las consultas al repositorio.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando los datos del DTO no superan la validación,
        /// se retorne un error de validación con el mensaje correspondiente.
        /// Contempla fechas de inicio o cierre obligatorias y un estado inválido.
        /// </summary>
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

        /// <summary>
        /// Verifica que, al intentar editar un formulario cuyo ID no existe,
        /// se retorne un error de tipo NotFound y no se intente modificar ningún formulario.
        /// </summary>
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

        /// <summary>
        /// Verifica que, al habilitar un formulario cuando ya existe otro habilitado,
        /// se retorne un error de tipo Conflict y no se realice la modificación.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando no existen conflictos, el formulario se modifique
        /// correctamente con las fechas y el estado indicados en el DTO.
        /// Contempla tanto el estado HABILITADO como DESHABILITADO.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando ocurre un error de persistencia durante la edición,
        /// se retorne un error inesperado. Contempla fallos al listar los formularios
        /// y al ejecutar la modificación.
        /// </summary>
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