// ****************************************************************************
// Project:  Unit Tests
// File:     OptionContextTest.cs
// Author:   Latency McLaughlin
// Date:     10/18/2026
// ****************************************************************************

using GetOpts.NET;

namespace Unit_Tests;

public class OptionContextTest
{
    [Fact]
    public void Exceptions()
    {
        var p = new OptionSet
        {
            {
                "a=", _ =>
                {
                    /* ignore */
                }
            }
        };
        var c = new OptionContext(p);
        Utils.AssertException(typeof(InvalidOperationException), "OptionContext.Option is null.", c, v =>
        {
            _ = v.OptionValues[0];
        });
        c.Option = p[0];
        Utils.AssertException(typeof(ArgumentOutOfRangeException), "Specified argument was out of the range of valid values. (Parameter 'index')", c, v =>
        {
            _ = v.OptionValues[2];
        });
        c.OptionName = "-a";
        Utils.AssertException(typeof(OptionException), "Missing required value for option '-a'.", c, v =>
        {
            _ = v.OptionValues[0];
        });
    }
}