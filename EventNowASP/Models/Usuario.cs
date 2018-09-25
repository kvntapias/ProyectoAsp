using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EventNowASP.Models
{
    [Table("Usuario")]
    public class Usuario : IValidatableObject
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(20, ErrorMessage = "Nombre invalido, Minimo 20 caracteres")]
        public string NombreEmpresa { get; set; }
        [Required]
        [Index("Ix_User", Order = 1, IsUnique = true)]
        [MaxLength(10, ErrorMessage = "Nombre de Usuario invalido, Minimo 10 caracteres")]
        public string Username { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Dirección de correo invalida")]
        [Index("Ix_Correo", Order = 1, IsUnique = true)]
        public string Correo { get; set; }
        [Required]
        [MaxLength(18, ErrorMessage = "Contraseña invalida, Minimo 18 caracteres")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }
        public int Rol { get; set; }
 
        public string NIT  { get; set; }


        //Validar datos existentes
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            EventoDbContext db = new EventoDbContext();
            var validateName = db.Usuario.FirstOrDefault
            (x => x.Username == Username && x.Id != Id);
            if (validateName != null)
            {
                ValidationResult errorMessage = new ValidationResult
                ("El Usuario ya existe", new[] { "Username" });
                yield return errorMessage;
            }
            else
            {
                yield return ValidationResult.Success;
            }

            var validarMail = db.Usuario.FirstOrDefault
            (x => x.Correo == Correo && x.Id != Id);
            if (validarMail != null)
            {
                ValidationResult errorMessage = new ValidationResult
                ("El email ya existe", new[] { "Correo" });
                yield return errorMessage;
            }
            else
            {
                yield return ValidationResult.Success;
            }

            var validarNit = db.Usuario.FirstOrDefault(x => x.NIT == NIT && x.Id != Id);
            if (validarNit != null)
            {
                ValidationResult errorMessage = new ValidationResult
                ("El nit ya existe", new[] { "NIT" });
                yield return errorMessage;
            }
            else
            {
                yield return ValidationResult.Success;
            }
        }
    }
}