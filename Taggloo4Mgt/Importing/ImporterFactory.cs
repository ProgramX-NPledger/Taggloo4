using Taggloo4Mgt.Importing.Importers;

namespace Taggloo4Mgt.Importing;

public class ImporterFactory
{
    public static IEnumerable<IImporter> GetImporters()
    {
        return new IImporter[]
        {
            new WordImporter(),
            new PhraseImporter(),
            new WordTranslationImporter(),
            new PhraseTranslationImporter(),
            new TranslatedPhraseImporter()
        }.OrderBy(q=>q.Ordinal);
    }
}