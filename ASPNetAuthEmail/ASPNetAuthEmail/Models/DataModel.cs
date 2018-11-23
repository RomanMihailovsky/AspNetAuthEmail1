using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ASPNetAuthEmail.Models
{

    public class Realtors
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class crm_CalcOptions
    {
        public int id { get; set; }
        public double Value { get; set; }
        public int Type { get; set; }
        public int paramOption { get; set; }
    }


    public class CRM_SPR_SourceOrder
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OptionsList
    {
        public List<string> optList { get; set; }
        public List<Int16> idOptList { get; set; } 
    }

    public class ParamsLoalViewModel
    {
        public CRM_Abonent CRMAbonent { get; set; }
        public IEnumerable<CRM_ProductNames_V> CRMProductNamesV { get; set; }

        public CRM_ProductNames_V CRMProductNames { get; set; }

    }

    public class sqlqueryRet
    {
        string res { get; set; }
    }


    public class Users
    {
        [Key]
        public int id { get; set;}
        public string Name { get; set; }
    }

    public class ASPNetUsers
    {
        [Key]
        public string Id { get; set; }
        public int id_user { get; set; }
    }


    public class CRM_AbKnt
    {
        [Key]
        public int id_petit { get; set; }
        
        public int id_order { get; set; }

        public int id_Abonent { get; set; } // id_Abonent Не уникально
        
        public string SurName { get; set; }
        
        public string Name { get; set; }
        
        public string Otch { get; set; }

        public string Kontakt { get; set; }

        public int Prioritet { get; set; }

        public int? PredSolSB { get; set; }

    }


    public class CRM_Abonent
    {
        [Key]
        [Required]
        public Int64 id_ab { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите фамилию")]
        public string SurName { get; set; }
        
        [Required(ErrorMessage = "Пожалуйста, укажите имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите отчество")]
        public string Otch { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите дату рождения")]
        //[Range(1958, 2001, ErrorMessage = "Недопустимый год")] // Такая проверка для int Year
        //[DataType(DataType.Date)]
        //[Remote("ValidateDate", "Tst")]
        public DateTime? BirthDay { get; set; } ///

        [Required(ErrorMessage = "Пожалуйста, укажите номер")]
        public string Phone { get; set; }

        [Display(Name = "Продукт")]
        public int? ProductValue { get; set; }

	    public int? Mat_kapital { get; set; }  // Сумма МК

        [Required(ErrorMessage = "Пожалуйста, укажите стоимость жилья")]
        [Range(10000, 20000000, ErrorMessage = "Стоимость жилья вне диапазона")]
        public int? price { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите первоначальный взнос")]
        [Range(0, 20000000, ErrorMessage = "Первоначальный взнос вне диапазона")]
	    public int? perv_vznos { get; set; }

        //[Range(500000, 10000000, ErrorMessage = "Сумма займа от 500000 до 10000000 руб.")]
        [Required(ErrorMessage = "Пожалуйста, укажите сумму займа")]
        [Range(10000, 20000000, ErrorMessage = "Сумма займа вне диапазона")]
        public int? zaim{ get; set; }

        //[Range(36, 360, ErrorMessage = "Срок займа 36-360 мес.")]
        [Required(ErrorMessage = "Пожалуйста, укажите срок займа")]
        [Range(3, 360, ErrorMessage = "Срок займа вне диапазона")]
        public int? srok{ get; set; }

        public int? dety{ get; set; }

        public string options { get; set; }

        public int Prioritet { get; set; }

        // В виде полей пар-ров заявки dohod1..dohod4 удобны для БЫСТРОГО расчета
        // Но кривая привязка к ДР, хотя заполнение
        public int? dohod1 { get; set; }
        public int? dohod2 { get; set; }
        public int? dohod3 { get; set; }
        public int? dohod4 { get; set; }

        public int? alim1 { get; set; }
        public int? alim2 { get; set; }
        public int? alim3 { get; set; }
        public int? alim4 { get; set; }

        public int? cred1 { get; set; }
        public int? cred2 { get; set; }
        public int? cred3 { get; set; }
        public int? cred4 { get; set; }

        public int? card1 { get; set; }
        public int? card2 { get; set; }
        public int? card3 { get; set; }
        public int? card4 { get; set; }

        //public DateTime? BirthDay { get; set; }

    }


    public class CRM_Kontakt
    {
        [Key]
        public Int64 id { get; set; }

        [Required(ErrorMessage = "Пожалуйста, номер телефона")]
        public string Kontakt { get; set; }
    }


    public class CRM_Petition
    {
        [Key]
        public int id_Petit { get; set; }

        public int Id_Abonent { get; set; }

        public int id_Kontakt { get; set; }
    }


    public class CRM_PetitOrder
    {
        [Key]
        public int id_Petit { get; set; }

        public int Id_Order { get; set; }

    }

    // Опции продукта
    public class CRM_Option
    {
        [Key]
        public Int16 id_Option { get; set; }

        public string NameOp { get; set; }        

        public Int16 id_Product { get; set; }
    }


    public class CRM_ProductNames_V
    {
        [Key]
        public int Value { get; set; }
        public string Description { get; set; }
    }


    public class CRM_CalcLoan
    { 
        [Key]
        public Int64 id_ab { get; set; }
        public int ProductValue { get; set; }
        public string ProductName { get; set; }
        public int cntPerson { get; set; }
        public string options { get; set; }

        public double stavka { get; set; }
        public double HiStavka { get; set; }

        public string perv_vznos { get; set; }
        public string zaim { get; set; }
        public string price { get; set; }
        public string srok { get; set; }
        public int? srokFirst { get; set; }

        public int dohod1 { get; set; }
        public int dohod2 { get; set; }
        public int dohod3 { get; set; }
        public int dohod4 { get; set; }

        public int alim1 { get; set; }
        public int alim2 { get; set; }
        public int alim3 { get; set; }
        public int alim4 { get; set; }

        public int cred1 { get; set; }
        public int cred2 { get; set; }
        public int cred3 { get; set; }
        public int cred4 { get; set; }

        public int card1 { get; set; }
        public int card2 { get; set; }
        public int card3 { get; set; }
        public int card4 { get; set; }

        public int rashod1 { get; set; }
        public int rashod2 { get; set; }
        public int rashod3 { get; set; }
        public int rashod4 { get; set; }

        public int dety { get; set; }

        public double AnSrok { get; set; }        
        public double An180 {get; set;}
        public double An240 {get; set;}
        public double An300 {get; set;}
        public double An360 {get; set;}
		// Аннуитет с Повышенной ставкой (для КПК, для СИ) 	-- Аннуитет после получения МК для МК, т.е. 
        public double AnSrok_Chg {get; set;}
        public double An180_Chg {get; set;}
        public double An240_Chg {get; set;}
        public double An300_Chg {get; set;}
        public double An360_Chg {get; set;}
		// Сумма плановых процентов
        public double SppSr {get; set;}
        public double Spp180 {get; set;}
        public double Spp240 {get; set;}
        public double Spp300 {get; set;}
        public double Spp360 {get; set;}
		// Сумма плановых процентов (СИ, СИ КПК)
        public double SppSr_Fml {get; set;}
        public double Spp180_Fml {get; set;}
        public double Spp240_Fml {get; set;}
        public double Spp300_Fml {get; set;}
        public double Spp360_Fml {get; set;}
		// Сумма плановых процентов (МК)
        public double SppSr_MK {get; set;}
        public double Spp180_MK {get; set;}
        public double Spp240_MK {get; set;}
        public double Spp300_MK {get; set;}
        public double Spp360_MK { get; set; }

        public double SppSr_Diff { get; set; }

        public int mat_kapital { get; set; }

        public double Strah { get; set; }

        public string userName { get; set; }
        public string userIPNum { get; set; }
        public string userEmail { get; set; }

        public double Pay_59_Diff { get; set; }
        public double OnceDebt { get; set; }


    
    }




    //public class CRM_Doc
    //{
    //    [Key]
    //    public int id_doc { get; set; }
    //    public int id_order { get; set; }
    //    public int id_ab { get; set; }
    //    public string name { get; set; }
    //    public string path { get; set; }
    //}

    //// CRM_Order 
    //public class crm_Order
    //{
    //    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public Int64 id_Order { get; set; }
    //    public int Id_status { get; set; }

    //    public Int64 Id_Klient { get; set; }
    //    [ForeignKey("Id_Klient")]
    //    public crm_Abonent crm_Abonent { get; set; }
    //}

    //// CRM_Abonent
    //public class crm_Abonent
    //{
    //    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public Int64 id_ab { get; set; }
    //    public string Surname { get; set; }
    //    public string Name { get; set; }
    //    public string Otch { get; set; }
    //}

    //// crm_OrderAbon
    //public class CRM_OrderAbon
    //{
    //    [Key]
    //    public Int64 id_Order { get; set; }

    //    [ForeignKey("Id_Klient")]
    //    public Int64 Id_Klient { get; set; }

    //    [Key]
    //    public Int64 id_ab { get; set; }
    //    public string SurName { get; set; }
    //}


    //// crm_OrderAbonentView
    //public class CRM_OrderAbonViewModel
    //{
    //    public IEnumerable<CRM_OrderAbon> CRM_OrderAbons { get; set; }
    //}


    //class dbCntxt : DbContext
    //{
    //    public dbCntxt()
    //        : base("Ikur20171023")
    //    {
    //    }

    //    //---------------------------------------------
    //    public DbSet<crm_Order> CRM_Order { get; set; }
    //    public DbSet<crm_Abonent> CRM_Abonent { get; set; }
    //}

    
}