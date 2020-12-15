using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ImpuestoPagoViewModel
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Compensacion { get; set; }
        public long EstadoId { get; set; }
        public string EstadoNombre { get; set; }
        public EstadoViewModel Estado { get; set; }
    }
}
