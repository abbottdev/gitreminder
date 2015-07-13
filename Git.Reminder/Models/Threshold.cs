using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Diagnostics;

namespace Git.Reminder.Models
{
    public class Threshold : ReactiveObject
    {
        private Range bad;
        private Range ok;
        private Range good;
        private ObservableAsPropertyHelper<string> state;

        public string State
        {
            get
            {
                return this.state.Value;
            }
        }

        public Range Good
        {
            get { return good; }
            set { this.RaiseAndSetIfChanged(ref this.good, value); }
        }

        public Range OK
        {
            get { return ok; }
            set { this.RaiseAndSetIfChanged(ref this.ok, value); }
        }

        public Range Bad
        {
            get { return bad; }
            set { this.RaiseAndSetIfChanged(ref this.bad, value); }
        }



        public Threshold(IObservable<int> thresholdState)
        {
            //this.WhenAny(vm => vm.Good, vm => vm.Bad, vm => vm.OK, (g, b, o) =>
            //{

            //});

            this.Good = new Range();
            this.Bad = new Range();
            this.OK = new Range();

            var obs = thresholdState.Select(value =>
               { 
                   if (value.Between(this.Good.Min, this.Good.Max))
                       return "Good";

                   if (value.Between(this.OK.Min, this.ok.Max))
                       return "OK";

                   if (value.Between(this.Bad.Min, this.bad.Max))
                       return "Bad";

                   return "";
               });

            this.state = obs.ToProperty(this, vm => vm.State);
            obs.Subscribe(_ =>
            {
                Trace.WriteLine(_);
            });
            
        }
    }

    public class Range : ReactiveObject
    {
        private int min;
        private int max;

        public int Min
        {
            get { return min; }
            set { this.RaiseAndSetIfChanged(ref this.min, value); }
        }

        public int Max
        {
            get { return max; }
            set { this.RaiseAndSetIfChanged(ref this.max, value); }
        }


    }
}
