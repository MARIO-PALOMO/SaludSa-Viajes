using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using Data.Repositorios;
using Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de envío de emails.
    /// </summary>
    public class EmailBLL
    {
        /// <summary>
        /// Proceso para enviar un email sin adjunto para el subsistema de compras.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="Cuerpo">Cuerpo del email.</param>
        /// <param name="Asunto">Asunto del email.</param>
        /// <param name="EmailsDestino">Listado de destinatarios.</param>
        /// <param name="EmailsCopia">Listado de destinatarios copia.</param>
        /// <param name="TareaId">Identificador de la tarea que se está ejecutando.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void Enviar(ContenedorVariablesSesion sesion,
            string Cuerpo,
            string Asunto,
            List<EmailDestinatarioViewModel> EmailsDestino,
            List<EmailDestinatarioViewModel> EmailsCopia,
            long? TareaId,
            ApplicationDbContext db)
        {
            EmailViewModel email = new EmailViewModel()
            {
                Cuerpo = Cuerpo,
                Asunto = Asunto,
                IdAplicacion = "13",  // Cambiar por 19
                IdTransaccion = null,
                NumeroIdentificacion = null,
                Contrato = null,
                NombreOrigen = sesion.EmailNombreOrigen,
                EmailOrigen = sesion.EmailEmailOrigen,
                EmailsDestino = EmailsDestino,
                EmailsCopia = EmailsCopia,
                TiempoEspera = sesion.EmailTiempoEspera
            };

            if (sesion.EmailDestinatario != null && sesion.EmailDestinatario.Trim() != string.Empty)
            {
                email.EmailsDestino = new List<EmailDestinatarioViewModel>();
                email.EmailsCopia = new List<EmailDestinatarioViewModel>();

                email.EmailsDestino.Add(new EmailDestinatarioViewModel()
                {
                    Nombre = sesion.EmailDestinatario,
                    Direccion = sesion.EmailDestinatario
                });
            }

            var respuesta = EmailRCL.Enviar(sesion, email);

            if (respuesta.IdRequerimiento != null && respuesta.IdRequerimiento != string.Empty)
            {
                var historial = new HistorialEmail()
                {
                    Cuerpo = Cuerpo,
                    Asunto = Asunto,
                    Fecha = DateTime.Now,
                    IdRequerimiento = respuesta.IdRequerimiento,
                    Enviado = respuesta.Enviado,
                    TareaId = TareaId
                };

                historial.Respuesta = string.Join(";", respuesta.Mensajes);

                HistorialEmailDAL.SalvarHistorial(historial, db);
            }
            else
            {
                var emailPendiente = new EmailPendiente()
                {
                    Cuerpo = Cuerpo,
                    Asunto = Asunto,
                    Razon = respuesta.Mensajes.Count() > 0 ? respuesta.Mensajes.FirstOrDefault() : null,
                    FechaRegistro = DateTime.Now,
                    TareaId = TareaId
                };

                foreach(var EmailDestino in EmailsDestino)
                {
                    emailPendiente.EmailsDestino.Add(new EmailDestinatario() {
                        Nombre = EmailDestino.Nombre,
                        Direccion = EmailDestino.Direccion
                    });
                }

                if(EmailsCopia != null)
                {
                    foreach (var EmailCopia in EmailsCopia)
                    {
                        emailPendiente.EmailsCopia.Add(new EmailDestinatario()
                        {
                            Nombre = EmailCopia.Nombre,
                            Direccion = EmailCopia.Direccion
                        });
                    }
                }

                EmailPendienteDAL.SalvarEmailPendiente(emailPendiente, db);
            }
        }

        /// <summary>
        /// Proceso para enviar un email sin adjunto para el subsistema de pagos.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="Cuerpo">Cuerpo del email.</param>
        /// <param name="Asunto">Asunto del email.</param>
        /// <param name="EmailsDestino">Listado de destinatarios.</param>
        /// <param name="EmailsCopia">Listado de destinatarios copia.</param>
        /// <param name="TareaId">Identificador de la tarea que se está ejecutando.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void EnviarPago(ContenedorVariablesSesion sesion,
            string Cuerpo,
            string Asunto,
            List<EmailDestinatarioViewModel> EmailsDestino,
            List<EmailDestinatarioViewModel> EmailsCopia,
            long? TareaId,
            ApplicationDbContext db)
        {
            EmailViewModel email = new EmailViewModel()
            {
                Cuerpo = Cuerpo,
                Asunto = Asunto,
                IdAplicacion = "13",  // Cambiar por 19
                IdTransaccion = null,
                NumeroIdentificacion = null,
                Contrato = null,
                NombreOrigen = sesion.EmailNombreOrigen,
                EmailOrigen = sesion.EmailEmailOrigen,
                EmailsDestino = EmailsDestino,
                EmailsCopia = EmailsCopia,
                TiempoEspera = sesion.EmailTiempoEspera
            };

            if (sesion.EmailDestinatario != null && sesion.EmailDestinatario.Trim() != string.Empty)
            {
                email.EmailsDestino = new List<EmailDestinatarioViewModel>();
                email.EmailsCopia = new List<EmailDestinatarioViewModel>();

                email.EmailsDestino.Add(new EmailDestinatarioViewModel()
                {
                    Nombre = sesion.EmailDestinatario,
                    Direccion = sesion.EmailDestinatario
                });
            }

            var respuesta = EmailRCL.Enviar(sesion, email);

            if (respuesta.IdRequerimiento != null && respuesta.IdRequerimiento != string.Empty)
            {
                var historial = new HistorialEmail()
                {
                    Cuerpo = Cuerpo,
                    Asunto = Asunto,
                    Fecha = DateTime.Now,
                    IdRequerimiento = respuesta.IdRequerimiento,
                    Enviado = respuesta.Enviado,
                    TareaPagoId = TareaId
                };

                historial.Respuesta = string.Join(";", respuesta.Mensajes);

                HistorialEmailDAL.SalvarHistorial(historial, db);
            }
            /*else
            {
                var emailPendiente = new EmailPendiente()
                {
                    Cuerpo = Cuerpo,
                    Asunto = Asunto,
                    Razon = respuesta.Mensajes.Count() > 0 ? respuesta.Mensajes.FirstOrDefault() : null,
                    FechaRegistro = DateTime.Now,
                    TareaPagoId = TareaId
                };

                foreach (var EmailDestino in EmailsDestino)
                {
                    emailPendiente.EmailsDestino.Add(new EmailDestinatario()
                    {
                        Nombre = EmailDestino.Nombre,
                        Direccion = EmailDestino.Direccion
                    });
                }

                if (EmailsCopia != null)
                {
                    foreach (var EmailCopia in EmailsCopia)
                    {
                        emailPendiente.EmailsCopia.Add(new EmailDestinatario()
                        {
                            Nombre = EmailCopia.Nombre,
                            Direccion = EmailCopia.Direccion
                        });
                    }
                }

                EmailPendienteDAL.SalvarEmailPendiente(emailPendiente, db);
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="Cuerpo">Cuerpo del email.</param>
        /// <param name="Asunto">Asunto del email.</param>
        /// <param name="EmailsDestino">Listado de destinatarios.</param>
        /// <param name="EmailsCopia">Listado de destinatarios copia.</param>
        /// <param name="Adjunto">Archivo adjunto al email.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void EnviarConAdjunto(ContenedorVariablesSesion sesion,
            string Cuerpo,
            string Asunto,
            List<EmailDestinatarioViewModel> EmailsDestino,
            List<EmailDestinatarioViewModel> EmailsCopia,
            byte[] Adjunto,
            ApplicationDbContext db)
        {
            EmailAdjuntoViewModel email = new EmailAdjuntoViewModel()
            {
                Adjuntos = new List<AdjuntoCorreoViewModel>() {
                    new AdjuntoCorreoViewModel()
                    {
                        Nombre = "Orden-Compra.pdf",
                        Contenido = Adjunto
                    }
                },
                Cuerpo = Cuerpo,
                Asunto = Asunto,
                IdAplicacion = "13",  // Cambiar por 19
                IdTransaccion = null,
                NumeroIdentificacion = null,
                Contrato = null,
                NombreOrigen = sesion.EmailNombreOrigen,
                EmailOrigen = sesion.EmailEmailOrigen,
                EmailsDestino = EmailsDestino,
                EmailsCopia = EmailsCopia,
                TiempoEspera = sesion.EmailTiempoEspera
            };

            if (sesion.EmailDestinatario != null && sesion.EmailDestinatario.Trim() != string.Empty)
            {
                email.EmailsDestino = new List<EmailDestinatarioViewModel>();
                email.EmailsCopia = new List<EmailDestinatarioViewModel>();

                email.EmailsDestino.Add(new EmailDestinatarioViewModel()
                {
                    Nombre = sesion.EmailDestinatario,
                    Direccion = sesion.EmailDestinatario
                });
            }

            var respuesta = EmailRCL.EnviarConAdjunto(sesion, email);

            if (respuesta.IdRequerimiento != null && respuesta.IdRequerimiento != string.Empty)
            {
                var historial = new HistorialEmail()
                {
                    Cuerpo = Cuerpo,
                    Asunto = Asunto,
                    Fecha = DateTime.Now,
                    IdRequerimiento = respuesta.IdRequerimiento,
                    Enviado = respuesta.Enviado,
                    TareaId = null
                };

                historial.Respuesta = string.Join(";", respuesta.Mensajes);

                HistorialEmailDAL.SalvarHistorial(historial, db);
            }
            else
            {
                //var emailPendiente = new EmailPendiente()
                //{
                //    Cuerpo = Cuerpo,
                //    Asunto = Asunto,
                //    Razon = respuesta.Mensajes.Count() > 0 ? respuesta.Mensajes.FirstOrDefault() : null,
                //    FechaRegistro = DateTime.Now,
                //    TareaId = null
                //};

                //foreach (var EmailDestino in EmailsDestino)
                //{
                //    emailPendiente.EmailsDestino.Add(new EmailDestinatario()
                //    {
                //        Nombre = EmailDestino.Nombre,
                //        Direccion = EmailDestino.Direccion
                //    });
                //}

                //if (EmailsCopia != null)
                //{
                //    foreach (var EmailCopia in EmailsCopia)
                //    {
                //        emailPendiente.EmailsCopia.Add(new EmailDestinatario()
                //        {
                //            Nombre = EmailCopia.Nombre,
                //            Direccion = EmailCopia.Direccion
                //        });
                //    }
                //}

                //EmailPendienteDAL.SalvarEmailPendiente(emailPendiente, db);
            }
        }
    }
}
