using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASPNetAuthEmail.Models;
using ASPNetAuthEmail.Abstract;
using ASPNetAuthEmail.Concrete;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.IO;

using ASPNetAuthEmail.Hubs;
using ASPNetAuthEmail.Jobs;
using System.Threading;

//using Microsoft.AspNet.SignalR;

namespace ASPNetAuthEmail.Controllers
{
    [Authorize]

    public class TstController : Controller
    {

        public int pageSize = 10;

        //dbCntxt context = new dbCntxt();

        EFDbContext context; //= new EFDbContext();

        ICRM_TabOrdRepository repository;

        ICRM_DocRepository  repositorydoc;

        //MsgScheduler msgScheduler;

        //int IdUser = 0;        

        //public UserManager<ApplicationUser> UserManager { get; private set; }


        public TstController(ICRM_TabOrdRepository repo, ICRM_DocRepository repodoc)
        {

            // Создание нового объекта repository осущ-ся Ninject-ом
            repository = repo;
            repositorydoc = repodoc;
            context = new EFDbContext();

            //string Id = HttpContext.User.Identity.GetUserId();
            //ASPNetUsers aspnetusers = context.ASPNetUsers.Find(Id);
            //int iduser = aspnetusers.id_user;
           
        }

        public string getUserName()
        {
            return HttpContext.User.Identity.Name;
        }

        // ******************************************* PreviewParamsLoan * ПАРАМЕТРЫ ЗАЙМА * *****************************
        public PartialViewResult PreviewParamsLoan(int idklient = 0  /*Передать id_Klient*/)
        { 
            CRM_Abonent crm_abonent = context.CRM_Abonent.Find(idklient);

            //string sqltext = String.Format("select dbo.CRM_getProductNameByValue({0})", crm_abonent.ProductValue);
            //CRM_Option productname = context.Database.SqlQuery<CRM_Option>("select Description as NameOp from NS_GetTypeZaim() where value = @product",
            //                                                        new SqlParameter("product", crm_abonent.ProductValue)).FirstOrDefault();

            if ( ! (crm_abonent.ProductValue == null || crm_abonent.ProductValue == 0) )
            {
                CRM_ProductNames_V productname = context.CRM_ProductNames_V.Where(p => p.Value == crm_abonent.ProductValue).FirstOrDefault();
                ViewBag.productname = productname.Description;
            }

            return PartialView(crm_abonent);
        }

        // ******************************************* PreviewListDoc * ПРИКРЕПЛЕННЫЕ ДОК-ТЫ * *****************************
        public PartialViewResult PreviewListDoc(int id = -1 /*id = id_ab */)
        {  // Если для заявки id_ab = 0 // При добавлении док-тов для Заявки исключить возможность добавления персональных док-ов "Паспорт", "Согласие"

            //if (ModelState.IsValid)
            //{
                IEnumerable<CRM_Docs_V> crm_doc = context.CRM_Docs_V.Where(c => c.id_ab == id);
            //}

            return PartialView(crm_doc);
        }

        // *******************************************  Список РИЭЛТОРОВ ******************************
        public PartialViewResult CmbBx_Realtors(int idorder = 0)
        {
            @ViewBag.Id_order = idorder;
            IEnumerable<Realtors> realtors = context.Realtors.OrderBy(p => p.Name);
            return PartialView(realtors);
        }

        // *******************************************  Список Источников заявки ******************************
        public PartialViewResult CmbBx_CRM_SPR_SourceOrder(/*int idorder = 0*/)
        {
            //@ViewBag.Id_order = idorder;
            IEnumerable<CRM_SPR_SourceOrder> crm_spr_sourceorder = context.CRM_SPR_SourceOrder;
            return PartialView(crm_spr_sourceorder);
        }

        // ******************************************* PreviewSPR_DeloDoc * СПРАВОЧНИК ДОК-ТОВ * *****************************
        public PartialViewResult PreviewSPR_DeloDoc(int id = 0)
        { 
            IEnumerable<SPR_DeloDoc> spr_delodoc = context.SPR_DeloDoc.Where( p => (p.status & 4194304) > 0 && p.TemplateNameFile.Contains("ФИО")  );

            return PartialView(spr_delodoc);
        }

        // ******************************************* PrevProductNames * СПРАВОЧНИК ПРОДУКТОВ * *****************************
        public PartialViewResult PrevProductNames(int id_product = 0 /*, int productval = 0*/ )
        {
            IEnumerable<CRM_ProductNames_V> crm_products = context.CRM_ProductNames_V;

            // *** Передаем номер продукта ВНУТРЬ САМОГО Представления 
            ViewBag.id_product = id_product;


            return PartialView(crm_products);
        }

        // ********** PrevCheckOptionsProduct * СПИСОК ОПЦИЙ c Checkbox Выводится при EitParamsLoan ( CRM_Option - View ) **************
        public PartialViewResult PrevCheckOptionsProduct(int id_product = 44, string options = "0")
        {
            // ********  Передается options "4,3,6"
            ViewBag.in_options = options;

            string sqltext = String.Format("select * from CRM_Option where id_product = {0}", id_product);

            IEnumerable<CRM_Option> crm_option = context.Database.SqlQuery<CRM_Option>(sqltext);

            return PartialView(crm_option);
        }

        // ************ JSON список Опиций (Используется в EditParamsLoan)  *****************************
        //[HttpPost]
        public JsonResult JsonGetOptions(int id_product)
        {
            var optList = context.CRM_Option.Where(p => p.id_Product == id_product).ToList();
            return Json( optList, JsonRequestBehavior.AllowGet);
        }

        // Получение Опций проекта для передачи в ф-ю CRM_CalcLoan( )
        
