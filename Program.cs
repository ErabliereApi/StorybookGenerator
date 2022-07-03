// StorybookGenerator is a command line tool that allow to parse a angular application and generate a stories file foreach component.

// First initialise de IConfigurationBuilder with the command line arguments.
// Then use the configuration to create the IConfiguration object.
using Microsoft.Extensions.Options;

var configurationRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var services = new ServiceCollection();

services.Configure<AppOptions>(configurationRoot);
services.AddTransient<IStoriesGenerator, StoriesGenerator>();

var provider = services.BuildServiceProvider();

var appOption = provider.GetRequiredService<IOptions<AppOptions>>().Value;

var files = Directory.GetFiles(appOption.SrcFolder, "*.component.ts", SearchOption.AllDirectories);

foreach (var file in files) {
    var gen = provider.GetRequiredService<IStoriesGenerator>();

    gen.GenerateStories(file);
}