// ****************************************************************************
// Project:  Unit Tests
// File:     OptionTest.cs
// Author:   Latency McLaughlin
// Date:     10/18/2026
// ****************************************************************************

using GetOpts.NET;

namespace Unit_Tests;

internal class DefaultOption : Option
{
    public DefaultOption(string prototypes, string? description) : base(prototypes, description)
    { }

    public DefaultOption(string prototypes, string? description, int c) : base(prototypes, description, c)
    { }

    protected override void OnParseComplete(OptionContext c) => throw new NotImplementedException();
}

public class OptionTest
{
    [Fact]
    public void Exceptions()
    {
        Utils.AssertArgumentNull("prototype", () => Activator.CreateInstance(typeof(DefaultOption), [null, null]));
        Utils.AssertException<object?>(typeof(ArgumentException), "Cannot be the empty string. (Parameter 'prototype')", null, v =>
        {
            _ = new DefaultOption("", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Empty option names are not supported. (Parameter 'prototype')", null, v =>
        {
            _ = new DefaultOption("a|b||c=", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Conflicting option types: '=' vs. ':'. (Parameter 'prototype')", null, v =>
        {
            _ = new DefaultOption("a=|b:", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "The default option handler '<>' cannot require values. (Parameter 'prototype')", null, v =>
        {
            _ = new DefaultOption("<>=", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "The default option handler '<>' cannot require values. (Parameter 'prototype')", null, v =>
        {
            _ = new DefaultOption("<>:", null);
        });
        Utils.AssertException<object?>(null, null, null, v =>
        {
            _ = new DefaultOption("t|<>=", null, 1);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "The default option handler '<>' cannot require values. (Parameter 'prototype')", null, v =>
        {
            _ = new DefaultOption("t|<>=", null, 2);
        });
        Utils.AssertException<object?>(null, null, null, v =>
        {
            _ = new DefaultOption("a|b=", null, 2);
        });
        Utils.AssertException<object?>(typeof(ArgumentOutOfRangeException), "Specified argument was out of the range of valid values. (Parameter 'maxValueCount')", null, v =>
        {
            _ = new DefaultOption("a", null, -1);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Cannot provide maxValueCount of 0 for OptionValueType.Required or OptionValueType.Optional. (Parameter 'maxValueCount')", null, v =>
        {
            _ = new DefaultOption("a=", null, 0);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={\". (Parameter 'name')", null, v =>
        {
            _ = new DefaultOption("a={", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Ill-formed name/value separator found in \"a=}\". (Parameter 'name')", null, v =>
        {
            _ = new DefaultOption("a=}", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={{}}\". (Parameter 'name')", null, v =>
        {
            _ = new DefaultOption("a={{}}", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={}}\". (Parameter 'name')", null, v =>
        {
            _ = new DefaultOption("a={}}", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={}{\". (Parameter 'name')", null, v =>
        {
            _ = new DefaultOption("a={}{", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Cannot provide key/value separators for Options taking 1 value(s). (Parameter 'prototype')", null, v =>
        {
            _ = new DefaultOption("a==", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Cannot provide key/value separators for Options taking 1 value(s). (Parameter 'prototype')", null, v =>
        {
            _ = new DefaultOption("a={}", null);
        });
        Utils.AssertException<object?>(typeof(ArgumentException), "Cannot provide key/value separators for Options taking 1 value(s). (Parameter 'prototype')", null, v =>
        {
            _ = new DefaultOption("a=+-*/", null);
        });
        Utils.AssertException<object?>(null, null, null, v =>
        {
            _ = new DefaultOption("a", null, 0);
        });
        Utils.AssertException<object?>(null, null, null, v =>
        {
            _ = new DefaultOption("a", null, 0);
        });
        Utils.AssertException<object?>(null, null, null, _ =>
        {
            var d = new DefaultOption("a", null);
            Assert.Empty(d.GetValueSeparators());
        });
        Utils.AssertException<object?>(null, null, null, _ =>
        {
            var d = new DefaultOption("a=", null, 1);
            var s = d.GetValueSeparators();
            Assert.Empty(s);
        });
        Utils.AssertException<object?>(null, null, null, _ =>
        {
            var d = new DefaultOption("a=", null, 2);
            var s = d.GetValueSeparators();
            Assert.Equal(2,   s.Length);
            Assert.Equal(":", s[0]);
            Assert.Equal("=", s[1]);
        });
        Utils.AssertException<object?>(null, null, null, _ =>
        {
            var d = new DefaultOption("a={}", null, 2);
            var s = d.GetValueSeparators();
            Assert.Empty(s);
        });
        Utils.AssertException<object?>(null, null, null, _ =>
        {
            var d = new DefaultOption("a={-->}{=>}", null, 2);
            var s = d.GetValueSeparators();
            Assert.Equal(2,     s.Length);
            Assert.Equal("-->", s[0]);
            Assert.Equal("=>",  s[1]);
        });
        Utils.AssertException<object?>(null, null, null, _ =>
        {
            var d = new DefaultOption("a=+-*/", null, 2);
            var s = d.GetValueSeparators();
            Assert.Equal(4,   s.Length);
            Assert.Equal("+", s[0]);
            Assert.Equal("-", s[1]);
            Assert.Equal("*", s[2]);
            Assert.Equal("/", s[3]);
        });
    }
}
