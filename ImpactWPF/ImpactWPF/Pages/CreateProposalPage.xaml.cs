﻿// <copyright file="CreateProposalPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ImpactWPF.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using EfCore.context;
    using EfCore.dto;
    using EfCore.entity;
    using EfCore.service.impl;
    using NLog;

    /// <summary>
    /// Interaction logic for CreateProposalPage.xaml.
    /// </summary>
    public partial class CreateProposalPage : Page
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ImpactDbContext dbContext;
        private readonly RequestServiceImpl requestService;
        private readonly UserServiceImpl userService;
        private readonly List<string> selectedCategoriesList = new List<string>();

        public CreateProposalPage()
        {
            this.InitializeComponent();

            Logger.Info("Сторінка для створення нової пропозиції успішно ініціалізована");

            this.dbContext = new ImpactDbContext();
            this.requestService = new RequestServiceImpl(this.dbContext);
            this.userService = new UserServiceImpl(this.dbContext);
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in this.FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void UserMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.UserMenuGrid.Visibility == Visibility.Collapsed)
            {
                this.UserMenuGrid.Visibility = Visibility.Visible;
                Logger.Info("Користувач відкрив спадне навігаційне меню користувача");
            }
            else
            {
                this.UserMenuGrid.Visibility = Visibility.Collapsed;
                Logger.Info("Користувач закрив спадне навігаційне меню користувача");
            }
        }

        private void HomePage_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Logger.Info("Користувач перейшов на домашню сторінку");
            this.NavigationService?.Navigate(new HomePage());
        }

        private void CreateProposalPage_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Logger.Info("Користувач перейшов на сторінку для створення нової пропозиції");
            this.NavigationService?.Navigate(new CreateProposalPage());
        }

        private void CreateOrderPage_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Logger.Info("Користувач перейшов на сторінку для створення нового замовлення");
            this.NavigationService?.Navigate(new CreateOrderPage());
        }

        private void AdminPage_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Logger.Info("Користувач перейшов на сторінку адміна з таблицею запитів");
            this.NavigationService?.Navigate(new AdminPage());
        }

        private void SupportPage_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Logger.Info("Користувач перейшов на сторінку техпідтримки");
            this.NavigationService?.Navigate(new SupportPage());
        }

        private void CreateProposalButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Info("Початок процесу створення нової пропозиції");

                if (!this.ValidateFields())
                {
                    Logger.Error("Валідація полів вводу не пройшла успішно");
                    return;
                }

                string proposalName = this.proposalNameRequest.tbInput.Text;
                string description = this.descriptionRequest.tbInput.Text;
                string contactPhone = this.contactPhoneRequest.tbInput.Text;
                string contactEmail = this.contactEmailRequest.tbInput.Text;
                string location = this.locationRequest.tbInput.Text;

                List<int> selectedCategoryIds = this.GetCategoryIds(this.selectedCategoriesList);

                RequestDTO requestDTO = new RequestDTO
                {
                    RequestName = proposalName,
                    Description = description,
                    ContactPhone = contactPhone,
                    ContactEmail = contactEmail,
                    Location = location,
                    CreatorUserRef = this.userService.GetUserByEmail(UserSession.Instance.UserEmail).UserId,
                    RoleRef = 2,
                    Categories = selectedCategoryIds,
                };

                this.requestService.CreateRequest(requestDTO);
                Logger.Info("Нова пропозиція успішно створена");

                this.NavigationService?.Navigate(new CreateProposalPage());
            }
            catch (Exception ex)
            {
                Logger.Error($"Помилка при створенні пропозиції: {ex.Message}");
            }
        }

        private List<int> GetCategoryIds(List<string> categoryNames)
        {
            List<int> categoryIds = new List<int>();

            foreach (var categoryName in categoryNames)
            {
                var category = this.dbContext.RequestCategories.FirstOrDefault(c => c.CategoryName == categoryName);

                if (category != null)
                {
                    categoryIds.Add(category.CategoryId);
                }
            }

            Logger.Info("Успішно отримано id категорій за їх іменами");
            return categoryIds;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                StackPanel stackPanel = checkBox.Content as StackPanel;

                TextBlock textBlock = stackPanel?.Children.OfType<TextBlock>().FirstOrDefault();

                if (textBlock != null)
                {
                    string categoryName = textBlock.Text;

                    if (!this.selectedCategoriesList.Contains(categoryName))
                    {
                        this.selectedCategoriesList.Add(categoryName);
                        Logger.Info($"Користувач обрав категорію: {categoryName}");
                    }
                }

                Border border = checkBox.Parent as Border;
                if (border != null)
                {
                    border.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFE, 0xE8, 0x83));
                }

                if (stackPanel != null)
                {
                    Image image = stackPanel.Children.OfType<Image>().FirstOrDefault();
                    if (image != null)
                    {
                        RotateTransform rotateTransform = image.RenderTransform as RotateTransform;
                        if (rotateTransform != null)
                        {
                            double centerX = image.ActualWidth / 2;
                            double centerY = image.ActualHeight / 2;

                            rotateTransform.CenterX = centerX;
                            rotateTransform.CenterY = centerY;
                            rotateTransform.Angle = 45;
                        }
                    }
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                StackPanel stackPanel = checkBox.Content as StackPanel;

                TextBlock textBlock = stackPanel?.Children.OfType<TextBlock>().FirstOrDefault();

                if (textBlock != null)
                {
                    string categoryName = textBlock.Text;

                    if (this.selectedCategoriesList.Contains(categoryName))
                    {
                        this.selectedCategoriesList.Remove(categoryName);
                        Logger.Info($"Користувач вилучив категорію з обраних: {categoryName}");
                    }
                }

                Border border = checkBox.Parent as Border;
                if (border != null)
                {
                    border.Background = new SolidColorBrush(Colors.White);
                }

                if (stackPanel != null)
                {
                    Image image = stackPanel.Children.OfType<Image>().FirstOrDefault();
                    if (image != null)
                    {
                        RotateTransform rotateTransform = image.RenderTransform as RotateTransform;
                        if (rotateTransform != null)
                        {
                            rotateTransform.Angle = 0;
                        }
                    }
                }
            }
        }

        private void Categories_Click(object sender, RoutedEventArgs e)
        {
            if (this.Categories.Visibility == Visibility.Collapsed)
            {
                this.Categories.Visibility = Visibility.Visible;
                Logger.Info("Користувач відкрив спадний список з категоріями");
            }
            else
            {
                this.Categories.Visibility = Visibility.Collapsed;
                Logger.Info("Користувач закрив спадний список з категоріями");
            }
        }

        private bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");
            return regex.IsMatch(email);
        }

        private bool IsPhoneNumberValid(string name)
        {
            Regex regex = new Regex(@"^\+?\d{1,4}?[-.\s]?\(?\d{1,}\)?[-.\s]?\d{1,}[-.\s]?\d{1,}$");
            return regex.IsMatch(name);
        }

        private bool ValidateFields()
        {
            if (!this.IsValidEmail(this.contactEmailRequest.tbInput.Text))
            {
                Logger.Warn("Некоректний формат електронної адреси");
                MessageBox.Show("Некоректний формат електронної адреси! Приклад: example@mail.com", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (this.proposalNameRequest.tbInput.Text.Length <= 1)
            {
                Logger.Warn("Назва пропозиції містить менше 1 символа");
                MessageBox.Show("Назва пропозиції повинно містити більше 1 символа!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!this.IsPhoneNumberValid(this.contactPhoneRequest.tbInput.Text))
            {
                Logger.Warn("Некоректний формат номера телефону");
                MessageBox.Show(
                    "Некоректний формат номера телефону! \n Приклади: +1234567890\r\n+1 (123) 456-7890\r\n123.456.7890\r\n123-456-7890\r\n1234567890",
                    "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (this.selectedCategoriesList.Count > 3 || this.selectedCategoriesList.Count < 0)
            {
                Logger.Warn("Користувач не обрав від 1 до 3 категорій");
                MessageBox.Show("Будь ласка, оберіть від 1 до 3 категорій!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (this.descriptionRequest.tbInput.Text.Length < 0 || this.descriptionRequest.tbInput.Text.Length > 200)
            {
                Logger.Warn("Опис запиту не містить від 0 до 200 символів");
                MessageBox.Show("Опис запиту повинен містити від 0 до 200 символів!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            Logger.Info("Валідація полів вводу успішно завершена.");
            return true;
        }
    }
}
