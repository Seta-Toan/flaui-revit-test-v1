using FlaUI.Core;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using System;
using WinForms = System.Windows.Forms;
using ClassLibrary1.Utilities;
using System.Linq;

namespace ClassLibrary1.Setup
{
    public static class DialogHandlers
    {
        public static void HandleTrialDialogInSetup(UIA3Automation automation)
        {
            try
            {
                Console.WriteLine("🔍 BƯỚC 1: Tìm trial dialog...");
                var startTime = DateTime.Now;
                
                var desktop = automation.GetDesktop();
                AutomationElement? webViewDialog = null;
                
                // Tìm kiếm nhanh với timeout ngắn
                Console.WriteLine(" Tìm WebView dialog với timeout ngắn...");
                
                // Method 1: Tìm theo AutomationId (nhanh nhất) - GIỮ LẠI
                try
                {
                    Console.WriteLine("🔍 Tìm theo AutomationId 'QApplication.WebView2BrowserDlg'...");
                    webViewDialog = desktop.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByAutomationId("QApplication.WebView2BrowserDlg")));
                    
                    if (webViewDialog != null)
                    {
                        Console.WriteLine($"✅ Tìm thấy trial dialog theo AutomationId: '{webViewDialog.Name}'");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Lỗi khi tìm theo AutomationId: {ex.Message}");
                }
                
                // Method 2: Tìm theo tên nếu Method 1 thất bại - GIỮ LẠI
                if (webViewDialog == null)
                {
                    try
                    {
                        Console.WriteLine("🔍 Tìm theo tên 'WebView2 WebBrowser'...");
                        webViewDialog = desktop.FindFirstDescendant(cf => 
                            cf.ByControlType(ControlType.Window).And(cf.ByName("WebView2 WebBrowser")));
                        
                        if (webViewDialog != null)
                        {
                            Console.WriteLine($"✅ Tìm thấy trial dialog theo tên: '{webViewDialog.Name}'");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Lỗi khi tìm theo tên: {ex.Message}");
                    }
                }
                
                // XÓA: Method 3 và 4 - Quá chậm, không cần thiết
                
                var searchTime = DateTime.Now - startTime;
                Console.WriteLine($"⏱️ Thời gian tìm kiếm trial dialog: {searchTime.TotalSeconds:F1}s");
                
                if (webViewDialog != null)
                {
                    Console.WriteLine($"✅ Tìm thấy trial dialog: '{webViewDialog.Name}'");
                    
                    // Xử lý nhanh: Tìm nút đóng
                    var closeStartTime = DateTime.Now;
                    var closeButton = FindCloseButtonInDialog(webViewDialog);
                    
                    if (closeButton != null)
                    {
                        Console.WriteLine($"✅ Tìm thấy nút đóng: '{closeButton.Name}' (Type: {closeButton.ControlType})");
                        try
                        {
                            Console.WriteLine("🖱️ Đang click nút đóng trial dialog...");
                            closeButton.Click();
                            System.Threading.Thread.Sleep(200); // Giảm từ 500ms xuống 200ms
                            Console.WriteLine("✅ Đã click nút đóng trial dialog thành công");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Lỗi khi click nút đóng: {ex.Message}");
                            Console.WriteLine("🔄 Sử dụng keyboard shortcut ESC...");
                            System.Windows.Forms.SendKeys.SendWait("{ESC}");
                            System.Threading.Thread.Sleep(200); // Giảm từ 500ms xuống 200ms
                            Console.WriteLine("✅ Đã gửi ESC để đóng trial dialog");
                        }
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Không tìm thấy nút đóng, sử dụng ESC...");
                        System.Windows.Forms.SendKeys.SendWait("{ESC}");
                        System.Threading.Thread.Sleep(200); // Giảm từ 500ms xuống 200ms
                        Console.WriteLine("✅ Đã gửi ESC để đóng trial dialog");
                    }
                    
                    var closeTime = DateTime.Now - closeStartTime;
                    Console.WriteLine($"⏱️ Thời gian xử lý trial dialog: {closeTime.TotalSeconds:F1}s");
                }
                else
                {
                    Console.WriteLine("ℹ️ Không tìm thấy trial dialog nào - tiếp tục tìm security dialog");
                }
                
                var totalTime = DateTime.Now - startTime;
                Console.WriteLine($"⏱️ Tổng thời gian xử lý trial dialog: {totalTime.TotalSeconds:F1}s");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi xử lý trial dialog: {ex.Message}");
            }
        }

