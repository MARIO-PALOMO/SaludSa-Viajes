using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para remisión de ordenes hijas.
    /// </summary>
    public class OrdenHijaRemisionDAL
    {
        /// <summary>
        /// Proceso para salvar una remisión de orden hija.
        /// </summary>
        /// <param name="OrdenHijaRemision">Objeto que contiene los datos de la remisión de horden hija a salvar.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void SalvarOrdenHijaRemision(
            OrdenHijaRemision OrdenHijaRemision, 
            ApplicationDbContext db)
        {            
            db.OrdenesHijaRemision.Add(OrdenHijaRemision);

            db.SaveChanges();
        }
    }
}
