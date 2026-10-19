// 04****************************************************************************
// Project:  NDesk.Options.Core
// File:     Option.cs
// Author:   Latency McLaughlin
// Date:     04/04/2024
// ****************************************************************************

using System.ComponentModel;

namespace GetOpts.NET;

public abstract class Option
{
    private static readonly char[] NameTerminator =
    [
        '=',
        ':'
    ];

    protected Option(string prototype, string? description) : this(prototype, description, 1)
    { }

    protected Option(string prototype, string? description, int maxValueCount)
    {
        if (prototype == null)
            throw new ArgumentNullException(nameof(prototype));
        if (prototype.Length == 0)
            throw new ArgumentException("Cannot be the empty string.", nameof(prototype));
        if (maxValueCount < 0)
            throw new ArgumentOutOfRangeException(nameof(maxValueCount));

        Prototype       = prototype;
        Names           = prototype.Split('|');
        Description     = description;
        MaxValueCount   = maxValueCount;
        OptionValueType = ParsePrototype();

        if (MaxValueCount == 0 && OptionValueType != OptionValueType.None)
            throw new ArgumentException("Cannot provide maxValueCount of 0 for OptionValueType.Required or " + "OptionValueType.Optional.", nameof(maxValueCount));
        if (OptionValueType == OptionValueType.None && maxValueCount > 1)
            throw new ArgumentException($"Cannot provide maxValueCount of {maxValueCount} for OptionValueType.None.", nameof(maxValueCount));
        if (Array.IndexOf(Names, "<>") >= 0 && ((Names.Length == 1 && OptionValueType != OptionValueType.None) || (Names.Length > 1 && MaxValueCount > 1)))
            throw new ArgumentException("The default option handler '<>' cannot require values.", nameof(prototype));
    }

    public string Prototype { get; }

    public string? Description { get; }

    public OptionValueType OptionValueType { get; }

    public int MaxValueCount { get; }

    internal string[] Names { get; }

    internal string[]? ValueSeparators { get; private set; }

    public string[] GetNames() => (string[])Names.Clone();

    public string[] GetValueSeparators() => ValueSeparators == null ? [] : (string[])ValueSeparators.Clone();

    protected static T Parse<T>(string? value, OptionContext c)
    {
        if (c == null)
            throw new ArgumentNullException(nameof(c));

        var conv = TypeDescriptor.GetConverter(typeof(T));
        T   t    = default!;
        try
        {
            if (value != null)
                t = (T)conv.ConvertFromString(value)!;
        }
        catch (Exception e)
        {
            throw new OptionException(c.OptionSet.MessageLocalizer($"Could not convert string `{value}' to type {typeof(T).Name} for option `{c.OptionName}'.") ?? throw new InvalidOperationException(), c.OptionName, e);
        }

        return t;
    }

    private OptionValueType ParsePrototype()
    {
        var type = '\0';
        var seps = new List<string>();
        for (var i = 0; i < Names.Length; ++i)
        {
            var name = Names[i];
            if (name.Length == 0)
                throw new ArgumentException("Empty option names are not supported.");

            var end = name.IndexOfAny(NameTerminator);
            if (end == -1)
                continue;
            Names[i] = name[..end];
            type     = type == '\0' || type == name[end] ? name[end] : throw new ArgumentException($"Conflicting option types: '{type}' vs. '{name[end]}'.");
            AddSeparators(name, end, seps);
        }

        if (type == '\0')
            return OptionValueType.None;

        ValueSeparators = MaxValueCount switch
        {
            <= 1 when seps.Count != 0 => throw new ArgumentException($"Cannot provide key/value separators for Options taking {MaxValueCount} value(s)."),
            > 1                       => seps.Count == 0 ? [":", "="] : seps is [{ Length: 0 }] ? null : [.. seps],
            _                         => ValueSeparators
        };

        return type == '=' ? OptionValueType.Required : OptionValueType.Optional;
    }

    private static void AddSeparators(string name, int end, ICollection<string> seps)
    {
        var start = -1;
        for (var i = end + 1; i < name.Length; ++i)
            switch (name[i])
            {
                case '{':
                    if (start != -1)
                        throw new ArgumentException($"Ill-formed name/value separator found in \"{name}\".", nameof(name));
                    start = i + 1;
                    break;
                case '}':
                    if (start == -1)
                        throw new ArgumentException($"Ill-formed name/value separator found in \"{name}\".", nameof(name));
                    seps.Add(name.Substring(start, i - start));
                    start = -1;
                    break;
                default:
                    if (start == -1)
                        seps.Add(name[i].ToString());
                    break;
            }

        if (start != -1)
            throw new ArgumentException($"Ill-formed name/value separator found in \"{name}\".", nameof(name));
    }

    public void Invoke(OptionContext c)
    {
        if (c == null)
            throw new ArgumentNullException(nameof(c));

        OnParseComplete(c);
        c.OptionName = null;
        c.Option     = null;
        c.OptionValues.Clear();
    }

    protected abstract void OnParseComplete(OptionContext c);

    public override string ToString() => Prototype;
}
