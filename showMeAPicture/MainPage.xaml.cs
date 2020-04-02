using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace showMeAPicture
{
    public sealed partial class MainPage : Page
    {
        const string SubscriptionKey = "";
        const string URIBase = "https://api.cognitive.microsoft.com/bing/v7.0/images/search";
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            //If the user presses Enter, read the search terms and use them to find and image.

            if(e.Key == Windows.System.VirtualKey.Enter && searchTermsTextBox.Text.Trim().Length > 0)
            {
                // Search for an image by using the Bing image search API, supplying the search term entered in the XAML text box.

                string imageURL = FindUrlOfImage(searchTermsTextBox.Text);

                // Display the first image found
                foundObjectImage.Source = new BitmapImage(new Uri(imageURL, UriKind.Absolute));
                
            }

        }

        struct SearchResult
        {
            public String jsonResult;
            public Dictionary<String, String> relevantHeaders;
        }

        private string FindUrlOfImage(string targetString)
        {
            // Call the method that searches
            SearchResult result = PerformBingImageSearch(targetString);

            // Process the JSON respons from the Bing API and get the first image URL
            JsonObject jsonObj = JsonObject.Parse(result.jsonResult);
            JsonArray jsonResults = jsonObj.GetNamedArray("value");
            if (jsonResults.Count > 0)
            {
                JsonObject first_result = jsonResults.GetObjectAt(0);
                String imageURL = first_result.GetNamedString("contentUrl");
                return imageURL;
            }
            else
                return "https://docs.microsoft.com/learn/windows/build-internet-connected-windows10-apps/media/imagenotfound.png";

        }

        static SearchResult PerformBingImageSearch(string searchTerms)
        {
            // Create query to Bing API
            string uriQuery = URIBase + "?q=" + Uri.EscapeDataString(searchTerms);
            WebRequest request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = SubscriptionKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // Create result object
            SearchResult searchResult = new SearchResult()
            {
                jsonResult = json,
                relevantHeaders = new Dictionary<string, string>()
            };

            // Extract the Bing HTTP Headers

            foreach (String header in response.Headers)
            {
                if (header.StartsWith("BingAPIS-") || header.StartsWith("X-MSEdge-"))
                    searchResult.relevantHeaders[header] = response.Headers[header];
            }

            return searchResult;

        }

    }
}
