// ****************************************************************************
// Project:  Unit Tests
// File:     Foo.cs
// Author:   Latency McLaughlin
// Date:     10/18/2026
// ****************************************************************************

using System.ComponentModel;

namespace Unit_Tests;

[TypeConverter(typeof(FooConverter))]
internal class Foo
{
    public static readonly Foo A = new("A");
    public static readonly Foo B = new("B");
    private readonly string _s;

    private Foo(string s) => _s = s;

    public override string ToString() => _s;
}