        private static AutomationElement? FindCloseButtonInDialog(AutomationElement dialog)
        {
            try
            {
                Console.WriteLine("🔍 Đang tìm nút đóng trong dialog...");
                
                // Chỉ tìm theo class name "btn-close no-outline" (nút đóng thực sự)
                var closeByClass = FindCloseButtonBySpecificClassName(dialog);
                if (closeByClass != null)
                {
                    Console.WriteLine("✅ Tìm thấy nút đóng theo class name");
                    return closeByClass;
                }
                
                Console.WriteLine("ℹ️ Không tìm thấy nút đóng");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi tìm nút đóng: {ex.Message}");
                return null;
            }
        }

        private static AutomationElement? FindCloseButtonBySpecificClassName(AutomationElement dialog)
        {
            try
            {
                // Chỉ tìm class name cụ thể cho nút đóng
                var specificClassNames = new[] { "btn-close no-outline" };
                
                Console.WriteLine("🔍 Tìm kiếm nút đóng theo class name 'btn-close no-outline'...");
                
                // Với WebView dialog, cần tìm kiếm khác
                Console.WriteLine($"🔍 Dialog type: {dialog.ControlType}, Name: '{dialog.Name}'");
                
                // Tìm tất cả các control có thể click được
                var allControls = dialog.FindAllDescendants(cf => cf.ByControlType(ControlType.Button)
                    .Or(cf.ByControlType(ControlType.Hyperlink)).Or(cf.ByControlType(ControlType.Custom))
                    .Or(cf.ByControlType(ControlType.Image)));
                
                Console.WriteLine($"🔍 Tìm thấy {allControls.Length} controls để kiểm tra");
                
                // Nếu không tìm thấy controls, thử tìm theo cách khác
                if (allControls.Length == 0)
                {
                    Console.WriteLine("🔍 Không tìm thấy controls, thử tìm theo cách khác...");
                    
                    // Thử tìm tất cả elements con
                    var allElements = dialog.FindAllDescendants(cf => cf.ByControlType(ControlType.Window)
                        .Or(cf.ByControlType(ControlType.Pane)).Or(cf.ByControlType(ControlType.Custom))
                        .Or(cf.ByControlType(ControlType.Button)).Or(cf.ByControlType(ControlType.Hyperlink))
                        .Or(cf.ByControlType(ControlType.Image)).Or(cf.ByControlType(ControlType.Text)));
                    Console.WriteLine($"🔍 Tìm thấy {allElements.Length} elements tổng cộng");
                    
                    // Log một số elements đầu tiên để debug
                    for (int i = 0; i < Math.Min(allElements.Length, 10); i++)
                    {
                        try
                        {
                            var element = allElements[i];
                            var elementName = element.Name ?? "NO NAME";
                            var elementType = element.ControlType.ToString();
                            var elementClassName = AutomationHelper.GetSafeClassName(element);
                            
                            Console.WriteLine($"🔍 Element {i + 1}: Name='{elementName}', Type={elementType}, Class='{elementClassName}'");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Lỗi khi đọc element {i + 1}: {ex.Message}");
                        }
                    }
                    
                    // Thử tìm theo tên "close", "X", "Close"
                    var closeByName = allElements.FirstOrDefault(e => 
                    {
                        try
                        {
                            var name = e.Name?.ToLower() ?? "";
                            return name.Contains("close") || name.Contains("x") || name == "close";
                        }
                        catch { return false; }
                    });
                    
                    if (closeByName != null)
                    {
                        Console.WriteLine($"✅ Tìm thấy nút đóng theo tên: '{closeByName.Name}'");
                        return closeByName;
                    }
                }
                
                // Log tất cả controls để debug
                for (int i = 0; i < allControls.Length; i++)
                {
                    var control = allControls[i];
                    try
                    {
                        var className = AutomationHelper.GetSafeClassName(control);
                        var controlName = control.Name ?? "NO NAME";
                        var controlType = control.ControlType.ToString();
                        
                        Console.WriteLine($"🔍 Control {i + 1}: Name='{controlName}', Type={controlType}, Class='{className}'");
                        
                        if (!string.IsNullOrEmpty(className))
                        {
                            var classNameLower = className.ToLower();
                            foreach (var specificClass in specificClassNames)
                            {
                                // Tìm kiếm CHÍNH XÁC class name, không phải contains
                                if (classNameLower.Equals(specificClass.ToLower()))
                                {
                                    Console.WriteLine($"✅ Tìm thấy nút đóng theo class name CHÍNH XÁC: '{className}' (Name: '{controlName}')");
                                    return control;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Lỗi khi đọc control {i + 1}: {ex.Message}");
                    }
                }
                
                Console.WriteLine("ℹ️ Không tìm thấy control nào có class name CHÍNH XÁC 'btn-close no-outline'");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi tìm theo class name cụ thể: {ex.Message}");
                return null;
            }
        }

        public static void AutoEnterProject(Window mainWindow)
        {
            try
            {
                Console.WriteLine("🔍 BƯỚC 3: Tự động vào project...");
                
                // Chờ ngắn hơn để Revit ổn định
                System.Threading.Thread.Sleep(200); // Giảm từ 1000ms xuống 200ms
                
                // Method 1: Tìm Recent Projects (ƯU TIÊN CAO NHẤT) - GIỮ LẠI
                Console.WriteLine("🔍 Tìm Recent Projects (ưu tiên cao nhất)...");
                var recentProject = FindRecentProject(mainWindow);
                if (recentProject != null)
                {
                    Console.WriteLine($"✅ Tìm thấy project gần đây: '{recentProject.Name}'");
                    try
                    {
                        Console.WriteLine("🖱️ Đang click vào project gần đây...");
                        recentProject.Click();
                        System.Threading.Thread.Sleep(500); // Giảm từ 2000ms xuống 500ms
                        Console.WriteLine("✅ Đã click vào project gần đây thành công");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Lỗi khi click project gần đây: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ Không tìm thấy Recent Projects");
                }
                
                // Method 2: Tìm Available Projects (TRÁNH AUTODESK CLOUD) - GIỮ LẠI
                Console.WriteLine("🔍 Tìm Available Projects (tránh Autodesk cloud)...");
                var availableProject = FindAvailableProject(mainWindow);
                if (availableProject != null)
                {
                    Console.WriteLine($"✅ Tìm thấy project có sẵn: '{availableProject.Name}'");
                    try
                    {
                        Console.WriteLine("🖱️ Đang click vào project có sẵn...");
                        availableProject.Click();
                        System.Threading.Thread.Sleep(500); // Giảm từ 2000ms xuống 500ms
                        Console.WriteLine("✅ Đã click vào project có sẵn thành công");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Lỗi khi click project có sẵn: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ Không tìm thấy Available Projects");
                }
                
                // Method 3: Tạo project mới (TRÁNH AUTODESK CLOUD) - GIỮ LẠI
                Console.WriteLine("🔍 Không tìm thấy project nào, tạo project mới (tránh Autodesk cloud)...");
                var newProjectButton = FindNewProjectButton(mainWindow);
                if (newProjectButton != null)
                {
                    Console.WriteLine($"✅ Tìm thấy nút tạo project mới: '{newProjectButton.Name}'");
                    try
                    {
                        Console.WriteLine("🖱️ Đang click vào nút tạo project mới...");
                        newProjectButton.Click();
                        System.Threading.Thread.Sleep(200); // Giảm từ 1000ms xuống 200ms
                        Console.WriteLine("✅ Đã click vào nút tạo project mới thành công");
                        
                        // Chờ dialog tạo project mới xuất hiện
                        System.Threading.Thread.Sleep(200); // Giảm từ 1000ms xuống 200ms
                        
                        // Xử lý dialog tạo project mới nếu có
                        HandleNewProjectDialog(mainWindow);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Lỗi khi click nút tạo project mới: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ Không tìm thấy nút tạo project mới");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi tự động vào project: {ex.Message}");
            }
        }

        private static void HandleNewProjectDialog(Window mainWindow)
        {
            try
            {
                Console.WriteLine("🔍 Xử lý dialog tạo project mới...");
                
                // Tìm dialog tạo project mới
                var newProjectDialog = mainWindow.FindFirstDescendant(cf => 
                    cf.ByControlType(ControlType.Window).And(cf.ByName("New Project")));
                
                if (newProjectDialog == null)
                {
                    newProjectDialog = mainWindow.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Create New Project")));
                }
                
                if (newProjectDialog == null)
                {
                    newProjectDialog = mainWindow.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Project Template")));
                }
                
                if (newProjectDialog != null)
                {
                    Console.WriteLine($"✅ Tìm thấy dialog tạo project mới: '{newProjectDialog.Name}'");
                    
                    // Tìm template "None" hoặc "Empty Project"
                    var noneTemplate = newProjectDialog.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.ListItem).And(cf.ByName("None")));
                    
                    if (noneTemplate == null)
                    {
                        noneTemplate = newProjectDialog.FindFirstDescendant(cf => 
                            cf.ByControlType(ControlType.ListItem).And(cf.ByName("Empty Project")));
                    }
                    
                    if (noneTemplate == null)
                    {
                        noneTemplate = newProjectDialog.FindFirstDescendant(cf => 
                            cf.ByControlType(ControlType.ListItem).And(cf.ByName("No template")));
                    }
                    
                    if (noneTemplate != null)
                    {
                        Console.WriteLine($"✅ Tìm thấy template 'None': '{noneTemplate.Name}'");
                        try
                        {
                            Console.WriteLine("🖱️ Đang click vào template 'None'...");
                            noneTemplate.Click();
                            System.Threading.Thread.Sleep(2000);
                            Console.WriteLine("✅ Đã click vào template 'None' thành công");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Lỗi khi click template 'None': {ex.Message}");
                        }
                    }
                    
                    // Tìm nút "OK" hoặc "Create"
                    var okButton = newProjectDialog.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Button).And(cf.ByName("OK")));
                    
                    if (okButton == null)
                    {
                        okButton = newProjectDialog.FindFirstDescendant(cf => 
                            cf.ByControlType(ControlType.Button).And(cf.ByName("Create")));
                    }
                    
                    if (okButton == null)
                    {
                        okButton = newProjectDialog.FindFirstDescendant(cf => 
                            cf.ByControlType(ControlType.Button).And(cf.ByName("Finish")));
                    }
                    
                    if (okButton != null)
                    {
                        Console.WriteLine($"✅ Tìm thấy nút tạo project: '{okButton.Name}'");
                        try
                        {
                            Console.WriteLine("🖱️ Đang click nút tạo project...");
                            okButton.Click();
                            System.Threading.Thread.Sleep(3000);
                            Console.WriteLine("✅ Đã click nút tạo project thành công");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Lỗi khi click nút tạo project: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ Không tìm thấy dialog tạo project mới");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi xử lý dialog tạo project mới: {ex.Message}");
            }
        }

        private static AutomationElement? FindAvailableProject(Window mainWindow)
        {
            try
            {
                Console.WriteLine("🔍 Tìm Available Projects (tránh Autodesk cloud)...");
                
                // Tìm tất cả các control có thể là project
                var allControls = mainWindow.FindAllDescendants(cf => 
                    cf.ByControlType(ControlType.ListItem).Or(cf.ByControlType(ControlType.Hyperlink))
                    .Or(cf.ByControlType(ControlType.Button)).Or(cf.ByControlType(ControlType.Text)));
                
                foreach (var control in allControls)
                {
                    try
                    {
                        var name = control.Name;
                        if (!string.IsNullOrEmpty(name))
                        {
                            // KIỂM TRA QUAN TRỌNG: Tránh Autodesk cloud
                            if (name.Contains("Autodesk") || name.Contains("cloud") || name.Contains("Cloud") || 
                                name.Contains("You currently don't have access") || name.Contains("Revit cloud models"))
                            {
                                Console.WriteLine($"⚠️ Bỏ qua Autodesk cloud: '{name}'");
                                continue;
                            }
                            
                            // Kiểm tra xem có phải là project không
                            if (name.EndsWith(".rvt") || 
                                name.Contains("Project") || 
                                name.Contains("Revit") ||
                                name.Contains("Architectural") ||
                                name.Contains("Structural") ||
                                name.Contains("MEP") ||
                                name.Contains("Template"))
                            {
                                Console.WriteLine($"✅ Tìm thấy project có sẵn: '{name}'");
                                return control;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Lỗi khi đọc tên project: {ex.Message}");
                    }
                }
                
                Console.WriteLine("ℹ️ Không tìm thấy Available Projects");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi tìm Available Project: {ex.Message}");
                return null;
            }
        }

        private static AutomationElement? FindRecentProject(Window mainWindow)
        {
            try
            {
                Console.WriteLine("🔍 Tìm Recent Projects...");
                
                // Tìm section "Recent" trước
                var recentSection = mainWindow.FindFirstDescendant(cf => 
                    cf.ByControlType(ControlType.Group).And(cf.ByName("Recent")));
                
                if (recentSection == null)
                {
                    recentSection = mainWindow.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Group).And(cf.ByName("Recent Projects")));
                }
                
                if (recentSection == null)
                {
                    recentSection = mainWindow.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Pane).And(cf.ByName("Recent")));
                }
                
                if (recentSection == null)
                {
                    recentSection = mainWindow.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Pane).And(cf.ByName("Recent Projects")));
                }
                
                if (recentSection != null)
                {
                    Console.WriteLine($"✅ Tìm thấy section Recent: '{recentSection.Name}'");
                    
                    // Tìm project đầu tiên trong Recent section
                    var recentProjects = recentSection.FindAllDescendants(cf => 
                        cf.ByControlType(ControlType.ListItem).Or(cf.ByControlType(ControlType.Hyperlink))
                        .Or(cf.ByControlType(ControlType.Button)).Or(cf.ByControlType(ControlType.Text)));
                    
                    foreach (var project in recentProjects)
                    {
                        try
                        {
                            var name = project.Name;
                            if (!string.IsNullOrEmpty(name) && 
                                (name.EndsWith(".rvt") || name.Contains("Project") || name.Contains("Revit")))
                            {
                                Console.WriteLine($"✅ Tìm thấy project trong Recent: '{name}'");
                                return project;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Lỗi khi đọc tên project: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ Không tìm thấy section Recent, tìm trực tiếp...");
                }
                
                // Tìm trực tiếp nếu không tìm thấy section
                var allProjects = mainWindow.FindAllDescendants(cf => 
                    cf.ByControlType(ControlType.ListItem).Or(cf.ByControlType(ControlType.Hyperlink))
                    .Or(cf.ByControlType(ControlType.Button)).Or(cf.ByControlType(ControlType.Text)));
                
                foreach (var project in allProjects)
                {
                    try
                    {
                        var name = project.Name;
                        if (!string.IsNullOrEmpty(name) && 
                            (name.EndsWith(".rvt") || name.Contains("Project") || name.Contains("Revit")))
                        {
                            Console.WriteLine($"✅ Tìm thấy project: '{name}'");
                            return project;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Lỗi khi đọc tên project: {ex.Message}");
                    }
                }
                
                Console.WriteLine("ℹ️ Không tìm thấy Recent Projects");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi tìm Recent Project: {ex.Message}");
                return null;
            }
        }

        private static AutomationElement? FindNewProjectButton(Window mainWindow)
        {
            try
            {
                Console.WriteLine("🔍 Tìm nút tạo project mới (tránh Autodesk cloud)...");
                
                // Tìm các nút có thể tạo project mới - TRÁNH AUTODESK CLOUD
                var newProjectButtons = mainWindow.FindAllDescendants(cf => 
                    cf.ByControlType(ControlType.Button).And(cf.ByName("New")));
                
                if (newProjectButtons.Length > 0)
                {
                    Console.WriteLine($"✅ Tìm thấy {newProjectButtons.Length} nút tạo project mới");
                    foreach (var button in newProjectButtons)
                    {
                        try
                        {
                            var buttonName = button.Name ?? "NO NAME";
                            // KIỂM TRA QUAN TRỌNG: Tránh Autodesk cloud
                            if (buttonName.Contains("Autodesk") || buttonName.Contains("cloud") || buttonName.Contains("Cloud"))
                            {
                                Console.WriteLine($"⚠️ Bỏ qua nút Autodesk cloud: '{buttonName}'");
                                continue;
                            }
                            
                            Console.WriteLine($"  - Nút: '{buttonName}' (Type: {button.ControlType})");
                            return button; // Trả về nút đầu tiên không phải Autodesk cloud
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Lỗi khi đọc nút: {ex.Message}");
                        }
                    }
                }
                
                // Tìm theo tên chứa từ khóa - TRÁNH AUTODESK CLOUD
                var allButtons = mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Button));
                foreach (var button in allButtons)
                {
                    try
                    {
                        var name = button.Name?.ToLower() ?? "";
                        // KIỂM TRA QUAN TRỌNG: Tránh Autodesk cloud
                        if (name.Contains("autodesk") || name.Contains("cloud"))
                        {
                            continue;
                        }
                        
                        if (name.Contains("new") || name.Contains("create") || name.Contains("start") || name.Contains("project"))
                        {
                            Console.WriteLine($"✅ Tìm thấy nút tạo project theo từ khóa: '{button.Name}'");
                            return button;
                        }
                    }
                    catch { }
                }
                
                Console.WriteLine("ℹ️ Không tìm thấy nút tạo project mới");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi tìm nút tạo project mới: {ex.Message}");
                return null;
            }
        }

        public static bool IsProjectLoaded(Window mainWindow)
        {
            try
            {
                Console.WriteLine("🔍 Kiểm tra xem project đã load chưa...");
                
                // Kiểm tra xem có tab nào hiển thị không (dấu hiệu project đã load)
                var tabs = mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.TabItem));
                
                if (tabs.Length > 0)
                {
                    Console.WriteLine($"✅ Tìm thấy {tabs.Length} tabs, project đã load thành công");
                    return true;
                }
                
                // Kiểm tra xem có ribbon bar không
                var ribbonBar = mainWindow.FindFirstDescendant(cf => 
                    cf.ByControlType(ControlType.ToolBar).And(cf.ByName("Ribbon")));
                
                if (ribbonBar != null)
                {
                    Console.WriteLine("✅ Tìm thấy Ribbon bar, project đã load thành công");
                    return true;
                }
                
                // Kiểm tra xem có menu bar không
                var menuBar = mainWindow.FindFirstDescendant(cf => 
                    cf.ByControlType(ControlType.MenuBar));
                
                if (menuBar != null)
                {
                    Console.WriteLine("✅ Tìm thấy Menu bar, project đã load thành công");
                    return true;
                }
                
                Console.WriteLine("ℹ️ Chưa thấy dấu hiệu project đã load");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi kiểm tra project load: {ex.Message}");
                return false;
            }
        }