        public ViewResult CalcLoanRes(int id_order = 0)
        {

            CRM_TabOrd crm_tabord = repository.FindRec(id_order);

            SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur_newSQL;Integrated Security=True");

            string options = crm_tabord.options; //context.CRM_TabOrd.Select(p => new {p.options}).Where(p => p.id_Klient == id_ab);
            // опции текущей заявки
            if (options == "" || options == null) 
                options = "0";

            SqlCommand cmd = new SqlCommand(String.Format("select isNull( sum(paramOption), 0) from crm_CalcOptions where id in ( {0} )", options), conn);
            SqlCommand cmd2 = new SqlCommand(String.Format("Select isNull( dbo.MinNum( max([value]), sum([value]) ), 0) FROM crm_CalcOptions where [type] = -1 and id in ( {0} )", options), conn);
            SqlCommand cmd3 = new SqlCommand(String.Format("Select isNull( sum([value]), 0 ) FROM crm_CalcOptions where [type] = 1 and id in  ( {0} )", options), conn);
            conn.Open();

            int paramOption = (Int32)cmd.ExecuteScalar();
            double decStavka = (double)cmd2.ExecuteScalar();
            double incStavka = (double)cmd3.ExecuteScalar();

            conn.Close();

            ViewBag.paramOption = paramOption;
            ViewBag.decStavka = decStavka;
            ViewBag.incStavka = incStavka;
            int cntPerson = 1; // Кол-во участников заявки (При значении 0 считается кол-во участников по CRM_Tab group by id_ab)

            CRM_Abonent crm_abonent = context.CRM_Abonent.Find(crm_tabord.id_Klient);

            if (crm_abonent.dohod2 > 0 /*|| (crm_abonent.alim2.GetValueOrDefault() + crm_abonent.cred2.GetValueOrDefault() + crm_abonent.card2.GetValueOrDefault() ) > 0*/)
                cntPerson = 2;
            if (crm_abonent.dohod3 > 0 /*|| (crm_abonent.alim3.GetValueOrDefault() + crm_abonent.cred3.GetValueOrDefault() + crm_abonent.card3.GetValueOrDefault() ) > 0*/)
                cntPerson = 3;
            if (crm_abonent.dohod4 > 0 /*|| (crm_abonent.alim4.GetValueOrDefault() + crm_abonent.cred4.GetValueOrDefault() + crm_abonent.card4.GetValueOrDefault() ) > 0*/)
                cntPerson = 4;

            // Если cntPerson = 0 есть расчёт внутри dbo.CRM_CalcLoan( {0}, {1}, {2}, {3}, {4}, {5} )
            // Получение расчитанных значений  
            string sqltxt = String.Format("select * from dbo.CRM_CalcLoan( {0}, {1}, {2}, {3}, {4}, {5} )", crm_tabord.id_Klient, id_order, cntPerson, paramOption,
                 decStavka.ToString().Replace(',', '.'), incStavka.ToString().Replace(',', '.'));

            IEnumerable<CRM_CalcLoan> crm_calcloan = context.Database.SqlQuery<CRM_CalcLoan>(sqltxt);

            return View(crm_calcloan);
            //int paramOption = context.crm_CalcOptions.Where(p => p.id.CompareTo(1) == 1 ).Sum(p => p.paramOption);
            //var optList = context.CRM_Option.Where(p => p.id_Product == id_product).ToList();
            //return Json(optList, JsonRequestBehavior.AllowGet);
        }



        // ************ JSON CRM_TabOrder *****************************
        public JsonResult JsonGetCRM_TabOrd(string name)
        {
            string Id = HttpContext.User.Identity.GetUserId();
            ASPNetUsers aspnetusers = context.ASPNetUsers.Find(Id);
            int iduser = aspnetusers.id_user;

            var crmList = context.CRM_TabOrd
                    //.Select(p => new { p.id_Order, p.SurName, p.Name, p.Otch, p.Date, p.Id_status, p.Id_User, p.Status, p.StageStatus })
                    .Where(p => p.SurName.Contains(name) && p.Id_status > -1 && p.Id_status != 8 && p.Id_status != 9 && p.Id_status != 10 && p.Id_status != 19 && p.Id_status != 20 && p.Id_User == iduser)
                    .OrderByDescending(d => d.Date)
                    .ToList();

            return Json( crmList, JsonRequestBehavior.AllowGet);
        }
        // ************************************************************


        // ******************************************* PrevOptionsProduct * СПИСОК ОПЦИЙ ( CRM_Option - View ) ***************
        public PartialViewResult PrevOptionsProduct(int id_product = 0, string options = "0")
        {
            //IEnumerable<CRM_Option> crm_option = context.CRM_Option.Where(p => p.id_Product == id_product);
            //if ( String.IsNullOrEmpty(options) ) options = "0";

            string sqltext = String.Format("select * from CRM_Option where id_product = {0} and id_Option in ( {1} ) ", id_product, options); 

            IEnumerable<CRM_Option> crm_option = context.Database.SqlQuery<CRM_Option>(sqltext);

            return PartialView(crm_option);
        }

        //// ******************************************* PreviewListAb * СПИСОК УЧАСТНИКОВ ЗАЯВКИ * *****************************
        public PartialViewResult PreviewListAb(int id = 0)
        {
            IEnumerable<CRM_AbKnt> crm_abknt = context.CRM_AbKnt.Where(c => c.id_order == id);

            ViewBag.Id_order = id;
            //var crm_doc = repositorydoc.CRM_Docs.Select(p => new { p.id_order, p.id_doc, p.id_ab, p.name, p.path, p.date, p.size, p.type_doc, p.spr_delodoc.description }).Where(c => c.id_order == id);

            //SelectList realtorsList = new SelectList(context.Realtors, "Id", "Name", 0);
            //ViewBag.realtorsList = realtorsList;

            return PartialView(crm_abknt);
        }


        public ActionResult Index(string fsurname = "", int iduser = 23, int page = 1 )
        {

            // запуск выполнения информатора при создании контроллера
            MsgScheduler.Start(HttpContext.User.Identity.Name);


            string Id = HttpContext.User.Identity.GetUserId();
            ASPNetUsers aspnetusers = context.ASPNetUsers.Find(Id);
            iduser = aspnetusers.id_user;
            //iduser = 23;

            //ViewBag.iduser = String.Format("Идентификатор пользователя: {0}", iduser);

            bool Isnullfsurname = String.IsNullOrWhiteSpace(fsurname);
            
            CRM_TabOrd_ViewModel model = new CRM_TabOrd_ViewModel
            {  
                CRM_TabOrds = repository.CRM_TabOrds                    
                    .Where(p => p.Id_status > -1 && p.Id_status != 8 && p.Id_status != 9 && p.Id_status != 10
                        && p.Id_status != 19 && p.Id_status != 20 && p.Id_User == iduser && p.SurName != null)
                    .Where(p => p.SurName.Contains(Isnullfsurname ? p.SurName : fsurname))
                    .OrderByDescending(d => d.Date),
                    //.Skip((page - 1) * pageSize)
                    //.Take(pageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,

                    /*
                    TotalItems = iduser == 0 ? repository.CRM_TabOrds.Where(p => p.Id_status > -1 && p.SurName != null)
                                                                     .Where(p => p.SurName.Contains(Isnullfsurname ? p.SurName : fsurname))
                                                                     .Count()
                                                 : repository.CRM_TabOrds.Where(p => p.Id_status > -1 && p.Id_User == iduser && p.SurName != null)
                                                                     .Where(p => p.SurName.Contains(Isnullfsurname ? p.SurName : fsurname))
                                                                     .Count()
                     */
                     TotalItems = 0

                },
                CurrentUser = iduser,
                FilterSurName = fsurname
            };

