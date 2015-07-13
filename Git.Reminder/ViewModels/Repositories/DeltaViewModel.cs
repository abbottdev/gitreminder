using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace Git.Reminder.ViewModels
{
    public class DeltaViewModel<T> : ReactiveObject
    {
        private ObservableAsPropertyHelper<T> changed;
        private ObservableAsPropertyHelper<T> added;
        private ObservableAsPropertyHelper<T> removed;

        public DeltaViewModel(IObservable<T> changed, IObservable<T> added, IObservable<T> removed)
        {
            this.changed = changed.ToProperty(this, vm => vm.Modified, default(T), Scheduler.Immediate);
            this.added = added.ToProperty(this, vm => vm.Added, default(T), Scheduler.Immediate);
            this.removed = removed.ToProperty(this, vm => vm.Removed, default(T), Scheduler.Immediate);
        }

        public T Modified
        {
            get
            {
                return this.changed.Value;
            }
        }

        public T Added
        {
            get
            {
                return this.added.Value;
            }
        }

        public T Removed
        {
            get
            {
                return this.removed.Value;
            }
        }

    }
}
