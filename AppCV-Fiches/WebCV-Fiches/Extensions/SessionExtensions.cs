using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Http
{
    public static class SessionExtensions
    {
        public static void Set<T>(this Microsoft.AspNetCore.Http.ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this Microsoft.AspNetCore.Http.ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
