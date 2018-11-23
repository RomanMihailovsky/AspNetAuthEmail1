using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Moq;
using Ninject;
using ASPNetAuthEmail.Abstract;
using ASPNetAuthEmail.Concrete;

namespace ASPNetAuthEmail.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            // Здесь размещаются привязки

            kernel.Bind<IOrderRepository>().To<EFOrderRepository>();

            kernel.Bind<ICRM_TabOrdRepository>().To<EFCRM_TabOrdRepository>();

            kernel.Bind<ICRM_DocRepository>().To<EFCRM_DocRepository>();

        }
    }
}