﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace PriyemnayaKomissiya_TechnicalSecretary_.Controls
{
    /// <summary>
    /// Логика взаимодействия для Articles.xaml
    /// </summary>
    public partial class Articles : UserControl
    {
        public RoutedEventHandler BlockCheckBox;
        private readonly string connectionString;
        public List<CheckBox> checkBoxes = new List<CheckBox>();
        public Articles()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("SELECT Наименование, ПолноеНаименование, Примечание FROM Статьи", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                bool toLeftColumn = true;
                while (reader.Read())
                {
                    CheckBox checkBox = new CheckBox()
                    {
                        Style = (Style)FindResource("CheckBoxStyleObshchiy"),
                        Margin = new Thickness(20, 10, 0, 0)
                    };

                    checkBox.Content = reader.GetString(reader.GetOrdinal("ПолноеНаименование"));

                    if (reader[reader.GetOrdinal("Примечание")] != DBNull.Value)
                    {
                        checkBox.ToolTip = reader.GetString(reader.GetOrdinal("Примечание"));
                    }

                    if (checkBox.Content.ToString() == "Сирота")
                    {
                        checkBox.Click += BlockAnotherCB;
                    }

                    checkBoxes.Add(checkBox);
                    if (toLeftColumn)
                    {
                        LeftColun.Children.Add(checkBox);
                    }
                    else
                    {
                        RightColumn.Children.Add(checkBox);
                    }
                    toLeftColumn = !toLeftColumn;
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Заполнение статей");
            }
        }

        private void BlockAnotherCB(object sender, RoutedEventArgs e)
        {
            BlockCheckBox?.Invoke(sender, e);
        }

        public void Clear()
        {
            foreach (CheckBox checkBox in checkBoxes)
            {
                checkBox.IsChecked = false;
            }
        }
    }
}
