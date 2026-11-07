using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Validadores;
using Administradores.Models.Formularios.DTOs;

namespace AdministradoresTest.BR
{
    [TestClass]
    public class HabilitacionFormularioValidadorTest
    {
        private HabilitacionFormularioValidador _validador = null!;

        [TestInitialize]
        public void Inicializar()
        {
            _validador = new HabilitacionFormularioValidador();
        }

        #region ValidarCrear
        /// <summary>
        /// Verifica que la validación retorne un error cuando los datos recibidos son nulos.
        /// </summary>
        [TestMethod]
        public void ValidarCrear_DatosNulos_RetornaError()
        {
            // Arrange
            CrearHabilitacionFormularioDto? data = null;

            // Act
            var resultado = _validador.ValidarCrear(data);

            // Assert
            Assert.AreEqual(
                "Los datos del formulario son obligatorios.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación retorne un error cuando el año del formulario es menor o igual a cero.
        /// </summary>
        [TestMethod]
        public void ValidarCrear_AñoMenorOIgualACero_RetornaError()
        {
            // Arrange
            var data = CrearDto();
            data.Hab_Form_Año = 0;

            // Act
            var resultado = _validador.ValidarCrear(data);

            // Assert
            Assert.AreEqual(
                "El año del formulario no es válido.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación retorne un error cuando la fecha de inicio no está especificada.
        /// </summary>
        [TestMethod]
        public void ValidarCrear_FechaInicioDefault_RetornaError()
        {
            // Arrange
            var data = CrearDto();
            data.Hab_Form_Fecha_Inicio = default;

            // Act
            var resultado = _validador.ValidarCrear(data);

            // Assert
            Assert.AreEqual(
                "La fecha de inicio es obligatoria.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación retorne un error cuando la fecha de cierre no está especificada.
        /// </summary>
        [TestMethod]
        public void ValidarCrear_FechaCierreDefault_RetornaError()
        {
            // Arrange
            var data = CrearDto();
            data.Hab_Form_Fecha_Cierre = default;

            // Act
            var resultado = _validador.ValidarCrear(data);

            // Assert
            Assert.AreEqual(
                "La fecha de cierre es obligatoria.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación retorne un error cuando la fecha de inicio es posterior
        /// o igual a la fecha de cierre.
        /// </summary>
        [TestMethod]
        public void ValidarCrear_FechaInicioPosteriorOIgualAFechaCierre_RetornaError()
        {
            // Arrange
            var data = CrearDto();
            data.Hab_Form_Fecha_Inicio = new DateTime(2026, 10, 20);
            data.Hab_Form_Fecha_Cierre = new DateTime(2026, 10, 20);

            // Act
            var resultado = _validador.ValidarCrear(data);

            // Assert
            Assert.AreEqual(
                "La fecha de inicio debe ser anterior a la fecha de cierre.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación no retorne errores cuando todos los datos proporcionados
        /// para crear la habilitación del formulario son válidos.
        /// </summary>
        [TestMethod]
        public void ValidarCrear_DatosValidos_RetornaNull()
        {
            // Arrange
            var data = CrearDto();

            // Act
            var resultado = _validador.ValidarCrear(data);

            // Assert
            Assert.IsNull(resultado);
        }

        #endregion

        #region ValidarEditar
        /// <summary>
        /// Verifica que la validación retorne un error cuando los datos recibidos son nulos.
        /// </summary>
        [TestMethod]
        public void ValidarEditar_DatosNulos_RetornaError()
        {
            // Arrange
            EditarHabilitacionFormularioDto? data = null;

            // Act
            var resultado = _validador.ValidarEditar(data);

            // Assert
            Assert.AreEqual(
                "Los datos del formulario son obligatorios.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación retorne un error cuando la fecha de inicio no está especificada.
        /// </summary>
        [TestMethod]
        public void ValidarEditar_FechaInicioDefault_RetornaError()
        {
            // Arrange
            var data = EditarDto();
            data.Hab_Form_Fecha_Inicio = default;

            // Act
            var resultado = _validador.ValidarEditar(data);

            // Assert
            Assert.AreEqual(
                "La fecha de inicio es obligatoria.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación retorne un error cuando la fecha de cierre no está especificada.
        /// </summary>
        [TestMethod]
        public void ValidarEditar_FechaCierreDefault_RetornaError()
        {
            // Arrange
            var data = EditarDto();
            data.Hab_Form_Fecha_Cierre = default;

            // Act
            var resultado = _validador.ValidarEditar(data);

            // Assert
            Assert.AreEqual(
                "La fecha de cierre es obligatoria.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación retorne un error cuando la fecha de inicio es posterior
        /// o igual a la fecha de cierre.
        /// </summary>
        [TestMethod]
        public void ValidarEditar_FechaInicioPosteriorOIgualAFechaCierre_RetornaError()
        {
            // Arrange
            var data = EditarDto();
            data.Hab_Form_Fecha_Inicio = new DateTime(2026, 10, 20);
            data.Hab_Form_Fecha_Cierre = new DateTime(2026, 10, 20);

            // Act
            var resultado = _validador.ValidarEditar(data);

            // Assert
            Assert.AreEqual(
                "La fecha de inicio debe ser anterior a la fecha de cierre.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación retorne un error cuando el estado del formulario está vacío
        /// o contiene únicamente espacios en blanco.
        /// </summary>
        [TestMethod]
        public void ValidarEditar_EstadoVacio_RetornaError()
        {
            // Arrange
            var data = EditarDto();
            data.Hab_Form_Estado = " ";

            // Act
            var resultado = _validador.ValidarEditar(data);

            // Assert
            Assert.AreEqual(
                "El estado del formulario es obligatorio.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación retorne un error cuando el estado proporcionado
        /// no corresponde a ninguno de los valores permitidos.
        /// </summary>
        [TestMethod]
        public void ValidarEditar_EstadoInvalido_RetornaError()
        {
            // Arrange
            var data = EditarDto();
            data.Hab_Form_Estado = "PENDIENTE";

            // Act
            var resultado = _validador.ValidarEditar(data);

            // Assert
            Assert.AreEqual(
                "El estado debe ser HABILITADO o DESHABILITADO.",
                resultado);
        }
        /// <summary>
        /// Verifica que la validación no retorne errores cuando el estado del formulario es HABILITADO.
        /// </summary>
        [TestMethod]
        public void ValidarEditar_EstadoHabilitado_RetornaNull()
        {
            // Arrange
            var data = EditarDto();
            data.Hab_Form_Estado = "HABILITADO";

            // Act
            var resultado = _validador.ValidarEditar(data);

            // Assert
            Assert.IsNull(resultado);
        }
        /// <summary>
        /// Verifica que la validación no retorne errores cuando el estado del formulario es DESHABILITADO.
        /// </summary>
        [TestMethod]
        public void ValidarEditar_EstadoDeshabilitado_RetornaNull()
        {
            // Arrange
            var data = EditarDto();
            data.Hab_Form_Estado = "DESHABILITADO";

            // Act
            var resultado = _validador.ValidarEditar(data);

            // Assert
            Assert.IsNull(resultado);
        }
        /// <summary>
        /// Verifica que la validación acepte el estado HABILITADO cuando contiene espacios
        /// al inicio y al final y está escrito en minúsculas.
        /// </summary>
        [TestMethod]
        public void ValidarEditar_EstadoConEspaciosYMinusculas_RetornaNull()
        {
            // Arrange
            var data = EditarDto();
            data.Hab_Form_Estado = " habilitado ";

            // Act
            var resultado = _validador.ValidarEditar(data);

            // Assert
            Assert.IsNull(resultado);
        }
        /// <summary>
        /// Verifica que la validación no retorne errores cuando todos los datos proporcionados
        /// para editar la habilitación del formulario son válidos.
        /// </summary>
        [TestMethod]
        public void ValidarEditar_DatosValidos_RetornaNull()
        {
            // Arrange
            var data = EditarDto();

            // Act
            var resultado = _validador.ValidarEditar(data);

            // Assert
            Assert.IsNull(resultado);
        }

        #endregion

        #region Datos de prueba
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
                Hab_Form_Fecha_Inicio = new DateTime(2026, 10, 1),
                Hab_Form_Fecha_Cierre = new DateTime(2026, 11, 1)
            };
        }
        /// <summary>
        /// Crea un objeto de transferencia de datos con valores predefinidos para las pruebas
        /// de edición de una habilitación de formulario.
        /// </summary>
        /// <returns>Una instancia de <see cref="EditarHabilitacionFormularioDto"/> con datos de prueba.</returns>
        private static EditarHabilitacionFormularioDto EditarDto()
        {
            return new EditarHabilitacionFormularioDto
            {
                Hab_Form_Fecha_Inicio = new DateTime(2026, 10, 1),
                Hab_Form_Fecha_Cierre = new DateTime(2026, 11, 1),
                Hab_Form_Estado = "HABILITADO"
            };
        }

        #endregion
    }
}