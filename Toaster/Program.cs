using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Windows.Foundation.Collections;

namespace Toaster
{
    static class Properties
    {
        public const string
            PROGRAMPATH = "--programpath",
            PARAMETER = "--parameter",
            ADAPTIVE_TEXT = "--text",
            APP_LOGO = "--logo",
            HERO_IMAGE = "--heroimage",
            INLINE_IMAGE = "--inlineimage",
            ATTRIBUTION_TEXT = "--attribution",
            TIMESTAMP = "--timestamp",
            BUTTON = "--button",
            INPUT_BOX = "--inputbox",
            SELECTION_BOX = "--selectionbox";
    }

    public class Program
    {
        internal string str = "";
        public static void Main(string[] args)
        {
            // Check if app was activated from toast
            bool m = ToastNotificationManagerCompat.WasCurrentProcessToastActivated();
            if (!m)
            {
                // Parse arguments and create toast
                ParseArguments(args);
            }
            else
            {
                bool done = false;
                Process rainmeter = new Process();

                // Listen to notification activation
                ToastNotificationManagerCompat.OnActivated += toastArgs =>
                {
                    // Obtain the arguments from the notification
                    ToastArguments ar = ToastArguments.Parse(toastArgs.Argument);

                    // Obtain any user input (text boxes, menu selections) from the notification
                    ValueSet userInput = toastArgs.UserInput;

                    string[] programArgs = ar["arguments"].Split(new string[] { "|" }, 2, StringSplitOptions.RemoveEmptyEntries);

                    rainmeter.StartInfo.FileName = programArgs[0];

                    foreach (KeyValuePair<string, object> pair in userInput)
                    {
                        string arg = pair.Value.ToString();
                        programArgs[1] = programArgs[1].Replace("$" + pair.Key + "$", arg.Replace("\r", "\n"));
                        programArgs[1] = programArgs[1].Replace("$" + pair.Key + ":CRLF$", arg.Replace("\r", "#CRLF#"));
                    }

                    rainmeter.StartInfo.Arguments = programArgs[1];
                    try
                    {
                        rainmeter.Start();
                        done = true;
                    }
                    catch (SystemException exec)
                    {
                        MessageBox.Show("Couldn't start program. Error: " + exec);
                        done = true;
                    }
                };

                // Keep the app running until OnActivated event is raised and is completed
                while (!done)
                    Thread.Sleep(100);
            }
        }

