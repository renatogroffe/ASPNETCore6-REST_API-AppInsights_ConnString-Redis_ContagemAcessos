using System.Runtime.InteropServices;

namespace APIContagem;

public static class ContagemInfo
{
    public static string Local { get; }
    public static string Kernel { get; }
    public static string Framework { get; }
    public static string Mensagem { get; }

    static ContagemInfo()
    {
        Local = "APIContagemRedis";
        Kernel = Environment.OSVersion.VersionString;
        Framework = RuntimeInformation.FrameworkDescription;
        Mensagem = "Testes com Redis + Application Insights";
    }
}