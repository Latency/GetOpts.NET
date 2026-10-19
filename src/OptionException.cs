// 1****************************************************************************
// Project:  NDesk.Options
// File:     OptionException.cs
// Author:   Latency McLaughlin
// Date:     1/21/2024
// ****************************************************************************

namespace GetOpts.NET;

[Serializable]
public class OptionException : Exception
{
    public OptionException(string message, string? optionName) : base(message) => OptionName = optionName;

    public OptionException(string message, string? optionName, Exception innerException) : base(message, innerException) => OptionName = optionName;

    public string? OptionName { get; }
}