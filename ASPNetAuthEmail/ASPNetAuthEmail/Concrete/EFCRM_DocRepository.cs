using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ASPNetAuthEmail.Abstract;
using ASPNetAuthEmail.Models;

namespace ASPNetAuthEmail.Concrete
{
    public class EFCRM_DocRepository : ICRM_DocRepository
    {
        EFDbContext context = new EFDbContext();

        public IEnumerable<CRM_Docs> CRM_DocsRepo
        {
            //get { return context.CRM_Docs_V; }
            get { return context.CRM_Docs; }
            set { ; }
        }

        // Сохранение
        public void SaveDoc(CRM_Docs crm_doc)
        {
            if (crm_doc.id_doc == 0)
                //context.CRM_Docs_V.Add(crm_doc);
                context.CRM_Docs.Add(crm_doc);
            else
            {
                //CRM_Docs dbEntry = context.CRM_Docs_V.Find(crm_doc.id_doc);
                CRM_Docs dbEntry = context.CRM_Docs.Find(crm_doc.id_doc);

          
                if (dbEntry != null)
                {
                    dbEntry.name = crm_doc.name;
                    dbEntry.path = crm_doc.path;
                    dbEntry.id_ab = crm_doc.id_ab;
                    dbEntry.id_order = crm_doc.id_order;
                }
            }
            context.SaveChanges();
        }

        // Удаление
        public int DeleteDoc(int Id_doc)
        {
            //CRM_Docs dbEntry = context.CRM_Docs_V.Find(Id_doc);

            CRM_Docs dbEntry = context.CRM_Docs.Find(Id_doc);

            //CRM_Docs dbEntry = context.CRM_Docs.FirstOrDefault (c => c.id_doc == Id_doc);

            int res = 0;

            if (dbEntry != null)
            {
                //context.CRM_Docs_V.Remove(dbEntry);
                context.CRM_Docs.Remove(dbEntry);
                context.SaveChanges();
                res = 1;
            }

            return res;
        }


        // Поиск
        public CRM_Docs FindDoc(int Id_doc)
        {

            CRM_Docs dbEntry = context.CRM_Docs.Find(Id_doc);

            //CRM_Docs dbEntry = context.CRM_Docs.FirstOrDefault(c => c.id_doc == Id_doc);

            return dbEntry;
        }


    }
}