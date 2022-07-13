using System;
using FluentAssertions;
using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;
using Xunit;
using Moq;

namespace JobApplicationLibrary.UnitTest;

public class ApplicationEvaluateUnitTest
{
    //UnitOfWork_Condition_ExpectedResult
    [Fact]
    public void Application_WithUnderAge_ShouldTransferredToAutoRejected()
    {
        // Arrange
        var ev = new ApplicationEvaluator(null);

        var form = new JobApplication()
        {
            Applicant = new Applicant()
            {
                Age = 17
            }
        };
        // Act
        var appResult = ev.Evaluate(form);

        // Assert
        // Assert.Equal(ApplicationResult.AutoRejected, appResult);
        appResult.Should().Be(ApplicationResult.AutoRejected);
    }

    [Fact]
    public void Application_WithNoTechStack_ShouldTransferredToAutoRejected()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        mockValidator.DefaultValue = DefaultValue.Mock;
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
        mockValidator.Setup(i => i.IsValid(It.IsAny<string>()))
            .Returns(true);

        // mockValidator.Setup(i => i.IsValid(It.IsAny<string>()))
        //     .Throws<ArgumentNullException>();

        var ev = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication()
        {
            Applicant = new Applicant()
            {
                Age = 40,
            },
            TechStackList = new() {""}
        };
        // Act
        var appResult = ev.Evaluate(form);

        // Assert
        // Assert.Equal(ApplicationResult.AutoRejected, appResult);
        appResult.Should().Be(ApplicationResult.AutoRejected);
    }

    [Fact]
    public void Application_WithTechStackOver75P_ShouldTransferredToAutoAccepted()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
        mockValidator.Setup(i => i.IsValid(It.IsAny<string>()))
            .Returns(true);
        var ev = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication()
        {
            Applicant = new Applicant()
            {
                Age = 40
            },
            TechStackList = new() {"C#", "RabbitMQ", "Microservice", "Visual Studio"},
            YearsOfExperience = 16
        };
        // Act
        var appResult = ev.Evaluate(form);

        // Assert
        // Assert.Equal(ApplicationResult.AutoAccepted, appResult);
        appResult.Should().Be(ApplicationResult.AutoAccepted);
    }

    [Fact]
    public void Application_WithInValidIdentityNumber_TransferredToHr()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);
        mockValidator.DefaultValue = DefaultValue.Mock;
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
        mockValidator.Setup(i => i.IsValid(It.IsAny<string>()))
            .Returns(false);

        var ev = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication()
        {
            Applicant = new Applicant()
            {
                Age = 40
            },
        };
        // Act
        var appResult = ev.Evaluate(form);

        // Assert
        // Assert.Equal(ApplicationResult.TransferredToHR, appResult);
        appResult.Should().Be(ApplicationResult.TransferredToHR);
    }

    [Fact]
    public void Application_WithOfficeLocation_TransferredToCTO()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country)
            .Returns("SPAIN");

        var ev = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication()
        {
            Applicant = new Applicant()
            {
                Age = 40
            },
        };
        // Act
        var appResult = ev.Evaluate(form);

        // Assert
        // Assert.Equal(ApplicationResult.TransferredToCTO, appResult);
        appResult.Should().Be(ApplicationResult.TransferredToCTO);
    }

    [Fact]
    public void Application_WithOver50_ValidationModeDetailed()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);
        mockValidator.SetupAllProperties();
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country)
            .Returns("SPAIN");
        var ev = new ApplicationEvaluator(mockValidator.Object);
        var form = new JobApplication()
        {
            Applicant = new Applicant()
            {
                Age = 51
            },
        };
        // Act
        var appResult = ev.Evaluate(form);

        // Assert
        // Assert.Equal(ValidationMode.Deatiled, mockValidator.Object.ValidationMode);
        mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Deatiled);
    }

    [Fact]
    public void Application_WithNullApplicant_ThrowsArgumentNullException()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);

        var ev = new ApplicationEvaluator(mockValidator.Object);
        var form = new JobApplication();
        // Act
        Action appResultAction = () => ev.Evaluate(form).Should();

        // Assert
        appResultAction.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Application_WithDefaultValue_IsValidCalled()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        mockValidator.DefaultValue = DefaultValue.Mock;
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

        var ev = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication()
        {
            Applicant = new Applicant()
            {
                Age = 40,
                IdentityNumber = "123"
            },
        };
        // Act
        var appResult = ev.Evaluate(form);

        // Assert
        mockValidator.Verify(i => i.IsValid(It.IsAny<string>()),
            "IsValid method should be called with 123");
    }

    [Fact]
    public void Application_WithYoungAge_IsValidNeverCalled()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        mockValidator.DefaultValue = DefaultValue.Mock;
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

        var ev = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication()
        {
            Applicant = new Applicant()
            {
                Age = 15,
            },
        };
        // Act
        var appResult = ev.Evaluate(form);

        // Assert
        mockValidator.Verify(i => i.IsValid(It.IsAny<string>()),
            Times.Exactly(0));
    }
}