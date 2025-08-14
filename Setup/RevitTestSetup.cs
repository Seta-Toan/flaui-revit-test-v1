using NUnit.Framework;
using FlaUI.Core;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using System.Diagnostics;
using System;
using ClassLibrary1.Setup;

namespace ClassLibrary1.Setup
{
    [TestFixture]
    public abstract class RevitTestSetup
    {
        protected FlaUI.Core.Application? revitApp;
        protected UIA3Automation? automation;
        protected Window? mainWindow;
        protected Process? revitProcess;

        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            try
            {
                var totalStartTime = DateTime.Now;
                Console.WriteLine(" === ONETIME SETUP: Khởi động Revit một lần duy nhất ===");
                
                // XÓA: Không cần kill tất cả Revit processes
                // Console.WriteLine("🔄 Đóng tất cả Revit instances...");
                // ProcessManager.KillAllRevitProcesses();
                // System.Threading.Thread.Sleep(1000);
                
                // Khởi động Revit một lần duy nhất
                var revitStartTime = DateTime.Now;
                Console.WriteLine(" Khởi động Revit...");
                var processStartInfo = new ProcessStartInfo(@"D:\Revit\Autodesk\Revit 2026\Revit.exe")
                {
                    UseShellExecute = true,
                    WorkingDirectory = @"D:\Revit\Autodesk\Revit 2026\"
                };
                
                revitApp = FlaUI.Core.Application.Launch(processStartInfo);
                revitProcess = Process.GetProcessById(revitApp.ProcessId);
                automation = new UIA3Automation();
                
                Console.WriteLine($"✅ Revit started với PID: {revitApp.ProcessId}");
                
                // Chờ Revit khởi động với timeout ngắn hơn
                Console.WriteLine("⏳ Chờ Revit khởi động (2 giây)..."); // Giảm từ 5s xuống 2s
                System.Threading.Thread.Sleep(2000);
                
                var revitTime = DateTime.Now - revitStartTime;
                Console.WriteLine($"⏱️ Thời gian Revit khởi động: {revitTime.TotalSeconds:F1}s");
                
                // BƯỚC 1: Xử lý trial dialog với timeout
                var trialStartTime = DateTime.Now;
                Console.WriteLine("🔄 BƯỚC 1: Xử lý trial dialog...");
                DialogHandlers.HandleTrialDialogInSetup(automation);
                
                var trialTime = DateTime.Now - trialStartTime;
                Console.WriteLine($"⏱️ Thời gian xử lý trial dialog: {trialTime.TotalSeconds:F1}s");
                
                // Chờ thêm và BƯỚC 2: Xử lý security dialog
                Console.WriteLine("⏳ Chờ thêm (0.5 giây) rồi xử lý security dialog..."); // Giảm từ 2s xuống 0.5s
                System.Threading.Thread.Sleep(500);
                
                var securityStartTime = DateTime.Now;
                Console.WriteLine("🔄 BƯỚC 2: Xử lý security dialog...");
                DialogHandlers.HandleSecurityDialogInSetup(automation);
                
                var securityTime = DateTime.Now - securityStartTime;
                Console.WriteLine($"⏱️ Thời gian xử lý security dialog: {securityTime.TotalSeconds:F1}s");
                
                // Bây giờ mới tìm main window
                Console.WriteLine("🔍 Tìm main window sau khi xử lý dialogs...");
                System.Threading.Thread.Sleep(200); // Giảm từ 1000ms xuống 200ms
                
                // Kiểm tra xem Revit đã sẵn sàng chưa
                if (!IsRevitReady(revitApp!, automation))
                {
                    Console.WriteLine("⚠️ Revit chưa sẵn sàng, chờ thêm (1 giây)..."); // Giảm từ 3s xuống 1s
                    System.Threading.Thread.Sleep(1000);
                }
                
                // Tìm main window với retry mechanism
                var mainWindowStartTime = DateTime.Now;
                mainWindow = FindMainWindowWithRetry(revitApp!, automation);
                Assert.That(mainWindow, Is.Not.Null, "Không tìm thấy cửa sổ chính của Revit.");
                
                var mainWindowTime = DateTime.Now - mainWindowStartTime;
                Console.WriteLine($"✅ Main window found: {mainWindow!.Title}");
                Console.WriteLine($"⏱️ Thời gian tìm main window: {mainWindowTime.TotalSeconds:F1}s");
                
                // Chờ thêm để Revit hoàn toàn sẵn sàng
                Console.WriteLine("⏳ Chờ Revit ổn định (0.5 giây)..."); // Giảm từ 2s xuống 0.5s
                System.Threading.Thread.Sleep(500);
                
                // BƯỚC 3: Tự động vào project sau khi xử lý dialogs
                var projectStartTime = DateTime.Now;
                Console.WriteLine("🔄 BƯỚC 3: Tự động vào project...");
                DialogHandlers.AutoEnterProject(mainWindow);
                
                // Chờ project load xong và kiểm tra
                Console.WriteLine("⏳ Chờ project load (1 giây)..."); // Giảm từ 3s xuống 1s
                System.Threading.Thread.Sleep(1000);
                
                // Kiểm tra xem project đã load thành công chưa
                var projectLoaded = DialogHandlers.IsProjectLoaded(mainWindow);
                if (projectLoaded)
                {
                    Console.WriteLine("✅ OneTimeSetup: Project đã load thành công");
                }
                else
                {
                    Console.WriteLine("⚠️ OneTimeSetup: Project có thể chưa load hoàn toàn, chờ thêm (1 giây)..."); // Giảm từ 3s xuống 1s
                    System.Threading.Thread.Sleep(1000);
                    
                    // Kiểm tra lại
                    projectLoaded = DialogHandlers.IsProjectLoaded(mainWindow);
                    if (projectLoaded)
                    {
                        Console.WriteLine("✅ OneTimeSetup: Project đã load thành công sau khi chờ thêm");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ OneTimeSetup: Không thể xác nhận project đã load");
                    }
                }
                
                var projectTime = DateTime.Now - projectStartTime;
                Console.WriteLine($"⏱️ Thời gian vào project: {projectTime.TotalSeconds:F1}s");
                
                var totalTime = DateTime.Now - totalStartTime;
                Console.WriteLine("✅ OneTimeSetup: Hoàn tất setup Revit một lần duy nhất");
                Console.WriteLine($"⏱️ Tổng thời gian OneTimeSetup: {totalTime.TotalSeconds:F1}s");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi trong OneTimeSetup: {ex.Message}");
                CleanupRevit();
                throw;
            }
        }

