﻿using EfCore.entity;
using EfCore.service.impl;
using EFCore.service.impl;
using ImpactWPF.Pages;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImpactWPF
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MyProgressBar.Visibility = Visibility.Visible;

            string email = userEmailLogin.tbInput.Text;
            string password = userPasswordLogin.pbInput.Password;

            AuthServiceImpl authService = new AuthServiceImpl(new EfCore.context.ImpactDbContext());
            if (await Task.Run(() => authService.AuthenticateUser(email, password)))
            {
                string role = authService.GetUserRoleByEmail(email).RoleName;
                UserSession.Instance.Login(email, role);
                NavigationService?.Navigate(new HomePage());
            }
            else
            {
                MessageBox.Show("Неправильна адреса електронної пошти або пароль.");
            }

            MyProgressBar.Visibility = Visibility.Collapsed;
        }


        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegistrationPage());
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ForgotPasswordPage());
        }
    }
}
