using FluentAssertions;
using FluentValidation.TestHelper;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Xunit;

namespace Restaurants.Application.Tests.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandValidatorTests
{
    [Fact]
    public void Validator_ForValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateRestaurantCommand
        {
            Name = "Test",
            Description = "A nice place to eat.",
            Category = "Italian",
            ContactEmail = "test@test.com",
            PostalCode = "12-345"
        };

        var validator = new CreateRestaurantCommandValidator();

        //act
        var testResult = validator.TestValidate(command);

        // Assert
        testResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_ForInvalidCommand_ShouldHaveValidationErrors()
    {
        // Arrange
        var command = new CreateRestaurantCommand
        {
            Name = "Te",
            Description = "A nice place to eat.",
            Category = "Italy",
            ContactEmail = "@test.com",
            PostalCode = "12345"
        };

        var validator = new CreateRestaurantCommandValidator();

        //act
        var testResult = validator.TestValidate(command);

        // Assert
        testResult.ShouldHaveValidationErrorFor(c => c.Name);
        testResult.ShouldHaveValidationErrorFor(c => c.Category);
        testResult.ShouldHaveValidationErrorFor(c => c.ContactEmail);
        testResult.ShouldHaveValidationErrorFor(c => c.PostalCode);
    }

    [Theory]
    [InlineData("Italian")]
    [InlineData("Mexican")]
    [InlineData("Japanese")]
    [InlineData("American")]
    [InlineData("Indian")]
    public void Validator_ForValidCategory_ShouldNotHaveValidationErrors(string category)
    {
        // Arrange
        var command = new CreateRestaurantCommand
        {
            Category = category 
        };

        var validator = new CreateRestaurantCommandValidator();

        //act
        var testResult = validator.TestValidate(command);

        // Assert
        testResult.ShouldNotHaveValidationErrorFor(c => c.Category);

    }

    [Theory]
    [InlineData("10220")]
    [InlineData("102-30")]
    [InlineData("10 220")]
    [InlineData("10-2 20")]
    public void Validator_ForInvalidCategory_ShouldHaveValidationErrors(string postalcode)
    {
        // Arrange
        var command = new CreateRestaurantCommand
        {
            PostalCode = postalcode
        };

        var validator = new CreateRestaurantCommandValidator();

        //act
        var testResult = validator.TestValidate(command);

        // Assert
        testResult.ShouldHaveValidationErrorFor(c => c.PostalCode);

    }
}