        private Window? FindMainWindowWithRetry(FlaUI.Core.Application app, UIA3Automation automation)
        {
            const int maxRetries = 2; // Giảm từ 5 xuống 2
            const int timeoutSeconds = 10; // Giảm từ 30s xuống 10s
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    Console.WriteLine($"Lần thử {attempt}/{maxRetries}: Tìm main window với timeout {timeoutSeconds}s...");
                    
                    var window = app.GetMainWindow(automation, TimeSpan.FromSeconds(timeoutSeconds));
                    if (window != null)
                    {
                        Console.WriteLine($"✓ Tìm thấy main window ở lần thử {attempt}");
                        return window;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lần thử {attempt} thất bại: {ex.Message}");
                    
                    if (attempt < maxRetries)
                    {
                        Console.WriteLine($"Chờ 2 giây trước khi thử lại..."); // Giảm từ 10s xuống 2s
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }
            
            Console.WriteLine($"✗ Không thể tìm thấy main window sau {maxRetries} lần thử");
            return null;
        }

        [SetUp]
        public virtual void Setup()
        {
            try
            {
                Console.WriteLine("=== SETUP: Kiểm tra Revit đã sẵn sàng ===");
                
                // Kiểm tra xem Revit có còn chạy không
                if (revitApp == null || revitApp.HasExited || mainWindow == null)
                {
                    Console.WriteLine("Revit đã bị đóng, khởi động lại...");
                    OneTimeSetup();
                    return;
                }
                
                // Kiểm tra xem main window có còn hiển thị không
                try
                {
                    var isVisible = mainWindow!.Properties.IsOffscreen;
                    if (isVisible)
                    {
                        Console.WriteLine("Main window không hiển thị, khởi động lại...");
                        OneTimeSetup();
                        return;
                    }
                }
                catch
                {
                    Console.WriteLine("Không thể kiểm tra main window, khởi động lại...");
                    OneTimeSetup();
                    return;
                }
                
                Console.WriteLine("✓ Revit đã sẵn sàng, tiếp tục test");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong Setup: {ex.Message}");
                OneTimeSetup(); // Fallback
            }
        }

        private bool IsRevitReady(FlaUI.Core.Application app, UIA3Automation automation)
        {
            try
            {
                // Kiểm tra xem có process nào đang chạy không
                if (app.HasExited)
                {
                    Console.WriteLine("Revit app đã thoát");
                    return false;
                }
                
                // Kiểm tra xem process có còn hoạt động không
                var process = Process.GetProcessById(app.ProcessId);
                if (process.HasExited)
                {
                    Console.WriteLine("Revit process đã thoát");
                    return false;
                }
                
                // Kiểm tra xem có thể tìm thấy desktop elements không
                var desktop = automation.GetDesktop();
                var revitWindows = desktop.FindAllDescendants(cf => 
                    cf.ByControlType(ControlType.Window).And(cf.ByName("Revit")));
                
                if (revitWindows.Length > 0)
                {
                    Console.WriteLine($"Tìm thấy {revitWindows.Length} Revit windows");
                    return true;
                }
                
                // Kiểm tra xem có window nào có tên chứa "Revit" không
                var allWindows = desktop.FindAllDescendants(cf => cf.ByControlType(ControlType.Window));
                foreach (var window in allWindows)
                {
                    try
                    {
                        var name = window.Name;
                        if (!string.IsNullOrEmpty(name) && name.Contains("Revit"))
                        {
                            Console.WriteLine($"Tìm thấy Revit window: '{name}'");
                            return true;
                        }
                    }
                    catch
                    {
                        // Bỏ qua lỗi khi đọc tên window
                    }
                }
                
                Console.WriteLine("Không tìm thấy Revit windows");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra Revit ready: {ex.Message}");
                return false;
            }
        }

        [TearDown]
        public virtual void Teardown()
        {
            Console.WriteLine("=== TEARDOWN: Kiểm tra trạng thái Revit ===");
            
            // KHÔNG đóng Revit sau mỗi test
            // Chỉ kiểm tra xem Revit có còn hoạt động không
            try
            {
                if (revitApp != null && !revitApp.HasExited && mainWindow != null)
                {
                    Console.WriteLine("✓ Revit vẫn hoạt động, giữ nguyên cho test tiếp theo");
                }
                else
                {
                    Console.WriteLine("⚠ Revit đã bị đóng, sẽ khởi động lại trong test tiếp theo");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra trạng thái Revit: {ex.Message}");
            }
        }

        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
            Console.WriteLine("=== ONETIME TEARDOWN: Đóng Revit sau khi tất cả test hoàn thành ===");
            
            // Đóng Revit sau khi tất cả test hoàn thành
            CleanupRevit();
            
            // Đảm bảo tất cả Revit processes đều được đóng
            // ProcessManager.KillAllRevitProcesses();
            
            Console.WriteLine("✓ Đã đóng Revit và cleanup hoàn tất");
        }

        protected virtual void CleanupRevit()
        {
            try
            {
                // Đóng main window trước
                if (mainWindow != null && !mainWindow.Properties.IsOffscreen)
                {
                    try
                    {
                        Console.WriteLine("Đang đóng main window...");
                        // Thử gửi Alt+F4 để đóng window
                        mainWindow.Focus();
                        System.Windows.Forms.SendKeys.SendWait("%{F4}");
                        System.Threading.Thread.Sleep(500); // Giảm từ 2000ms xuống 500ms
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể đóng main window: {ex.Message}");
                    }
                }

                // Đóng application
                if (revitApp != null)
                {
                    try
                    {
                        Console.WriteLine("Đang đóng Revit application...");
                        revitApp.Close();
                        System.Threading.Thread.Sleep(1000); 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể đóng application: {ex.Message}");
                    }
                }

                // Force kill process nếu cần
                if (revitProcess != null && !revitProcess.HasExited)
                {
                    try
                    {
                        Console.WriteLine("Force killing Revit process...");
                        revitProcess.Kill();
                        revitProcess.WaitForExit(2000); // Giảm từ 5000ms xuống 2000ms
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể kill process: {ex.Message}");
                    }
                }

                // Dispose automation
                if (automation != null)
                {
                    try
                    {
                        automation.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể dispose automation: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong cleanup: {ex.Message}");
            }
            finally
            {
                revitApp = null;
                mainWindow = null;
                automation = null;
                revitProcess = null;
            }
        }
    }
}
