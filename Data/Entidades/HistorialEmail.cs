using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entidades
{
    public class HistorialEmail
    {
        [Key]
        public long Id { get; set; }
        public string Cuerpo { get; set; }
        public string Asunto { get; set; }
        public DateTime Fecha { get; set; }

        public string IdRequerimiento { get; set; }
        public bool Enviado { get; set; }
        public string Respuesta { get; set; }

        [ForeignKey("Tarea")]
        public long? TareaId { get; set; }
        public virtual Tarea Tarea { get; set; }

        [ForeignKey("TareaPago")]
        public long? TareaPagoId { get; set; }
        public virtual TareaPago TareaPago { get; set; }
    }
}
