using System;
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
    /// Логика взаимодействия для формы статей
    /// </summary>
    public partial class Articles : UserControl
    {
        public RoutedEventHandler BlockCheckBox; //команда бокирования льготы Сирота
        private readonly string connectionString;
        public List<CheckBox> checkBoxes = new List<CheckBox>(); //Список статей
        public Articles()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try //Заполнение статей
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("SELECT Наименование, ПолноеНаименование, Примечание FROM Статьи", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                bool toLeftColumn = true; //Определяет в какую колонку будет записана статья
                while (reader.Read())
                {
                    CheckBox checkBox = new CheckBox()
                    {
                        Style = (Style)FindResource("CheckBoxStyleObshchiy"),
                        Margin = new Thickness(20, 10, 0, 0),
                        Content = reader.GetString(reader.GetOrdinal("ПолноеНаименование"))
                    };

                    if (reader[reader.GetOrdinal("Примечание")] != DBNull.Value) //добавление подсказки если оно есть
                    {
                        checkBox.ToolTip = reader.GetString(reader.GetOrdinal("Примечание"));
                    }

                    if (checkBox.Content.ToString() == "Сирота")
                    {
                        checkBox.Click += BlockAnotherCB;
                    } //добавление обработчика для блокирования льготы сирота

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
        /// <summary>
        /// Вызов команды для блокирования льготы Строта
        /// </summary>
        private void BlockAnotherCB(object sender, RoutedEventArgs e)
        {
            BlockCheckBox?.Invoke(sender, e);
        }
        /// <summary>
        /// Установка всей статей на false
        /// </summary>
        public void Clear()
        {
            foreach (CheckBox checkBox in checkBoxes)
            {
                checkBox.IsChecked = false;
            }
        }
    }
}
