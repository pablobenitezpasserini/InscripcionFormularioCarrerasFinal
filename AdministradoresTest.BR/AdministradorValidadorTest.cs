using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Validadores;
using Administradores.Models.Administradores.DTOs;

namespace AdministradoresTest.BR
{
    [TestClass]
    public class AdministradorValidadorTest
    {
        private AdministradorValidador _validador = null!;

        [TestInitialize]
        public void Inicializar()
        {
            _validador = new AdministradorValidador();
        }

        private static CrearAdminDto CrearAdminValido() => new()
        {
            Admin_Nom_Ape = "Juan Pérez",
            Admin_DNI = "12345678",
            Admin_Tipo_DNI = "DNI",
            Admin_Nom_Usuario = "juanperez",
            Admin_Contra = "ClaveSegura123"
        };

        private static EditarAdminDto EditarAdminValido() => new()
        {
            Admin_Nom_Ape = "Juan Pérez",
            Admin_DNI = "12345678",
            Admin_Tipo_DNI = "DNI",
            Admin_Nom_Usuario = "juanperez",
            Admin_Contra = "ClaveSegura123"
        };

        [TestMethod]
        public void ValidarCrear_AdminNulo_RetornaError()
        {
            // Arrange
            CrearAdminDto? admin = null;

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "Los datos del administrador son obligatorios.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_DatosValidos_RetornaNull()
        {
            // Arrange
            var admin = CrearAdminValido();

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValidarCrear_NombreVacio_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_Nom_Ape = " ";

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El nombre y apellido son obligatorios.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_DniVacio_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_DNI = "";

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El DNI es obligatorio.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_TipoDniVacio_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_Tipo_DNI = null;

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El tipo de DNI es obligatorio.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_UsuarioVacio_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_Nom_Usuario = " ";

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El nombre de usuario es obligatorio.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_ContrasenaVacia_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_Contra = "";

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "La contraseña es obligatoria.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_DniConLetras_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_DNI = "1234A678";

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El DNI solo puede contener números.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_DniConEspaciosInternos_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_DNI = "1234 678";

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El DNI solo puede contener números.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_NombreConMasDe100Caracteres_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_Nom_Ape = new string('A', 101);

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El nombre y apellido no pueden superar los 100 caracteres.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_DniConMenosDe8Caracteres_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_DNI = "1234567";

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El DNI tiene que contener 8 caracteres.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_DniConMasDe8Caracteres_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_DNI = "123456789";

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El DNI tiene que contener 8 caracteres.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_TipoDniConMasDe40Caracteres_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_Tipo_DNI = new string('A', 41);

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El tipo de DNI no puede superar los 40 caracteres.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_UsuarioConMasDe30Caracteres_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_Nom_Usuario = new string('A', 31);

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "El nombre de usuario no puede superar los 30 caracteres.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_ContrasenaConMasDe255Caracteres_RetornaError()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_Contra = new string('A', 256);

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.AreEqual(
                "La contraseña no puede superar los 255 caracteres.",
                resultado);
        }

        [TestMethod]
        public void ValidarCrear_CamposConEspaciosExternos_RetornaNull()
        {
            // Arrange
            var admin = CrearAdminValido();
            admin.Admin_Nom_Ape = "  Juan Pérez  ";
            admin.Admin_DNI = " 12345678 ";
            admin.Admin_Tipo_DNI = " DNI ";
            admin.Admin_Nom_Usuario = " juanperez ";

            // Act
            var resultado = _validador.ValidarCrear(admin);

            // Assert
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValidarEditar_AdminNulo_RetornaError()
        {
            // Arrange
            EditarAdminDto? admin = null;

            // Act
            var resultado = _validador.ValidarEditar(admin);

            // Assert
            Assert.AreEqual(
                "Los datos del administrador son obligatorios.",
                resultado);
        }

        [TestMethod]
        public void ValidarEditar_DatosValidos_RetornaNull()
        {
            // Arrange
            var admin = EditarAdminValido();

            // Act
            var resultado = _validador.ValidarEditar(admin);

            // Assert
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValidarEditar_DniInvalido_RetornaError()
        {
            // Arrange
            var admin = EditarAdminValido();
            admin.Admin_DNI = "12ABC678";

            // Act
            var resultado = _validador.ValidarEditar(admin);

            // Assert
            Assert.AreEqual(
                "El DNI solo puede contener números.",
                resultado);
        }

        [TestMethod]
        public void ValidarEditar_NombreVacio_RetornaError()
        {
            // Arrange
            var admin = EditarAdminValido();
            admin.Admin_Nom_Ape = null;

            // Act
            var resultado = _validador.ValidarEditar(admin);

            // Assert
            Assert.AreEqual(
                "El nombre y apellido son obligatorios.",
                resultado);
        }

        [TestMethod]
        public void ValidarEditar_UsuarioDemasiadoLargo_RetornaError()
        {
            // Arrange
            var admin = EditarAdminValido();
            admin.Admin_Nom_Usuario = new string('A', 31);

            // Act
            var resultado = _validador.ValidarEditar(admin);

            // Assert
            Assert.AreEqual(
                "El nombre de usuario no puede superar los 30 caracteres.",
                resultado);
        }
    }
}