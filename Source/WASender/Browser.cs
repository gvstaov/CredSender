using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaAutoReplyBot;
using WASender.Model;

namespace WASender
{
    public partial class Browser : MyMaterialPopOp
    {
        public event EventHandler browserInitDoneEvnet;
        public event EventHandler browserCloseEvent;
        public event EventHandler browserWAPI_InjectedEvent;
        public event EventHandler browserWAPI_do_Counter_increment;
        //public event EventHandler browser_do_add_new_data_row;
        public event Action<Browser, _DataItem_GoogleMap> on_DataItem_GoogleMap;


        public string url = "";
        public int scollDelay = 4;
        public bool isGmap = false;
        bool isWPPInjected = false;

        List<_DataItem_GoogleMap> _DataItem_GoogleMapList;



        protected virtual void do_add_new_data_row(_DataItem_GoogleMap e)
        {
            if (on_DataItem_GoogleMap != null)
            {
                on_DataItem_GoogleMap.Invoke(this, e);
            }
        }

        protected virtual void OnSearch_Completed(EventArgs e)
        {
            if (browserWAPI_do_Counter_increment != null)
            {
                browserWAPI_do_Counter_increment.Invoke(this, e);
            }
        }

        protected virtual void OnWAPI_Injected(EventArgs e)
        {
            if (browserWAPI_InjectedEvent != null)
            {
                browserWAPI_InjectedEvent.Invoke(this, e);
            }
        }

        protected virtual void OnbroserClose(EventArgs e)
        {
            if (browserCloseEvent != null)
            {
                browserCloseEvent.Invoke(this, e);
            }
        }
        protected virtual void OnbroserInitDoneEvnet(EventArgs e)
        {
            //CloseButtonClicked?.Invoke(this, e);
            if (browserInitDoneEvnet != null)
                browserInitDoneEvnet.Invoke(this, e);
        }

        string[] protocols = {
            "http:",
            "https:",
            "ftp:",  
            "sftp:",
            "scp:",
            "edge:",
            "javascript:",
            "chrome-search:",
            "data:",
            "about:",
            "file:"
        };
        // The URL of the home page.
        const string home = "https://ntp.msn.com/edge/ntp?locale=en&dsp=0&sp=Google";
        // Prefix for searching on Google.
        const string googlePrefix = "https://www.google.com/search?q=";
        // The default home page prompt.
        const string prompt = "Where do you want to go today?";
        // JavaScript for going back and forward in the history.
        const string goBack = "window.history.back();";
        const string goForward = "window.history.forward();";
        // JavaScript for reloading the page.
        const string refreshScript = "location.reload();";
        // A list of all loaded userscripts.
        List<string> userscripts = new List<string>();

        public Browser()
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;

            webView.NavigationStarting += UrlCheck;
            // Add listener for NavigationComplete to load userscripts
            webView.NavigationCompleted += RunUserscripts;
            // And to get the title
            webView.NavigationCompleted += NavigationComplete;
            webView.NavigationStarting += webView_NavigationStarting;
        }


