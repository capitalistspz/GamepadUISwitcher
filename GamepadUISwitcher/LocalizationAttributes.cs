using System;

namespace GamepadUISwitcher;

[AttributeUsage(AttributeTargets.Field)]
public class LocalizationKeyAttribute (string key, bool ignorePrefix = false) : Attribute
{
    public string Key { get; } = key;
    public bool IgnorePrefix { get; } = ignorePrefix;
}

[AttributeUsage(AttributeTargets.Enum)]
public class LocalizationKeyPrefixAttribute (string keyPrefix) : Attribute
{
    public string KeyPrefix { get; } = keyPrefix;
}