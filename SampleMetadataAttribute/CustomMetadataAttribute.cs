using System;
using System.Reflection;

[assembly: AssemblyMetadata("Custom", "Custom")]

[AttributeUsage(AttributeTargets.Assembly)]
public class CustomMetadataAttribute : Attribute
{
    public CustomMetadataAttribute(string key, string value) { }
}