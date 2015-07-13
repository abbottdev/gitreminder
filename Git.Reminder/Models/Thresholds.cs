using Git.Reminder.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Git.Reminder.Models
{
    public class Thresholds : ReactiveObject
    {

        private Threshold behindThreshold;
        private Threshold aheadThreshold;
        private Threshold linesAddedThreshold;
        private Threshold linesRemovedThreshold;
        private Threshold filesAddedThreshold;
        private Threshold filesRemovedThreshold;

        public Threshold FilesRemoved
        {
            get
            {
                return this.filesRemovedThreshold;
            }
        }

        public Threshold FilesAdded
        {
            get { return filesAddedThreshold; }
        }
 
        public Threshold CommitsBehind
        {
            get
            {
                return this.behindThreshold;
            }
        }

        public Threshold CommitsAhead
        {
            get
            {
                return this.aheadThreshold;
            }
        }

        public Threshold LinesAdded
        {
            get
            {
                return this.linesAddedThreshold;
            }
        }

        public Threshold LinesRemoved
        {
            get
            {
                return this.linesRemovedThreshold;
            }
        }

        public Thresholds(IObservable<int> behind, IObservable<int> commitsAhead, IObservable<int> linesAdded, IObservable<int> linesRemoved, IObservable<int> filesAdded, IObservable<int> filesRemoved)
        {
            this.behindThreshold = new Threshold(behind);
            this.aheadThreshold = new Threshold(commitsAhead);
            this.linesAddedThreshold = new Threshold(linesAdded);
            this.linesRemovedThreshold = new Threshold(linesRemoved);
            this.filesAddedThreshold = new Threshold(filesAdded);
            this.filesRemovedThreshold = new Threshold(filesRemoved);

            LoadThresholds();

        }

        private void LoadThresholds()
        {
            this.LinesAdded.Bad.Min = 80;
            this.LinesAdded.Bad.Max = 9999999;
            this.LinesAdded.OK.Min = 30;
            this.LinesAdded.OK.Max = 79;
            this.LinesAdded.Good.Min = 0;
            this.LinesAdded.Good.Max = 20;

            this.LinesRemoved.Bad.Min = 80;
            this.LinesRemoved.Bad.Max = 9999999;
            this.LinesRemoved.OK.Min = 30;
            this.LinesRemoved.OK.Max = 79;
            this.LinesRemoved.Good.Min = 0;
            this.LinesRemoved.Good.Max = 20;

            this.FilesAdded.Bad.Min = 6;
            this.FilesAdded.Bad.Max = 9999999;
            this.FilesAdded.OK.Min = 2;
            this.FilesAdded.OK.Max = 5;
            this.FilesAdded.Good.Min = 0;
            this.FilesAdded.Good.Max = 2;

            this.FilesRemoved.Bad.Min = 6;
            this.FilesRemoved.Bad.Max = 9999999;
            this.FilesRemoved.OK.Min = 2;
            this.FilesRemoved.OK.Max = 5;
            this.FilesRemoved.Good.Min = 0;
            this.FilesRemoved.Good.Max = 2;

            this.CommitsAhead.Bad.Min = 6;
            this.CommitsAhead.Bad.Max = 9999999;
            this.CommitsAhead.OK.Min = 2;
            this.CommitsAhead.OK.Max = 5;
            this.CommitsAhead.Good.Min = 0;
            this.CommitsAhead.Good.Max = 2;

            this.CommitsBehind.Bad.Min = 6;
            this.CommitsBehind.Bad.Max = 9999999;
            this.CommitsBehind.OK.Min = 2;
            this.CommitsBehind.OK.Max = 5;
            this.CommitsBehind.Good.Min = 0;
            this.CommitsBehind.Good.Max = 2;
            
        }
    }
}
