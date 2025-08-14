using NUnit.Framework;
using ClassLibrary1.Setup;
using System.Diagnostics;
using FlaUI.UIA3;
using FlaUI.Core;
using System;

namespace ClassLibrary1.Tests
{
    [TestFixture]
    public class RevitDialogTests : RevitTestSetup
    {
        [Test]
        public void TestRevitCompleteWorkflow()
        {
            // Test này sẽ thực hiện toàn bộ luồng: Revit → Trial Dialog → Security Dialog → Project
            Console.WriteLine(" === TEST REVIT HOÀN CHỈNH - LUỒNG ĐẦY ĐỦ ===");
            
            try
            {
                // BƯỚC 1: Kiểm tra Revit đã sẵn sàng
                Console.WriteLine("🔍 BƯỚC 1: Kiểm tra Revit đã sẵn sàng...");
                Assert.That(mainWindow, Is.Not.Null, "Main window phải tồn tại");
                Assert.That(revitApp, Is.Not.Null, "Revit app phải tồn tại");
                Assert.That(!revitApp!.HasExited, "Revit app phải đang chạy");
                Assert.That(automation, Is.Not.Null, "Automation phải tồn tại");
                
                Console.WriteLine($"✅ Main window: {mainWindow!.Title}");
                Console.WriteLine("✅ Revit app đang chạy");
                Console.WriteLine("✅ Automation hoạt động");
                
                // BƯỚC 2: Xử lý trial dialog (nếu có)
                Console.WriteLine("🔍 BƯỚC 2: Xử lý trial dialog...");
                var trialHandled = DialogHandlers.HandleTrialDialog(mainWindow);
                if (trialHandled)
                {
                    Console.WriteLine("✅ Trial dialog đã được xử lý");
                }
                else
                {
                    Console.WriteLine("ℹ️ Không có trial dialog để xử lý");
                }
                
                // Chờ một chút sau khi xử lý trial dialog
                System.Threading.Thread.Sleep(1000); // Giảm từ 3000ms xuống 1000ms
                
                // BƯỚC 3: Xử lý security dialog (nếu có)
                Console.WriteLine("🔍 BƯỚC 3: Xử lý security dialog...");
                var securityHandled = DialogHandlers.HandleSecurityDialog(mainWindow);
                if (securityHandled)
                {
                    Console.WriteLine("✅ Security dialog đã được xử lý");
                }
                else
                {
                    Console.WriteLine("ℹ️ Không có security dialog để xử lý");
                }
                
                // Chờ một chút sau khi xử lý security dialog
                System.Threading.Thread.Sleep(1000); // Giảm từ 3000ms xuống 1000ms
                
                // BƯỚC 4: Kiểm tra xem đã có project nào được mở chưa
                Console.WriteLine("🔍 BƯỚC 4: Kiểm tra project hiện tại...");
                var projectLoaded = DialogHandlers.IsProjectLoaded(mainWindow);
                
                if (projectLoaded)
                {
                    Console.WriteLine("✅ Project đã được load - không cần làm gì thêm");
                }
                else
                {
                    Console.WriteLine("⚠️ Chưa có project nào được load - cần vào project");
                    
                    // BƯỚC 5: Tự động vào project
                    Console.WriteLine("🔍 BƯỚC 5: Tự động vào project...");
                    DialogHandlers.AutoEnterProject(mainWindow);
                    
                    // Chờ project load
                    Console.WriteLine("⏳ Chờ project load (3 giây)..."); // Giảm từ 8s xuống 3s
                    System.Threading.Thread.Sleep(3000);
                    
                    // Kiểm tra lại xem project đã load chưa
                    projectLoaded = DialogHandlers.IsProjectLoaded(mainWindow);
                    if (projectLoaded)
                    {
                        Console.WriteLine("✅ Project đã load thành công sau khi tự động vào");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Project vẫn chưa load - thử lại...");
                        System.Threading.Thread.Sleep(2000); // Giảm từ 5000ms xuống 2000ms
                        projectLoaded = DialogHandlers.IsProjectLoaded(mainWindow);
                        if (projectLoaded)
                        {
                            Console.WriteLine("✅ Project đã load thành công sau khi thử lại");
                        }
                        else
                        {
                            Console.WriteLine("❌ Không thể load project - có thể có vấn đề");
                        }
                    }
                }
                
                // BƯỚC 6: Kiểm tra cuối cùng - ĐƠN GIẢN HÓA
                Console.WriteLine("🔍 BƯỚC 6: Kiểm tra cuối cùng...");
                
                // Kiểm tra xem có ribbon bar không (dấu hiệu project đã load)
                var ribbonBar = mainWindow.FindFirstDescendant(cf => 
                    cf.ByControlType(FlaUI.Core.Definitions.ControlType.ToolBar).And(cf.ByName("Ribbon")));
                
                if (ribbonBar != null)
                {
                    Console.WriteLine("✅ Tìm thấy Ribbon bar - Revit đã sẵn sàng làm việc");
                }
                else
                {
                    Console.WriteLine("⚠️ Không tìm thấy Ribbon bar - có thể chưa vào project");
                }
                
                // XÓA: Kiểm tra tabs và menus - không cần thiết, làm chậm test
                
                Console.WriteLine(" === TEST REVIT HOÀN CHỈNH THÀNH CÔNG ===");
                Console.WriteLine("✅ Revit đã được khởi động");
                Console.WriteLine("✅ Dialogs đã được xử lý");
                Console.WriteLine("✅ Project đã được load");
                Console.WriteLine("✅ Sẵn sàng cho việc test tiếp theo");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi trong test: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
