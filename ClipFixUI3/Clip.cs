using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace ClipFixUI3
{
    class Clip
    {
        DataPackage dataPackage;
        public Clip()
        {

        }

        public void CopyToClipboard(String content)
        {
            dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(content);
            Clipboard.SetContent(dataPackage);
        }

        public async Task<String> TurnLinkIntoSafeLink(String link)
        {
            if (link.StartsWith("https://") || link.StartsWith("http://"))
            {
                if (link.StartsWith("https://x.com") || link.StartsWith("http://x.com") ||
                    link.StartsWith("https://www.x.com") || link.StartsWith("http://www.x.com"))
                {
                    link = XLink(link);
                }
                else if (link.StartsWith("http://b23.tv") || link.StartsWith("https://b23.tv"))
                {
                    link = await b23Link(link);
                }
                else if (link.StartsWith("https://www.bilibili.com") || link.StartsWith("http://www.bilibili.com") ||
                         link.StartsWith("https://bilibili.com") || link.StartsWith("http://bilibili.com"))
                {
                    link = BiliLink(link);
                }
                

            }
            return link;
        }

        private String XLink(String link) {
            return link.Replace("x.com", "fxtwitter.com");
        }

        private async Task<String> b23Link(String link)
        {
            var uri = await GetRedirectedUrlAsync(link);
            return GetCleanUrl(uri);
        }
        private String BiliLink(String link)
        {
            return GetCleanUrl(new Uri(link));
        }

        private static async Task<Uri> GetRedirectedUrlAsync(string url)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false // 禁止自动跳转
            };

            using (var client = new HttpClient(handler))
            {
                try
                {
                    var response = await client.GetAsync(url);

                    if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
                    {
                        if (response.Headers.Location != null)
                        {
                            Uri redirectUri = response.Headers.Location;
                            // 如果是相对路径，合成完整路径
                            if (!redirectUri.IsAbsoluteUri)
                            {
                                redirectUri = new Uri(new Uri(url), redirectUri);
                            }
                            return redirectUri;
                        }
                    }

                    // 没有重定向
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }
            }
        }
        public static string GetCleanUrl(Uri uri)
        {
            try
            {
                string cleanUrl = uri.GetLeftPart(UriPartial.Path);
                return cleanUrl;
            }
            catch (Exception ex)
            {
                // 可以返回 null 或提示错误
                return null;
            }
        }
    }
}
