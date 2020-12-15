using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class RecepcionViewModel
    {
        public long Id { get; set; }
        public int NumeroRecepcion { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public bool Aprobada { get; set; }
        public long OrdenMadreId { get; set; }
        public long EstadoId { get; set; }
        public string NumeroOrdenMadre { get; set; }
        public long? NumeroSolicitud { get; set; }
        public string EmpresaParaLaQueSeCompra { get; set; }
        public string NumeroOrdenHija { get; set; }

        public List<RecepcionLineaViewModel> RecepcionLineas { get; set; }
        public List<ComprobanteElectronicoViewModel> ComprobantesElectronicos { get; set; }
    }
}
