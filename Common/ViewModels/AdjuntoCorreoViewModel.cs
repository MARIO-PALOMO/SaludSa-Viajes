﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class AdjuntoCorreoViewModel
    {
        public string Nombre { get; set; }
        public byte[] Contenido { get; set; }
        public string RutaArchivo { get; set; }
    }
}
