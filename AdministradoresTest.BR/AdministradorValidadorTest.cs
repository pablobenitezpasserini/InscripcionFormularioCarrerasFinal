using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Validadores;
using Administradores.Models.Administradores.DTOs;

namespace AdministradoresTest.BR
{
    /// <summary>
    /// Contiene las pruebas unitarias de la clase <see cref="AdministradorValidador"/>,
    /// verificando las validaciones aplicadas a los datos de administradores.
    /// </summary>
    [TestClass]
    public class AdministradorValidadorTest
    {
        /// <summary>
        /// Instancia del validador de administradores utilizada para ejecutar las pruebas unitarias.
        /// </summary>
        private AdministradorValidador _validador = null!;

        /// <summary>
        /// Inicializa una nueva instancia del validador antes de la ejecución de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _validador = new AdministradorValidador();
        }

        /// <summary>
        /// Crea y devuelve un DTO de creación de administrador con datos válidos
        /// para utilizar en las pruebas unitarias.
        /// </summary>
        /// <returns>Un <see cref="CrearAdminDto"/> con datos válidos.</returns>
        private static CrearAdminDto CrearAdminValido() => new()
        {
            Admin_Nom_Ape = "Juan Pérez",
            Admin_DNI = "12345678",
            Admin_Tipo_DNI = "DNI",
            Admin_Nom_Usuario = "juanperez",
            Admin_Contra = "ClaveSegura123"
        };

        /// <summary>
        /// Crea y devuelve un DTO de edición de administrador con datos válidos
        /// para utilizar en las pruebas unitarias.
        /// </summary>
        /// <returns>Un <see cref="EditarAdminDto"/> con datos válidos.</returns>
        private static EditarAdminDto EditarAdminValido() => new()
        {
            Admin_Nom_Ape = "Juan Pérez",
            Admin_DNI = "12345678",
            Admin_Tipo_DNI = "DNI",
            Admin_Nom_Usuario = "juanperez",
            Admin_Contra = "ClaveSegura123"
        };

        /// <summary>
        /// Verifica que, cuando se intenta validar la creación de un administrador
        /// con un DTO nulo, el validador retorne el mensaje indicando que los datos son obligatorios.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando se proporcionan todos los datos requeridos de un administrador,
        /// el validador no retorne ningún mensaje de error.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el nombre y apellido del administrador están vacíos
        /// o contienen únicamente espacios, el validador retorne el mensaje de error correspondiente.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el DNI del administrador está vacío,
        /// el validador retorne el mensaje indicando que este dato es obligatorio.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el tipo de DNI del administrador es nulo,
        /// el validador retorne el mensaje indicando que este dato es obligatorio.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el nombre de usuario está vacío o contiene únicamente espacios,
        /// el validador retorne el mensaje indicando que este dato es obligatorio.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando la contraseña del administrador está vacía,
        /// el validador retorne el mensaje indicando que este dato es obligatorio.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el DNI contiene caracteres alfabéticos,
        /// el validador retorne un error indicando que solo se permiten números.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el DNI contiene espacios internos,
        /// el validador retorne un error indicando que solo se permiten números.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el nombre y apellido del administrador supera los 100 caracteres,
        /// el validador retorne un error indicando que se excedió la longitud máxima permitida.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el DNI contiene menos de 8 caracteres,
        /// el validador retorne un error indicando que debe tener exactamente 8 caracteres.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el DNI contiene más de 8 caracteres,
        /// el validador retorne un error indicando que debe tener exactamente 8 caracteres.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el tipo de DNI supera los 40 caracteres,
        /// el validador retorne un error indicando que se excedió la longitud máxima permitida.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el nombre de usuario supera los 30 caracteres,
        /// el validador retorne un error indicando que se excedió la longitud máxima permitida.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando la contraseña supera los 255 caracteres,
        /// el validador retorne un error indicando que se excedió la longitud máxima permitida.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando los campos de texto contienen espacios al inicio o al final,
        /// el validador acepte los datos y no retorne ningún mensaje de error.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando se intenta validar la edición de un administrador
        /// con un DTO nulo, el validador retorne el mensaje indicando que los datos son obligatorios.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando se proporcionan datos válidos para editar un administrador,
        /// el validador no retorne ningún mensaje de error.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el DNI contiene caracteres que no son numéricos,
        /// el validador retorne un error indicando que solo se permiten números.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el nombre y apellido del administrador son nulos,
        /// el validador retorne el mensaje indicando que estos datos son obligatorios.
        /// </summary>
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

        /// <summary>
        /// Verifica que, cuando el nombre de usuario supera los 30 caracteres,
        /// el validador retorne un error indicando que se excedió la longitud máxima permitida.
        /// </summary>
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