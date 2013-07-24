namespace eCentral.Web.Framework.UI
{
    public interface IPageTitleBuilder
    {
        void AddTitleParts(params string[] parts);
        void AppendTitleParts(params string[] parts);
        string GenerateTitle(bool addDefaultTitle);

        void AddScriptParts(ResourceLocation location, params string[] parts);
        void AppendScriptParts(ResourceLocation location, params string[] parts);
        string GenerateScripts(ResourceLocation location);

        void AddCssFileParts(ResourceLocation location, params string[] parts);
        void AppendCssFileParts(ResourceLocation location, params string[] parts);
        string GenerateCssFiles(ResourceLocation location);


        void AddCanonicalUrlParts(params string[] parts);
        void AppendCanonicalUrlParts(params string[] parts);
        string GenerateCanonicalUrls();
    }
}
