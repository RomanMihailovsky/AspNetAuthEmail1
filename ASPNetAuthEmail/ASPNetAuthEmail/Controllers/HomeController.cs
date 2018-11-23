using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ASPNetAuthEmail.Abstract;
using ASPNetAuthEmail.Models;
//using ASPNetAuthEmail.Infrastructure;

namespace ASPNetAuthEmail.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        //IGameRepository repository;
        IOrderRepository repository;

        //public HomeController(IGameRepository repo)
        public HomeController(IOrderRepository repo)
        {
            repository = repo;
        }

        public ActionResult Index(int Idrealtor = 1)
        {
            //Dictionary<string, object> data = new Dictionary<string, object>(); //data.Add("Ключ", "Значение");

            return View(repository.Orders);   //return View(repository.Orders.Where(p => p.IdRealtor == Idrealtor));
        }

        // Добавить запись
        public ViewResult Create()
        {
            return View("Edit", new Order());
        }

        // Редактирование сущности 
        public ViewResult Edit(int orderId = 1)
        {
            Order order = repository.Orders
                .FirstOrDefault(g => g.OrderId == orderId);

            return View(order);
        }

        // Сохранение внесенных изменений
        [HttpPost] 
        //public ActionResult Edit(Game game, HttpPostedFileBase image = null)
        public ActionResult Edit(Order order, HttpPostedFileBase postedfile = null)
        {
            if (ModelState.IsValid)
            {
                //if (postedfile != null)
                //{
                //    order.fileMimeType = postedfile.ContentType;
                //    order.fileData = new byte[postedfile.ContentLength];
                //    postedfile.InputStream.Read(order.ImageData, 0, postedfile.ContentLength);
                //}

                repository.SaveOrder(order);  //repository.SaveGame(game);

                TempData["message"] = string.Format("Изменения \"{0}\" сохранены", order.SurName);
                return RedirectToAction("Index");
            }
            else
            {
                // Что-то не так со значениями данных
                return View(order);  //return View(game);
            }
        }


        public ViewResult Contact()
        {
            return View();  //ViewBag.Message = "АО ИКУР";
        }


        public ActionResult About()
        {
            ViewBag.Message = "Контроллер About.";

            return View();
        }

    }
}