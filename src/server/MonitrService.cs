using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;

namespace Monitr
{
    public class MonitrService : IHostedService, IDisposable, IObservable<StatsRecord>
    {
        private ICollection<IObserver<StatsRecord>> _observers;
        private System.Timers.Timer _timer;
        private readonly String _args;

        public MonitrService()
        {
            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += GenStats;
            _observers = new List<IObserver<StatsRecord>>();
            var hash = new Dictionary<String, String>
            {
                {"Id", ".ID" },
                {"Name", ".Name" },
                {"CPUPercentage", ".CPUPerc" },
                {"MemoryUsage", ".MemUsage" },
                {"NetIO", ".NetIO" },
                {"BlockIO", ".BlockIO" },
                {"MemoryPercentage", ".MemPerc" },
                {"PIDs", ".PIDs" }
            };
            var stats = hash.Select(kvp => $"\\\"{kvp.Key}\\\":\\\"{{{{ {kvp.Value }}}}}\\\"");
            _args = $"stats --no-stream --format \"{{{String.Join(',', stats)}}}\"";
        }

        private void GenStats(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = _args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            })
            {
                process.Start();
                var results = process.StandardOutput.ReadToEnd().Split('\n');
                process.WaitForExit();

                foreach (var observer in _observers)
                {
                    observer.OnNext(new StatsRecord
                    {
                        Stats = JsonConvert.DeserializeObject<IEnumerable<RawStats>>($"[{String.Join(',', results)}]")
                    });
                }
            }
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Dispose();
            _observers.Clear();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Stop();
            return Task.CompletedTask;
        }

        public IDisposable Subscribe(IObserver<StatsRecord> observer)
        {
            _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ICollection<IObserver<StatsRecord>> _observers;
            private readonly IObserver<StatsRecord> _observer;

            public Unsubscriber(ICollection<IObserver<StatsRecord>> observers, IObserver<StatsRecord> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }

    public class StatsRecord
    {
        public IEnumerable<RawStats> Stats { get; set; }
    }

    public class RawStats
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String CpuPercentage { get; set; }
        public String MemoryUsage { get; set; }
        public String MemoryPercentage { get; set; }
        public String NetIO { get; set; }
        public String BlockIO { get; set; }
        public String PIDs { get; set; }
    }
}
