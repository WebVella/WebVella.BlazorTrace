[![Documentation](https://img.shields.io/badge/Documentation-blue?style=for-the-badge)](https://github.com/WebVella/WebVella.BlazorTrace/wiki)
[![Dotnet](https://img.shields.io/badge/platform-.NET-blue?style=for-the-badge)](https://www.nuget.org/packages/WebVella.BlazorTrace)
[![GitHub Repo stars](https://img.shields.io/github/stars/WebVella/WebVella.BlazorTrace?style=for-the-badge)](https://github.com/WebVella/WebVella.BlazorTrace/stargazers)
[![Nuget version](https://img.shields.io/nuget/v/WebVella.BlazorTrace?style=for-the-badge)](https://www.nuget.org/packages/WebVella.BlazorTrace)
[![Nuget download](https://img.shields.io/nuget/dt/WebVella.BlazorTrace?style=for-the-badge)](https://www.nuget.org/packages/WebVella.BlazorTrace)
[![MIT License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/LICENSE)

## What is BlazorTrace?
An easy to add library that will enable you to get detailed information about your Blazor components rerenders and memory, as well as compare it with different snapshots that you created. It is targeting Blazor UI developers and presents the information in a simple and focused way. BlazorTrace will help you develop better, faster and more consistent user experience with your Blazor applications.

## About Us
We are a small team of early Blazor adopters that created several complex Blazor applications that work in production. We prefer working with WebAssembly deployments but SSR is getting a favorite fast. We have 15+ experience in creating .net projects. Here is some examples of our work

| | | |
|---|---|---|
| [WebVella ERP](https://github.com/WebVella/WebVella-ERP) | [![GitHub Repo stars](https://img.shields.io/github/stars/WebVella/WebVella-ERP?style=for-the-badge)](https://github.com/WebVella/WebVella-ERP/stargazers) | [![Nuget download](https://img.shields.io/nuget/dt/WebVella.ERP?style=for-the-badge)](https://www.nuget.org/packages/WebVella.ERP) 
| [Document Templates Library](https://github.com/WebVella/WebVella.DocumentTemplates) | [![GitHub Repo stars](https://img.shields.io/github/stars/WebVella/WebVella.DocumentTemplates?style=for-the-badge)](https://github.com/WebVella/WebVella.DocumentTemplates/stargazers) | [![Nuget download](https://img.shields.io/nuget/dt/WebVella.DocumentTemplates?style=for-the-badge)](https://www.nuget.org/packages/WebVella.DocumentTemplates)
| [Tefter](https://github.com/WebVella/WebVella.Tefter) | [![GitHub Repo stars](https://img.shields.io/github/stars/WebVella/WebVella.Tefter?style=for-the-badge)](https://github.com/WebVella/WebVella.Tefter/stargazers) | [![Nuget download](https://img.shields.io/nuget/dt/WebVella.Tefter?style=for-the-badge)](https://www.nuget.org/packages/WebVella.Tefter) 
| [Npgsql.Extensions](https://github.com/WebVella/WebVella.Npgsql.Extensions) | [![GitHub Repo stars](https://img.shields.io/github/stars/WebVella/WebVella.Npgsql.Extensions?style=for-the-badge)](https://github.com/WebVella/WebVella.Npgsql.Extensions/stargazers) | [![Nuget download](https://img.shields.io/nuget/dt/WebVella.Npgsql.Extensions?style=for-the-badge)](https://www.nuget.org/packages/WebVella.Npgsql.Extensions)

## How to get it
You can either clone this repository or get the [Nuget package](https://www.nuget.org/packages/WebVella.BlazorTrace)

## Please help by giving a star
GitHub stars guide developers toward great tools. If you find this project valuable, please give it a star – it helps the community and takes just a second!⭐

## Documentation
You can find our documentation in the [Wiki section of this repository](https://github.com/WebVella/WebVella.BlazorTrace/wiki)

## Get Started
To start using BlazorTrace you need to do the following simple steps:

1. Add the latest version of the [WebVella.BlazorTrace Nuget package](https://www.nuget.org/packages/WebVella.BlazorTrace) to your component holding projects directly. It is important to be directly referenced, so the ```FodyWeavers.xml``` and ```WvBlazorTraceModule.cs``` can be generated in the projects root!
2. Add the following lines in your ```Program.cs``` file. You can get more info about options to fine tune or extending SignalR hub size for larger snapshot in the [wiki](https://github.com/WebVella/WebVella.BlazorTrace/wiki).

``` csharp
builder.Services.AddBlazorTrace();
```
3. In your ```_Imports.razor``` file add the following lines so all supported components can start being monitored

``` razor
@using WebVella.BlazorTrace;
@attribute [WvBlazorTrace]
```
4. Add the BlazorTrace component at the end of your ```App.razor``` or ```Routes.razor``` component (depending on your project type)

``` razor
<Router AppAssembly="@typeof(App).Assembly">
...
</Router>
<WvBlazorTrace/> @* <-- INSERT HERE *@
```
5. Rebuild the solution
6. Thats it. You can start reviewing the data. PRO TIP: Use the F1 (show) and Esc (hide) to save time.

### Method OnEnter/OnExit call information

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/trace-modal-methods.png)

### Log signals information

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/trace-modal-signals.png)

### Trace calls detail information

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/trace-list-modal.png)

### Memory detail information

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/memory-modal.png)

### Limit hits

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/limits-modal.png)

### Snapshots

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/trace-modal-snapshots.png)

### Muted traces

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/trace-modal-muted.png)

## License
BlazorTrace is distributed under the MIT license.

## Acknowledgments
BlazorTrace wouldn't be possible without the incredible encouragement and support of amazing people and communities. Thanks to all of you!

### [jhsheets](https://github.com/jhsheets)
For being our fist contributor and thus boosting our motivation to make BlazorTrace better.

### [LlamaNL](https://www.reddit.com/user/LlamaNL/)
He saved time to all of us, by finding a way how to create faster tracer intergation with an Attribute and FODY

### [Tension-Maleficent](https://www.reddit.com/user/Tension-Maleficent/)
For helping with the FODY implementation

### [derekwelton](https://github.com/derekwelton)
Provided the idea of creating CSV and JSON exports of the data for easier postpocessing and analizing

### [/r/dotnet](https://www.reddit.com/r/dotnet/), [/r/Blazor](https://www.reddit.com/r/Blazor/), [/r/csharp](https://www.reddit.com/r/csharp/)
These Reddit communities are home to incredibly welcoming and knowledgeable people, always ready to offer help with questions, problems, or advice. 

