using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNetAuthEmail.Models
{

    public class CRM_Docs
    {
        [Key]
        public int id_doc { get; set; }
        public int id_order { get; set; }
        public int id_ab { get; set; }

        [Display(Name = "Наименование документа")]
        public string name { get; set; }

        public string path { get; set; }

        public DateTime? date { get; set; }
        public int size { get; set; }

        public int type_doc { get; set; }

        public int? id_user { get; set; }
        //[ForeignKey("type_doc")]
        //public SPR_DeloDoc spr_delodoc { get; set; }
    }


    public class CRM_Docs_V
    {
        [Key]
        [Required]
        public int id_doc { get; set; }
        public int id_order { get; set; }
        public int id_ab { get; set; }

        [Display(Name = "Наименование документа")]
        public string name { get; set; }

        public string path { get; set; }

        public DateTime? date { get; set; }
        public int size { get; set; }

        public int type_doc { get; set; }

        public int? id_user { get; set; }

        public string description { get; set; }
        
    }


    public class CRM_DocsViewModel
    {
        public CRM_Docs CRM_Docs { get; set; }

        //public IEnumerable<CRM_Docs> CRM_Docs { get; set; }

        public string ReturnUrl { get; set; }
    }

    // Справочник типов документов
    public class SPR_DeloDoc
    {
        [Key]
        public int id_spr { get; set; }

        public string description { get; set; }

        public Int64? status { get; set; }

        public string TemplateNameFile { get; set; }
    }


}