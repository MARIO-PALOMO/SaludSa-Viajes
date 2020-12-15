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
    /// Repositorio de consultas para ordenes hijas.
    /// </summary>
    public class OrdenHijaDAL
    {
        /// <summary>
        /// Proceso para salvar una orden hija.
        /// </summary>
        /// <param name="OrdenHija">Objeto que contiene los datos de la horden hija a salvar.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void SalvarOrdenHija(
            OrdenHija OrdenHija, 
            ApplicationDbContext db)
        {            
            db.OrdenesHija.Add(OrdenHija);

            db.SaveChanges();
        }

        /// <summary>
        /// Proceso para actualizar una orden hija.
        /// </summary>
        /// <param name="OrdenHija">Objeto que contiene los datos de la horden hija a actualizar.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void ActualizarOrdenHija(
            OrdenHija OrdenHija,
            ApplicationDbContext db)
        {
            db.Entry(OrdenHija).State = EntityState.Modified;

            db.SaveChanges();
        }
    }
}
