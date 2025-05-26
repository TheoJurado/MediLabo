namespace Frontend.Models
{
    public static class SessionExtensions
    {
        public static bool IsUserLoggedIn(this ISession session)
        {
            return session.GetString("IsLoggedIn") == "true";
        }

        public static bool IsUserOrganizer(this ISession session)
        {
            return session.GetString("IsOrganizer") == "true";
        }
    }
}