        static void ParseArguments(string[] args)
        {
            int text = 0;
            bool attribution = false;
            bool applogo = false;
            bool heroimage = false;
            bool inlineimg = false;
            int buttons = 0;
            int input = 0;
            string programpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Rainmeter\Rainmeter.exe";
            string parameter = "[!Log \"Toasted from Rainmeter!\"]";

            List<Hashtable> toastContent = new List<Hashtable>();

            int i = 0;
            if (args.Length >= 2)
            {
                while (i < args.Length - 1)
                {
                    Hashtable toastElement = new Hashtable();
                    if (args[i + 1].StartsWith("--"))
                    {
                        i++;
                        continue;
                    }
                    else if (args[i].ToLower().Equals(Properties.ADAPTIVE_TEXT))
                    {
                        if (text < 3)
                        {
                            toastElement.Add("Type", "Text");
                            toastElement.Add("Args", args[++i]);
                            toastContent.Add(toastElement);
                            text++;
                        }
                    }
                    else if (args[i].ToLower().Equals(Properties.ATTRIBUTION_TEXT))
                    {
                        if (!attribution)
                        {
                            toastElement.Add("Type", "Attribution");
                            toastElement.Add("Args", args[++i]);
                            toastContent.Add(toastElement);
                            attribution = true;
                        }
                    }
                    else if (args[i].ToLower().Equals(Properties.APP_LOGO))
                    {
                        if (!applogo)
                        {
                            toastElement.Add("Type", "AppLogo");
                            toastElement.Add("Args", args[++i]);
                            toastContent.Add(toastElement);
                            applogo = true;
                        }
                    }
                    else if (args[i].ToLower().Equals(Properties.HERO_IMAGE))
                    {
                        if (!heroimage)
                        {
                            toastElement.Add("Type", "HeroImage");
                            toastElement.Add("Args", args[++i]);
                            toastContent.Add(toastElement);
                            heroimage = true;
                        }
                    }
                    else if (args[i].ToLower().Equals(Properties.INLINE_IMAGE))
                    {
                        if (!inlineimg)
                        {
                            toastElement.Add("Type", "InlineImage");
                            toastElement.Add("Args", args[++i]);
                            toastContent.Add(toastElement);
                            inlineimg = true;
                        }
                    }
                    else if (args[i].ToLower().Equals(Properties.BUTTON))
                    {
                        if (buttons < 5)
                        {
                            toastElement.Add("Type", "Button");
                            List<string> buttonArgs = new List<string>();
                            while (i < args.Length - 1)
                            {
                                if (!args[i + 1].StartsWith("--"))
                                    buttonArgs.Add(args[++i]);
                                else
                                    break;
                            }
                            toastElement.Add("Args", buttonArgs.ToArray());
                            toastContent.Add(toastElement);
                            buttons++;
                        }
                    }
                    else if (args[i].ToLower().Equals(Properties.INPUT_BOX))
                    {
                        if (input < 5)
                        {
                            toastElement.Add("Type", "InputBox");
                            List<string> inputArgs = new List<string>();
                            while (i < args.Length - 1)
                            {
                                if (!args[i + 1].StartsWith("--"))
                                    inputArgs.Add(args[++i]);
                                else
                                    break;
                            }
                            toastElement.Add("Args", inputArgs.ToArray());
                            toastContent.Add(toastElement);
                            input++;
                        }
                    }
                    else if (args[i].ToLower().Equals(Properties.SELECTION_BOX))
                    {
                        if (input < 5)
                        {
                            toastElement.Add("Type", "SelectionBox");
                            if (i < args.Length - 2 && !args[i + 1].StartsWith("--"))
                            {
                                toastElement.Add("Id", args[++i]);
                            }
                            List<string[]> selectionItems = new List<string[]>();
                            string defaultEntry = null;
                            while (i < args.Length - 1)
                            {
                                if (args[i + 1].StartsWith("--")) break;
                                string[] slArgs = args[++i].Split(new string[] { "|" }, 2, StringSplitOptions.RemoveEmptyEntries);
                                if (slArgs.Length < 2)
                                    continue;
                                if (slArgs[0].StartsWith("&default;"))
                                {
                                    slArgs[0] = slArgs[0].Remove(0, 9);
                                    if (string.IsNullOrEmpty(defaultEntry))
                                        defaultEntry = slArgs[0];
                                }
                                selectionItems.Add(slArgs);
                            }
                            if (selectionItems.Count > 5) selectionItems.RemoveRange(6, selectionItems.Count - 5);
                            if (selectionItems.Count != 0)
                            {
                                Hashtable selectionHash = new Hashtable();
                                if (!string.IsNullOrEmpty(defaultEntry))
                                    selectionHash.Add("Default", defaultEntry);
                                selectionHash.Add("List", selectionItems);
                                toastElement.Add("Args", selectionHash);
                            }
                            toastContent.Add(toastElement);
                            input++;
                        }
                    }
                    else if (args[i].ToLower().Equals(Properties.PROGRAMPATH))
                    {
                        programpath = args[++i];
                    }
                    else if (args[i].Equals(Properties.PARAMETER))
                    {
                        parameter = args[++i];
                    }
                    i++;
                }
                ToastIt(programpath, parameter, toastContent);
            }
        }

        static void ToastIt(string programpath, string parameter, List<Hashtable> toastContent)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;

            ToastContentBuilder toast = new ToastContentBuilder();
            toast.AddArgument("arguments", programpath + "|" + parameter);

