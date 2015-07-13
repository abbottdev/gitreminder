using Ninject;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Git.Reminder
{
    //public class NinjectResolver : IMutableDependencyResolver
    //{
    //    private IKernel kernel;
    //    Dictionary<Tuple<Type, string>, List<Func<object>>> _registry;
    //    Dictionary<Tuple<Type, string>, List<Action<IDisposable>>> _callbackRegistry;


    //    public NinjectResolver(IKernel kernel)
    //    {
    //        this.kernel = kernel;

    //        _registry = new Dictionary<Tuple<Type, string>, List<Func<object>>>();

    //        _callbackRegistry = new Dictionary<Tuple<Type, string>, List<Action<IDisposable>>>();

    //    }

    //    #region IDependencyResolver Members

    //    public object GetService(Type serviceType, string contract = null)
    //    {
    //        if (string.IsNullOrEmpty(contract))
    //        {
    //            return this.kernel.Get(serviceType);
    //        }
    //        else
    //        {
    //            return this.kernel.Get(serviceType, contract);
    //        }
    //    }

    //    public IEnumerable<object> GetServices(Type serviceType, string contract = null)
    //    {
    //        if (string.IsNullOrEmpty(contract))
    //        {
    //            return this.kernel.GetAll(serviceType);
    //        }
    //        else
    //        {
    //            return this.kernel.GetAll(serviceType, contract);
    //        }
    //    }

    //    #endregion

    //    #region IDisposable Members

    //    public void Dispose()
    //    {

    //    }

    //    #endregion


    //    #region IMutableDependencyResolver Members

    //    public void Register(Func<object> factory, Type serviceType, string contract = null)
    //    {
    //        if (string.IsNullOrEmpty(contract))
    //        {
    //            this.kernel.Bind(serviceType).ToMethod((ctx) => factory());
    //        }
    //        else
    //        {
    //            this.kernel.Bind(serviceType).ToMethod((ctx) => factory()).Named(contract);
    //        }
    //    }

    //    public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
    //    {
    //        return Disposable.Empty;
    //    }

    //    #endregion
    //}

}
