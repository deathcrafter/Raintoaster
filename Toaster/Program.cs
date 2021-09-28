using System;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;

namespace Toaster
{
    static class Properties
    {
        public const string
            PROGRAMPATH = "--programpath",
            TITLE_TEXT = "--title",
            ADAPTIVE_TEXT1 = "--adaptivetext1",
            ADAPTIVE_TEXT2 = "--adaptivetext2",
            APP_LOGO = "--logo",
            HERO_IMAGE = "--heroimage",
            INLINE_IMAGE = "--inlineimage",
            ATTRIBUTION_TEXT = "--attribution",
            TIMESTAMP = "--timestamp",
            BUTTON1 = "--button1",
            BUTTON2 = "--button2",
            BUTTON3 = "--button3",
            BUTTON4 = "--button4",
            BUTTON5 = "--button5",
            INPUT_BOX = "--inputbox";
    }
    
    public class Program
    { 
        public static void Main(string[] args)
        {
            bool m = ToastNotificationManagerCompat.WasCurrentProcessToastActivated();
            if (!m)
            {
                ArgumentParser(args);
            }
            else
            {
                MessageBox.Show("Activated");
                // Listen to notification activation
                ToastNotificationManagerCompat.OnActivated += toastArgs =>
                {
                    // Obtain the arguments from the notification
                    ToastArguments tstargs = ToastArguments.Parse(toastArgs.Argument);

                    // Obtain any user input (text boxes, menu selections) from the notification
                    ValueSet userInput = toastArgs.UserInput;

                    Process pws = new Process();
                    pws.StartInfo.FileName = "powershell.exe";
                    pws.Start();
                };
            }
        }

        public static void toastArgs(IActivatedEventArgs e)
        {
            if (e is ToastNotificationActivatedEventArgs)
            {
                ArgumentParser(new string[] { 
                    "--title",
                    "Activated from toast!"
                });
            }
        }

        static void ArgumentParser(string[] args)
        {
            int i = 0;
            Hashtable propHash = new Hashtable();
            if (args.Length >= 1)
            {
                while (i < args.Length)
                {
                    if (args[i].ToLower() == Properties.TITLE_TEXT)
                    {
                        propHash.Add("Title", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.ADAPTIVE_TEXT1)
                    {
                        propHash.Add("Text1", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.ADAPTIVE_TEXT2)
                    {
                        propHash.Add("Text2", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.APP_LOGO)
                    {
                        propHash.Add("AppLogo", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.HERO_IMAGE)
                    {
                        propHash.Add("HeroImage", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.INLINE_IMAGE)
                    {
                        propHash.Add("InlineImage", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.ATTRIBUTION_TEXT)
                    {
                        propHash.Add("Attribution", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.TIMESTAMP)
                    {
                        propHash.Add("Timestamp", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.BUTTON1)
                    {
                        propHash.Add("Button1", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.BUTTON2)
                    {
                        propHash.Add("Button2", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.BUTTON3)
                    {
                        propHash.Add("Button3", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.BUTTON4)
                    {
                        propHash.Add("Button4", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.BUTTON5)
                    {
                        propHash.Add("Button5", args[++i]);
                    }
                    else if (args[i].ToLower() == Properties.INPUT_BOX)
                    {
                        propHash.Add("InputBox", args[++i]);
                    }
                    i++;
                }
                if (propHash["Title"] == null)
                {
                    propHash["Title"] = "RainToast";
                }
                Toast(propHash);
            }
        }

        static void Toast(Hashtable propertyHash)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            var toastContent = new ToastContentBuilder();
            toastContent.AddText((string)propertyHash["Title"]);

            if (propertyHash.ContainsKey("Text1"))
            {
                toastContent.AddText((string)propertyHash["Text1"]);
            }
            if (propertyHash.ContainsKey("Text2"))
            {
                toastContent.AddText((string)propertyHash["Text2"]);
            }
            if (propertyHash.ContainsKey("AppLogo"))
            {
                toastContent.AddAppLogoOverride(new Uri("file://"+appPath+(string)propertyHash["AppLogo"], UriKind.Absolute), ToastGenericAppLogoCrop.Circle);
            }
            if (propertyHash.ContainsKey("HeroImage"))
            {
                toastContent.AddHeroImage(new Uri("file://"+appPath+(string)propertyHash["HeroImage"], UriKind.Absolute));
            }
            if (propertyHash.ContainsKey("InlineImage"))
            {
                toastContent.AddInlineImage(new Uri("file://"+appPath+(string)propertyHash["InlineImage"], UriKind.Absolute));
            }
            if (propertyHash.ContainsKey("Attribution"))
            {
                toastContent.AddAttributionText((string)propertyHash["Attribution"]);
            }
            if (propertyHash.ContainsKey("Timestamp"))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                string[] dateFormats =
                {
                    "dd-MM-yyyy",
                    "H:mm:ss",
                    "H:mm",
                    "dd-MM-yy",
                    "dd/MM/yy",
                    "dd-MM-yy H:mm:ss",
                    "dd/MM/yy H:mm:ss",
                    "dd-MM-yyyy H:mm:ss",
                    "dd/MM/yyyy H:mm:ss",
                    "dd-MM-yy H:mm",
                    "dd/MM/yy H:mm",
                    "dd-MM-yyyy H:mm",
                    "dd/MM/yyyy H:mm"
                };
                DateTime dateTime = DateTime.ParseExact((string)propertyHash["Timestamp"], dateFormats, provider, DateTimeStyles.None);
                toastContent.AddCustomTimeStamp(dateTime);
            }
            if (propertyHash.ContainsKey("InputBox"))
            {
                toastContent.AddInputTextBox("toastInput", (string)propertyHash["InputBox"]);
            }
            if (propertyHash.ContainsKey("Button1"))
            {
                string[] button1 = propertyHash["Button1"].ToString().Split(new string[] {"|"},3,StringSplitOptions.RemoveEmptyEntries);
                ToastButton button1b = new ToastButton().SetContent(button1[0]);
                if (button1.Length == 2)
                {
                    button1b.AddArgument("rainpath", button1[1]);
                }
                if (button1.Length == 3)
                {
                    button1b.AddArgument("bangs", button1[2]);
                }

                button1b.ActivationType = ToastActivationType.Background;
                toastContent.AddButton(button1b);
            }
            toastContent.Show();
        }
    }
}
