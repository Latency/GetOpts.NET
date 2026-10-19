## Navigation

* [Introduction](#introduction)
* [History](#history)
* [Solution](#solution)
* [Usage](#usage)

---

## Introduction

This project features a callback-based option parser for C#. It takes advantage of C# features such as collection initializers and lambda delegates to provide a short, concise specification of the option names to parse,
whether or not those options support values, and what to do when the option is encountered.

An `OptionSet` is created with one entry per option: a `|`-separated list of names (`v|verbose`), an optional `=` suffix when the option takes a value (`n|name=`),
and a callback that runs when the option is encountered. The callback receives the value, or `null` when the option has none.

## History

See:  [getopt](https://en.wikipedia.org/wiki/Getopt)

## Usage

### Declaring options

Options are declared with a collection initializer or with chained `Add` calls. Each entry is a `|`-separated list of names, an optional value
suffix, and a callback.

```csharp
var verbose   = 0;
var showHelp  = false;
var names     = new List<string>();

var p = new OptionSet
{
	{ "v|verbose", v => { if (v != null) ++verbose; } },
	{ "h|?|help",  v => showHelp = v != null },
	{ "n|name=",   v => names.Add(v!) },
};
```

```csharp
var p = new OptionSet()
	.Add("v|verbose", v => { if (v != null) ++verbose; })
	.Add("h|?|help",  v => showHelp = v != null)
	.Add("n|name=",   v => names.Add(v!));
```

### Parsing arguments

`Parse` runs the callbacks and returns the arguments that were not consumed by any option.

```csharp
List<string> extra = p.Parse(["-v", "--name=Alice", "-n", "Bob", "file1.txt", "file2.txt"]);
// verbose == 1, names == ["Alice", "Bob"], extra == ["file1.txt", "file2.txt"]
```

Everything after a lone `--` is treated as unprocessed.

### Prototype syntax

| Prototype | Meaning |
|---|---|
| `v\|verbose` | Flag. The callback receives the option name, or `null` for `-v-`. |
| `n\|name=` | Required value. |
| `l\|library:` | Optional value. The callback receives `null` when no value is given. |
| `D=` with two parameters | Key/value pair, e.g. `-DNAME=value`. |
| `<>` | Default handler. It receives every argument that is not an option. |

Values can be supplied as `-name=value`, `-name:value`, `--name value`, or `/name value`.
Single-character flags can be bundled (`-abc`), and a value option may end a bundle (`-vfFILE`).

### Boolean options

A flag followed by `+` or `-` turns it on or off. In the callback, a `null` value means the option was disabled.

```csharp
var debug = false;
var p = new OptionSet { { "debug", v => debug = v != null } };

p.Parse(["-debug"]);   // debug == true
p.Parse(["-debug+"]);  // debug == true
p.Parse(["-debug-"]);  // debug == false
```

### Typed values

The generic `Add<T>` overload converts the value using the type's `TypeConverter`. A failed conversion throws `OptionException`.

```csharp
var port    = 80;
var timeout = TimeSpan.Zero;

var p = new OptionSet
{
	{ "port=",    (int v)      => port    = v },
	{ "timeout=", (TimeSpan v) => timeout = v },
};

p.Parse(["--port=8080", "--timeout=00:00:30"]);
```

### Key/value options

Use the two-type `Add<TKey, TValue>` overload for options that take a key and a value.

```csharp
var defines = new Dictionary<string, int>();

var p = new OptionSet
{
	{ "D=", (string k, int v) => defines.Add(k, v) },
};

p.Parse(["-DWIDTH=80", "-D", "HEIGHT=24"]);
```

### Help text

Pass a description as the second argument, then call `WriteOptionDescriptions`.

```csharp
var p = new OptionSet
{
	{ "n|name=", "the {NAME} of the user", v => name = v },
	{ "v",       "increase message verbosity", v => ++verbosity },
	{ "h|help",  "show this message and exit", v => showHelp = v != null },
};

p.WriteOptionDescriptions(Console.Out);
```

```text
  -n, --name=NAME            the NAME of the user
  -v                         increase message verbosity
  -h, --help                 show this message and exit
```

### Error handling

Invalid input throws `OptionException`, for example a missing required value or an unconvertible value.
`OptionException.OptionName` holds the offending option.

```csharp
try
{
	p.Parse(args);
}
catch (OptionException e)
{
	Console.Error.WriteLine($"{e.OptionName}: {e.Message}");
}
```