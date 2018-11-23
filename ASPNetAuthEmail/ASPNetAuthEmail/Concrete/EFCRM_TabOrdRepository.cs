using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using ASPNetAuthEmail.Abstract;
using ASPNetAuthEmail.Models;

using System.Data.SqlClient;

namespace ASPNetAuthEmail.Concrete
{
    public class EFCRM_TabOrdRepository : ICRM_TabOrdRepository
    {
        EFDbContext context = new EFDbContext();

        public IEnumerable<CRM_TabOrd> CRM_TabOrds
        {
            get { return context.CRM_TabOrd; }
            set { ; }
        }

        // Сохранение и Добавление
        public int SaveRec(CRM_TabOrd crm_tabord, string ntel, int id_user, int idsourceorder)
        {
            if (crm_tabord.id_Order == 0)  // Добавление Заявки через хранимую процедуру "AddNewOrder" 
            {

                string phone = crm_tabord.Phone.Replace("(", "").Replace(")", "").Replace("-", "");

                //SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur20171023;Integrated Security=True");
                SqlConnection conn = new SqlConnection(@"Data Source=SERVERKRAFT;Initial Catalog=Ikur_newSQL;Integrated Security=True");

                //Через List     
                List<SqlParameter>  paramlist = new List<SqlParameter>();
                //***************************************                
                paramlist.Add( new SqlParameter("@SurName", crm_tabord.SurName ));
                paramlist.Add( new SqlParameter("@Name", crm_tabord.Name));
                paramlist.Add( new SqlParameter("@Otch", crm_tabord.Otch));
                paramlist.Add(new SqlParameter("@NTel",  phone));
                paramlist.Add( new SqlParameter("@IdUser", id_user.ToString() ));
                paramlist.Add( new SqlParameter("@Note", crm_tabord.Note));
                paramlist.Add(new SqlParameter("@Dety", "0" ) );
                paramlist.Add( new SqlParameter("@Married", "0"));
                paramlist.Add( new SqlParameter("@Dohod", "0"));
                paramlist.Add( new SqlParameter("@Mat_Kapital", "0"));
                paramlist.Add( new SqlParameter("@Zaim", "0"));
                paramlist.Add( new SqlParameter("@Srok", "0"));
                paramlist.Add( new SqlParameter("@PervVz", "0"));
                paramlist.Add(new SqlParameter("@SourceOrder", idsourceorder.ToString() )); /* Источник заявки */
                paramlist.Add( new SqlParameter("@ReasonAdd", "0"));
                paramlist.Add( new SqlParameter("@flSoftLn", "0" ));
                paramlist.Add(new SqlParameter("@BirthDay", crm_tabord.BirthDay)); 
                //***************************************

                string sqltext = "AddNewOrder";

                SqlCommand command = new SqlCommand(sqltext, conn);
                command.CommandType = CommandType.StoredProcedure;

                foreach(SqlParameter p in paramlist) 
                {
                    command.Parameters.Add(p);
                }

                // ---------------- опеределяем выходной параметр
                SqlParameter outIdOrder = new SqlParameter
                {
                    ParameterName = "@outIdOrder", // имя параметра
                    SqlDbType = SqlDbType.Int // тип параметра
                };

                outIdOrder.Direction = ParameterDirection.Output; // указываем, что параметр будет выходным
                command.Parameters.Add(outIdOrder);

                conn.Open();
                command.ExecuteNonQuery();

                conn.Close();
               

                if (outIdOrder.Value.ToString() != "")
                {
                    return (int)outIdOrder.Value;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                // Здесь СДЕЛАТЬ поиск сущностей CRM_Order, CRM_Abonent
                CRM_TabOrd dbEntry = context.CRM_TabOrd.Find(crm_tabord.id_Order);

                if (dbEntry != null)
                {
                    //dbEntry.SurName = crm_tabord.SurName;
                    //dbEntry.Name = crm_tabord.Name;
                    //dbEntry.Otch = crm_tabord.Otch;
                    dbEntry.Note = crm_tabord.Note;

                    //dbEntry.ImageData = game.ImageData;
                    //dbEntry.ImageMimeType = game.ImageMimeType;

                    context.SaveChanges();

                    return (int)dbEntry.id_Order;

                }
                else
                {
                    return 0;
                }
            }
            
        }

        // Удаление
        public CRM_TabOrd DeleteRec(int orderId)
        {
            CRM_TabOrd dbEntry = context.CRM_TabOrd.Find(orderId);
            if (dbEntry != null)
            {
                context.CRM_TabOrd.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }


        // Поиск
        public CRM_TabOrd FindRec(int orderId)
        {
            CRM_TabOrd dbEntry = context.CRM_TabOrd.Find(orderId);
            return dbEntry;
        }

    }
}


// Через List     
//List<SqlParameter>  paramlist = new List<SqlParameter>();
////***************************************                
//paramlist.Add( new SqlParameter("@SurName", crm_tabord.SurName ));
//paramlist.Add( new SqlParameter("@Name", crm_tabord.Name));
//paramlist.Add( new SqlParameter("@Otch", crm_tabord.Otch));
//paramlist.Add( new SqlParameter("@NTel", "9120000000"));
//paramlist.Add( new SqlParameter("@IdUser", 23 /*crm_tabord.Id_User*/));
//paramlist.Add( new SqlParameter("@Note", crm_tabord.Note));
//paramlist.Add( new SqlParameter("@Dety", 0));
//paramlist.Add( new SqlParameter("@Married", ""));
//paramlist.Add( new SqlParameter("@Dohod", 0));
//paramlist.Add( new SqlParameter("@Mat_Kapital", 0));
//paramlist.Add( new SqlParameter("@Zaim", 0));
//paramlist.Add( new SqlParameter("@Srok", 0));
//paramlist.Add( new SqlParameter("@PervVz", 0));
//paramlist.Add( new SqlParameter("@SourceOrder", 0)); /* Источник заявки */
//paramlist.Add( new SqlParameter("@ReasonAdd", 0));
//paramlist.Add( new SqlParameter("@flSoftLn", 0 )); 
////***************************************


//string sqltext = String.Format("Exec AddNewOrder '{0}', '{1}', '{2}', '{3}', {4}, '{5}', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0",
//     crm_tabord.SurName,
//     crm_tabord.Name,
//     crm_tabord.Otch,
//     crm_tabord.Phone.Replace("(","").Replace(")","").Replace("-",""),
//     23,
//     crm_tabord.Note
//    );

//var result = context.Database.
//    ExecuteSqlCommand("Exec AddNewOrder @SurName, @Name, @Otch, @NTel, @IdUser, @Note, @Dety, @Married, @Dohod, @Mat_Kapital, @Zaim, @Srok, @PervVz, @SourceOrder, @ReasonAdd, @flSoftLn",
//        new SqlParameter("@SurName", crm_tabord.SurName),
//        new SqlParameter("@Name", crm_tabord.Name),
//        new SqlParameter("@Otch", crm_tabord.Otch),
//        new SqlParameter("@NTel", crm_tabord.Phone),
//        new SqlParameter("@IdUser", 23 /*crm_tabord.Id_User*/),
//        new SqlParameter("@Note", crm_tabord.Note),
//        new SqlParameter("@Dety", "0"),
//        new SqlParameter("@Married", "0"),
//        new SqlParameter("@Dohod", "0"),
//        new SqlParameter("@Mat_Kapital", "0"),
//        new SqlParameter("@Zaim", "0"),
//        new SqlParameter("@Srok", "0"),
//        new SqlParameter("@PervVz", '0'),
//        new SqlParameter("@SourceOrder", "0"), /* Источник заявки */
//        new SqlParameter("@ReasonAdd", "0"),
//        new SqlParameter("@flSoftLn", "0"),
//        new SqlParameter("@outIdOrder", SqlDbType.Int, null, ParameterDirection.Output,................................)

//    );

