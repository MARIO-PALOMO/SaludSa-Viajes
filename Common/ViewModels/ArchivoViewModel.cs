using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ArchivoViewModel
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public byte[] Contenido { get; set; }
        public long Tamanio { get; set; }
        public long Version { get; set; }
        public long IdObjeto { get; set; }
        public DateTime FechaMdoficacion { get; set; }
    }
}
