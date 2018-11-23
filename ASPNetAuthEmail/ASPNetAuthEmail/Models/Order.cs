using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations ;
using System.Web.Mvc;

namespace ASPNetAuthEmail.Models
{
    public class Order
    {

        //[HiddenInput(DisplayValue = false)]
        //[Column("id_Order")]

        [Display(Name = "Номер заявки")]
        [Key]
        public int OrderId { get; set; }

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

        [Display(Name = "Телефон")]
        [DataType(DataType.PhoneNumber)]        
        [Required(ErrorMessage = "Пожалуйста, укажите номер телефона")]
        public string Phone { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int IdRealtor { get; set; }

        // [DataType(DataType.Date)]  // Формат Даты

        //public byte[] fileData { get; set; }
        //public string fileMimeType { get; set; }

    }
}