using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ClipFixUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Clipboard.ContentChanged += Clipboard_ContentChanged;
        }

        private async void displayDialogAndAskStartUp()
        {
            var dialog = new ContentDialog
            {
                Title = "ClipFixUI3",
                Content = "Do you want to enable ClipFixUI3 to start automatically when Windows starts?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"

            };

            dialog.XamlRoot = this.Content.XamlRoot;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var autoStartUp = new AutoStartUp();
                bool success = await autoStartUp.EnableStartup();
                if (success)
                {
                    StartUpBox.IsChecked = true;
                }
                else
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Failed to enable automatic startup. Please check your system settings.",
                        CloseButtonText = "OK"
                    };
                    await errorDialog.ShowAsync();
                }
            }
            else
            {
                // 用户选择了不启用自动启动
            }
            
        }

        private void CheckStartUpStatus()
        {
            var autoStartUp = new AutoStartUp();
            autoStartUp.IsEnabled().ContinueWith(task =>
            {
                var isEnabled = task.Result;
                DispatcherQueue.TryEnqueue(() =>
                {
                    StartUpBox.IsChecked = isEnabled;
                });
            });
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!(localSettings.Values.ContainsKey("HasLaunchedBefore")))
            {
                // 设置标志位，表示已经启动过了
                localSettings.Values["HasLaunchedBefore"] = true;

                displayDialogAndAskStartUp();
            }
            else
            {
                // 检查自动启动状态
                CheckStartUpStatus();
            }
        }

        private void StartUpBox_Click(object sender, RoutedEventArgs e)
        {
            if (StartUpBox.IsChecked == true)
            {
                var autoStartUp = new AutoStartUp();
                autoStartUp.EnableStartup().ContinueWith(task =>
                {
                    if (!task.Result)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            StartUpBox.IsChecked = false;
                            var errorDialog = new ContentDialog
                            {
                                Title = "Error",
                                Content = "Failed to enable automatic startup. Please check your system settings.",
                                CloseButtonText = "OK"
                            };
                            errorDialog.XamlRoot = this.Content.XamlRoot;
                            errorDialog.ShowAsync();
                        });
                    }
                });
            }
            else
            {
                var autoStartUp = new AutoStartUp();
                autoStartUp.DisableStartup();
            }
        }
        private async void Clipboard_ContentChanged(object sender, object e)
        {
            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                string text = await dataPackageView.GetTextAsync();
                if (!string.IsNullOrEmpty(text))
                {
                    var clip = new Clip();
                    string safeLink = await clip.TurnLinkIntoSafeLink(text);
                    if (safeLink != text)
                    {
                        clip.CopyToClipboard(safeLink);
                    }
                }

            }
        }
    }
}
