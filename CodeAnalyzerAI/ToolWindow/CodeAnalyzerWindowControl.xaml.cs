using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CodeAnalyzerAI.ToolWindow
{
    /// <summary>
    /// Interaction logic for CodeAnalyzerWindowControl.
    /// </summary>
    public partial class CodeAnalyzerWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeAnalyzerWindowControl"/> class.
        /// </summary>
        public CodeAnalyzerWindowControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "CodeAnalyzerWindow");
        }
        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dte = (EnvDTE.DTE)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
                var activeDocument = dte.ActiveDocument;
                if (activeDocument != null)
                {
                    TextDocument textDocument = (TextDocument)activeDocument.Object("TextDocument");
                    EditPoint editPoint = textDocument.StartPoint.CreateEditPoint();
                    string codeText = editPoint.GetText(textDocument.EndPoint);

                    OutputTextBox.Text = "Sending code to ChatGPT...";

                    string response = await SendToChatGPT(codeText);

                    OutputTextBox.Text = response;
                }
                else
                {
                    OutputTextBox.Text = "No active document found.";
                }
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"Error: {ex.Message}";
            }
        }
        private async Task<string> SendToChatGPT(string code)
        {


            string apiKey = "";
            string endpoint = "https://api.deepseek.com/chat/completions";

           

            using (HttpClient client = new HttpClient())
            {
              

                var requestBody = new
                {
                    model = "deepseek-chat",
                    messages = new[]
                 {
                    new { role = "system", content = "You are an AI assistant helping to analyze C# code." },
                    new { role = "user", content = code }
                },
                    stream = false
                };

                string json = JsonConvert.SerializeObject(requestBody);
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Authorization", $"Bearer {apiKey}");

                try
                {
                    HttpResponseMessage response = await client.SendAsync(request);
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
}