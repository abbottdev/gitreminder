using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace System.Reactive.Linq
{
    public static class ObservableExtensions
    {
        public static IObservable<int> AnonymousMerge<T, T1, T2, T3, T4>(IObservable<T1> obs1, IObservable<T2> obs2, IObservable<T3> obs3, IObservable<T4> obs4)
        {
            return obs1.AnonymousMerge(obs2, 0).AnonymousMerge(obs3, 0).AnonymousMerge(obs4, 0);
        }

        public static IObservable<T> AnonymousMerge<T, T1, T2>(this IObservable<T1> obs1, IObservable<T2> obs2, T value)
        {
            return obs1
                .Select(a => value)
                .Merge(obs2.Select(b => value));
        }

        public static IObservable<T> ThrottleFirst<T>(this IObservable<T> source, TimeSpan duration)
        {
            return source.Publish(p => p.Take(1).Concat(p.Throttle(duration).Take(1)).Repeat());    
        }

        public static IObservable<T> SampleEx<T, S>(this IObservable<T> source, IObservable<S> samples)
        {
            // This is different from the Rx version in that source elements will be repeated, and that
            // we complete when either sequence ends. 
            return Observable.Create((IObserver<T> obs) =>
            {
                object gate = new object();
                bool hasSource = false;
                var value = default(T);

                return new CompositeDisposable(
                    source.Synchronize(gate).Do(v => { value = v; hasSource = true; }).Subscribe(obs),
                    samples.Synchronize(gate).Subscribe(_ => { if (hasSource) obs.OnNext(value); })
                );
            });
        }

    }
}
