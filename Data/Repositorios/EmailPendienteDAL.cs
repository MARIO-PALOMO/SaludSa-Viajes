using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para emails pendientes.
    /// </summary>
    public class EmailPendienteDAL
    {
        /// <summary>
        /// Proceso para salvar un email pendiente.
        /// </summary>
        /// <param name="emailPendiente">Objeto que contiene los datos del email pendiente a salvar.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void SalvarEmailPendiente(
            EmailPendiente emailPendiente, 
            ApplicationDbContext db)
        {
            db.EmailsPendiente.Add(emailPendiente);
            db.SaveChanges();
        }

        /// <summary>
        /// Proceso para obtener los emails pendientes.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<EmailPendienteViewModel></returns>
        public static List<EmailPendienteViewModel> ObtenerItems(ApplicationDbContext db)
        {
            List<EmailPendienteViewModel> resultado = null;

            resultado = db.EmailsPendiente.Where(x => x.FechaEnvio == null && x.TareaId != null && x.TareaPagoId == null).Select(x => new EmailPendienteViewModel()
            {
                Id = x.Id,
                Cuerpo = x.Cuerpo,
                Asunto = x.Asunto,
                Razon = x.Razon,
                FechaRegistro = x.FechaRegistro,
                Tarea = db.Tareas.Where(y => y.Id == x.TareaId).Select(y => new TareaViewModel() {
                    Id = y.Id,
                    Actividad = y.Actividad,
                    FechaCreacion = y.FechaCreacion
                }).FirstOrDefault(),
                EmailsDestino = x.EmailsDestino.Select(z => new EmailDestinatarioViewModel() {
                    Nombre = z.Nombre,
                    Direccion = z.Direccion
                }).ToList(),
                EmailsCopia = x.EmailsCopia.Select(z => new EmailDestinatarioViewModel()
                {
                    Nombre = z.Nombre,
                    Direccion = z.Direccion
                }).ToList()
            }).ToList();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener un email pendiente.
        /// </summary>
        /// <param name="Id">Identificador del email pendiente.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>EmailPendienteViewModel</returns>
        public static EmailPendienteViewModel ObtenerItem(
            long Id, 
            ApplicationDbContext db)
        {
            return db.EmailsPendiente.Where(x => x.Id == Id).Select(x => new EmailPendienteViewModel()
            {
                Id = x.Id,
                Cuerpo = x.Cuerpo,
                Asunto = x.Asunto,
                Razon = x.Razon,
                FechaRegistro = x.FechaRegistro,
                Tarea = db.Tareas.Where(y => y.Id == x.TareaId).Select(y => new TareaViewModel()
                {
                    Id = y.Id,
                    Actividad = y.Actividad,
                    FechaCreacion = y.FechaCreacion
                }).FirstOrDefault(),
                EmailsDestino = x.EmailsDestino.Select(z => new EmailDestinatarioViewModel()
                {
                    Nombre = z.Nombre,
                    Direccion = z.Direccion
                }).ToList(),
                EmailsCopia = x.EmailsCopia.Select(z => new EmailDestinatarioViewModel()
                {
                    Nombre = z.Nombre,
                    Direccion = z.Direccion
                }).ToList()
            }).FirstOrDefault();
        }
    }
}
