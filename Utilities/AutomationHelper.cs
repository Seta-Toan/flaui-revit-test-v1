using FlaUI.Core.AutomationElements;
using FlaUI.Core.Exceptions;
using System;

namespace ClassLibrary1.Utilities
{
    public static class AutomationHelper
    {
        public static string GetSafeClassName(AutomationElement element)
        {
            try
            {
                return element.ClassName ?? "N/A";
            }
            catch (PropertyNotSupportedException)
            {
                return "PropertyNotSupported";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public static bool IsElementVisible(AutomationElement element)
        {
            try
            {
                return element != null && !element.Properties.IsOffscreen;
            }
            catch
            {
                return false;
            }
        }

        public static bool WaitForElement(AutomationElement? element, int timeoutSeconds = 10)
        {
            if (element == null) return false;
            
            var startTime = DateTime.Now;
            while (DateTime.Now - startTime < TimeSpan.FromSeconds(timeoutSeconds))
            {
                if (IsElementVisible(element))
                    return true;
                System.Threading.Thread.Sleep(500);
            }
            return false;
        }
    }
}