            //SendMessage("Сообщение Перед View");

            return View(model);


            //return View( repository.CRM_TabOrds);
        }


        public PartialViewResult Pages(string fsurname = "", int iduser = 23, int page = 1)
        {
            string Id = HttpContext.User.Identity.GetUserId();
            ASPNetUsers aspnetusers = context.ASPNetUsers.Find(Id);
            iduser = aspnetusers.id_user;
            //iduser = 23;

            bool Isnullfsurname = String.IsNullOrWhiteSpace(fsurname);

            CRM_TabOrd_ViewModel model = new CRM_TabOrd_ViewModel
            {
                CRM_TabOrds = repository.CRM_TabOrds                    
                    .Where(p => p.Id_status > -1 && p.Id_status != 8 && p.Id_status != 9 && p.Id_status != 10
                        && p.Id_status != 19 && p.Id_status != 20 && p.Id_User == iduser && p.SurName != null)
                    .Where(p => p.SurName.Contains(Isnullfsurname ? p.SurName : fsurname))
                    .OrderByDescending(d => d.Date)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,

                    TotalItems = iduser == 0 ? repository.CRM_TabOrds.Where(p => p.Id_status > -1 && p.SurName != null)
                                                                     .Where(p => p.SurName.Contains(Isnullfsurname ? p.SurName : fsurname))
                                                                     .Count()
                                                 : repository.CRM_TabOrds                                                    
                                                    .Where(p => p.Id_status > -1 && p.Id_status != 8 && p.Id_status != 9 && p.Id_status != 10
                                                                && p.Id_status != 19 && p.Id_status != 20 && p.Id_User == iduser && p.SurName != null)
                                                    .Where(p => p.SurName.Contains(Isnullfsurname ? p.SurName : fsurname))
                                                                     .Count()

                    //TotalItems = iduser == 0 ? repository.CRM_TabOrds.Where(p => p.Id_status > -1 && p.SurName.Contains( Isnullfsurname ? p.SurName : fsurname ) ).Count()
                    //                             : repository.CRM_TabOrds.Where(p => p.Id_status > -1 && p.SurName.Contains( Isnullfsurname ? p.SurName : fsurname ) && p.Id_User == iduser).Count()
                },
                CurrentUser = iduser,
                FilterSurName = fsurname
            };


