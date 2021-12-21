using CitizenFX.Core;
using log4net.Appender;
using log4net.Core;

namespace BudPlaza.BladeX
{
    public class CitizenFxAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            Debug.Write(loggingEvent.RenderedMessage);
        }
    }
}
