using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ASPNetAuthEmail.Models;

namespace ASPNetAuthEmail.Abstract
{
    public interface ICRM_DocRepository
    {
        IEnumerable<CRM_Docs> CRM_DocsRepo { get; set; }
        void SaveDoc(CRM_Docs crm_doc);
        //CRM_Docs DeleteDoc(int orderId);
        int DeleteDoc(int Id_doc);

        CRM_Docs FindDoc(int Id_doc);

    }

}