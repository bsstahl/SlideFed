using SlideFed.PublishCli;
using SlideFed.PublishCore;

if (args.Length == 0)
{
	await Console.Error.WriteLineAsync("Usage: SlideFed.PublishCli <slide-deck-guid> [--repository <yaml-repo-path>] [--actor <actor-id>]").ConfigureAwait(false);
	return 2;
}

var deckIdentifier = args[0];
var actor = ParseActor(args) ?? "local-presenter";
var repositoryPath = ParseRepositoryPath(args) ?? Environment.GetEnvironmentVariable("LIQUIDVICTOR_YAML_REPOSITORY");

if (string.IsNullOrWhiteSpace(repositoryPath))
{
	await Console.Error.WriteLineAsync("A YAML repository path is required. Use --repository or set LIQUIDVICTOR_YAML_REPOSITORY.")
		.ConfigureAwait(false);
	return 2;
}

var deckLoader = new YamlDeckSourceLoader(repositoryPath);
var activityPublisher = new ConsoleActivityPublisher();
var publisher = new DeckPackagePublisher(deckLoader, activityPublisher);

try
{
	var summary = await publisher.PublishAsync(new PublishRequest(deckIdentifier, actor)).ConfigureAwait(false);
	await Console.Out.WriteLineAsync(
			$"Published deck '{summary.DeckId}' with {summary.PublishedSlideCount} slides and {summary.PublishedContentItemCount} content items.")
		.ConfigureAwait(false);
	return 0;
}
catch (DeckPublishValidationException ex)
{
	await Console.Error.WriteLineAsync($"Validation failed: {ex.Message}").ConfigureAwait(false);
	return 1;
}

static string? ParseActor(IReadOnlyList<string> arguments)
{
	return ParseOption(arguments, "--actor");
}

static string? ParseRepositoryPath(IReadOnlyList<string> arguments)
{
	return ParseOption(arguments, "--repository");
}

static string? ParseOption(IReadOnlyList<string> arguments, string optionName)
{
	for (var i = 1; i < arguments.Count - 1; i++)
	{
		if (string.Equals(arguments[i], optionName, StringComparison.Ordinal))
		{
			return arguments[i + 1];
		}
	}

	return null;
}
