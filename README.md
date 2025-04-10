# <img src="https://github.com/UBF21/Vali-Time/blob/main/Vali-Time/logo-Vali-Time.png?raw=true" alt="Logo de Vali Time" style="width: 46px; height: 46px; max-width: 300px;"> Vali-Time - Time Unit Conversion and Management for .NET


## Introduction 🚀

Welcome to Vali-Time, a lightweight .NET library designed to simplify time unit conversions and management. Whether you're working with milliseconds, seconds, minutes, or hours, Vali-Time provides a fluent and intuitive API to convert times, sum multiple values, break down times into components, and format them into human-readable strings. With automatic unit detection and cultural formatting support, this library is perfect for applications requiring accurate time handling and integrates seamlessly into any .NET project.

## Installation 📦

To add **Vali-Time** to your .NET project, install it via NuGet with the following command:

```sh
dotnet add package Vali-Time
```

Ensure your project targets a compatible .NET version (e.g., .NET Standard 2.0 or later). Vali-Time is lightweight and has minimal dependencies, making it an easy addition to your application.

## Usage 🛠️

Vali-Time focuses on converting time between units, summing times, breaking them down, and formatting them for display. The library provides a simple class, TimeConverter, with methods to perform these operations with precision and flexibility.

### Basic Example

Here’s how you can convert and format a time value:

```csharp
using Vali_Time.Converters;
using Vali_Time.Enums;

var converter = new TimeConverter();

// Convert 1.5 hours to seconds
decimal seconds = converter.Convert(1.5m, TimeUnit.Hours, TimeUnit.Seconds);
Console.WriteLine(seconds); // Outputs: 5400

// Format a time in minutes
string formattedTime = converter.FormatTime(123.456m, TimeUnit.Minutes);
Console.WriteLine(formattedTime); // Outputs: "123.46 min"
```

## Key Methods 📝

**Vali-Time** offers a straightforward API for time management. Below are the key methods provided by the **TimeConverter** class:

### Convert 🏗️

Converts a file size from one unit to another with precision:

```csharp
var converter = new TimeConverter();

// Convert 2.5 minutes to seconds with 2 decimal places
decimal seconds = converter.Convert(2.5m, TimeUnit.Minutes, TimeUnit.Seconds, decimalPlaces: 2);
Console.WriteLine(seconds); // Outputs: 150.00
```
### SumTimes 🎨

Sums multiple time values in different units and returns the result in a specified unit:

```csharp
var converter = new ValiFileSize();

// Format 1,234,567 bytes as megabytes
string formatted = converter.FormatSize(1234567, FileSizeUnit.Megabytes, decimalPlaces: 3);
Console.WriteLine(formatted); // Outputs: "1.177 MB"
```
### FormatTime 🎨

Formats a time value into a human-readable string with customizable decimal places and cultural formatting:

```csharp
var converter = new TimeConverter();

// Format 1234.567 seconds as minutes
string formatted = converter.FormatTime(1234.567m, TimeUnit.Minutes, decimalPlaces: 3);
Console.WriteLine(formatted); // Outputs: "20.576 min"
```

### GetBestUnit 🔍

Automatically determines the most appropriate unit for a given time in seconds:

```csharp
var converter = new TimeConverter();

// Find the best unit for 7200 seconds
var (time, unit) = converter.GetBestUnit(7200m);
string bestFormat = converter.FormatTime(time, unit);
Console.WriteLine(bestFormat); // Outputs: "2.00 h"
```

### Breakdown 🧩

Breaks down a time in seconds into a dictionary of units (hours, minutes, seconds, milliseconds):

```csharp
var converter = new TimeConverter();

// Breakdown 3665.678 seconds
var breakdown = converter.Breakdown(3665.678m);
Console.WriteLine($"{breakdown[TimeUnit.Hours]}h {breakdown[TimeUnit.Minutes]}m {breakdown[TimeUnit.Seconds]}s {breakdown[TimeUnit.Milliseconds]}ms");
// Outputs: "1h 1m 5s 678ms"
```

## Working with Advanced Features 🧩

**Vali-Time** supports advanced use cases like cultural formatting and precise time management:

### Cultural Formatting

Format times according to specific cultures:

```csharp
using System.Globalization;

var converter = new TimeConverter();
var germanCulture = new CultureInfo("de-DE");

string formatted = converter.FormatTime(1234.567m, TimeUnit.Seconds, 2, germanCulture);
Console.WriteLine(formatted); // Outputs: "1234,57 s" (uses comma as decimal separator)
```

### Combining Features

Convert, sum, and format times in one flow:

```csharp
var converter = new TimeConverter();
var times = new (decimal, TimeUnit)[]
{
    (1.5m, TimeUnit.Hours),
    (30m, TimeUnit.Minutes)
};
decimal totalSeconds = converter.Add(TimeUnit.Seconds, times);
var (bestTime, bestUnit) = converter.GetBestUnit(totalSeconds);
string result = converter.FormatTime(bestTime, bestUnit);
Console.WriteLine(result); // Outputs: "2.00 h"
```

## Comparison: Without vs. With Vali-FileSize ⚖️

### Without Vali-Time (Manual Conversion)

Manually handling time conversions can be error-prone and cumbersome:

```csharp
decimal hours = 1.5m;
decimal seconds = hours * 3600;
decimal totalSeconds = seconds + (30m * 60);
Console.WriteLine($"{totalSeconds:F2} s"); // Outputs: "7200.00 s"
```

### With Vali-Time (Simplified Conversion)

**Vali-Time** streamlines the process with a clean and precise API:

```csharp
var converter = new TimeConverter();
var times = new List<(decimal, TimeUnit)>()
{
    (1.5m, TimeUnit.Hours),
    (30m, TimeUnit.Minutes)
};
decimal totalSeconds = converter.Add(TimeUnit.Seconds, times);
string formatted = converter.FormatTime(totalSeconds, TimeUnit.Seconds);
Console.WriteLine(formatted); // Outputs: "7200.00 s"
```
## Features and Enhancements 🌟

### Recent Updates

- Initial release (v1.0.0) with support for conversions across milliseconds, seconds, minutes, and hours.
-Added (or SumTimes) for summing multiple time values with precision.
- Introduced Breakdown for decomposing time into detailed components..
- Included FormatTime with customizable decimal precision and cultural support.
- Ensured robust validation with negative time checks and comprehensive exception handling.

### Planned Features

- Support for additional units like days, weeks, and months.
- Enhanced formatting options, such as combined unit strings (e.g., "1h 30min 45s").
- Integration with **TimeSpan** for broader compatibility.

Follow the project on GitHub for updates on new features and improvements!

## Donations 💖

If you find ***Vali-Time*** useful and would like to support its development, consider making a donation:

- **For Latin America**: [Donate via MercadoPago](https://link.mercadopago.com.pe/felipermm)
- **For International Donations**: [Donate via PayPal](https://paypal.me/felipeRMM?country.x=PE&locale.x=es_XC)


Your contributions help keep this project alive and improve its development! 🚀

## License 📜
This project is licensed under the [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0).

## Contributions 🤝
Feel free to open issues and submit pull requests to improve this library!
