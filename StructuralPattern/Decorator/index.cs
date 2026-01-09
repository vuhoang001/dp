namespace DesignPattern.StructuralPattern.Decorator;

public interface ITextWriter
{
    string Write(string text);
}

public class PlainTextWriter : ITextWriter
{
    public string Write(string text) => text;
}

public abstract class TextWriteDecorator(ITextWriter inner) : ITextWriter
{
    public virtual string Write(string text) => inner.Write(text);
}

public class UppercaseWriteDecorator(ITextWriter inner) : TextWriteDecorator(inner)
{
    public override string Write(string text)
    {
        text = text.ToUpper();
        return base.Write(text);
    }
}

public class HtmlWriteDecorator(ITextWriter inner) : TextWriteDecorator(inner)
{
    private readonly ITextWriter _inner = inner;

    public override string Write(string text)
        => $"<p>{_inner.Write(text)}</p>";
}