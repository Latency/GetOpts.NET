// ****************************************************************************
// Project:  Unit Tests
// File:     Utils.cs
// Author:   Latency McLaughlin
// Date:     10/18/2026
// ****************************************************************************

using System.Reflection;
using System.Text.RegularExpressions;

namespace Unit_Tests;

internal static class Utils
{
    // .NET Framework formats "Parameter name: x" on a new line; .NET Core+ uses "(Parameter 'x')".
    private static string? Normalize(string? m)
    {
        if (m == null)
            return null;
        var i = m.IndexOf(" (Parameter '", StringComparison.Ordinal);
        if (i >= 0)
            return m[..i];
        i = m.IndexOf("Parameter name:", StringComparison.Ordinal);
        m = i >= 0 ? m[..i].TrimEnd() : m;
        return Regex.Replace(m, "The given key '[^']*' was", "The given key was");
    }

    // Simulates callers that ignore nullable warnings by passing null through reflection.
    public static void AssertArgumentNull(string paramName, Action action)
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            var actual = e is TargetInvocationException { InnerException: { } inner } ? inner : e;
            var ane    = Assert.IsType<ArgumentNullException>(actual);
            Assert.Equal(paramName, ane.ParamName);
            return;
        }

        Assert.Fail($"Expected {nameof(ArgumentNullException)} for parameter '{paramName}'.");
    }

    public static void AssertException<T>(Type? exception, string? message, T a, Action<T> action)
    {
        Type?   actualType    = null;
        string? stack         = null;
        string? actualMessage = null;
        try
        {
            action(a);
        }
        catch (Exception e)
        {
            actualType    = e.GetType();
            actualMessage = e.Message;
            if (!(actualType == exception))
                stack = e.ToString();
        }

        if (actualType != exception)
            throw new InvalidOperationException($"Assertion failed: Expected Exception Type {exception}, got {actualType}.{Environment.NewLine}Actual Exception: {stack}");

        if (Normalize(actualMessage) != Normalize(message))
            throw new InvalidOperationException($"Assertion failed:{Environment.NewLine}\tExpected: {message}{Environment.NewLine}\t  Actual: {actualMessage}");
    }
}