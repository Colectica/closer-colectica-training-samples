using Algenta.Colectica.Model.Ddi;
using Algenta.Colectica.Model.Repository;
using Algenta.Colectica.Model.Utility;
using Algenta.Colectica.Repository.Client;

// Set up the connection to the repository.
var settings = new RepositoryConnectionInfo();
settings.TransportMethod = RepositoryTransportMethod.REST;
settings.AuthenticationMethod = RepositoryAuthenticationMethod.UserName;
settings.Url = "https://closer.sandbox.colectica.org/";
settings.UserName = "workshop@colectica.com";
settings.Password = "CLOSERworkshop2022";
var client = new RestRepositoryClient(settings);


// Call the /info/ API and output some basic information.
var info = client.GetRepositoryInfo();
Console.WriteLine("Admin? " + info.CanAdministrator);


// Search for a concept.
var facet = new SearchFacet();
facet.ItemTypes.Add(DdiItemType.Concept);
facet.SearchTerms.Add("Retirement");
SearchResponse searchResponse = client.Search(facet);

Console.WriteLine($"Got {searchResponse.Results.Count} results.");
var conceptResult = searchResponse.Results[0];
Console.WriteLine(conceptResult.DisplayLabel);

// For that concept, get all related variable groups.
var graphFacet = new GraphSearchFacet();
graphFacet.TargetItem = conceptResult.CompositeId;
graphFacet.ItemTypes.Add(DdiItemType.VariableGroup);
var variableGroupResult = client.GetRepositoryItemDescriptionsByObject(graphFacet);

Console.WriteLine(variableGroupResult.Count);

//
// For each variable group
foreach (var group in variableGroupResult)
{
    // Search for items that reference this variable group.
    var setFacet = new SetSearchFacet();
    setFacet.ReverseTraversal = true;
    var setResult = client.SearchTypedSet(group.CompositeId, setFacet);
    var setDescriptions = client.GetRepositoryItemDescriptions(setResult.ToIdentifierCollection());
    var formatter = new DdiItemFormatter();

    // Output information about each item.
    string setSummary = string.Join("\n", 
        setDescriptions.Select(x => x.DisplayLabel + " " + formatter.GetLabelForItemType(x.ItemType))
    );

    Console.WriteLine("Group: " + group.DisplayLabel);
    Console.WriteLine(setSummary);
    Console.WriteLine("----------------------------------------");

    var variableGroup = client.GetItem(group.CompositeId, ChildReferenceProcessing.Populate) as VariableGroup;

    // Find any Questions and CodeLists that are referenced by Variables within this VariableGroup.
    var questionSetFacet = new SetSearchFacet();
    questionSetFacet.ItemTypes.Add(DdiItemType.QuestionItem);
    questionSetFacet.LeafItemTypes.Add(DdiItemType.CodeList);
    var allQuestionIds = client.SearchTypedSet(variableGroup.CompositeId, questionSetFacet);

    // Output information about each Question we found.
    var questionMetadata = client.GetRepositoryItemDescriptions(allQuestionIds.ToIdentifierCollection());
    foreach (var q in questionMetadata)
    {
        Console.WriteLine(q.ItemName.Best + " - " + q.Summary.Best);
    }

    Console.WriteLine("----------------------------------------");
}

