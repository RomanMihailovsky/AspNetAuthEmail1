using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using ASPNetAuthEmail.Models;


namespace ASPNetAuthEmail.Concrete
{

    public class EFDbContext : DbContext
    {

        public EFDbContext()
            /*: base("Ikur20171023")*/
            : base("Ikur_newSQL")
            
        {    
        }

        // в IdentityModels.cs определение контекста размещения таблиц Доступов Ролей
        // AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserLogin, AspNetClaims
        //public ApplicationDbContext()
        //    : base("DefaultConnection")
        public DbSet<crm_CalcOptions> crm_CalcOptions { get; set; }

        public DbSet<Realtors> Realtors { get; set; }

        public DbSet<ASPNetUsers> ASPNetUsers { get; set; }

        public DbSet<Users> Users { get; set; }
        
        public DbSet<Game> Games { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<CRM_TabOrd> CRM_TabOrd { get; set; }

        public DbSet<CRM_Docs_V> CRM_Docs_V { get; set; }

        public DbSet<CRM_Docs> CRM_Docs { get; set; }

        public DbSet<SPR_DeloDoc> SPR_DeloDoc { get; set; }

        public DbSet<CRM_AbKnt> CRM_AbKnt { get; set; }

        public DbSet<CRM_Abonent> CRM_Abonent { get; set; }

        public DbSet<CRM_Kontakt> CRM_Kontakt { get; set; }

        public DbSet<CRM_Petition> CRM_Petition { get; set; }

        public DbSet<CRM_PetitOrder> CRM_PetitOrder { get; set; }

        public DbSet<CRM_Option> CRM_Option { get; set; }

        public DbSet<CRM_ProductNames_V> CRM_ProductNames_V { get; set; }

        public DbSet<CRM_SPR_SourceOrder> CRM_SPR_SourceOrder { get; set; }

        public DbSet<CRM_CalcLoan> CRM_CalcLoan {get; set;}

        //public DbSet<CRM_OrderAb> CRM_OrderAb { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<crm_Order>()
            //.HasRequired(p => p.crm_Abonent)
            //.HasForeignKey(p => p.Id_Klient);

            // Переопределение первичного ключа Fluent API
            //modelBuilder.Entity<CRM_TabOrd>().HasKey(p => p.id_Order);
            // Сопоставление класса с таблицей
            //modelBuilder.Entity<Phone>().ToTable("Mobiles");


            //// Сопоставление модели с несколькими таблицами
            //modelBuilder.Entity<CRM_OrderAbon>()
            //.Map(m =>
            //{
            //    m.Properties(p => new { p.id_Order, p.Id_Klient });
            //    m.ToTable("CRM_Order");
            //})
            //.Map(m =>
            //{
            //    m.Properties(p => new { p.SurName });
            //    m.ToTable("CRM_Abonent");
            //});


 
            // Связь многие-ко-многим
            //modelBuilder.Entity<Order>()
            //    .HasMany(p => p.Companies)
            //    .WithMany(c => c.Phones)
            //    .Map(m =>
            //    {
            //        m.ToTable("MobileCompanies");
            //        m.MapLeftKey("MobileId");
            //        m.MapRightKey("CompanyId");
            //    }); ;



            // использование Fluent API
            //base.OnModelCreating(modelBuilder);
        }


    }
}
