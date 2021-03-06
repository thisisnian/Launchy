﻿using ManagedWinapi.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Launchy
{
    public class Entry : IEntry, INotifyPropertyChanged
    {
        public delegate void ExecuteDelegate();

        string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                propertyChanged("Title");                
            }
        }

        Brush _background;
        [XmlIgnore]
        public Brush Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                propertyChanged("Background");
            }
        }

        public event ExecuteDelegate OnExecute;

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        string _command;
        public string Command
        {
            get { return _command; }
            set
            {
                _command = value;
                if (!string.IsNullOrWhiteSpace(Command))
                {
                    try
                    {
                        Icon = System.Drawing.Icon.ExtractAssociatedIcon(Command).ToImageSource();
                    }
                    catch (Exception)
                    {

                    }
                }
                propertyChanged("Command");
            }
        }

        ImageSource _icon;
        [XmlIgnore]
        public ImageSource Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                propertyChanged("Icon");
            }
        }

        public Entry()
        {
            Title = "";
            Command = "";
            Icon = null;
            Background = Brushes.Transparent;

            OnExecute = startProcess;
        }

        private void propertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public Entry(string title, string cmd)
        {
            Title = title;
            Command = cmd;
            Background = Brushes.Transparent;

            OnExecute = startProcess;
        }

        private void startProcess()
        {
            var win = !string.IsNullOrWhiteSpace(Title) ? SystemWindow.DesktopWindow.AllDescendantWindows.FirstOrDefault(x => x.Title.Equals(Title)) : null;
            if (win != null)
            {
                if (win.WindowState == System.Windows.Forms.FormWindowState.Minimized)
                {
                    int SW_RESTORE = 9;
                    ShowWindow(win.HWnd, SW_RESTORE);
                }

                SystemWindow.ForegroundWindow = win;
            }
            else
            {
                Process.Start(Command);
            }
        }

        public void Execute()
        {
            if (OnExecute != null)
                OnExecute();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
