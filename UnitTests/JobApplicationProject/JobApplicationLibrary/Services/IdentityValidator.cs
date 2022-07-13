namespace JobApplicationLibrary.Services;

public class IdentiyValidator: IIdentityValidator
{
    public bool IsValid(string identityNumber)
    {
        return true;
    }

    public ICountryDataProvider CountryDataProvider { get; }
    public ValidationMode ValidationMode { get; set; }

    public string Country { get; }

    public bool CheckConnectionToRemoteServer()
    {
        throw new NotSupportedException();
    }
}

