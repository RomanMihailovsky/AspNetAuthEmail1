using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

using ASPNetAuthEmail.Abstract;
using ASPNetAuthEmail.Concrete;
using ASPNetAuthEmail.Controllers;
using ASPNetAuthEmail.Models;

using System.Web.Mvc;
using Moq;


namespace ASPNetAuthEmail.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Edit()
        {
            // Arrange. Организация - создание имитированного хранилища данных
            Mock<ICRM_TabOrdRepository> mockCRM_TabOrd = new Mock<ICRM_TabOrdRepository>();
            Mock<ICRM_DocRepository> mockCRM_Doc = new Mock<ICRM_DocRepository>();

            mockCRM_TabOrd.Setup(m => m.CRM_TabOrds).Returns(new List<CRM_TabOrd>
            {
                new CRM_TabOrd { id_Order = 0},
                new CRM_TabOrd { id_Order = 20000}
            });

            // Организация - создание контроллера
            TstController controller = new TstController(mockCRM_TabOrd.Object, mockCRM_Doc.Object );

            // Action. Действие
            CRM_TabOrd crm_tabord1 = controller.EditOrder(0).Model as CRM_TabOrd;
            CRM_TabOrd crm_tabord2 = controller.EditOrder(20000).Model as CRM_TabOrd;

            // Assert. Утверждение
            Assert.AreEqual(0, crm_tabord1.id_Order );
            Assert.AreEqual(20000, crm_tabord2.id_Order );            
        }

        [TestMethod]
        public void Can_RemoveDoc()
        {
            // ************ Arrange. Организация - создание имитированного хранилища данных
            Mock<ICRM_TabOrdRepository> mockCRM_TabOrd = new Mock<ICRM_TabOrdRepository>();
            Mock<ICRM_DocRepository> mockCRM_Doc = new Mock<ICRM_DocRepository>();

            mockCRM_TabOrd.Setup(m => m.CRM_TabOrds).Returns(new List<CRM_TabOrd>
            {
                new CRM_TabOrd { id_Order = 4484}
            });

            mockCRM_Doc.Setup( m => m.CRM_DocsRepo ).Returns(new List<CRM_Docs> 
            {
                new CRM_Docs { id_doc = 6249}
            });

            // ************ Организация - создание контроллера
            TstController controller = new TstController(mockCRM_TabOrd.Object, mockCRM_Doc.Object);

            // ************ Action. Действие
            //CRM_Docs crm_docs = controller.RemoveDoc(6249, 4484).Model as CRM_Docs;

            //int removedoc = controller.RemoveDoc(6249, 4484);

            //int deleted = mockCRM_Doc.Object.DeleteDoc(6249);

            CRM_Docs crm_doc1 = mockCRM_Doc.Object.FindDoc(6249);

            CRM_Docs crm_doc2 = mockCRM_Doc.Object.CRM_DocsRepo.FirstOrDefault(c => c.id_doc == 6249);

            //CRM_TabOrd crm_tabord1 = mockCRM_TabOrd.Object.FindRec(4484);
            
            // ************ Assert. Утверждение
            //Assert.AreEqual(1, removedoc);

            //Assert.AreEqual(1, deleted);

            Assert.AreEqual(6249, crm_doc2.id_doc);

            //Assert.AreEqual( 4484, crm_tabord1.id_Order );

            Assert.AreEqual(6249, crm_doc1.id_doc );

        }

    }
}
