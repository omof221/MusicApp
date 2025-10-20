namespace MusicApp.Application.Abstractions;

public interface IOpenAiService
{
    Task<string> ExplainAsync(string userProfile, List<string> songTitles);
}
