# MudCalendar
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/danheron/Heron.MudCalendar/build-test-mudcalendar.yml?branch=dev&logo=github&style=flat-square)
![Codecov](https://img.shields.io/codecov/c/github/danheron/Heron.MudCalendar?logo=codecov&logoColor=white&style=flat-square&token=EP53WKLLLX)
[![GitHub](https://img.shields.io/github/license/danheron/Heron.MudCalendar?color=594ae2&logo=github&style=flat-square)](https://github.com/danheron/Heron.MudCalendar/blob/master/LICENSE)
[![GitHub last commit](https://img.shields.io/github/last-commit/danheron/Heron.MudCalendar?color=594ae2&style=flat-square&logo=github)](https://github.com/danheron/Heron.MudCalendar)
[![Nuget version](https://img.shields.io/nuget/v/Heron.MudCalendar?color=ff4081&label=nuget%20version&logo=nuget&style=flat-square)](https://www.nuget.org/packages/Heron.MudCalendar/)

A simple but powerful calendar component for MudBlazor.

## Features

- Multiple views (month, week, day)
- Customizable events
- Easy integration with your existing MudBlazor project

## Documentation

Documentation and examples are available [here](https://danheron.github.io/Heron.MudCalendar).

## Getting Started

MudCalendar relies on MudBlazor. Follow the installation instructions for [MudBlazor](https://mudblazor.com/getting-started/installation)

Once your project is setup with MudBlazor you can install the MudCalendar package.

```
dotnet add package Heron.MudCalendar
```

Add the following to `_Imports.razor`.

```razor
@using Heron.MudCalendar
```

Add the MudCalendar component to your razor page/component.

```razor
<MudCalendar />
```

Check out the examples of how to use and customize the MudCalendar component.

## Support

For any issues or feature requests, please open a new issue on GitHub.

## Building From Source

The project includes 2 solution files:

- Heron.MudCalendar.sln
- Heron.MudCalendarWithDocs.sln

The Heron.MudCalendar.sln solution contains the MudCalendar project and Unit Test projects.

The Heron.MudCalendarWithDocs.sln solution also contains the Docs project.  Build this is a bit more complicated to compile because it uses the MudBlazor.Docs project.  To build this project you need to clone the [danheron/MudBlazor](https://github.com/danheron/MudBlazor/tree/mudcalendar) repo and checkout the mudcalendar branch.

The repositories should be cloned into the same parent directory e.g.

MyProjects  
|-> MudBlazor  
|-> Heron.MudCalendar

Build the projects in the following order:

1. Heron.MudCalendar (Debug)
2. Heron.MudCalendar (Release)
3. MudBlazor.Docs (Release)
4. Heron.MudCalendar.Docs

