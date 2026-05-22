namespace GbIrl.Spec.Tests.Support;

/// <summary>
/// Expected Razor Page routes per assessment spec (TESTS.md).
/// </summary>
public static class AppRoutes
{
    public const string Upload = "/Upload";
    public const string Items = "/Items";

    public static string Edit(int id) => $"/Items/Edit/{id}";
}
