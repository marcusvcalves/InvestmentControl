namespace InvestmentControl.Infrastructure.Utils;

public static class PathUtils
{
    /// <summary>
    /// Busca o diretório da solução
    /// </summary>
    /// <param name="startPath">Caminho inicial</param>
    /// <returns>Diretório da solução</returns>
    public static string? GetSolutionPath(string? startPath = null)
    {
        startPath ??= Directory.GetCurrentDirectory();

        var directory = new DirectoryInfo(startPath);
        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        return directory?.FullName;
    }
}
