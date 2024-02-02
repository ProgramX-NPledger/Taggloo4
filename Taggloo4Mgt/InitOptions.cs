using CommandLine;

namespace Taggloo4Mgt;

/*
 * Initialising a Taggloo instance is a one time only utility.
 * It must set a configuration to prevent subsequent attempts when a Password isn't provided
 * Initialisation performs a migration and can be executed
 * 
 */
[Verb("init", HelpText = "Initialise a Taggloo4 instance")]
public class InitOptions
{
    [Option(HelpText="Url of Taggloo 4 API",Required = true)]
    public required string Url { get; set; }
    
    [Option(HelpText = "Log processing")]
    public bool Log { get; set; } = false;

}