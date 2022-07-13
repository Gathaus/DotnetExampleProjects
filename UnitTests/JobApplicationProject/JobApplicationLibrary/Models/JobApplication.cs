namespace JobApplicationLibrary.Models;

public class JobApplication
{
    public Applicant Applicant { get; set; }
    public int YearsOfExperience { get; set; }
    public List<String> TechStackList { get; set; }

}

