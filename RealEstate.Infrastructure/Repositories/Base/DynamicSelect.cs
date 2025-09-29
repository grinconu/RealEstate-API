using System.Linq.Dynamic.Core;
using System.Reflection;

namespace RealEstate.Infrastructure.Repositories.Base;

public static class DynamicSelect
{
    private static readonly List<string> BasicTypes =
        ["string", "byte", "int16", "int32", "int", "double", "boolean", "guid", "datetime", "timespan", "single", "nullable`1"];
    public static IQueryable<TModel> DynamicSelection<TModel>(this IQueryable queryable)
    {
        var modelProperties = typeof(TModel).GetPropertiesInfo();
        var fields = modelProperties.IdentityProperties();
        
        var queryFields = $"new ({string.Join(", ", fields)})";
        
        return queryable.Select<TModel>(queryFields);
    }

    private static ICollection<PropertyInfo> GetPropertiesInfo(this Type tModel, bool isBase = false)
    {
        var response = new List<PropertyInfo>();
        
        var result = tModel.BaseType?.GetPropertiesInfo(true);
        if (result?.Count > 0) response.AddRange(result);

        if (isBase)
            return tModel.GetPropsFromModel(response);

        var propChild = tModel.GetPropsFromModel(response);
        if(propChild?.Count > 0) response.AddRange(propChild);

        return response;
    }

    private static ICollection<PropertyInfo> GetPropsFromModel(this IReflect tType, IEnumerable<PropertyInfo> props)
    {
        return tType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pt => !props.Select(p => p.Name).Contains(pt.Name))
            .ToList();
    }

    private static ICollection<string> IdentityProperties(this IEnumerable<PropertyInfo> properties, string? entityParent = null)
    {
        var response = new List<string>();
        
        foreach (var prop in properties)
        {
            if (BasicTypes.Contains(prop.PropertyType.Name.ToLower()))
            {
                var decoratorValue = prop.GetDecoratorValue();
                if (!string.IsNullOrEmpty(decoratorValue))
                {
                    response.Add(string.IsNullOrEmpty(entityParent) ? decoratorValue : $"{entityParent}.{decoratorValue}");
                    continue;
                }
                response.Add(string.IsNullOrEmpty(entityParent) ? prop.Name : $"{entityParent}.{prop.Name}");
                continue;
            }

            var childProps = prop.PropertyType?.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var childResponse = childProps?.IdentityProperties(prop.Name);
            if (childResponse?.Count > 0) response.AddRange(childResponse);
        }

        return response;
    }
    
    private static string GetDecoratorValue(this MemberInfo prop)
    {
        var decoratorSettings = prop.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.Name.Equals("EntityProperty"));
        if(decoratorSettings is { ConstructorArguments.Count: > 0 })
            return decoratorSettings.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;

        return string.Empty;
    }
}