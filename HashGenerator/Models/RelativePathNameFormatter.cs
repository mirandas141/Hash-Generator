namespace HashGenerator.Models;

internal class RelativePathNameFormatter : INameFormatter
{
    private readonly string _basePath;

    public RelativePathNameFormatter(string basePath)
    {
        if (basePath.EndsWith(Path.DirectorySeparatorChar)
            || Directory.Exists(basePath))
        {
            _basePath = basePath;
            if (!_basePath.EndsWith(Path.DirectorySeparatorChar))
                _basePath += Path.DirectorySeparatorChar;
            return;
        }
        _basePath = Directory.GetParent(basePath).FullName;
    }

    public string Format(string path)
    {
        return path.Replace(_basePath, "");
    }
}
