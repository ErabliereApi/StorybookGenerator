public class AppOptions {

    public string SrcFolder { get; set; } = "";

    public string DestFolder { get; set; } = "";

    public string? Template { get; set; } = "";

    /// <summary>
    /// A lazy loading property for the template content
    /// </summary>
    private string? _templateContent;

    public string TemplateContent 
    {
         get 
         {
            if (Template is null)
            {
                return "";
            }
            
            if (_templateContent is null)
            {
                _templateContent = File.ReadAllText(Template);
            }

            return _templateContent;
        } 
    }

    public string TemplateComponentName()
    {
        var templateComponentName = Path.GetFileNameWithoutExtension(Template)?.Replace(".stories", "");

        if (templateComponentName is null)
        {
            throw new InvalidOperationException("TemplateComponentName is required");
        }

        return $"{templateComponentName}Component";
    }
}