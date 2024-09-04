using Taggloo4.Web.Contract;

namespace Taggloo4.Web.Translation;

public class DefaultTranslatorConfiguration : ITranslatorConfiguration
{
    public bool IsEnabled { get; } = true;
}