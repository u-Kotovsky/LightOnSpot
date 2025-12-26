using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace LightOnSpotApp.Services
{
    public class MainFrameService
    {
        private static MainFrameService instance;
        public static MainFrameService Instance 
        {
            get
            {
                return instance ??= new MainFrameService(); 
            }
        }

        private Frame mainFrame;
        public Frame MainFrame { get { return mainFrame; } set { mainFrame = value; } }
    }
}
