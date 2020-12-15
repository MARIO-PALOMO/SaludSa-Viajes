using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class UsuarioViewModel
    {
        public string Apellidos { get; set; }
        public string ApellidosNombres { get; set; }
        public string Cargo { get; set; }
        public string Cedula { get; set; }
        public string CiudadCodigo { get; set; }
        public string CiudadDescripcion { get; set; }
        public string CompaniaCodigo { get; set; }
        public string CompaniaDescripcion { get; set; }
        public string Departamento { get; set; }
        public string Email { get; set; }
        public string Extension { get; set; }
        public UsuarioViewModel JefeInmediato { get; set; }
        public string NombreCompleto { get; set; }
        public string Nombres { get; set; }
        public string NombresApellidos { get; set; }
        public string Telefono { get; set; }
        public string Usuario { get; set; }
        public string UsuarioDominio { get; set; }
    }
}
