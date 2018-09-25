using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace EventNowASP.Models
{
    [Table("Evento")]
    public class Evento
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Ingrese el titulo")]
        [MaxLength(50, ErrorMessage = "Titulo invalido, Maximo 50 caracteres")]
        public string titulo { get; set; }

        [Required(ErrorMessage = "Seleccione la categoria")]
        public string categoria { get; set; }

        public string imagen { get; set; }

        [MaxLength(500, ErrorMessage = "Descripcion invalida, Maximo 500 caracteres")]
        [Required(ErrorMessage = "Ingrese una descripcion")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "Seleccione la fecha")]
        public string fecha { get; set; }

        [Display(Name = "Fecha de finalización")]
        [Required(ErrorMessage = "Seleccione la fecha de finalización")]
        public string fechafin { get; set; }

        [Required(ErrorMessage = "Ingrese la hora de inicio")]
        public string horaI { get; set; }

        [Required(ErrorMessage = "Ingrese la hora de finalizacion")]
        public string horaF { get; set; }

        [Required(ErrorMessage = "Ingrese la ubicación")]
        public string ubicacion { get; set; }

        public string entidad { get; set; }

        [Required(ErrorMessage = "Seleccione el tipo de evento")]
        public string tipo { get; set; }
        public DateTime AddedOn { get; set; }
        public Evento()
        {
            AddedOn = DateTime.Now;
        }

        [ForeignKey("idu")]
        public int iduser { get; set; }
        public Usuario idu { get; set; }

    }


    //CreamoS el contexto de datos OFFicial
    public class EventoDbContext : DbContext
    {
        public EventoDbContext() : base("name=Conexion") { }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
    }
}