            return PartialView(model);
        }



        // *******************************************  Редактирование Abonent Участника **********************
        public ViewResult EditAbonent(int id = 0, int idorder = 0)
        {
            //CRM_Abonent crm_abonent = context.CRM_Abonent.Find( Convert.ToInt64( id ) );

            CRM_Abonent crm_abonent = context.CRM_Abonent.Find(id);

            //int id_order = 0; // Определение id_order
            if (id > 0)
            {
                //CRM_AbKnt crm_abknt = context.CRM_AbKnt.FirstOrDefault(p => p.id_Abonent == id /*id_ab*/);
                //id_order = (int)crm_abknt.id_order;

                ViewData["id_order"] = idorder; // найденное значение
                ViewData["id_ab"] = id;
                //ViewBag.phonenum = 

                return View(crm_abonent);
            }
            else
            {
                ViewData["id_order"] = idorder; // передаваемый параметр

                return View( new CRM_Abonent() );
            }
            
        }

        //************************** Добавить участника заявки *******************************
        public ViewResult CreateAb( int idorder = 0)
        {
            // После "Добавить участника заяки" Варианты
            // "Вернуться к заявке",           return RedirectToAction("EditOrder", "Tst", new { id = id_order }); 
            // "Сохранить" (После добавления), return RedirectToAction("EditOrder", "Tst", new { id = id_order }); 

            return View("EditAbonent", new CRM_Abonent());

            //return RedirectToAction("EditAbonent", "Tst", new { id = 0, idorder = idorder }); 
        }


        // ************************************** Добавить запись *****************************************
        public ViewResult Create()
        {
            ViewBag.idorder = 0;

            //if (crm_tabord.SurName == null)
            //{
            return View("EditOrder", new CRM_TabOrd());
            //}
            //else
            //{
            //    return View("EditOrder", crm_tabord);
            //}


        }


        // ******************************************* Редактирование Order Заявки ************************
        // Отображение для Редактирования сущности 
        public ViewResult EditOrder(int id = 0 /*, CRM_TabOrd par_crm_tabord = null*/)
        {

            CRM_TabOrd crm_tabord = repository.FindRec(id);

            //var routeValues = GetRouteData().Values;

            //// DropDown
            //// ---------------- Формирование ComboBox "Источники заявок" -----------------------
            //SelectList pvalues = new SelectList(context.CRM_SPR_SourceOrder, "Id", "Name", 1);
            //ViewBag.SourceList = pvalues;

            ViewBag.idorder = id;

            //ViewBag.CustomVariable = RouteData.Values["id"];
            //if (par_crm_tabord != null)
            //{
            //    return View(par_crm_tabord);
            //}
            //else
            //{
                return View(crm_tabord);
            //}
        }

        // ****************************** Редактирование ПАРАМЕТРОВ ЗАЯВКИ ************************************
        public ViewResult EditParamsLoan(int idklient = 0, int id_order = 0)
        {
            if (idklient == 0)
            {
                return View();
            }
            else
            {
                CRM_Abonent crm_abonent = context.CRM_Abonent.Find(idklient);

                ViewBag.id_order = id_order;
                //ParamsLoalViewModel paramsLoalViewModel = new ParamsLoalViewModel();
                //paramsLoalViewModel.CRMAbonent = crm_abonent;

                //IEnumerable<CRM_ProductNames_V> crm_productnames_v = context.CRM_ProductNames_V.Where(p => p.Value == crm_abonent.ProductValue);
                if (crm_abonent.ProductValue == 0 || crm_abonent.ProductValue == null )
                    ViewBag.id_product = 44;
                else
                    ViewBag.id_product = crm_abonent.ProductValue;

                // *********** Альтернативный вывод DropDownList ****************
                //CRM_ProductNames_V crm_productnames = context.CRM_ProductNames_V.Find(ViewBag.id_product);
                //paramsLoalViewModel.CRMProductNames = crm_productnames;
                //---------------------------------------------------------------
                SelectList pvalues = new SelectList(context.CRM_ProductNames_V, "Value", "Description", ViewBag.id_product);
                ViewBag.PValues = pvalues;
                // *********** **************** *************** *****************

                //IEnumerable<CRM_ProductNames_V> crm_productnames_v = context.CRM_ProductNames_V;
                //paramsLoalViewModel.CRMProductNamesV = crm_productnames_v;

                return View(crm_abonent);
                //return View(paramsLoalViewModel);
            }
            
        }

        // ==================================== Заявка сформирована =============================
        public ActionResult OrderFormed(int id_order = 0)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur_newSQL;Integrated Security=True");
            conn.Open();
            SqlCommand cmd = new SqlCommand(String.Format("UPDATE CRM_Order SET id_status = 23, Date = '{1}' WHERE id_order = {0}", id_order, DateTime.Now), conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            { }
            conn.Close();
            return RedirectToAction("Index");
        }

        // ====================== Получить id из users по Id из AspNetUsers =========================
        private int GetIdUser()
        {
            string Id = HttpContext.User.Identity.GetUserId();
            ASPNetUsers aspnetusers = context.ASPNetUsers.Find(Id);           
            return aspnetusers.id_user;
        }

        // ========== Отправить на Проверку АИЖК (ПИ) ========================
        public ActionResult SendtoAHML_Property(int id_order = 0)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur_newSQL;Integrated Security=True");
            conn.Open();
            string qstr = String.Format(" Update  CRM_Order SET Id_Status = 40 WHERE Id_Order = {0} ", id_order);

            SqlCommand cmd = new SqlCommand(qstr, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            { }
            conn.Close();
            return RedirectToAction("Index");
           
        }

        // ========== Отправить на Проверку АИЖК (Заемщик) ========================
        public ActionResult SendtoAHML(int id_order = 0)
        {
            int iduser = GetIdUser();
            SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur_newSQL;Integrated Security=True");
            conn.Open();
            //-------------------------------------------------------------------------------------
            int? id_status = 0;

            CRM_TabOrd crm_order = context.CRM_TabOrd.Find(id_order);
            id_status = crm_order.Id_status;

            string qstr = "";
            int Idresp = 0;

            if (id_status == 41) // 41 - Возвращена на доработку (ИСУР)  
            {
                // Найдем кто вернул на доработку и затем его назначить ответственным
                qstr = String.Format("select id_user from crm_HistoryOrder where id_Order = {0} and id_event = 28 " +
                                    " and date = ( select max(date) from crm_HistoryOrder where id_Order = {0} and id_event = 28 ) ", id_order);

                SqlCommand sql = new SqlCommand(qstr, conn);
                SqlDataReader data = sql.ExecuteReader();
                if (data.Read())
                {
                    Idresp = (int)data["id_user"];
                }
                data.Close();

                // В CRM_HistoryOrder кто id_user назначил отв-го, и id_Event = 19 Назначен Ответственный для Проверки АИЖК(ИСУР), id_resp Назначенный ответстсвенным
                qstr = String.Format(" insert  into CRM_HistoryOrder (date, id_event, id_order, id_user, id_resp) " +
                "values ('{0}', {1}, {2}, {3}, {4} ) ", DateTime.Now, 19, id_order, iduser, Idresp /*Найденное значение*/);
                // -------- ПОСМОТРЕТЬ ИЗМЕНЕНИЕ СТАТУСА, чтобы назначился // Проверка АИЖК (Заёмщик) = 38

            }
            else
            {
                // В триггере onInsEvent присваивается CRM_Order.id_status = 2 (сбор документов)
                //string qstr = String.Format(" Update  CRM_Order SET Id_Status = 2, IdStageStatus = 31 WHERE Id_Order = {0} ", this.IdOrd);
                qstr = String.Format(" insert  into CRM_HistoryOrder (date, id_event, id_order, id_user) " +
                                                    "values ('{0}', {1}, {2}, {3} ) ", DateTime.Now,
                                                    25 /*В РаспредЦентр для проверки ИСУР (З-к)*/, id_order, iduser);
            }

            //-------------------------------------------------------------------------------------
            SqlCommand cmd = new SqlCommand(qstr, conn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            { }
            conn.Close();
            return RedirectToAction("Index");

        }

        // ========== Отправить на повторную Предпроврку СБ =======================================
        public ActionResult SendtoPreviewDepSec(int id_order = 0)
        {
            //int iduser = GetIdUser();

            SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur_newSQL;Integrated Security=True");
            conn.Open();
            string qstr = String.Format(" insert  into CRM_HistoryOrder (date, id_event, id_order, id_user, id_resp) " +
                        "values ('{0}', {1}, {2}, {3}, {4} ) ", DateTime.Now, 15, id_order, GetIdUser(), 53);

            SqlCommand cmd = new SqlCommand(qstr, conn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            { }
            conn.Close();
            return RedirectToAction("Index");

        }

        
        //public ActionResult EditParamsLoan(CRM_Abonent crm_abonent, int id_order = 0, string options = "0", int productval = 0 /*using(..productval = @ViewBag.id_product  )*/)
        //public ActionResult EditParamsLoan(ParamsLoalViewModel paramsLoalViewModel, int id_order = 0, string options = "0" /*, int productval = 44*/ /*using(..productval = @ViewBag.id_product  )*/)
        [HttpPost] // -- Сохранение Параметров Заявки в т.ч. Опций
        public ActionResult EditParamsLoan(CRM_Abonent crm_abonent, int id_order = 0, string optionsIn = "0" /*, int productval = 44*/ /*using(..productval = @ViewBag.id_product  )*/)
        {

            //if (crm_abonent.zaim < 500000)
            //{
            //    SelectList pvalues = new SelectList(context.CRM_ProductNames_V, "Value", "Description", ViewBag.id_product); // send 07-ноя
            //    ViewBag.PValues = pvalues; // send 07-ноя

            //    return View(crm_abonent); // send 07-ноя
            //}

            if (ModelState.IsValid) // Необходимые Поля Заполнены
            {

               CRM_Abonent dbEntity = context.CRM_Abonent.Find(crm_abonent.id_ab);
               if (dbEntity != null)
               {
                   dbEntity.zaim = crm_abonent.zaim;
                   dbEntity.perv_vznos = crm_abonent.perv_vznos;
                   dbEntity.price = crm_abonent.price;
                   dbEntity.srok = crm_abonent.srok;
                   dbEntity.dety = crm_abonent.dety;
                   dbEntity.options = optionsIn;
                   dbEntity.Mat_kapital = crm_abonent.Mat_kapital;
                   dbEntity.ProductValue = crm_abonent.ProductValue;

                   dbEntity.dohod1 = crm_abonent.dohod1;
                   dbEntity.dohod2 = crm_abonent.dohod2;
                   dbEntity.dohod3 = crm_abonent.dohod3;
                   dbEntity.dohod4 = crm_abonent.dohod4;

                   dbEntity.alim1 = crm_abonent.alim1;
                   dbEntity.alim2 = crm_abonent.alim2;
                   dbEntity.alim3 = crm_abonent.alim3;
                   dbEntity.alim4 = crm_abonent.alim4;

                   dbEntity.cred1 = crm_abonent.cred1;
                   dbEntity.cred2 = crm_abonent.cred2;
                   dbEntity.cred3 = crm_abonent.cred3;
                   dbEntity.cred4 = crm_abonent.cred4;

                   dbEntity.card1 = crm_abonent.card1;
                   dbEntity.card2 = crm_abonent.card2;
                   dbEntity.card3 = crm_abonent.card3;
                   dbEntity.card4 = crm_abonent.card4;
                   //dbEntity.ProductValue = productval;
                   // dbEntity.ProductValue = Request.Form["productval"];

                   context.SaveChanges();
               }

               ViewBag.idorder = id_order;
               TempData["messagesuccess"] = String.Format("Изменения сохранены");
               return RedirectToAction("EditOrder", "Tst", new { id = id_order });

            }
            else
            {
                //if (ModelState.IsValidField("zaim") == false)
                ViewBag.id_order = id_order;
                //ViewBag.ProductValue = crm_abonent.ProductValue; // rem 07-ноя
                
                //TempData["messagedanger"] = String.Format("Изменения не сохранены");    // rem 07-ноя
                //return RedirectToAction("EditOrder", "Tst", new { id = id_order });     // rem 07-ноя

                // *********** Альтернативный вывод DropDownList ****************
                //CRM_ProductNames_V crm_productnames = context.CRM_ProductNames_V.Find(ViewBag.id_product);
                //paramsLoalViewModel.CRMProductNames = crm_productnames;
                //---------------------------------------------------------------
                SelectList pvalues = new SelectList(context.CRM_ProductNames_V, "Value", "Description", ViewBag.id_product); // send 07-ноя
                ViewBag.PValues = pvalues; // send 07-ноя


                return View(crm_abonent); // send 07-ноя

            }

        }

        // *********************  EditOrder Сохранение внесенных изменений ****************************
        [HttpPost]
        public ActionResult EditOrder(CRM_TabOrd crm_tabord, string ntel, HttpPostedFileBase postedfile = null, int idsourceorder = 0)
        {
            int id_order = (int)crm_tabord.id_Order;


            if (ModelState.IsValid) // Необходимые Поля Заполнены
            {
               
                if ( String.IsNullOrEmpty(crm_tabord.Note) )
                                                        crm_tabord.Note = "";

                // ------- Проверка на совпадени номера телефона. ----Вынести в функцию? -------------------------------------------
                string phonenum =crm_tabord.Phone.Replace("(", "").Replace(")", "").Replace("-", "");

                //SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur20171023;Integrated Security=True");
                SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur_newSQL;Integrated Security=True");

                SqlCommand cmd = new SqlCommand("SELECT dbo.CRM_IdentityOrderByPhone( @idOrder, @Phone)", conn);

                SqlParameter paramIdOrder = new SqlParameter("@idOrder", id_order); cmd.Parameters.Add(paramIdOrder);
                SqlParameter paramPhone = new SqlParameter("@Phone", phonenum); cmd.Parameters.Add(paramPhone);

                conn.Open();
                Int32 identOrder = (Int32)cmd.ExecuteScalar();
                    conn.Close();
                // ----------------------------------------------------------------------------------------------------------------------

                if ( (identOrder > 0) && (crm_tabord.id_Order == 0) )
                {                    
                    TempData["messagedanger"] = String.Format("Клиент с номером {0} участвует в заявке {1}", phonenum, identOrder);
                    return RedirectToAction("Create", "Tst");
                }

                if ( (idsourceorder == 0) && (crm_tabord.id_Order == 0) )
                {
                    TempData["messagedanger"] = "Необходимо указать Источник заявки";
                    //return null;
                    //return Content("Необходимо указать Источник заявки");
                    //return  Redirect("/Tst/Create");
                    //return RedirectToAction("Create", "Tst", new { crm_tabord });
                    
                    //#########################################
                    //return RedirectToAction("EditOrder", "Tst", new { id = 0, par_crm_tabord = crm_tabord });

                    return RedirectToAction("Create", "Tst");
                }

                // --- Добавить сохранение "Согласия на обработку ПД". Поле IAgree (bit) в CRM_Abonent. В AddNewOrder добавить параметр @IAgree-- в CRM_AddNewOrder
                
                string Id = HttpContext.User.Identity.GetUserId();
                ASPNetUsers aspnetusers = context.ASPNetUsers.Find(Id);
                int iduser = aspnetusers.id_user;

                crm_tabord.IdSourceOrder = idsourceorder;

                
                // ************************************************ Сохранение в репозитории *********************************
                id_order = repository.SaveRec(crm_tabord, ntel, iduser, idsourceorder);     // *---------- SaveRec ----------*
                // -----------------------------------------------------------------------------------------------------------

                if (crm_tabord.id_Order == 0)
                {   // Произошло Добавление заявки AddNewOrder
                    TempData["messagesuccess"] = string.Format("Заявка \"{0}\" успешно добавлена", crm_tabord.SurName);
                    ViewBag.idorder = id_order;
                    return RedirectToAction("EditOrder", "Tst", new { id = id_order });
                }
                else
                {   // Произошло Сохранение заявки
                    //SendMessage( String.Format( "Информация по заявке {0} сохранена", id_order) );

                    return RedirectToAction("Index");
                }

            }
            else
            {   // Что-то не так со значениями данных  
                // здесь id_order == 0 
                ViewBag.idorder = id_order;

                TempData["messagedanger"] = String.Format("Ошибка сохранения");
                //return RedirectToAction("Create", "Tst");
                //SendMessage(String.Format("Отмена сохранения по заявке {0}", id_order));

                return View(crm_tabord);  
            }
        }


        //public ActionResult AddSaveRealtor(Realtors realtor)
        //{
        //    return View(realtor);
        //}

        public JsonResult JsonSearch(string idrealtor /*, string idorder*/)
        {
            var jsondata = idrealtor;
            return Json(jsondata, JsonRequestBehavior.AllowGet);

            //var jsondata = context.Realtors.Where(a => a.Name.Contains(name) ).ToList<Realtors>();
            //return Json(jsondata, JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        public ActionResult AddSaveRealtor(int idorder = 0, int idrealtor = 0)
        {

            if ( idorder > 0 && idrealtor > 0 ) // ---------------- ДОБАВЛЕНИЕ РИЭЛТОРА (УЧАСТНИКА ЗАЯВКИ) В ОБРАЩЕНИЯ  -------------------
            {

                SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur_newSQL;Integrated Security=True");

                string Id = HttpContext.User.Identity.GetUserId();
                ASPNetUsers aspnetusers = context.ASPNetUsers.Find(Id);
                int iduser = aspnetusers.id_user;
                //int iduser = 23;

                Realtors realtor = context.Realtors.Find(idrealtor); // Жук
                string[] snm = realtor.Name.Split(' ');

                List<SqlParameter> paramlist = new List<SqlParameter>();
                //***************************************                
                paramlist.Add(new SqlParameter("@flag", "3"));
                paramlist.Add(new SqlParameter("@Id_Order", idorder.ToString()));
                paramlist.Add(new SqlParameter("@Id_ab", "0"));
                paramlist.Add(new SqlParameter("@Id_kont", "0"));
                paramlist.Add(new SqlParameter("@SurName", (snm.Length > 0) ? snm[0] : "" ) );
                paramlist.Add(new SqlParameter("@Name", (snm.Length > 1) ? snm[1] : "" ));
                paramlist.Add(new SqlParameter("@Otch", (snm.Length > 2) ? snm[2] : ""));
                paramlist.Add(new SqlParameter("@Prior", "3")); /*3-риэлтор*/
                paramlist.Add(new SqlParameter("@Note", ""));
                paramlist.Add(new SqlParameter("@Kont", realtor.Phone )); /* КОНТАКТ*/
                paramlist.Add(new SqlParameter("@IdTypeKnt", "-1"));
                paramlist.Add(new SqlParameter("@IdUser", iduser.ToString()));
                paramlist.Add(new SqlParameter("@Cont", ""));
                paramlist.Add(new SqlParameter("@InOut", "0")); /* Исходящий*/
                paramlist.Add(new SqlParameter("@Nt", "")); /*Note для обращения*/
                paramlist.Add(new SqlParameter("@Res", "-2"));
                paramlist.Add(new SqlParameter("@IdRealtor",realtor.Id ));
                //paramlist.Add(new SqlParameter("@BirthDay", crm_abonent.BirthDay));
                //***************************************

                string sqltext = "CRM_AddNewPetit";

                SqlCommand command = new SqlCommand(sqltext, conn);
                command.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in paramlist)
                {
                    command.Parameters.Add(p);
                }

                conn.Open();
                command.ExecuteNonQuery();

                conn.Close();

                TempData["messagesuccess"] = String.Format("Риэлтор успешно добавлен" );

            }

            return RedirectToAction("EditOrder", "Tst", new { id = idorder });

        }

        // =========  Push-уведомление на SignalR =====================================================
        private void SendMessage(string message)
        {
            // Получаем контекст хаба            
            var context =  Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<nHub>();
            // отправляем сообщение
            context.Clients.All.addMessage(message);
        }


        // ================================================================================================


        // *******************************************  Сохранение Abonent Участника **********************
        [HttpPost]
        public ActionResult EditAbonent(CRM_Abonent crm_abonent /*, string phonenum*/,  int idorder = 0, /*, HttpPostedFileBase postedfile = null*/ bool Основной = false)
        {

            if (ModelState.IsValid)
            {
                CRM_Abonent dbEntity;

                dbEntity = context.CRM_Abonent.Find(crm_abonent.id_ab);


                // ------- Проверка на совпадени номера телефона. ----Вынести в функцию? -------------------------------------------
                string phonenum = crm_abonent.Phone.Replace("(", "").Replace(")", "").Replace("-", "");

                //CRM_Abonent new_crm_abonent = new CRM_Abonent(); //dbEntity = context.CRM_Abonent.Add(new_crm_abonent);

                //SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur20171023;Integrated Security=True");
                SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur_newSQL;Integrated Security=True");

                SqlCommand cmd = new SqlCommand("SELECT dbo.CRM_IdentityOrderByPhone( @idOrder, @Phone)", conn);

                SqlParameter paramIdOrder = new SqlParameter("@idOrder", idorder); cmd.Parameters.Add(paramIdOrder);
                SqlParameter paramPhone = new SqlParameter("@Phone", phonenum); cmd.Parameters.Add(paramPhone);

                conn.Open();
                Int32 identOrder = (Int32)cmd.ExecuteScalar();
                conn.Close();
                // ----------------------------------------------------------------------------------------------------------------------
                if (identOrder > 0)
                {
                    //ViewBag.Message = String.Format("Абонент с номером {0} учавствует в заявке {1}", phonenum, identOrder);                        
                    TempData["messagedanger"] = String.Format("Клиент с номером {0} участвует в заявке {1}", phonenum, identOrder);
                    return RedirectToAction("EditOrder", "Tst", new { id = idorder });
                }


                if (dbEntity == null) // ---------------- СОЗДАНИЕ УЧАСТНИКА ЗАЯВКИ ------------------------
                {

                    string Id = HttpContext.User.Identity.GetUserId();
                    ASPNetUsers aspnetusers = context.ASPNetUsers.Find(Id);
                    int iduser = aspnetusers.id_user;
                    //int iduser = 23;

                    //cmd.CommandText = ;
                    //int ident = context.Database.SqlQuery("SELECT dbo.CRM_IdentityOrderByPhone( @idOrder, @Phone)", paramIdOrder, paramPhone);

                    //Через List     
                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    //***************************************                
                    paramlist.Add(new SqlParameter("@flag", "3"));
                    paramlist.Add(new SqlParameter("@Id_Order", idorder.ToString() ));
                    paramlist.Add(new SqlParameter("@Id_ab", "0"));
                    paramlist.Add(new SqlParameter("@Id_kont", "0"));
                    paramlist.Add(new SqlParameter("@SurName", crm_abonent.SurName));
                    paramlist.Add(new SqlParameter("@Name", crm_abonent.Name));
                    paramlist.Add(new SqlParameter("@Otch", crm_abonent.Otch));
                    paramlist.Add(new SqlParameter("@Prior", "2")); /*1-основной, 2-уч-к, 3-риэлтор*/
                    paramlist.Add(new SqlParameter("@Note", ""));
                    paramlist.Add(new SqlParameter("@Kont", phonenum)); /* КОНТАКТ*/ /* КОНТАКТ*//* КОНТАКТ*//* КОНТАКТ*//* КОНТАКТ*//* КОНТАКТ*/
                    paramlist.Add(new SqlParameter("@IdTypeKnt", "-1"));
                    paramlist.Add(new SqlParameter("@IdUser", iduser.ToString() ));
                    paramlist.Add(new SqlParameter("@Cont", ""));
                    paramlist.Add(new SqlParameter("@InOut", "0")); /* Исходящий*/
                    paramlist.Add(new SqlParameter("@Nt", "")); /*Note для обращения*/
                    paramlist.Add(new SqlParameter("@Res", "-2"));
                    paramlist.Add(new SqlParameter("@IdRealtor", "0"));
                    paramlist.Add(new SqlParameter("@BirthDay", crm_abonent.BirthDay));
                    //***************************************

                    string sqltext = "CRM_AddNewPetit";

                    SqlCommand command = new SqlCommand(sqltext, conn);
                    command.CommandType = CommandType.StoredProcedure;

                    foreach (SqlParameter p in paramlist)
                    {
                        command.Parameters.Add(p);
                    }

                    conn.Open();
                    command.ExecuteNonQuery();

                    conn.Close();

                    TempData["messagesuccess"] = String.Format("Участник {0} успешно добавлен", crm_abonent.SurName); 

                    return RedirectToAction("EditOrder", "Tst", new { id = idorder });

                }
                else            // ----------------------- РЕДАКТИРОВАНИЕ ДАННЫХ УЧАСТНИКА ЗАЯВКИ ---------------------------------
                {

                    dbEntity.SurName = crm_abonent.SurName;
                    dbEntity.Name = crm_abonent.Name;
                    dbEntity.Otch = crm_abonent.Otch;
                    dbEntity.BirthDay = crm_abonent.BirthDay;
                    dbEntity.Phone = crm_abonent.Phone.Replace("(","").Replace(")","").Replace("-","");

                    int id_ab = Convert.ToInt32(crm_abonent.id_ab); // подгатавливаем переменную для использования в LINQ
                    Int64 id = context.CRM_Petition.Where(p => p.Id_Abonent == id_ab).Max( p => p.id_Kontakt ); // определяем id в CRM_Kontakt
                    CRM_Kontakt crm_kontakt = context.CRM_Kontakt.Find(id);  // переходим к id в CRM_Kontakt
                    crm_kontakt.Kontakt = dbEntity.Phone;  // обновляем Номер в CRM_Kontakt

                    context.SaveChanges(); // Сохраняем

                    //=================================== Изменение Основного участника заявки  =====================================================
                    if (dbEntity.Prioritet == 2 && Основной  /*Если отмечено "Основной" */ /*&& notuserOdrer == "0"*/)
                    {
                        //// Фиксируем Id_Klient
                        int id_klient;
                        //CRM_TabOrd crm_tabord = context.CRM_TabOrd.Find(idorder);
                        CRM_TabOrd crm_tabord = repository.FindRec(idorder);
                        id_klient = crm_tabord.id_Klient;

                        conn.Open();
                        // Переносим данные Предварительного расчета
                        string qStr = String.Format("Exec CRM_CalcParamOnAbon {0}, {1} ", id_klient, id_ab);
                        SqlCommand qSQL = new SqlCommand(qStr, conn); qSQL.ExecuteNonQuery();

                        // Перезаписываем Id_Klient новым значением
                        qStr = String.Format("UPDATE CRM_Order Set Id_Klient = {0} WHERE id_Order = {1}  ", id_ab, idorder);
                        qSQL = new SqlCommand(qStr, conn); qSQL.ExecuteNonQuery();

                        // Снимаем признак с "Осн."
                        qStr = String.Format("UPDATE CRM_Abonent Set Prioritet = 2 WHERE id_ab in " +
                            "( select Id_Abonent from crm_Petition where id_Petit in (select id_Petit from CRM_PetitOrder where id_order = {0}) ) and Prioritet = 1 "
                            , idorder);
                        qSQL = new SqlCommand(qStr, conn); qSQL.ExecuteNonQuery();
                        // Задаем новому "Осн."  
                        qStr = String.Format("UPDATE CRM_Abonent Set Prioritet = 1 WHERE id_ab = {0}  ", id_ab);
                        qSQL = new SqlCommand(qStr, conn); qSQL.ExecuteNonQuery();

                        conn.Close();

                    }
                    //===============================================================================================================================================

                    TempData["message"] = String.Format("Изменения \"{0}\" сохранены", crm_abonent.SurName);  // Показываем сохранение

                    CRM_AbKnt crm_abknt = context.CRM_AbKnt.FirstOrDefault(p => p.id_Abonent == id_ab); // Определение id_order для возврата EditOrder со списком участников заявки

                    return RedirectToAction("EditOrder", "Tst", new { id = crm_abknt.id_order });
                }
            }
            else 
            {
                ViewData["id_order"] = idorder;
                return View(crm_abonent);
            }

        }


        // ***************************** Удаление документа ***********************************
        public RedirectToRouteResult RemoveDoc(int id_doc, int id_order, int id_ab, string fileName)
        {

            // ------------------------- Удаление записи из таблицы --------------------------
            int deleted = repositorydoc.DeleteDoc(id_doc);

            // ------------------------- Удаление файла с диска -----------------------------            
            string path = String.Format( @"\\192.168.7.23\CRMDocs\{0}\", id_order);
            System.IO.FileInfo fileInf = new System.IO.FileInfo(System.IO.Path.Combine(path, fileName));
            if (fileInf.Exists)
            {
                fileInf.Delete();
            }

            return RedirectToAction("EditAbonent", "Tst", new { idorder = id_order, id = id_ab });
        }


        // ***************************** Удаление обращения ***********************************        
        public RedirectToRouteResult RemoveAbonent(int id_order, int id_petit)
        {
            //CRM_PetitOrder dbEntry = context.CRM_PetitOrder.Find( Convert.ToInt64(id_petit) );
            CRM_PetitOrder dbEntry = context.CRM_PetitOrder.Find( id_petit );

            if (dbEntry != null)
            {
                context.CRM_PetitOrder.Remove(dbEntry);
                context.SaveChanges();
            }

            return RedirectToAction("EditOrder", "Tst", new { id = id_order });
        }


        // ***************************** Добавление файла ***********************************
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, int id_order = 0 , int id_ab = 0, int typedoc = 0 )
        {
            if(upload != null && id_order != 0 && id_ab != 0 )
            {
                string path = String.Format( @"\\192.168.7.23\CRMDocs\{0}\", id_order); // путь по id_order

                // ******* Создаем папку при её отсутствии ********
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }            

                
                string fileName = System.IO.Path.GetFileName(upload.FileName); // получаем имя файла
                string fileExt = System.IO.Path.GetExtension(upload.FileName);  // получаем расширение файла

                // --- Переименование файла если Персональный документ
                if ( typedoc != 0 )
                {
                    SPR_DeloDoc spr_delodoc = context.SPR_DeloDoc.Find(typedoc);
                    CRM_Abonent crm_abonent = context.CRM_Abonent.Find(id_ab);   //Для подстановки ФИО в имя файла

                    fileName = String.Format("{0}_{1} {2} {3}{4}", 
                         spr_delodoc.TemplateNameFile.Replace("ФИО.tif","").Trim(),  // Наименование док-ты
                         crm_abonent.SurName.Trim(), // Фамилия
                         crm_abonent.Name.Trim(),   // Имя
                         crm_abonent.Otch.Trim(),  // Отчество
                         fileExt
                         );
                }
                
                // --- Проверка существования файла с тем же Именем
                CRM_Docs crm_doc_del = context.CRM_Docs.FirstOrDefault(c => c.id_ab == id_ab && c.id_order == id_order && c.name == fileName);

                if (crm_doc_del != null)
                {
                    context.CRM_Docs.Remove(crm_doc_del);
                }
                
                // сохраняем файл в папку Files в проекте
                upload.SaveAs( System.IO.Path.Combine( path, fileName) );

                string Id = HttpContext.User.Identity.GetUserId();
                ASPNetUsers aspnetusers = context.ASPNetUsers.Find(Id);
                int iduser = aspnetusers.id_user;
                //int iduser = 23;

                CRM_Docs crm_doc_new = new CRM_Docs();
                crm_doc_new.id_ab = id_ab;
                crm_doc_new.id_order = id_order;
                crm_doc_new.name = fileName;
                crm_doc_new.path = path;
                crm_doc_new.id_user = iduser;
                crm_doc_new.date = DateTime.Now;
                crm_doc_new.size = upload.ContentLength / 1024;
                crm_doc_new.type_doc = typedoc;

                //crm_doc.type_doc = "Присвоить тип документа";

                context.CRM_Docs.Add (crm_doc_new);

                context.SaveChanges();

            }

            //ViewData["id_order"] = id_order;

            return RedirectToAction("EditAbonent", "Tst", new { idorder = id_order, id = id_ab });

            
        }

        public ViewResult Contact()
        {
            return View();  
        }


        public ViewResult CalcLoan()
        {
            return View();
        }


        [HttpGet]
        public JsonResult ValidateDate(DateTime BirthDay)
        {
            //DateTime parsedDate;
            //if (!DateTime.TryParse(BirthDay, out parsedDate))
            //{
            //    return Json("Введите дату в формате (dd/mm/yyyy)",
            //      JsonRequestBehavior.AllowGet);
            //}
            //else 
            if (DateTime.Now < BirthDay.AddYears(18) )
            {
                return Json("Возраст младше 18 лет",
                  JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        
        // ******** НАБРОСКИ JOIN И ПР.
        //CRM_TabOrd crm_tabord = repository.CRM_TabOrds.FirstOrDefault(c => c.id_Order == id);
        //return PartialView(crm_tabord);

        //// Сыллка на объект не указывает на экземпляр объекта Натравить тест

        ////return View(repository.CRM_TabOrds);
        ////return View( repository.CRM_TabOrds.Where( c => c.id_Order == 132 ) );

        ////return View( context.CRM_Order.Where(c => c.id_Order < 132)  );

        ////crm_OrderAbonentView model = new crm_OrderAbonentView
        ////{
        //    /*crm_OrderAbonents =*/
        //return View(
        //     context.CRM_Order.Where(p => p.id_Order < 100)
        //     .Join(context.CRM_Abonent, // второй набор
        //     p => p.Id_Klient, // свойство-селектор объекта из первого набора
        //     c => c.id_ab, // свойство-селектор объекта из второго набора
        //     (p, c) => new // результат
        //       {
        //            id_order = p.id_Order,
        //            surname = c.Surname,
        //            //surname = c.Surname
        //       }) 
        ////};

        //);

        // === Так будет работать с использованием  IUserIdProvider
        // === http://programmerz.ru/questions/3858/signalr-sending-a-message-to-a-specific-user-using-iuseridprovider-new-2-0-question.html
        //// Получаем Id Пользователя
        //string Id = HttpContext.User.Identity.GetUserId();
        //// отправляем сообщение
        //context.Clients.User(Id).Message(message);


	}
}