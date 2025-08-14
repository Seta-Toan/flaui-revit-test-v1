# FlaUI Revit Test

Dự án kiểm thử tự động cho Autodesk Revit sử dụng FlaUI framework để thực hiện UI automation testing.

## 📋 Mô tả

Dự án này cung cấp một bộ công cụ kiểm thử tự động cho Revit, cho phép:
- Tự động hóa các thao tác giao diện người dùng trong Revit
- Kiểm thử các dialog và workflow phổ biến
- Tích hợp với hệ thống CI/CD
- Hỗ trợ kiểm thử cross-platform

## 🏗️ Kiến trúc dự án

```
FlaUIRevitTest/
├── Tests/                   # Thư mục chứa các test case
│   └── RevitDialogTests.cs  # Test cases cho Revit dialogs
├── Utilities/               # Các utility và helper classes
│   └── AutomationHelper.cs  # Helper cho UI automation
├── Setup/                   # Cấu hình và setup
├── ClassLibrary1.csproj     # File project C#
├── ClassLibrary1.sln        # Solution file
└── refefactorGuide.md       # Hướng dẫn refactor code
```

## 🚀 Yêu cầu hệ thống

- **.NET Framework 4.8**
- **Autodesk Revit** (phiên bản tương thích)
- **Windows OS** (do FlaUI chỉ hỗ trợ Windows)
- **Visual Studio 2019+** hoặc **VS Code**

## 📦 Dependencies

- **FlaUI.Core** (v5.0.0) - Core framework cho UI automation
- **FlaUI.UIA3** (v5.0.0) - UIA3 provider cho Windows
- **Microsoft.NET.Test.Sdk** (v17.14.1) - Test framework
- **NUnit** (v4.4.0) - Unit testing framework
- **NUnit3TestAdapter** (v5.1.0) - Test adapter cho Visual Studio

## 🛠️ Cài đặt

1. **Clone repository:**
   ```bash
   git clone [repository-url]
   cd FlaUIRevitTest
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Build project:**
   ```bash
   dotnet build
   ```

4. **Chạy tests:**
   ```bash
   dotnet test
   ```

## 📝 Cách sử dụng

### Chạy tests cơ bản

```csharp
// Chạy tất cả tests
dotnet test

// Chạy test cụ thể
dotnet test --filter "RevitDialogTests"

// Chạy với output chi tiết
dotnet test --logger "console;verbosity=detailed"
```

### Tích hợp với Visual Studio

1. Mở `ClassLibrary1.sln` trong Visual Studio
2. Build solution
3. Sử dụng Test Explorer để chạy tests

## 🔧 Cấu hình

### Timeout settings

Dự án sử dụng các timeout mặc định cho UI automation:
- **Default timeout**: 30 giây
- **Short timeout**: 10 giây
- **Long timeout**: 60 giây

### Revit settings

Đảm bảo Revit được cài đặt và có thể khởi chạy từ command line.

## 📊 Test Structure

### RevitDialogTests.cs

Chứa các test cases chính:
- Kiểm thử các dialog phổ biến trong Revit
- Xử lý trial dialog
- Kiểm thử web view dialogs
- Automation cho các workflow cơ bản

### AutomationHelper.cs

Cung cấp các utility methods:
- Tìm kiếm UI elements
- Xử lý common dialogs
- Helper functions cho automation

## 🚨 Lưu ý quan trọng

### Vấn đề đã biết

1. **Thread.Sleep usage**: Code hiện tại sử dụng nhiều `Thread.Sleep` có thể gây ra:
   - Tests chạy chậm
   - Timeout không nhất quán
   - Failures trên máy tính có hiệu suất khác nhau

2. **Code duplication**: Logic tìm kiếm UI elements bị lặp lại nhiều nơi

3. **Complex dialog handling**: Các phương thức xử lý dialog quá phức tạp

### Giải pháp đề xuất

Xem file `refefactorGuide.md` để biết chi tiết về cách cải thiện code.

## 🤝 Đóng góp

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📚 Tài liệu tham khảo

- [FlaUI Documentation](https://github.com/FlaUI/FlaUI)
- [NUnit Documentation](https://docs.nunit.org/)
- [Revit API Documentation](https://help.autodesk.com/view/RVT/2024/ENU/Revit-API-Developer-Guide/)

## 📞 Hỗ trợ

Nếu gặp vấn đề hoặc có câu hỏi, vui lòng:
1. Kiểm tra [Issues](../../issues) hiện có
2. Tạo issue mới với mô tả chi tiết
3. Liên hệ team development

---

**Lưu ý**: Dự án này đang trong giai đoạn phát triển và có thể có breaking changes. Vui lòng kiểm tra changelog trước khi update.
