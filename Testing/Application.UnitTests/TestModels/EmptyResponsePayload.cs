namespace Application.UnitTests;
/// <summary>
/// A class with no public properties or no properties at all 
/// will serialize into an empty JSON string
/// </summary>
public class EmptyResponsePayload
{
    private string Test = "Test";
}
