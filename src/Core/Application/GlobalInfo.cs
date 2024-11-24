using Core.News;

namespace Core.Application;

public class GlobalInfo
{
    #region Singleton Pattern

    private static GlobalInfo? _instance;

    public static GlobalInfo GetInstance() => _instance ??= new GlobalInfo();

    private GlobalInfo()
    {
    }

    #endregion

    public int TotalNews { get; set; }
    public DateTime LastNewsSearch { get; set; }
    public List<SubjectEnum> AvailableSubjects { get; set; } = [];
}