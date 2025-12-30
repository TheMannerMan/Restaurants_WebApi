namespace Restaurants.Infrastructure.Authorization;

public static class PolicyNames
{
    public const string HasNationality = nameof(HasNationality);
    public const string AtLeast20 = nameof(AtLeast20);

}

public static class AppClaimTypes
{
    public const string Nationality = nameof(Nationality);
    public const string DateOfBirth = nameof(DateOfBirth);

}
