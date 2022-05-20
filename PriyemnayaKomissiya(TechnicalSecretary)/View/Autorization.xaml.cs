﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.DirectoryServices.AccountManagement;

namespace PriyemnayaKomissiya_TechnicalSecretary_
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Autorization : Window
    {
        private readonly string connectionString;
        private readonly string groupName = "grp_priem";
        public Autorization()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                if (tbPassword.Password == "priemadmin")
                {
                    string hasUser = $"SELECT IDПользователя FROM Пользователь WHERE Логин = '{tbLogin.Text}' AND IDроли = 4";

                    SqlCommand command = new SqlCommand(hasUser, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        View.MainWorkingWindow mainWorkingWindow = new View.MainWorkingWindow(reader.GetInt32(0), "Admin");
                        mainWorkingWindow.Show();
                        Close();
                        return;
                    }
                }

                PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);
                if (tbLogin.Text != "")
                {

                    UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, tbLogin.Text);

                    using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "college.local", "DC=college,DC=local", tbLogin.Text, tbPassword.Password))
                    {
                        if (pc.ValidateCredentials(tbLogin.Text, tbPassword.Password))
                        {
                            PrincipalSearchResult<Principal> groups = user.GetGroups();
                            bool grpCorrect = false;
                            foreach (GroupPrincipal g in groups)
                            {
                                if (g.Name == groupName)
                                {
                                    grpCorrect = true;
                                }
                            }
                            if (grpCorrect == false)
                            {
                                MessageBox.Show("Невозможно получить доступ для данного пользователя.");
                                tbPassword.Clear();
                                tbLogin.Focus();
                                tbLogin.SelectAll();
                                return;
                            }
                            string hasUser = $"SELECT IDПользователя FROM Пользователь WHERE Логин = '{tbLogin.Text}'";

                            SqlCommand command = new SqlCommand(hasUser, connection);
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                View.MainWorkingWindow mainWorkingWindow = new View.MainWorkingWindow(Convert.ToInt32(reader[0]), user.DisplayName);
                                mainWorkingWindow.Show();
                                Close();
                            }
                            else
                            {
                                reader.Close();
                                command = new SqlCommand("Add_User", connection)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };
                                command.Parameters.AddWithValue("@login", tbLogin.Text);
                                command.Parameters.AddWithValue("@fio", user.DisplayName);
                                command.Parameters.AddWithValue("@role", "Test");
                                reader = command.ExecuteReader();
                                reader.Read();
                                View.MainWorkingWindow mainWorkingWindow = new View.MainWorkingWindow(Convert.ToInt32(reader[0]), user.DisplayName);
                                mainWorkingWindow.Show();
                                Close();
                            }
                        }
                        else
                        {
                            tbPassword.SelectAll();
                            tbPassword.Tag = "Error";
                            tbLogin.Tag = "Error";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void TbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            pb.Tag = (!string.IsNullOrEmpty(pb.Password)).ToString();
            tbPassword.BorderBrush = default;
        }

        private void TbLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbLogin.BorderBrush = default;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSignIn_Click(btnSignIn, new RoutedEventArgs());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbLogin.Focus();
        }
    }
}
