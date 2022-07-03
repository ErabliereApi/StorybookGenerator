// StorybookGenerator is a command line tool that allow to parse a angular application and generate a stories file foreach component.

using System.Text;
using Microsoft.Extensions.Options;

internal class StoriesGenerator : IStoriesGenerator
{
    private AppOptions appOption;

    public StoriesGenerator(IOptions<AppOptions> appOption)
    {
        this.appOption = appOption.Value;
    }

    public void GenerateStories(string componentSourceCode)
    {
        var componentName = Path.GetFileNameWithoutExtension(componentSourceCode).Replace(".component", "");
        
        var sortiesName = GetStoriesName(componentName);

        var destination = Path.Combine(appOption.DestFolder, $"{sortiesName}.stories.ts");

        if (destination == appOption.Template)
        {
            return;
        }

        var t = appOption.TemplateComponentName();

        var newComponentName = $"{sortiesName}Component";

        var newStoriesContent = appOption.TemplateContent.Replace(t, newComponentName);

        newStoriesContent = EditImport(newStoriesContent, componentSourceCode, componentName, newComponentName);

        File.WriteAllText(Path.Combine(appOption.DestFolder, $"{sortiesName}.stories.ts"), newStoriesContent);
    }

    private string EditImport(string newStoriesContent, string componentSourceCode, string componentName, string newCompoenentName)
    {
        var line = newStoriesContent.Split('\n').Where(l => l.StartsWith($"import {{ {newCompoenentName} }}")).Single();

        var tcn = ToAngularFileName(appOption.TemplateComponentName());

        var newLine = line.Replace(tcn, ToAngularFileName(newCompoenentName));

        var folder = GetParents(Directory.GetParent(componentSourceCode));

        return newStoriesContent.Replace(line, newLine.Replace("access", folder))
                                .Replace(baseDatabFixture, defaultDataFixture)
                                .Replace(baseStories, defaultStories);
    }

    private string GetParents(DirectoryInfo? directoryInfo)
    {
        if (directoryInfo == null)
        {
            return "";
        }

        var parentStr = directoryInfo.Name;

        var parent = Directory.GetParent(directoryInfo.FullName);

        while (parent != null && parent?.Name != "src")
        {
            parentStr = $"{parent?.Name}/{parentStr}";

            parent = parent?.Parent;
        }

        return parentStr;
    }

    private string ToAngularFileName(string newCompoenentName)
    {
        var sb = new StringBuilder();

        newCompoenentName = newCompoenentName.Replace("Component", "");

        for (int i = 0; i < newCompoenentName.Length; i++)
        {
            if (char.IsUpper(newCompoenentName[i]) && i > 0)
            {
                sb.Append('-');
                sb.Append(char.ToLower(newCompoenentName[i]));
            }
            else
            {
                sb.Append(char.ToLower(newCompoenentName[i]));
            }
        }

        sb.Append(".component");

        return sb.ToString();
    }

    private string GetStoriesName(string fileName)
    {
        var sb = new StringBuilder();
        var mustBeUpper = true;

        for (int i = 0; i < fileName.Length; i++) 
        {
            if (mustBeUpper) 
            {
                sb.Append(char.ToUpper(fileName[i]));
                mustBeUpper = false;
            }
            else if (fileName[i] == '-') 
            {
                mustBeUpper = true;
            } 
            else 
            {
                sb.Append(fileName[i]);
            }
        }

        if (appOption.DestFolder is null) 
        {
            throw new InvalidOperationException("DestFolder is required");
        }

        return sb.ToString();
    }

    const string baseDatabFixture = @"var customerAccess = new CustomerAccess();
customerAccess.id = faker.datatype.uuid();
customerAccess.idCustomer = faker.datatype.uuid();
customerAccess.idErabliere = faker.datatype.uuid();
customerAccess.access = faker.datatype.number({min: 0, max: 15});
customerAccess.customer = new Customer();
customerAccess.customer.id = customerAccess.idCustomer;
customerAccess.customer.name = faker.name.firstName();
customerAccess.customer.email = faker.internet.email();
customerAccess.customer.uniqueName = customerAccess.customer.email;";

    const string defaultDataFixture = "var fixture = {};";

    const string baseStories = @"//👇 We create a “template” of how args map to rendering
const Template: Story = (args) => ({
  props: args,
});

//👇 Each story then reuses that template
export const Display = Template.bind({});

Display.args = {
  acces: customerAccess
};

export const Edit = Template.bind({});

Edit.args = {
  acces: customerAccess,
  displayEditAccess: true
};
";

    const string defaultStories = @"//👇 We create a “template” of how args map to rendering
const Template: Story = (args) => ({
  props: args,
});

//👇 Each story then reuses that template
export const Primary = Template.bind({});

Primary.args = {
  
};
";

}