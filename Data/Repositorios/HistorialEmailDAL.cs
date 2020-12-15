using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para historial de emails.
    /// </summary>
    public class HistorialEmailDAL
    {
        /// <summary>
        /// Proceso para salvar un historial de email
        /// </summary>
        /// <param name="historial">Objeto que contiene los datos del historial de email a salvar.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void SalvarHistorial(
            HistorialEmail historial, 
            ApplicationDbContext db)
        {
            db.HistorialesEmail.Add(historial);
            db.SaveChanges();
        }
    }
}
