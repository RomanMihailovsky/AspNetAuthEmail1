using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ASPNetAuthEmail.Models
{

    //public class CRM_TabOrdDetail
    //{
    //    public CRM_TabOrd CRM_TabOrd { get; set; }

    //    public CRM_Docs CRM_Doc { get; set; }

    //}
   
    public class CRM_TabOrd
    {

        [Display(Name = "Номер заявки")]
        [Key]
        [Required]
        public Int64 id_Order { get; set; }

        [Display(Name = "ID Абонента")]
        public Int32 id_Klient { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата создания")]        
        public DateTime Date { get; set; }

        //[DataType(DataType.Date)]
        //[Display(Name = "Дата назначения")]
        //public DateTime? DateAssigned { get; set; }

        [Display(Name = "Статус")]
        public string Status { get; set; }

        [Display(Name = "Стадия")]
        public string StageStatus { get; set; }

        [Display(Name = "Ответственный")]
        public string UserName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Пожалуйста, укажите фамилию")]
        public string SurName { get; set; }

        //[DataType(DataType.MultilineText)]
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Пожалуйста, укажите имя")]
        public string Name { get; set; }

        [Display(Name = "Отчество")]
        [Required(ErrorMessage = "Пожалуйста, укажите отчество")]
        public string Otch { get; set; }

        [Display(Name = "Контактный номер")]
        [Required(ErrorMessage = "Пожалуйста, укажите номер телефона")]
        //[UIHint("Object")]
        public string Phone { get; set; }

        public int? Id_status { get; set; }

        public bool? Delta { get; set; }

        public int Id_User { get; set; }

        public int? ProductValue { get; set; }

        public string options { get; set; }

        public string FirstNameUs { get; set; } 

        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        [Required(ErrorMessage = "Пожалуйста, укажите дату рождения")]
        public DateTime? BirthDay { get; set; }

        //[Required(ErrorMessage = "Заполните примечание")]
        [Display(Name = "Примечание")]
        public string Note { get; set; }

        public int? PredSolSB { get; set; }

        public int? IdStageStatus { get; set; }

        //[Required(ErrorMessage = "Пожалуйста, укажите Источник заявки")]
        public int? IdSourceOrder { get; set; }

        public int? zaim { get; set; }

        //Повторное объявление публичного свойства не показывается компилятором,
        // но при выполнении на странице возникает ошибка 
        //public int  Id_Klient { get; set; }

    }


    //class dbCntxt : DbContext
    //{
    //    public dbCntxt()
    //        : base("Ikur20171023")
    //    {
    //    }

    //    public DbSet<CRM_TabOrd> CRM_TabOrd { get; set; }
    //}


}