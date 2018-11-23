using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ASPNetAuthEmail.Models;

namespace ASPNetAuthEmail.Abstract
{
    public interface ICRM_TabOrdRepository
    {

        IEnumerable<CRM_TabOrd> CRM_TabOrds { get; }
        int SaveRec(CRM_TabOrd crm_tabord, string ntel, int id_user, int idsourceorder);
        CRM_TabOrd DeleteRec(int orderId);
        CRM_TabOrd FindRec(int orderId);

    }
}