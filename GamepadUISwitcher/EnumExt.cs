using System;
using System.Linq;
using System.Reflection;

namespace GamepadUISwitcher;

public static class EnumExt
{
    public static string GetLocalizationKey<T>(this T value) where T : Enum
    {
        var type = value.GetType();
        if (type is not { IsEnum: true })
        {
            throw new ArgumentException("Value must be an enum");
        }

        var prefixAttr = type.GetCustomAttribute<LocalizationKeyPrefixAttribute>();
        var prefix =  prefixAttr?.KeyPrefix ?? String.Empty; 

        var valString = value!.ToString();

        var memInfo = type.GetMember(valString);
        if (memInfo.Length <= 0) 
            return prefix + valString;
        
        var attributes = memInfo.First();
        if (attributes.GetCustomAttribute<LocalizationKeyAttribute>() is not { } locAttribute)
            return prefix + valString;
        
        if (locAttribute.IgnorePrefix)
            return locAttribute.Key;
        
        return prefix + locAttribute.Key;

    }
}