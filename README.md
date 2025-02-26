# CodeAnalyzerAI VSIX Extension

## 📌 Overview
CodeAnalyzerAI is a Visual Studio Extension (VSIX) that analyzes C# code in the active editor window and provides AI-powered suggestions for improvements. This extension integrates with Visual Studio to enhance the developer experience by offering real-time code analysis and refactoring suggestions.

## 🚀 Features
- **AI-Powered Code Analysis**: Sends the open C# code to DeepSeek for analysis and provides actionable suggestions.
- **Tool Window Support**: Displays analysis results in a dedicated Visual Studio tool window.
- **Custom Commands**: Easily trigger analysis via a toolbar button or context menu.
- **Seamless Integration**: Works within the Visual Studio environment without disrupting workflow.

## 🛠 Installation
### Prerequisites
- **Visual Studio 2022** (Community, Professional, or Enterprise)
- **.NET Framework 4.8+**
- **Visual Studio Extension Development Workload** (Installed via Visual Studio Installer)

### Steps to Install
1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo/CodeAnalyzerAI.git
   ```
2. Open the solution in **Visual Studio**.
3. Set `CodeAnalyzerAI` as the startup project.
4. Press `F5` to run in an **Experimental Instance** of Visual Studio.

## 🏗 Building & Running
1. **Ensure Dependencies are Installed**
   - Install required NuGet packages using:
     ```sh
     dotnet restore
     ```

2. **Build the Project**
   - Select `Release` or `Debug` mode.
   - Click **Build > Build Solution** (`Ctrl + Shift + B`).

3. **Run in Experimental Instance**
   - Start the extension by pressing `F5`.
   - A new Visual Studio window (Experimental Instance) will launch.

## 🛠 Troubleshooting
### Issue: `VSSDK1048: Error trying to read the VSIX manifest file`
**Solution:**
- Ensure the `Visual Studio extension development` workload is installed.
- Delete `bin` and `obj` folders and rebuild the solution.
- Reinstall `Microsoft.VSSDK.BuildTools` package:
  ```sh
  Uninstall-Package Microsoft.VSSDK.BuildTools -Force
  Install-Package Microsoft.VSSDK.BuildTools -Version 17.11.439
  ```

### Issue: `CS0120: An object reference is required for the non-static field, method, or property 'ToolWindowPane.Package'`
**Solution:**
- Ensure `JoinableTaskFactory.SwitchToMainThreadAsync()` is called before accessing `this.Package`.
- Use `await this.JoinableTaskFactory.SwitchToMainThreadAsync();` in the `InitializeAsync` method.

## 📜 License
This project is licensed under the **MIT License**.

## 📩 Contact
For issues or feature requests, please open an issue on **GitHub** or reach out via email: `your-email@example.com`.

---

### 🌟 Enjoy coding with CodeAnalyzerAI! 🚀
