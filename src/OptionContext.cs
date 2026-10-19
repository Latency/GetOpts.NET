// 04****************************************************************************
// Project:  NDesk.Options.Core
// File:     OptionContext.cs
// Author:   Latency McLaughlin
// Date:     04/04/2024
// ****************************************************************************

namespace GetOpts.NET;

public class OptionContext
{
    public OptionContext(OptionSet set)
    {
        OptionSet    = set;
        OptionValues = new(this);
    }

    public Option? Option { get; set; }

        public string? OptionName { get; set; }

    public int OptionIndex { get; set; }

    public OptionSet OptionSet { get; }

    public OptionValueCollection OptionValues { get; }
}