        public static void HandleSecurityDialogInSetup(UIA3Automation automation)
        {
            try
            {
                Console.WriteLine("🔍 BƯỚC 2: Tìm security dialog...");
                
                var desktop = automation.GetDesktop();
                
                // Tìm security dialog với timeout ngắn hơn
                var securityDialog = desktop.FindFirstDescendant(cf => 
                    cf.ByControlType(ControlType.Window).And(cf.ByName("Security - Unsigned Add-In")));
                
                if (securityDialog == null)
                {
                    Console.WriteLine("🔍 Không tìm thấy 'Security - Unsigned Add-In', thử tìm tên khác...");
                    securityDialog = desktop.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Security")));
                }
                
                if (securityDialog == null)
                {
                    Console.WriteLine("🔍 Không tìm thấy 'Security', thử tìm tên khác...");
                    securityDialog = desktop.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Add-In Security")));
                }
                
                if (securityDialog != null)
                {
                    Console.WriteLine($"✅ Tìm thấy security dialog: '{securityDialog.Name}'");
                    
                    var loadOnceButton = securityDialog.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Button).And(cf.ByName("Load Once")));
                    
                    if (loadOnceButton != null)
                    {
                        Console.WriteLine("✅ Tìm thấy nút 'Load Once'");
                        try
                        {
                            Console.WriteLine("🖱️ Đang click 'Load Once'...");
                            loadOnceButton.Click();
                            System.Threading.Thread.Sleep(300); // Giảm từ 1000ms xuống 300ms
                            Console.WriteLine("✅ Đã click 'Load Once' thành công");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Lỗi khi click 'Load Once': {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Không tìm thấy nút 'Load Once'");
                        // XÓA: Log tất cả buttons - không cần thiết, làm chậm test
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ Không tìm thấy security dialog nào");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi xử lý security dialog: {ex.Message}");
            }
        }

        public static bool HandleTrialDialog(Window mainWindow)
        {
            try
            {
                Console.WriteLine("🔍 Đang tìm dialog trial từ main window...");
                
                // Tìm trial dialog từ main window trước
                var webViewDialog = mainWindow.FindFirstDescendant(cf => 
                    cf.ByControlType(ControlType.Window).And(cf.ByAutomationId("QApplication.WebView2BrowserDlg")));
                
                if (webViewDialog == null)
                {
                    webViewDialog = mainWindow.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("WebView2 WebBrowser")));
                }
                
                // Nếu không tìm thấy từ main window, tìm từ desktop
                if (webViewDialog == null)
                {
                    Console.WriteLine("🔍 Không tìm thấy từ main window, tìm từ desktop...");
                    var desktop = mainWindow.Automation.GetDesktop();
                    webViewDialog = desktop.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByAutomationId("QApplication.WebView2BrowserDlg")));
                }
                
