using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;


namespace ASPNetAuthEmail.Hubs
{
    
    public class nHub: Hub
    {
        // Отправка сообщений
        public void Send(string message)
        {
            Clients.All.addMessage(message);
        }

        ////Отправляем сообщение пользователю
        //public void SendUserId(string userId, string message)
        //{
        //    Clients.User(userId).Send("User(userId).Send сообщение:" + message);
        //}

    }

    public class myHub : Hub
    {
        ////Отправляем сообщение всем подключенным клиентам
        //public void Message(string Message)
        //{
        //    Clients.All.Message("All сообщение:" + Message);
        //}

        ////Отправляем сообщение пользователю после обращения со стороны клиента
        //public void Send(string userId, string message)
        //{
        //    Clients.All.Message(userId, message);   
        //    //Clients.User(userId).Send("User(userId).Send сообщение:" + message);
        //}
        //public string NameUser;

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            //NameUser = name;
            Groups.Add(Context.ConnectionId, name);

            return base.OnConnected();
        }

        ////Отправляем сообщение пользователю
        //public void SendUser(string userId, string message)
        //{
        //    Clients.User(userId).Send(message);
        //}


    }


}