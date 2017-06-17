﻿using System;
using System.Windows;

namespace Shell.Views
{
    public partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://vk.com/bansheedark");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK);
            }
        }
    }
}
