namespace DesignPattern.StructuralPattern.Decorator;

public class AppException(string message, Exception? innerException = null) : Exception(message, innerException)
{
    public Exception GetInnermostException()
    {
        return InnerException switch
        {
            null               => this,
            AppException appEx => appEx.GetInnermostException(),
            _                  => InnerException
        };

    }
}