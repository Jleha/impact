﻿using EfCore.entity;
using ImpactWPF.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImpactWPF.Controls
{
    /// <summary>
    /// Interaction logic for ArchiveCardControl1.xaml
    /// </summary>
    public partial class ArchiveCardControl1 : UserControl
    {
        public ArchiveCardControl1()
        {
            InitializeComponent();
            deactivateImage.MouseLeftButtonDown += DeactivateImage_MouseLeftButtonDown;
            editImage.MouseLeftButtonDown += EditImage_MouseLeftButtonDown;
        }

        public static readonly DependencyProperty ArchiveRequestProperty =
        DependencyProperty.Register("ArchiveRequest", typeof(Request), typeof(ArchiveCardControl1));

        public Request ArchiveRequest
        {
            get { return (Request)GetValue(ArchiveRequestProperty); }
            set { SetValue(ArchiveRequestProperty, value); }
        }

        public event EventHandler DeactivateButtonClicked;

        protected virtual void OnDeactivateButtonClicked(EventArgs e)
        {
            DeactivateButtonClicked?.Invoke(this, e);
        }


        private void DeactivateImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement parent = this;
            while (parent != null && !(parent is AtchivePage))
            {
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }

            if (parent is AtchivePage archivePage && ArchiveRequest != null)
            {
                archivePage.ShowDeactivateGrid(ArchiveRequest);
            }
        }

        private void EditImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement parent = this;
            while (parent != null && !(parent is AtchivePage))
            {
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }

            if (parent is AtchivePage archivePage && ArchiveRequest != null)
            {
                archivePage.EditRequestPage(ArchiveRequest);
            }
        }

    }
}
