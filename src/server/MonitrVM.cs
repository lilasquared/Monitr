using System;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;

namespace Monitr
{
    public class MonitrVM : MulticastVM
    {
        IDisposable _subscription;

        public MonitrVM(IObservable<StatsRecord> observable)
        {
            var stats = AddProperty("Stats", Enumerable.Empty<RawStats>());

            _subscription = observable.Subscribe(record =>
            {
                stats.Value = record.Stats;
                PushUpdates();
            });
        }

        public override void Dispose()
        {
            base.Dispose();
            _subscription.Dispose();
        }
    }
}
