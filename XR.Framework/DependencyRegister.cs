/*********************************************************************************** 
*        Cteate by :   xierui
*        Date      :   2016/9/20 11:34:58 
*        Desc      :   公共IOC注册方法
************************************************************************************/
using System.Reflection;
using System.Web.Compilation;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using XR.BL.Core;
using XR.BL.Core.Data;
using XR.BL.Data;

namespace XR.Framework
{
    public class DependencyRegister
    {
        public static void Register()
        {
            var builder = RegisterService();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }

        public static ContainerBuilder RegisterService()
        {
            var builder = new ContainerBuilder();


            var baseType = typeof(IDependency);

            var assemblys = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
            var dataAccess = Assembly.GetExecutingAssembly();

            var AllServices = assemblys
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && p != baseType);
            //注入加载所有的继承了IDependency接口的类
            builder.RegisterControllers(assemblys.ToArray());

            builder.RegisterAssemblyTypes(assemblys.ToArray())
                   .Where(t => baseType.IsAssignableFrom(t) && t != baseType)
                   .AsImplementedInterfaces().InstancePerLifetimeScope();
            //泛型单独注册
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            return builder;
        }
    }
}
