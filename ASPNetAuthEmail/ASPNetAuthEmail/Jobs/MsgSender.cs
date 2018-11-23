using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;

using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

using ASPNetAuthEmail.Hubs;
using ASPNetAuthEmail.Concrete;
using ASPNetAuthEmail.Models;
//using System.Security.Principal;
using System.Threading;

namespace ASPNetAuthEmail.Jobs
{

    //// ========= Использование Quarz ==========================
    public class MsgSender : IJob
    {
        //public HttpContext httpCntxt;
        //EFDbContext dbcntxt = new EFDbContext();
        //HttpContext httpcntxt;

        public async Task Execute(IJobExecutionContext jobcontext)
        {

            JobDataMap dataMap = jobcontext.JobDetail.JobDataMap;

            string usrname = dataMap.GetString("usrname");
            long startdatetime = dataMap.GetLong("startdatetime");

            NewsStatusOrders(usrname, startdatetime);
        }

        // *** запомнить, что по заявке отправлено 
        private void NewsStatusOrders(string usrname, long startdatetime)
        {
            string name = usrname;

            DateTime dtactual = DateTime.Now.AddHours(-6); // 6 часов

            if ( startdatetime < DateTime.Now.AddSeconds(-61).Ticks )
                dtactual = DateTime.Now.AddSeconds(-61);

            using ( EFDbContext dbcntxt = new EFDbContext() )
            {
                var crm_tabord = dbcntxt.CRM_TabOrd.Where(p => p.Date > dtactual && p.FirstNameUs == name);

                foreach (var rec in crm_tabord)
                {
                    string message = String.Format("{5} {6}  изменена Заявка: ({0}) {1} {2} {3}  Статус: {4}", rec.id_Order, rec.SurName, rec.Name, rec.Otch, rec.Status, rec.Date.ToShortDateString(), rec.Date.ToShortTimeString()
                        /*,startdatetime*/
                        );

                    SendMessageUserId(name, message);
                }
            }

            //SendMessageUserId(name, "Контрольное сообщение " + startdatetime.ToString() +"  "+ DateTime.Now.AddSeconds(62).Ticks);

        }

        // =========  Push-уведомление на SignalR =====================================================
        private void SendMessageUserId(string name, string message)
        {
            // Получаем контекст хаба
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<myHub>();

            context.Clients.Group(name).Message(name, message);    // отправляем сообщение ( в myHub есть добавление пользователя в Группу )

        }
    }

    public class MsgScheduler
    {

        public static async void Start(string usrname)
        {

            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            // Имя пользователя получаем один раз из index при старте 
            if (scheduler.IsStarted)
                        return;

            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<MsgSender>()
                                       .UsingJobData("usrname", usrname)
                                       .UsingJobData("startdatetime", DateTime.Now.Ticks )
                                       .Build();

            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                .StartAt( DateTime.Now.AddSeconds(5) )  // запуск сразу после начала выполнения .StartNow() не срабатывает
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInSeconds(60)          // через 1 минуту
                    .RepeatForever())                   // бесконечное повторение
                .Build();                               // создаем триггер

            await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
        }
    }


}