using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;

namespace JobApplicationLibrary;

public class ApplicationEvaluator
{
    private const int MinAge = 18;
    private List<string> TechStackList = new() {"C#", "RabbitMQ", "Microservice", "Visual Studio"};
    private const int AutoAcceptedYearsOfExperience = 15;
    private readonly IIdentityValidator identityValidator;

    public ApplicationEvaluator(IIdentityValidator identityValidator)
    {
        this.identityValidator = identityValidator;
    }

    public ApplicationResult Evaluate(JobApplication form)
    {
        if (form.Applicant is null)
            throw new ArgumentNullException("HATA!");
        
        if (form.Applicant.Age < MinAge)
            return ApplicationResult.AutoRejected;

        if (identityValidator.CountryDataProvider.CountryData.Country != "TURKEY")
            return ApplicationResult.TransferredToCTO;

        identityValidator.ValidationMode = form.Applicant.Age > 50 ? ValidationMode.Deatiled : ValidationMode.Quick;
        
        var validIdentity = identityValidator.IsValid(form.Applicant.IdentityNumber);

        if (!validIdentity)
            return ApplicationResult.TransferredToHR;

        var sr = GetTechStackSimilarityRate(form.TechStackList);
        if (sr < 25)
            return ApplicationResult.AutoRejected;

        if (sr > 75 && form.YearsOfExperience > AutoAcceptedYearsOfExperience)
            return ApplicationResult.AutoAccepted;

        return ApplicationResult.AutoAccepted;
    }

    private int GetTechStackSimilarityRate(List<string> techStacks)
    {
        var matchedCount = techStacks
            .Where(i => TechStackList.Contains(i, StringComparer.OrdinalIgnoreCase))
            .Count();

        return (int) ((double) matchedCount / TechStackList.Count) * 100;
    }
}

public enum ApplicationResult
{
    AutoRejected,
    TransferredToHR,
    TransferredToLead,
    TransferredToCTO,
    AutoAccepted
}