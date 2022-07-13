using JobApplicationLibrary.Models;

namespace JobApplicationLibrary.Services;

public interface IIdentityValidator
{
    public bool IsValid(string identityNumber);

    ICountryDataProvider CountryDataProvider { get; }

    // bool CheckConnectionToRemoteServer();
    public ValidationMode ValidationMode { get; set; }
}

public interface ICountryData
{
    string Country { get; }
}

public interface ICountryDataProvider
{
    ICountryData CountryData { get; }
}

public enum ValidationMode
{
    Deatiled = 0,
    Quick = 1
}