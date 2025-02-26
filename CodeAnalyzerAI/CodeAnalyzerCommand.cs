using System;
using System.ComponentModel.Design;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeAnalyzerAI;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;

namespace CodeAnalyzerVSIX
{
    internal sealed class CodeAnalyzerCommand
    {
        private readonly AsyncPackage package;
        private static readonly HttpClient httpClient = new HttpClient();

        private CodeAnalyzerCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(Guid.Parse(CodeAnalyzerAIPackage.PackageGuidString), 0x0100);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            new CodeAnalyzerCommand(package, commandService);
        }

        private async void Execute(object sender, EventArgs e)
        {

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            DTE2 dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            Document activeDocument = dte?.ActiveDocument;

            if (activeDocument != null && activeDocument.Language == "CSharp")
            {
                TextDocument textDoc = activeDocument.Object("TextDocument") as TextDocument;
                EditPoint editPoint = textDoc?.StartPoint.CreateEditPoint();
                string code = editPoint?.GetText(textDoc.EndPoint);

                if (!string.IsNullOrEmpty(code))
                {
                    string response = await SendToChatGPT(code);
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        response,
                        "ChatGPT Analysis",
                        OLEMSGICON.OLEMSGICON_INFO,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }
        }

        private async Task<string> SendToChatGPT(string code)
        {
            string apiKey = "sk-0fdfb0dae87f4f06809fcd0dfac0a9ac";
            string endpoint = "https://api.deepseek.com/chat/completions";

            var requestBody = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new { role = "system", content = "You are an AI assistant helping to analyze C# code." },
                    new { role = "user", content = code }
                },
                stream= false
            };

            string json = JsonConvert.SerializeObject(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                string responseText = await response.Content.ReadAsStringAsync();

                // Debug output to check API response
                System.Diagnostics.Debug.WriteLine("ChatGPT Response: " + responseText);

                return responseText;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error sending request: " + ex.Message);
                return "Error fetching suggestions.";
            }
        }
    }
}
