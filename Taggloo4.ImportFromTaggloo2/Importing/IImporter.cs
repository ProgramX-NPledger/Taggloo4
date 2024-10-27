using System.Data.SqlClient;
using Taggloo4Mgt.Importing.Importers;

namespace Taggloo4Mgt.Importing;

public interface IImporter
{
    string Key { get; }
    int Ordinal { get; }

    Task<IImportSession> CreateSession(SqlConnection sqlConnection);
    
}