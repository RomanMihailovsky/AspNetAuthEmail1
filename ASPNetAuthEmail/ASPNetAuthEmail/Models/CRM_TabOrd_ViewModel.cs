using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPNetAuthEmail.Models
{
    public class CRM_TabOrd_ViewModel
    {
        public IEnumerable<CRM_TabOrd> CRM_TabOrds { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public int CurrentUser { get; set; }
        public string FilterSurName { get; set; }
    }
}