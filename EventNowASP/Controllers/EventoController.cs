using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventNowASP.Models;

namespace EventNowASP.Controllers
{
    public class EventoController : Controller
    {
        private EventoDbContext db = new EventoDbContext();

        // GET: Evento
        public ActionResult Index(string categoria, string busqueda, string tipo)
        {

            if (categoria != null)
            {
                var eventos = db.Evento.Where(x => x.categoria == categoria).ToList();
                return View(eventos);
            }
            if (!string.IsNullOrEmpty(busqueda))
            {
                var eventos = db.Evento.Where(x => x.titulo.Contains(busqueda) ||
                                                   x.descripcion.Contains(busqueda)
                
                ).ToList();
                return View(eventos);
            }

            if (tipo != null)
            {

                var tipos = db.Evento.Where(x => x.tipo == tipo).ToList();
                return View(tipos);
            }
            return View(db.Evento.ToList());
        }

        public ActionResult MyEvents()
        {
            if (Session["iduser"] != null)
            {
                int idu = Int32.Parse(Session["iduser"].ToString());
                var mydata = db.Evento.Where(x => x.iduser == idu).ToList();
                return View(mydata);
            }
            return View();
        }


        //Metodo Notificaciones
        public JsonResult GetNotifications()
        {
            var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;
            NotificationComponent noti = new NotificationComponent();
            var list = noti.GetEventos(notificationRegisterTime);
            Session["LastUpdate"] = DateTime.Now;
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Enviar lista de titulos a la vista para sugerir la busqueda
        public JsonResult GetSearchValue(string search)
        {
            var parametro = search;
            //Usa el método para traer los titulos de la base de datos
            var eventos = ConsultarEventos(parametro);
            return new JsonResult { Data = eventos, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Consultar sugerencias a la base de datos (retorna los titulos)
        public List<Evento> ConsultarEventos(string search)
        {
            EventNowEntities db = new EventNowEntities();
            var query = from x in db.Busqueda
                        where x.titulo.Contains(search)
                        select x;
          
            return query.ToList();
        }

        //Ver evento(detalles)
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evento evento = db.Evento.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return View(evento);
        }

        //Vista para publicar
        public ActionResult Create()
        {
            return View();
        }

        //Publicar Evento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,titulo,categoria,descripcion,fecha,fechafin,horaI,horaF,ubicacion,tipo")]
        Evento evento, HttpPostedFileBase imagen)
        {
            //Almacenar la imagen el carpeta de proyecto
            string path = Server.MapPath("~/Uploads/");
            //Si el directorio no existe, lo crea.
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //Ruta del archivo
            string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + imagen.FileName).ToLower();
            //Evia el archivo a la ruta creada
            imagen.SaveAs(Server.MapPath("~/Uploads/" + archivo));
            //Seteamos la ruta al atributo imagen el la Base de datos
            evento.imagen = archivo;
            evento.entidad = Convert.ToString(Session["NombreEmpresa"]);
            int iduser = Int32.Parse(Session["iduser"].ToString());
            evento.iduser = iduser;

            if (ModelState.IsValid)
            {
                db.Evento.Add(evento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(evento);
        }

        // Obtener datos para editar
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evento evento = db.Evento.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return View(evento);
        }

        //Editar evento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,titulo,categoria,descripcion,imagen,fecha,fechafin,horaI,horaF,ubicacion,entidad,tipo")]
        Evento evento,
        HttpPostedFileBase imagen,string defaultImg)
        {
            int iduser = Int32.Parse(Session["iduser"].ToString());
            //if user post new image, use httPostedFileBase to save it
            if (evento.imagen != null)
            {
                string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + imagen.FileName).ToLower();
                imagen.SaveAs(Server.MapPath("~/Uploads/" + archivo));
                evento.imagen = archivo;
            }
            else
            {
                //ifn´t set default image path to db
                evento.imagen = defaultImg;
            }
            evento.iduser = iduser;
            if (ModelState.IsValid)
            {
                db.Entry(evento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(evento);
        }

        //Confirmacion para eliminaar evento
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evento evento = db.Evento.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return View(evento);
        }

        //Eliminar evento
        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Evento evento = db.Evento.Find(id);
            db.Evento.Remove(evento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
