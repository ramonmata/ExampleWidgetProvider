using Microsoft.Windows.Widgets.Providers;

namespace ExampleWidgetProvider
{
    public class CompactWidgetInfo
    {
        public string widgetId { get; set; } = string.Empty;
        public string widgetName { get; set; } = string.Empty;
        public int customState = 0;
        public bool isActive = false;
    }

    internal class WidgetProvider : IWidgetProvider
    {
        public static Dictionary<string, CompactWidgetInfo> RunningWidgets = new Dictionary<string, CompactWidgetInfo>();

        // Class members of WidgetProvider
        const string weatherWidgetTemplate = @"
{
    ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""speak"": ""<s>The forecast for Seattle January 20 is mostly clear with a High of 51 degrees and Low of 40 degrees</s>"",
    ""backgroundImage"": ""https://messagecardplayground.azurewebsites.net/assets/Mostly%20Cloudy-Background.jpg"",
    ""body"": [
        {
            ""type"": ""TextBlock"",
            ""text"": ""Redmond, WA"",
            ""size"": ""large"",
            ""isSubtle"": true,
            ""wrap"": true
        },
        {
            ""type"": ""TextBlock"",
            ""text"": ""Mon, Nov 4, 2019 6:21 PM"",
            ""spacing"": ""none"",
            ""wrap"": true
        },
        {
            ""type"": ""ColumnSet"",
            ""columns"": [
                {
                    ""type"": ""Column"",
                    ""width"": ""auto"",
                    ""items"": [
                        {
                            ""type"": ""Image"",
                            ""url"": ""https://messagecardplayground.azurewebsites.net/assets/Mostly%20Cloudy-Square.png"",
                            ""size"": ""small"",
                            ""altText"": ""Mostly cloudy weather""
                        }
                    ]
                },
                {
                    ""type"": ""Column"",
                    ""width"": ""auto"",
                    ""items"": [
                        {
                            ""type"": ""TextBlock"",
                            ""text"": ""46"",
                            ""size"": ""extraLarge"",
                            ""spacing"": ""none"",
                            ""wrap"": true
                        }
                    ]
                },
                {
                    ""type"": ""Column"",
                    ""width"": ""stretch"",
                    ""items"": [
                        {
                            ""type"": ""TextBlock"",
                            ""text"": ""°F"",
                            ""weight"": ""bolder"",
                            ""spacing"": ""small"",
                            ""wrap"": true
                        }
                    ]
                },
                {
                    ""type"": ""Column"",
                    ""width"": ""stretch"",
                    ""items"": [
                        {
                            ""type"": ""TextBlock"",
                            ""text"": ""Hi 50"",
                            ""horizontalAlignment"": ""left"",
                            ""wrap"": true
                        },
                        {
                            ""type"": ""TextBlock"",
                            ""text"": ""Lo 41"",
                            ""horizontalAlignment"": ""left"",
                            ""spacing"": ""none"",
                            ""wrap"": true
                        }
                    ]
                }
            ]
        }
    ]
}";

        const string countWidgetTemplate = @"
{                                                                     
    ""type"": ""AdaptiveCard"",                                         
    ""body"": [                                                         
        {                                                               
            ""type"": ""TextBlock"",                                    
            ""text"": ""You have clicked the button ${count} times""    
        },
        {
                ""text"":""Rendering Only if Small"",
                ""type"":""TextBlock"",
                ""$when"":""${$host.widgetSize==\""small\""}""
        },
        {
                ""text"":""Rendering Only if Medium"",
                ""type"":""TextBlock"",
                ""$when"":""${$host.widgetSize==\""medium\""}""
        },
        {
            ""text"":""Rendering Only if Large"",
            ""type"":""TextBlock"",
            ""$when"":""${$host.widgetSize==\""large\""}""
        }                                                                    
    ],                                                                  
    ""actions"": [                                                      
        {                                                               
            ""type"": ""Action.Execute"",                               
            ""title"": ""Increment"",                                   
            ""verb"": ""inc""                                           
        }                                                               
    ],                                                                  
    ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
    ""version"": ""1.5""                                                
}";

        public void CreateWidget(WidgetContext widgetContext)
        {
            var widgetId = widgetContext.Id;
            var widgetName = widgetContext.DefinitionId;
            CompactWidgetInfo runningWidgetInfo = new CompactWidgetInfo() { widgetId = widgetId, widgetName = widgetName };
            RunningWidgets[widgetId] = runningWidgetInfo;

            // Update Widget
            UpdateWidget(runningWidgetInfo);
        }

        public void Activate(WidgetContext widgetContext)
        {
            var widgetId = widgetContext.Id;
            if (RunningWidgets.ContainsKey(widgetId))
            {
                var localWidgetInfo = RunningWidgets[widgetId];
                localWidgetInfo.isActive = true;
                UpdateWidget(localWidgetInfo);
            }
        }

        public void Deactivate(string widgetId)
        {
            if (RunningWidgets.ContainsKey(widgetId))
            {
                var localWidgetInfo = RunningWidgets[widgetId];
                localWidgetInfo.isActive = false;
            }
        }

        public void DeleteWidget(string widgetId, string customState)
        {
            RunningWidgets.Remove(widgetId);
            if (RunningWidgets.Count == 0)
            {
                emptyWidgetListEvent.Set();
            }
        }

        public void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs)
        {
            var verb = actionInvokedArgs.Verb;
            if (verb.Equals("inc"))
            {
                var widgetId = actionInvokedArgs.WidgetContext.Id;
                // You can access previous sent data by
                // var data = actionInvokedArgs.Data;
                if (RunningWidgets.ContainsKey(widgetId))
                {
                    var localWidgetInfo = RunningWidgets[widgetId];
                    localWidgetInfo.customState++;
                    UpdateWidget(localWidgetInfo);
                }
            }
        }

        public void OnWidgetContextChanged(WidgetContextChangedArgs contextChangedArgs)
        {
            var widgetContext = contextChangedArgs.WidgetContext;
            var widgetId = widgetContext.Id;
            var widgetSize = widgetContext.Size;

            Console.WriteLine(widgetSize.ToString());
            // Here we could be sending a distinct JSON template / data accordingly to the size of Widget
            if (RunningWidgets.ContainsKey(widgetId))
            {
                var localWidgetInfo = RunningWidgets[widgetId];
                UpdateWidget(localWidgetInfo);
            }
        }

        void UpdateWidget(CompactWidgetInfo localWidgetInfo)
        {
            WidgetUpdateRequestOptions updateOptions = new WidgetUpdateRequestOptions(localWidgetInfo.widgetId);
            
            string templateJSON = "";
            string dataJson = "";

            if (localWidgetInfo.widgetName == "Weather_Widget")
            {
                templateJSON = weatherWidgetTemplate.ToString();
                dataJson = "{}";
            } else if (localWidgetInfo.widgetName == "Counting_Widget")
            {
                templateJSON = countWidgetTemplate.ToString();
                dataJson = "{\"count\": " + localWidgetInfo.customState.ToString()+"}";
            }

            updateOptions.Template = templateJSON;
            updateOptions.Data = dataJson;

            // You can store custom state in the widget service thatr you are able to query at any time
            updateOptions.CustomState = localWidgetInfo.customState.ToString();
            WidgetManager.GetDefault().UpdateWidget(updateOptions);

        }


        // we set an event that will be used later to allow the app to exit when there are no enabled widgets
        static ManualResetEvent emptyWidgetListEvent = new ManualResetEvent(false);

        public static ManualResetEvent GetEmptyWidgetListEvent()
        {
            return emptyWidgetListEvent;
        }
    }
}
