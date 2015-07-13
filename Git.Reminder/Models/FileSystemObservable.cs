using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Git.Reminder.Models
{
    public class FileSystemObservable :  
        IObservable<Tuple<WatcherChangeTypes, string>>
    {
        private FileSystemWatcher fileSystemWatcher;
        private IObservable<FileSystemEventArgs> fileChangeEvents;

        public FileSystemObservable()
        {
            this.fileSystemWatcher = new FileSystemWatcher();
            this.fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            this.fileSystemWatcher.Filter = "*.*";

            var changed = Observable.FromEventPattern<FileSystemEventHandler, object, FileSystemEventArgs>(add => this.fileSystemWatcher.Changed += add, rem => this.fileSystemWatcher.Changed -= rem);
            var created = Observable.FromEventPattern<FileSystemEventHandler, object, FileSystemEventArgs>(add => this.fileSystemWatcher.Created += add, rem => this.fileSystemWatcher.Created -= rem);
            var removed = Observable.FromEventPattern<FileSystemEventHandler, object, FileSystemEventArgs>(add => this.fileSystemWatcher.Deleted += add, rem => this.fileSystemWatcher.Deleted -= rem);

            this.fileChangeEvents = changed.Merge(created).Merge(removed).Select(evt => evt.EventArgs);
        }

        public string Path
        {
            get
            {
                return this.fileSystemWatcher.Path;
            }
            set
            {
                this.fileSystemWatcher.Path = value;
                if (this.fileSystemWatcher.EnableRaisingEvents == false)
                    this.fileSystemWatcher.EnableRaisingEvents = true;
            }
        }


        #region IObservable<Tuple<WatcherChangeTypes,string>> Members

        public IDisposable Subscribe(IObserver<Tuple<WatcherChangeTypes, string>> observer)
        {
            return this.fileChangeEvents.Select(e => new Tuple<WatcherChangeTypes, string>(e.ChangeType, e.FullPath)).Subscribe(observer);
        }

        #endregion
    }
}
