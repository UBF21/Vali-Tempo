# Contributing to Vali-Tempo

Thank you for your interest in contributing to the Vali-Tempo ecosystem!
This guide explains how to report issues, propose changes, and submit pull requests.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [How to Contribute](#how-to-contribute)
- [Coding Standards](#coding-standards)
- [Tests](#tests)
- [Benchmarks](#benchmarks)
- [Commit Messages](#commit-messages)
- [Pull Request Checklist](#pull-request-checklist)

---

## Code of Conduct

Be respectful and constructive. Contributions of all kinds are welcome —
bug reports, documentation improvements, new features, and performance fixes.

---

## Project Structure

```
Vali-Tempo/
├── Vali-Time/           # Core time conversions and date arithmetic
├── Vali-Range/          # Date range creation, iteration, set operations
├── Vali-Calendar/       # Workday calculations, ISO weeks
├── Vali-Duration/       # Decimal-precision duration value type
├── Vali-CountDown/      # Deadline tracking and countdown
├── Vali-Age/            # Age calculation and birthday utilities
├── Vali-Schedule/       # Recurring schedule fluent builder
├── Vali-Holiday/        # Holiday providers for 35+ countries
├── Vali-TimeZone/       # Timezone conversion and discovery
├── Vali-Tempo/          # Meta-package (references all of the above)
├── Vali-Tempo.Tests/    # Unit and integration tests
└── Vali-Tempo.Benchmarks/ # BenchmarkDotNet performance benchmarks
```

Each package is independent and can be installed separately.
`Vali-Time` is the foundational dependency for most packages.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or later
- A C# IDE (Rider, Visual Studio, VS Code)

### Build

```bash
dotnet build Vali-Tempo.sln
```

### Run Tests

```bash
dotnet test Vali-Tempo.Tests/Vali-Tempo.Tests.csproj
```

### Run Coverage

```bash
./coverage.sh
```

### Run Benchmarks

```bash
cd Vali-Tempo.Benchmarks
dotnet run -c Release
```

---

## How to Contribute

### Reporting Bugs

Open an issue with:
- A clear title describing the problem
- Steps to reproduce
- Expected vs actual behaviour
- .NET version and OS

### Suggesting Features

Open an issue with the `enhancement` label. Describe:
- The use case / problem it solves
- The proposed API (method signatures, types)
- Whether it belongs in an existing package or warrants a new one

### Submitting a Pull Request

1. Fork the repository and create a branch from `main`:
   ```bash
   git checkout -b feat/my-feature
   ```
2. Make your changes following the [Coding Standards](#coding-standards) below
3. Add or update tests in `Vali-Tempo.Tests/`
4. Add or update benchmarks in `Vali-Tempo.Benchmarks/` if relevant
5. Run `dotnet test` and confirm all 408+ tests pass
6. Open a Pull Request against `main` with a clear description

---

## Coding Standards

- **Language**: C# with `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>`
- **Precision**: Use `decimal` for all time/duration calculations — never `double` or `float`
- **Immutability**: Prefer `readonly struct` for value types (`DateRange`, `ValiDuration`, `AgeResult`, `CalendarWeek`)
- **DI**: Every service class must implement an interface (`IValiX`) and have an `AddValiX()` extension method
- **No external dependencies**: Packages must not introduce dependencies beyond
  `Microsoft.Extensions.DependencyInjection.Abstractions` and cross-references within the ecosystem
- **No magic numbers**: Use `Constants.cs` or named constants — never inline numeric literals for time factors
- **XML docs**: Public API members must have `<summary>` documentation
- **Formatting**: Follow existing brace style and indentation (4 spaces)

---

## Tests

All tests live in `Vali-Tempo.Tests/Tests/`. Each package has its own test class:

| File | Package |
|------|---------|
| `ValiTimeTests.cs` | Vali-Time |
| `ValiDateTests.cs` | Vali-Time (ValiDate) |
| `ValiRangeTests.cs` | Vali-Range |
| `ValiCalendarTests.cs` | Vali-Calendar |
| `ValiDurationTests.cs` | Vali-Duration |
| `ValiCountdownTests.cs` | Vali-CountDown |
| `ValiAgeTests.cs` | Vali-Age |
| `ValiScheduleTests.cs` | Vali-Schedule |
| `ValiHolidayTests.cs` | Vali-Holiday |
| `EasterCalculatorTests.cs` | Vali-Holiday (Easter) |
| `ValiTimeZoneTests.cs` | Vali-TimeZone |
| `IntegrationTests.cs` | Cross-package integration |

Guidelines:
- Use `xUnit` with `[Fact]` and `[Theory]`
- Use fixed `DateTime` values — never `DateTime.Now` in tests
- Cover edge cases: Feb 29, leap years, DST transitions, negative values

---

## Benchmarks

Benchmarks live in `Vali-Tempo.Benchmarks/Benchmarks/`. Add a `[MemoryDiagnoser]` + `[SimpleJob]`
class for any new hot-path code. Use a `[GlobalSetup]` to avoid setup overhead in measured methods.

---

## Commit Messages

Follow the [Conventional Commits](https://www.conventionalcommits.org/) format:

```
<type>: <short description>

[optional body]
```

Types: `feat`, `fix`, `docs`, `perf`, `refactor`, `test`, `chore`

Examples:
```
feat(Vali-Holiday): add Japan holiday provider
fix(Vali-Age): correct NextBirthday for Feb 29 in non-leap years
perf(Vali-Holiday): replace O(n) LINQ scan with per-year HashSet cache
docs: update compatibility table in README
```

---

## Pull Request Checklist

Before opening a PR, make sure:

- [ ] All existing tests pass (`dotnet test`)
- [ ] New tests added for new functionality
- [ ] Public methods have XML `<summary>` docs
- [ ] No new external dependencies introduced
- [ ] `decimal` used for all numeric time calculations
- [ ] `CHANGELOG.md` updated under `[Unreleased]`
- [ ] PR description explains the *why*, not just the *what*

---

## License

By contributing, you agree that your contributions will be licensed under the
[Apache-2.0 License](LICENSE).

Apache-2.0 © 2025 Felipe Rafael Montenegro Morriberon
