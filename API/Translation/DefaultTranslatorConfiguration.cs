using API.Contract;

namespace API.Translation;

public class DefaultTranslatorConfiguration : ITranslatorConfiguration
{
    public bool IsEnabled { get; } = true;
}