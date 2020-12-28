using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Automation;

namespace PlayItShowIt
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Prop
        [DllImport("user32")]
        public static extern IntPtr GetDesktopWindow();

        string _browserText;
        public string BrowserText
        {
            get => _browserText;

            set
            {
                if (_browserText != value)
                {
                    _browserText = value;
                    NotifyPropertyChanged(nameof(BrowserText));
                }
            }
        }
        #endregion

        #region Ctr
        public MainWindowViewModel()
        {
            Init();
        }
        #endregion

        #region Methods
        private void Init()
        {
            AutomationElement main = AutomationElement.FromHandle(GetDesktopWindow());
            foreach (AutomationElement child in main.FindAll(TreeScope.Children, PropertyCondition.TrueCondition))
            {
                AutomationElement window = GetEdgeCommandsWindow(child);
                if (window == null) // not edge
                    continue;

                var tempTxt = GetEdgeTitle(child);
                BrowserText += BrowserText + "\n" + tempTxt;
                //Console.WriteLine("title:" + GetEdgeTitle(child));
                //Console.WriteLine("url:" + GetEdgeUrl(window));
                //Console.WriteLine();
            }
        }

        public static AutomationElement GetEdgeCommandsWindow(AutomationElement edgeWindow)
        {
            var result = edgeWindow.FindFirst(TreeScope.Children, new AndCondition(
                            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                            new PropertyCondition(AutomationElement.NameProperty, "Microsoft Edge")));
            return result;
        }

        public static string GetEdgeUrl(AutomationElement edgeCommandsWindow)
        {
            var adressEditBox = edgeCommandsWindow.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "addressEditBox"));

            return ((TextPattern)adressEditBox.GetCurrentPattern(TextPattern.Pattern)).DocumentRange.GetText(int.MaxValue);
        }

        public static string GetEdgeTitle(AutomationElement edgeWindow)
        {
            var adressEditBox = edgeWindow.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "TitleBar"));

            return adressEditBox.Current.Name;
        }

        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
