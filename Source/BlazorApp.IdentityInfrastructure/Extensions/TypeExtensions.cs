using System.Reflection;

namespace BlazorApp.CommonInfrastructure.Common.Extensions;

public static class TypeExtensions
{
    public static List<T> GetAllPublicConstantValues<T>(this Type type)
    {
        return type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
            .Select(x => x.GetRawConstantValue())
            .Where(x => x is not null)
            .Cast<T>()
            .ToList();
    }
}