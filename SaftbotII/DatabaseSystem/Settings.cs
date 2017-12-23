namespace SaftbotII.DatabaseSystem
{
    /// <summary>
    /// Settings unique to each server
    /// </summary>
    internal enum ServerSettings
    {
        PlebsCanDJ,
        UseGoogle
    }

    /// <summary>
    /// Settings unique to each user
    /// </summary>
    internal enum UserSettings
    {
        DJ,
        Admin,
        Ignored
    }
}