        private void webView_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            string _ur = e.Uri.ToString();
            addressBar.Text = _ur;
        }

        private async void NavigationComplete(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            await webView.ExecuteScriptAsync("window.chrome.webview.postMessage(\"title=\" + window.document.title);");
            if (this.isGmap)
            {
                await injectWPP();
            }
            OnWAPI_Injected(e);
        }

        private async Task<bool> injectWPP()
        {
            if (isWPPInjected == false)
            {
                await Task.Delay(4000);
                string scritp = getWPP();
                await webView.ExecuteScriptAsync(scritp);
                webView.CoreWebView2.WebMessageReceived += ReceiveLoginData;
                isWPPInjected = true;
            }
            _DataItem_GoogleMapList = new List<_DataItem_GoogleMap>();
            await Task.Delay(2000);
            return true;
        }

        private string getWPP()
        {
            try
            {
                return File.ReadAllText(@"scriptnew.js");
            }
            catch (Exception)
            {
                return null;
            }
        }


        private string sanitizeNumber(string MobileNumber)
        {
            if (MobileNumber.StartsWith("0"))
            {
                MobileNumber = MobileNumber.Substring(1);
            }

            MobileNumber = MobileNumber.Replace(@" ", "");
            MobileNumber = MobileNumber.Replace(@"(", "");
            MobileNumber = MobileNumber.Replace(@")", "");
            MobileNumber = MobileNumber.Replace(@"+", "");
            MobileNumber = MobileNumber.Replace(@"-", "");
            return MobileNumber;
        }
        void ReceiveLoginData(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            string loginDataAsJson = args.WebMessageAsJson;

            _DataItem_GoogleMap data = JsonConvert.DeserializeObject<_DataItem_GoogleMap>(loginDataAsJson);
            data.Phone = sanitizeNumber(data.Phone);
            _DataItem_GoogleMapList.Add(data);
            //this.newUI.addtoRow(data);
            var newargs = new EventArgs();


            //browser_do_add_new_(this,browser_do_add_new_data_row);
            do_add_new_data_row(data);
        }


        private async void RunUserscripts(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            foreach (string script in userscripts)
            {
                // Execute it
                await webView.ExecuteScriptAsync(script);
            }
        }

        public void stop()
        {
            isEnd = true;
        }

        private void UrlCheck(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            // Get the URL
            string uri = e.Uri;
            // If it's a secure URL (starts with https or sftp) or a settings page (edge or sv):
            if (uri.StartsWith("https:") || uri.StartsWith("sftp:") ||
                uri.StartsWith("edge:") || addressBar.Text.StartsWith("sv:"))
            {
                // Set the padlock icon to a padlock
                //padlock.Text = "";
                // Color it green
                //padlock.Foreground = Brushes.LightGreen;
                // Set its tooltip to "Secure"
                //padlock.ToolTip = "Secure";
                // Else:
            }
            else
            {
                // Set the padlock icon to a warning shield
                //padlock.Text = "";
                // Color it red
                //padlock.Foreground = Brushes.Red;
                // Set its tooltip to "Not secure"
                //padlock.ToolTip = "Not secure";
            }
        }
        private void Browser_Load(object sender, System.EventArgs e)
        {
            initLanguages();

            InitializeAsync();
        }

        private void initLanguages()
        {
            this.Text = Strings.Browser;
        }

        async void InitializeAsync()
        {
            webView.Visible = false;
            materialLabel1.Visible = true;
            materialLabel1.Text = Strings.Loading;
            webView.CreationProperties = new Microsoft.Web.WebView2.WinForms.CoreWebView2CreationProperties();
            webView.CreationProperties.UserDataFolder = Config.GetProfilesFolderPath() + "\\" + "PrivateBrowserProfile";

            // Initialize the WebView
            await webView.EnsureCoreWebView2Async(null);
            webView.Visible = true;
            materialLabel1.Visible = false;
            // Add listeners to the WebMessageReceived event
            webView.CoreWebView2.WebMessageReceived += UpdateAddressBar;
            webView.CoreWebView2.WebMessageReceived += ReadPayload;
            // Add a script to run when a page is loading
            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
                // This script just posts a message with the window's URL
                "window.chrome.webview.postMessage(\"uri=\" + window.document.URL);"
            );
            // If there is no userscript folder, create one
            //if (!Directory.Exists("userscripts")) Directory.CreateDirectory("userscripts");
            //// For each userscript:
            //foreach (string userscript in Directory.GetFiles("userscripts", "*.js"))
            //{
            //    // Read its content
            //    var usContent = File.ReadAllText(userscript);
            //    // Add it to the userscript list
            //    userscripts.Add(usContent);
            //}

            string nav = url == "" ? "https://www.google.com/search?q=whatsapp+group+links&oq=whatsapp+group+links&aqs=chrome.0.69i59j0i433i512j0i512j0i457i512j0i402j69i60l3.2696j0j7&sourceid=chrome&ie=UTF-8" : url;

            webView.CoreWebView2.Navigate(nav);

            OnbroserInitDoneEvnet(null);
        }

        private void ReadPayload(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            // Get the received message
            //string uri = e.TryGetWebMessageAsString();
            //// If the URI isn't a config page, return
            //if (!uri.Contains(Directory.GetCurrentDirectory().Replace('\\', '/') + "/config/")) return;
            //// If the message isn't a payload, return
            //if (!uri.StartsWith(":svpl:")) return;
            //// Remove header
            //uri = uri.Replace(":svpl:", "");
            //MessageBox.Show(uri);
        }

        bool isEnd = false;
        public async void start(string searchQuery)
        {
            isEnd = false;
            try
            {
                await webView.ExecuteScriptAsync("document.getElementById('searchboxinput').value='" + searchQuery + "'");
                await Task.Delay(4000);
                await webView.ExecuteScriptAsync("document.getElementsByClassName('mL3xi')[0].click()");
                await Task.Delay(4000);
                await webView.ExecuteScriptAsync("document.getElementsByClassName('mL3xi')[0].click()");
            }
            catch (Exception ex)
            {

            }
            perfotmScroller();
        }


        private async void perfotmScroller()
        {
            int scrollTopValue = 0;
            int _oldScrolltppValue = 0;
            int _sameCOunter = 0;
            try
            {
                while (isEnd == false)
                {
                    await webView.ExecuteScriptAsync("document.querySelectorAll('.m6QErb.DxyBCb.kA9KIf')[1].scrollTop = 1000000;");
                    await Task.Delay((scollDelay == 0 ? 4 : scollDelay) * 1000);

                    string _scrollTopValue = (string)await webView.ExecuteScriptAsync("document.querySelectorAll('.m6QErb.DxyBCb.kA9KIf')[1].scrollTop");
                    _oldScrolltppValue = scrollTopValue;
                    scrollTopValue = Convert.ToInt32(_scrollTopValue);
                    if (scrollTopValue == _oldScrolltppValue)
                    {
                        _sameCOunter = _sameCOunter + 1;
                    }
                    if (_sameCOunter > 7)
                    {
                        isEnd = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            OnSearch_Completed(null);
            //this.newUI.doCounerIncremet();

        }

        private void UpdateAddressBar(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                // Get the received message (the URI of the current page)
                string msg = e.TryGetWebMessageAsString();
                // If the message is a payload, return
                if (msg.StartsWith(":svpl:")) return;
                // If it's a URI, get the URI
                if (msg.StartsWith("url="))
                {
                    string uri = msg.Replace("url=", "");
                    // If the URI isn't a config page:
                    if (!uri.Contains(Directory.GetCurrentDirectory().Replace('\\', '/') + "/config/"))
                    {
                        // If the URI isn't the homepage, set the address bar's text to it
                        if (uri != home) addressBar.Text = uri;
                        // Else, set it to the browsing prompt
                        else addressBar.Text = prompt;
                    }
                }
                // If it's a title:
                if (msg.StartsWith("title="))
                {
                    // If there is a title sent:
                    //if (msg != "title=")
                    // Update the window title
                    //Title = msg.Replace("title=", "") + " - SurfView";
                }
            }
            catch (Exception ex)
            {
                string ee= e.Source;
                //if (IsValidURL(ee))
                    addressBar.Text = ee;
            }
        }

        bool IsValidURL(string URL)
        {
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Rgx.IsMatch(URL);
        }

        bool hasProtocol(string url)
        {
            // For each protocol:
            foreach (string p in protocols)
            {
                // If the URL starts with it, return true
                if (url.StartsWith(p)) return true;
            }
            // Return false if the URL doesn't start with any protocol
            return false;
        }
        // Is a given string a URL?
        bool isUrl(string url)
        {
            // Create a regex that matches a URL
            Regex reg = new Regex(@"^(^|\s)((https?:\/\/)?[\w-]+(\.[\w-]+)+\.?(:\d+)?(\/\S*)?)$");
            // Return if the given string matches it (so the string is a URL)
            return reg.IsMatch(url);
        }

        private void addressBar_KeyDown(object sender, KeyEventArgs e)
        {
            // If the user pressed Enter:
            if (e.KeyCode == Keys.Enter)
            {
                // If the WebView is initialized (just for safety):
                if (webView != null && webView.CoreWebView2 != null)
                {
                    addressBar.Text = addressBar.Text.Replace("‭", "");
                    // If the address bar's text has a supported protocol:
                    if (hasProtocol(addressBar.Text))
                    {
                        // Navigate to the text
                        webView.CoreWebView2.Navigate(addressBar.Text);
                        // Else, if the address bar's text is a URL:
                    }
                    else if (isUrl(addressBar.Text))
                    {
                        // Navigate to "http://" + the address bar's text
                        // Secure sites should redirect us to the https version
                        webView.CoreWebView2.Navigate("http://" + addressBar.Text);
                        // Else, if it's a config page (the text contains the sv: protocol)
                    }
                    else if (addressBar.Text.StartsWith("sv:"))
                    {
                        // Navigate to the correct config page
                        webView.CoreWebView2.Navigate(Directory.GetCurrentDirectory() + "/config/" + addressBar.Text.Replace("sv:", "") + ".html");
                        // Else:
                    }
                    else
                    {
                        // Search the address bar's text with Google, replacing the spaces with plus signs and removing the LRO
                        webView.CoreWebView2.Navigate(googlePrefix + addressBar.Text.Replace(' ', '+'));
                    }
                }
            }
        }

        private void back_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.ExecuteScriptAsync(goBack);
        }

        private void next_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.ExecuteScriptAsync(goForward);
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.ExecuteScriptAsync(refreshScript);
        }

        List<string> chatNames;
        public async Task<List<string>> getAllLinks()
        {
            string allHtml = await webView.ExecuteScriptAsync("document.documentElement.outerHTML;");

            //string sHtml = await webView21.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML");
            string sHtmlDecoded = System.Text.RegularExpressions.Regex.Unescape(allHtml);

            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(sHtmlDecoded);

            List<HtmlNode> links = doc.DocumentNode.SelectNodes("//a[@href]").ToList();

            chatNames = new List<string>();
            foreach (HtmlNode link in links)
            {
                string hrefValue = link.GetAttributeValue("href", string.Empty);

                if (hrefValue.StartsWith("https://chat.whatsapp.com/"))
                {
                    chatNames.Add(hrefValue);
                }
            }
            return chatNames;
        }

        private void Browser_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnbroserClose(e);
        }


    }
}
