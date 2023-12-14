﻿using ImpactWPF.Controls;
using ImpactWPF.Pages;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImpactWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new Animation());
            WindowState = WindowState.Maximized;
            anianimation.Play();

            // Встановлюємо обробник події завершення анімації
            anianimation.MediaEnded += MediaElement_MediaEnded;

            Logger.Info("Застосунок успішно запустився");
        }
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Обробка події завершення анімації
            anianimation.Visibility = Visibility.Collapsed; 
                                                            

            
            mainFrame.Navigate(new LoginPage());
        }        public void NavigateToPage(Page page)
        {
            mainFrame.Navigate(page);
        }
    }
}
    

