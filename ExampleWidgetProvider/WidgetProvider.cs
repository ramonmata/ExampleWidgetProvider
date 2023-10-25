using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.Widgets.Providers;

namespace ExampleWidgetProvider
{
    public class CompactWidgetInfo
    {
        public string widgetId { get; set; }
        public string widgetName { get; set; }
        public int customState = 0;
        public bool isActive = false;
    }

    internal class WidgetProvider : IWidgetProvider
    {
        public void Activate(WidgetContext widgetContext)
        {
            throw new NotImplementedException();
        }

        public void CreateWidget(WidgetContext widgetContext)
        {
            throw new NotImplementedException();
        }

        public void Deactivate(string widgetId)
        {
            throw new NotImplementedException();
        }

        public void DeleteWidget(string widgetId, string customState)
        {
            throw new NotImplementedException();
        }

        public void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs)
        {
            throw new NotImplementedException();
        }

        public void OnWidgetContextChanged(WidgetContextChangedArgs contextChangedArgs)
        {
            throw new NotImplementedException();
        }
    }
}
