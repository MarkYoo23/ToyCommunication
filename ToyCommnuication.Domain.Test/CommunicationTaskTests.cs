using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ToyCommnuication.Domain.Test
{
    public class CommunicationTaskTests
    {
        [Fact]
        public async Task ShouldCancleTask()
        {
            var isRunTask = false;
            var verylongMilies = 1000;
            var veryshortMilies = 1;

            var cts = new CancellationTokenSource();
            var job = Task.Run(() => 
            {
                isRunTask = false;
                Task.Delay(verylongMilies); 
            }, cts.Token);

            if (job != await Task.WhenAny(job, Task.Delay(veryshortMilies)))
            {
                cts.Cancel();
                job.Wait();
            }

            Assert.False(isRunTask);
        }
    }
}
