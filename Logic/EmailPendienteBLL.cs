using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using Data.Repositorios;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de emails pendientes de envío.
    /// </summary>
    public class EmailPendienteBLL
    {
        /// <summary>
        /// Proceso para obtener los emails pendientes.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<EmailPendienteViewModel></returns>
        public static List<EmailPendienteViewModel> ObtenerItems(ApplicationDbContext db)
        {
            return EmailPendienteDAL.ObtenerItems(db);
        }

        /// <summary>
        /// Proceso para obtener los datos de un email pendiente.
        /// </summary>
        /// <param name="Id">Identificador del email pendiente.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>EmailPendienteViewModel</returns>
        public static EmailPendienteViewModel ObtenerItem(
            long Id, 
            ApplicationDbContext db)
        {
            return EmailPendienteDAL.ObtenerItem(Id, db);
        }

        /// <summary>
        /// Proceso para enviar emails pendientes.
        /// </summary>
        /// <param name="EmailsEnviar">Listado de los identificadores de los emails a enviar.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        public static void Enviar(
            List<long> EmailsEnviar, 
            ApplicationDbContext db, 
            ContenedorVariablesSesion sesion)
        {
            if (EmailsEnviar == null || EmailsEnviar.Count() == 0)
            {
                throw new Exception("No se han seleccionado correos para enviar.");
            }

            foreach (var EmailEnviar in EmailsEnviar)
            {
                EmailPendiente emailPendiente = db.EmailsPendiente.Find(EmailEnviar);

                if (emailPendiente != null)
                {
                    if (emailPendiente.FechaEnvio == null)
                    {
                        List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>();
                        List<EmailDestinatarioViewModel> copias = new List<EmailDestinatarioViewModel>();

                        foreach(var EmailDestino in emailPendiente.EmailsDestino)
                        {
                            destinatarios.Add(new EmailDestinatarioViewModel() {
                                Nombre = EmailDestino.Nombre,
                                Direccion = EmailDestino.Direccion
                            });
                        }

                        if(emailPendiente.EmailsCopia != null)
                        {
                            foreach (var EmailCopia in emailPendiente.EmailsCopia)
                            {
                                copias.Add(new EmailDestinatarioViewModel()
                                {
                                    Nombre = EmailCopia.Nombre,
                                    Direccion = EmailCopia.Direccion
                                });
                            }
                        }

                        /*if(emailPendiente.TareaId == null)
                        {
                            EmailBLL.EnviarConAdjunto(sesion, emailPendiente.Cuerpo, emailPendiente.Asunto, destinatarios, copias, , db);
                        }
                        else
                        {*/
                            EmailBLL.Enviar(sesion, emailPendiente.Cuerpo, emailPendiente.Asunto, destinatarios, copias, emailPendiente.TareaId, db);
                       /* }*/

                        emailPendiente.FechaEnvio = DateTime.Now;
                        emailPendiente.UsuarioEnvio = sesion.usuario.Usuario;

                        db.Entry(emailPendiente).State = EntityState.Modified;
                    }
                }
            }
        }
    }
}
