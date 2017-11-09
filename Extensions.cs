using Newtonsoft.Json;

namespace location
{
    internal static class Extensions
    {
        public static string ToJson(this object obj, Formatting formatting = Formatting.Indented)
            => JsonConvert.SerializeObject(obj, formatting);
    }
}