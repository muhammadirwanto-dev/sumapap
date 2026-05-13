using System.Text;

namespace Sumapap.Common.Extensions
{
    public static class ExceptionExtensions
    {
        extension(Exception exception)
        {
            public string GetDeepMessage(bool writeNewLine = true) => exception.GetDeepMessageInternal(writeNewLine);

            private string GetDeepMessageInternal(bool writeNewLine = true, int level = 0)
            {
                if (exception is TaskCanceledException cte)
                {
                    return cte.Message;
                }

                string line = exception is HttpRequestException httpException
                    ? httpException.Message
                    : exception.Message;

                var sb = new StringBuilder();

                if (writeNewLine)
                {
                    sb.Append(new string(' ', level * 2)).AppendLine(line);
                }
                else
                {
                    sb.Append(line);
                }

                if (exception.InnerException != null)
                {
                    line = exception.InnerException.GetDeepMessageInternal(writeNewLine, level + 1);

                    if (!writeNewLine)
                    {
                        sb.Append(" > ");
                    }

                    sb.Append(line);
                }

                return sb.ToString().Trim();
            }
        }
    }
}
