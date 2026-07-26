using SlideFed.PublishCore;

namespace SlideFed.PublishCli;

internal sealed class ConsoleActivityPublisher : IActivityPublisher
{
    public async Task PublishCreateAsync(PublishObject activityObject, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Console.Out.WriteLineAsync($"Create {activityObject.Kind}: {activityObject.Id}").ConfigureAwait(false);
    }
}
