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

        private static CrearHabilitacionFormularioDto CrearDto()
        {
            return new CrearHabilitacionFormularioDto
            {
                Hab_Form_Año = 2026,
                Hab_Form_Fecha_Inicio = new DateTime(2026, 10, 1),
                Hab_Form_Fecha_Cierre = new DateTime(2026, 11, 1)
            };
        }

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