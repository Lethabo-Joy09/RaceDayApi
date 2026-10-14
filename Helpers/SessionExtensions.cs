using System.Text.Json;

namespace RaceDayApi.Helpers
{
    public static class SessionExtensions
    {
        // Store any object in session as JSON
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Retrieve any object from session as JSON
        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}