                if (webViewDialog == null)
                {
                    var desktop = mainWindow.Automation.GetDesktop();
                    webViewDialog = desktop.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("WebView2 WebBrowser")));
                }
                
                // Tìm các loại trial dialog khác
                if (webViewDialog == null)
                {
                    var desktop = mainWindow.Automation.GetDesktop();
                    webViewDialog = desktop.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Trial")));
                }
                
                if (webViewDialog == null)
                {
                    var desktop = mainWindow.Automation.GetDesktop();
                    webViewDialog = desktop.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Evaluation")));
                }
                
                if (webViewDialog != null)
                {
                    Console.WriteLine($"✅ Tìm thấy trial dialog: '{webViewDialog.Name}'");
                    
                    // Tìm nút đóng với logic mới
                    var closeButton = FindCloseButtonInDialog(webViewDialog);
                    
                    if (closeButton != null)
                    {
                        Console.WriteLine($"✅ Tìm thấy nút đóng: '{closeButton.Name}' (Type: {closeButton.ControlType})");
                        try
                        {
                            Console.WriteLine("🖱️ Đang click nút đóng...");
                            closeButton.Click();
                            System.Threading.Thread.Sleep(3000);
                            Console.WriteLine("✅ Đã click nút đóng thành công");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Lỗi khi click nút đóng: {ex.Message}");
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Không tìm thấy nút đóng");
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ Không tìm thấy trial dialog nào");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi xử lý trial dialog: {ex.Message}");
                return false;
            }
        }

        public static bool HandleSecurityDialog(Window mainWindow)
        {
            try
            {
                Console.WriteLine("🔍 Đang tìm security dialog từ main window...");
                
                // Tìm từ main window trước
                var securityDialog = mainWindow.FindFirstDescendant(cf => 
                    cf.ByControlType(ControlType.Window).And(cf.ByName("Security - Unsigned Add-In")));
                
                if (securityDialog == null)
                {
                    securityDialog = mainWindow.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Security")));
                }
                
                if (securityDialog == null)
                {
                    securityDialog = mainWindow.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Add-In Security")));
                }
                
                // Nếu không tìm thấy từ main window, tìm từ desktop
                if (securityDialog == null)
                {
                    Console.WriteLine("🔍 Không tìm thấy từ main window, tìm từ desktop...");
                    var desktop = mainWindow.Automation.GetDesktop();
                    securityDialog = desktop.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Security - Unsigned Add-In")));
                }
                
                if (securityDialog == null)
                {
                    var desktop = mainWindow.Automation.GetDesktop();
                    securityDialog = desktop.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Security")));
                }
                
                if (securityDialog != null)
                {
                    Console.WriteLine($"✅ Tìm thấy security dialog: '{securityDialog.Name}'");
                    
                    var loadOnceButton = securityDialog.FindFirstDescendant(cf => 
                        cf.ByControlType(ControlType.Button).And(cf.ByName("Load Once")));
                    
                    if (loadOnceButton != null)
                    {
                        Console.WriteLine("✅ Tìm thấy nút 'Load Once', đang click...");
                        try
                        {
                            Console.WriteLine("🖱️ Đang click 'Load Once'...");
                            loadOnceButton.Click();
                            System.Threading.Thread.Sleep(3000);
                            Console.WriteLine("✅ Đã click 'Load Once' thành công");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Lỗi khi click 'Load Once': {ex.Message}");
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Không tìm thấy nút 'Load Once'");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ Không tìm thấy security dialog nào");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi xử lý security dialog: {ex.Message}");
                return false;
            }
        }
    }
}