            List<string> inputid = new List<string>();

            bool terminated = false;

            foreach (Hashtable ht in toastContent)
            {
                if (ht["Type"].Equals("Text"))
                {
                    toast.AddText((string)ht["Args"]);
                }
                else if (ht["Type"].Equals("Attribution"))
                {
                    toast.AddAttributionText((string)ht["Args"]);
                }
                else if (ht["Type"].Equals("AppLogo"))
                {
                    string imgPath = appPath + (string)ht["Args"];
                    if (File.Exists(imgPath))
                        toast.AddAppLogoOverride(new Uri("file://" + imgPath, UriKind.Absolute), ToastGenericAppLogoCrop.Circle);
                    else
                    {
                        MessageBox.Show("Image path invalid. App Logo: \"" + imgPath + "\"");
                        terminated = true;
                        break;
                    }
                }
                else if (ht["Type"].Equals("HeroImage"))
                {
                    string imgPath = appPath + (string)ht["Args"];
                    if (File.Exists(imgPath))
                        toast.AddHeroImage(new Uri("file://" + imgPath, UriKind.Absolute));
                    else
                    {
                        MessageBox.Show("Image path invalid. Hero Image: \"" + imgPath + "\"");
                        terminated = true;
                        break;
                    }
                }
                else if (ht["Type"].Equals("InlineImage"))
                {
                    string imgPath = appPath + (string)ht["Args"];
                    if (File.Exists(imgPath))
                        toast.AddInlineImage(new Uri("file://" + imgPath, UriKind.Absolute));
                    else
                    {
                        MessageBox.Show("Image path invalid. Inline Image: \"" + imgPath + "\"");
                        terminated = true;
                        break;
                    }
                }
                else if (ht["Type"].Equals("InputBox"))
                {
                    string[] args = (string[])ht["Args"];
                    if (inputid.Contains(args[0]))
                    {
                        MessageBox.Show("Input id of elements can not be same.\nPlease change the Input Box id: " + args[0]);
                        terminated = true;
                        break;
                    }
                    inputid.Add(args[0]);
                    if (args.Length >= 2)
                        toast.AddInputTextBox(args[0], args[1]);
                    else
                        toast.AddInputTextBox(args[0], "");
                }
                else if (ht["Type"].Equals("SelectionBox"))
                {
                    if (inputid.Contains((string)ht["Id"]))
                    {
                        MessageBox.Show("Input id of elements can not be same.\nPlease change the Selection Box Id: " + (string)ht["Id"]);
                        terminated = true;
                        break;
                    }
                    inputid.Add((string)ht["Id"]);
                    Hashtable selectionHash = (Hashtable)ht["Args"];
                    ToastSelectionBox selBox = new ToastSelectionBox((string)ht["Id"]);
                    foreach (string[] args in (List<string[]>)selectionHash["List"])
                    {
                        ToastSelectionBoxItem selectionBoxItem = new ToastSelectionBoxItem(args[0], args[1]);
                        selBox.Items.Add(selectionBoxItem);
                    }
                    if (selectionHash.ContainsKey("Default"))
                        selBox.DefaultSelectionBoxItemId = (string)selectionHash["Default"];
                    toast.AddToastInput(selBox);
                }
                else if (ht["Type"].Equals("Button"))
                {
                    ToastButton toastButton = new ToastButton();
                    string[] buttonArgs = (string[])ht["Args"];
                    toastButton.SetContent(buttonArgs[0]);
                    if (buttonArgs.Length >= 2)
                    {
                        toastButton.AddArgument("arguments", programpath + "|" + buttonArgs[1]);
                    }
                    else
                    {
                        toastButton.AddArgument("arguments", programpath + "|" + "[!Log \"Toasted from Raintoast!\"]");
                    }
                    toast.AddButton(toastButton);
                }
            }
            if (!terminated)
                toast.Show();
        }
    }
}