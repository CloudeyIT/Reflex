namespace Cloudey.Reflex.Core.Setup;

/// <summary>
///     Include this assembly when scanning for services
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class IncludeAssemblyAttribute : Attribute { }