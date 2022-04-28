using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;

namespace PriyemnayaKomissiya_TechnicalSecretary_.View
{
    public partial class MainWorkingWindow : Window
    {
        private readonly string connectionString;

        private int planPriemaColumn = 0;
        private PlanPriema curentPlanPriema = null;
        private int AbiturientID = 0;
        private int PlanPriemaID = 0;
        private int UserID;
        private string UserName;
        private List<AbiturientDGItem> abiturients;
        private List<Canvas> planPriemaButtons = new List<Canvas>();
        private List<PlanPriema> planPriemaDGsource = new List<PlanPriema>();

        #region Общее
        public MainWorkingWindow(int id, string name)
        {
            InitializeComponent();
            UserID = id;
            UserName = name;
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            planPriemaButtons.Add(BlockPlana1);
            planPriemaButtons.Add(BlockPlana2);
            planPriemaButtons.Add(BlockPlana3);
            planPriemaButtons.Add(BlockPlana4);
            planPriemaButtons.Add(BlockPlana5);
            planPriemaButtons.Add(BlockPlana6);
            planPriemaButtons.Add(BlockPlana7);
            planPriemaButtons.Add(BlockPlana8);
            planPriemaButtons.Add(BlockPlana9);
            lUser_FIO.Text = UserName;
        }
        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Autorization authorization = new Autorization();
            this.Close();
            authorization.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var date = new StringBuilder(DateTime.Now.ToString("dddd, d MMMM"));
            date[0] = char.ToUpper(date[0]);
            lDate.Content = date.ToString();
            lbPlanPriemaYear.Content = "ПЛАН ПРИЁМА " + DateTime.Now.Year;

            //Заполнение специальностей
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_SpecialnostiName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@useFilter", 0);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    TabItem tabItem = new TabItem
                    {
                        Style = (Style)FindResource("TabItemStyle"),
                        Header = reader[0]
                    };
                    tabItem.PreviewMouseDown += new MouseButtonEventHandler(TabItem_MouseDown);
                    TabControl.Items.Add(tabItem);

                    TabItem tabItem1 = new TabItem
                    {
                        Style = (Style)FindResource("TabItemStyle"),
                        Header = reader[0]
                    };
                    tabItem1.PreviewMouseDown += new MouseButtonEventHandler(TabItem1_MouseDown);
                    TabControl1.Items.Add(tabItem1);

                    TabItem tabItem2 = new TabItem
                    {
                        Style = (Style)FindResource("TabItemStyle"),
                        Header = reader[0]
                    };
                    tabItem2.PreviewMouseDown += new MouseButtonEventHandler(TabItem2_MouseDown);
                    TabControl2.Items.Add(tabItem2);
                }
                connection.Close();
                TabControl.SelectedItem = TabControl.Items[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            PlaniPriemaLoad(((TabItem)TabControl.SelectedItem).Header.ToString());
        }

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PlaniPriemaLoad(((TabItem)sender).Header.ToString());
            PlanPriemaTable.Visibility = Visibility.Hidden;
            GridInfo.Visibility = Visibility.Hidden;
            addEditForm.Visibility = Visibility.Hidden;
        }

        private void TabItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(tabWork.SelectedIndex == 1 &&  TabControl1.SelectedItem != null)
                PlanPriemaTableLoad(((TabItem)TabControl1.SelectedItem).Header.ToString());
        }
        #endregion

        #region Работа со сводными ведомостями
        private void TabItem_IsVisibleChangedVedomosti(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (tabWork.SelectedIndex == 0 && TabControl.SelectedItem != null)
                PlaniPriemaLoad(((TabItem)TabControl.SelectedItem).Header.ToString());
        }
            #region Выбор плана приема
        private void PlanPriema_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("Get_PlanPrieaByID", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("id", Convert.ToInt32(canvas.Tag));
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                curentPlanPriema = new PlanPriema
                {
                    Id = Convert.ToInt32(canvas.Tag),
                    IdSpec = reader.GetInt32(0),
                    IdForm = reader.GetInt32(1),
                    IdFinance = reader.GetInt32(2),
                    Count = reader.GetInt32(3),
                    CountCelevihMest = reader.GetInt32(4),
                    Year = reader.GetString(5),
                    CodeSpec = reader.GetString(6),
                    NameSpec = reader.GetString(7),
                    NameForm = reader.GetString(8),
                    NameObrazovaie = reader.GetString(9),
                    NameFinance = reader.GetString(10),
                    Ct = reader.GetBoolean(11)
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            PlanPriemaTable.Visibility = Visibility.Visible;
            LabelFormaObrazovaniya.Text = canvas.Children[2].GetValue(TagProperty).ToString();
            AbiturientTableLoad(curentPlanPriema.Id);
            filterCB.SelectedIndex = 0;
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            PlaniPriemaLoad(((TabItem)TabControl.SelectedItem).Header.ToString());
        }

        private void MainWorkingWindowForm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                ButtonPos(4);
            }
            else if (this.Width < 1300)
            {
                ButtonPos(2);
            }
            else if (this.Width < 1600)
            {
                ButtonPos(3);
            }
            else
            {
                ButtonPos(4);
            }
        }
        #endregion
        #region Таблица данных
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AbiturientInfoShow();
        }

        private void Image_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            GridInfo.Visibility = Visibility.Hidden;
            AbiturientDGItem abiturient = (AbiturientDGItem)dataGridAbiturients.SelectedItem;
            AbiturientID = abiturient.ID;
            if (abiturient != null)
            {
                //очистка старых данных
                addEditForm.Visibility = Visibility.Visible;
                TabControlAddEditForm.SelectedIndex = 0;
                foreach (TabItem item in TabControlAddEditForm.Items)
                    item.Tag = "True";
                ClearData<StackPanel>(AddEditMainData);
                ClearData<StackPanel>(AddEditFormContacts);
                ClearData<StackPanel>(addEdifFormAtestati);
                ClearData<StackPanel>(addEdifFormCT);
                ClearData<StackPanel>(AddEditFormPassport);

                try
                {
                    string sql1 = "SELECT Наименование FROM Шкала";
                    SqlConnection connection1 = new SqlConnection(connectionString);
                    SqlCommand command1 = new SqlCommand(sql1, connection1);
                    connection1.Open();
                    SqlDataReader reader1 = command1.ExecuteReader();
                    CBScale.Items.Clear();
                    while (reader1.Read())
                        CBScale.Items.Add(reader1[0]);
                    addEdifFormAtestati.Height += 450;
                    CBScale.SelectedIndex = 0;
                    connection1.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Get_SpecialnostiName", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@useFilter", 1);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    addEditFormspecialnost.Items.Clear();
                    while (reader.Read())
                    {
                        addEditFormspecialnost.Items.Add(reader[0]);
                    }
                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                addEditFormspecialnost.SelectedIndex = TabControl.SelectedIndex;
                string[] dataFormiObucheniya = LabelFormaObrazovaniya.Text.ToString().Split('.');
                addEditFormobushenie.SelectedItem = dataFormiObucheniya[0];
                addEditFormFinansirovanie.SelectedItem = dataFormiObucheniya[1].Substring(1);
                addEditFormobrazovanie.SelectedItem = dataFormiObucheniya[2].Substring(1);
                //Запись данных
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Get_AbiturientMainInfo", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@abiturient", abiturient.ID);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    reader.Read();
                    addEditFormSurename.Text = reader[0].ToString();
                    addEditFormName.Text = reader[1].ToString();
                    addEditFormOtchestvo.Text = reader[2].ToString();
                    addEditFormShool.Text = reader[3].ToString();
                    addEditFormGraduationYear.Text = reader[4].ToString();
                    dateOfBirth.Text = reader[5].ToString().Split(' ')[0];
                    PassportDateVidachi.Text = reader[6].ToString().Split(' ')[0];
                    PassportSeriya.Text = reader[7].ToString();
                    PassportNomer.Text = reader[8].ToString();
                    PassportVidan.Text = reader[9].ToString();
                    PassportIdentNum.Text = reader[10].ToString();
                    AddFormGrajdanstvo.Text = reader[11].ToString();
                    textBoxWorkPlace.Text = reader[12].ToString();
                    textBoxDoljnost.Text = reader[13].ToString();
                    addEditFormObshejitie.IsChecked = reader[14].ToString() == "True";
                    addEditForm_CheckBox_DetiSiroti.IsChecked = reader[15].ToString() == "True";
                    addEditForm_CheckBox_Dogovor.IsChecked = reader[16].ToString() == "True";
                    addEditFormExamList.Text = reader[17].ToString();
                    reader.Close();

                    SqlConnection con = new SqlConnection(connectionString);
                    con.Open();
                    for (int k = 0; k < 2; k++)
                    {
                        StackPanel panel = Stati.Children[k] as StackPanel;
                        for (int j = 0; j < 3; j++)
                        {
                            CheckBox checkBox = panel.Children[j] as CheckBox;
                            SqlCommand command1 = new SqlCommand("HasStatya", con)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            command1.Parameters.AddWithValue("@abiturient", abiturient.ID);
                            command1.Parameters.AddWithValue("@statya", checkBox.Content);
                            SqlDataReader reader1 = command1.ExecuteReader();
                            if (reader1.HasRows)
                            {
                                checkBox.IsChecked = true;
                            }
                            reader1.Close();
                        }
                    }
                    con.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }//основные данные и паспортные данные
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand("Get_AbiturientaKontakti", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@abiturient", abiturient.ID);
                    SqlDataReader reader = command.ExecuteReader();
                    int lastPoint = 0;
                    while (reader.Read())
                    {
                        for (int i = lastPoint; i < AddEditFormContacts.Children.Count; i++)
                        {
                            string Tag = ((StackPanel)AddEditFormContacts.Children[i]).Tag.ToString();
                            if (Tag == "VisibleField" || Tag == "HIddenField")
                            {
                                try
                                {
                                    ComboBox comboBox = (ComboBox)((StackPanel)AddEditFormContacts.Children[i]).Children[3];
                                    string sql = "SELECT Наименование FROM ТипКонтакта";
                                    SqlConnection connection1 = new SqlConnection(connectionString);
                                    SqlCommand command1 = new SqlCommand(sql, connection1);
                                    connection1.Open();
                                    SqlDataReader reader1 = command1.ExecuteReader();
                                    comboBox.Items.Clear();
                                    while (reader1.Read())
                                        comboBox.Items.Add(reader1[0]);
                                    comboBox.SelectedIndex = 0;
                                    connection1.Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                                StackPanel stackPanel = AddEditFormContacts.Children[i] as StackPanel;
                                stackPanel.Visibility = Visibility.Visible;
                                ((ComboBox)stackPanel.Children[3]).SelectedItem = reader[2].ToString();
                                ((Xceed.Wpf.Toolkit.MaskedTextBox)stackPanel.Children[5]).Text = reader[3].ToString();
                                lastPoint = i + 1;
                                break;
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }//контактные данные
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand("Get_AbiturientaAttestat", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@abiturient", abiturient.ID);
                    SqlDataReader reader = command.ExecuteReader();
                    int lastPoint = 0;
                    while (reader.Read())
                    {
                        for (int i = lastPoint; i < addEdifFormAtestati.Children.Count; i++)
                        {
                            string Tag = ((StackPanel)addEdifFormAtestati.Children[i]).Tag.ToString();
                            if (Tag == "VisibleField" || Tag == "HIddenField")
                            {
                                try
                                {
                                    ComboBox comboBox = (ComboBox)((StackPanel)addEdifFormAtestati.Children[i]).Children[7];
                                    string sql1 = "SELECT Наименование FROM Шкала";
                                    SqlConnection connection1 = new SqlConnection(connectionString);
                                    SqlCommand command1 = new SqlCommand(sql1, connection1);
                                    connection1.Open();
                                    SqlDataReader reader1 = command1.ExecuteReader();
                                    comboBox.Items.Clear();
                                    while (reader1.Read())
                                        comboBox.Items.Add(reader1[0]);
                                    addEdifFormAtestati.Height += 450;
                                    comboBox.SelectedIndex = 0;
                                    connection1.Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                                StackPanel stackPanel = addEdifFormAtestati.Children[i] as StackPanel;
                                stackPanel.Visibility = Visibility.Visible;

                                ((TextBox)stackPanel.Children[3]).Text = reader[1].ToString();

                                Grid grid = stackPanel.Children[4] as Grid;
                                for (int j = 4; j < 32; j += 2)
                                {
                                    ((TextBox)grid.Children[j]).Text = reader[j / 2].ToString();
                                }
                                ((ComboBox)stackPanel.Children[7]).SelectedItem = reader[17].ToString();
                                lastPoint = i + 1;
                                break;
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }//Атестаты
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand("Get_AbiturientaSertificati", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@abiturient", abiturient.ID);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        for (int i = 0; i < addEdifFormCT.Children.Count; i++)
                        {
                            if (addEdifFormCT.Children[i].Visibility == Visibility.Collapsed)
                            {
                                Grid grid = ((StackPanel)addEdifFormCT.Children[i]).Children[2] as Grid;

                                ((TextBox)grid.Children[1]).Text = reader[1].ToString();
                                ((ComboBox)grid.Children[5]).SelectedItem = reader[2].ToString();
                                ((Xceed.Wpf.Toolkit.MaskedTextBox)grid.Children[3]).Text = reader[3].ToString();
                                ((TextBox)grid.Children[7]).Text = reader[4].ToString();
                                addEdifFormCT.Height += 257;
                                addEdifFormCT.Children[i].Visibility = Visibility.Visible;
                                break;
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }//сертификаты цт
            }
        } //нажатие кнопки редактирования

        private void Image_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            ((Image)sender).ContextMenu.IsOpen = true;
        }

        private void Table_PressDelete(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (dataGridAbiturients.SelectedItems.Count == 0) return;
                string delItemsName = "";
                int i = 0;
                {
                    AbiturientDGItem abiturient;
                    do
                    {
                        delItemsName += $"{((AbiturientDGItem)dataGridAbiturients.SelectedItems[i]).FIO}\n ";
                        abiturient = dataGridAbiturients.SelectedItems[i] as AbiturientDGItem;
                        i++;
                    } while (i < 3 && i < dataGridAbiturients.SelectedItems.Count && abiturient != null);
                    if (dataGridAbiturients.SelectedItems.Count - 3 > 0)
                        delItemsName += $"И еще {dataGridAbiturients.SelectedItems.Count - 3} запись(-и)";
                }

                if (MessageBox.Show($"Отметить данные записи как удаленные?\n\n {delItemsName}", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        foreach (AbiturientDGItem abiturient in dataGridAbiturients.SelectedItems)
                        {
                            SqlConnection connection = new SqlConnection(connectionString);
                            SqlCommand command = new SqlCommand("Del_AbiturientMarks", connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            command.Parameters.AddWithValue("@abiturient", abiturient.ID);
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        AbiturientTableLoad(curentPlanPriema.Id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        private void DataGridAbiturients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AbiturientInfoShow();
        }
        private void Abiturient_SetStatus(object sender, RoutedEventArgs e)
        {
            if ((AbiturientDGItem)dataGridAbiturients.SelectedItem == null) return;
            try
            {
                foreach (AbiturientDGItem abiturient in dataGridAbiturients.SelectedItems)
                {
                    string[] stat = ((MenuItem)sender).Tag.ToString().Split(',');
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    string sql = $"UPDATE Абитуриент SET Удалено = {stat[0]}, АбитуриентЗачислен = {stat[1]} WHERE IDАбитуриента = {abiturient.ID}";
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                }
                if (GridInfo.Visibility == Visibility.Hidden)
                {
                    bool isSortByRating = (bool)dataGridAbiturients.Tag; 
                    AbiturientTableLoad(curentPlanPriema.Id);
                    if (isSortByRating)
                    {
                        Button_Click(null, null);
                    }
                }
                else AbiturientInfoShow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Abiturient_Delete(object sender, RoutedEventArgs e)
        {
            if ((AbiturientDGItem)dataGridAbiturients.SelectedItem == null) return;
            if (MessageBox.Show($"Отметить данную запись как удаленную?\n\n  {((AbiturientDGItem)dataGridAbiturients.SelectedItem).FIO}", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Del_AbiturientMarks", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataGridAbiturients.SelectedItem).ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    AbiturientTableLoad(curentPlanPriema.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<AbiturientDGItem> newabiturients = new List<AbiturientDGItem>();

            newabiturients = abiturients.FindAll(x => Regex.IsMatch(x.FIO.ToLower(), $@"{textBoxSearch.Text.ToLower()}"));

            dataGridAbiturients.ItemsSource = newabiturients;
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (abiturients == null) return;
            if (((ComboBox)sender).SelectedIndex == 1)
                foreach (AbiturientDGItem item in abiturients)
                {
                    if (!Regex.IsMatch(item.Lgoti, $@"Договор"))
                        item.Hide = true;
                }
            else
            {
                foreach (AbiturientDGItem item in abiturients)
                    item.Hide = false;
            }
            TextBoxSearch_TextChanged(sender, null);
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PlanPriemaTable.Visibility = Visibility.Hidden;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("AbiturientsPriority", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@ID", curentPlanPriema.Id);

                SqlDataReader reader = command.ExecuteReader();
                List<AbiturientDGItem> list = new List<AbiturientDGItem>();
                while (reader.Read())
                {
                    int abiturientID = reader.GetInt32(0);
                    list.Add(abiturients.Find(x => x.ID == abiturientID));
                }
                reader.Close();
                abiturients = list;
                dataGridAbiturients.ItemsSource = abiturients;
                dataGridAbiturients.Tag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        private void ExcelContextMenuOpen(object sender, MouseButtonEventArgs e)
        {
            ((Image)sender).ContextMenu.IsOpen = true;
        }
        private void DataGridAbiturients_Sorting(object sender, DataGridSortingEventArgs e)
        {
            dataGridAbiturients.Tag = false;
        }
        private void PrintButton_Click(object sender, MouseButtonEventArgs e)
        {
            ToExcelButton_Click(sender, e);
        }
        private void ToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = System.IO.Path.GetDirectoryName(location);
            string fileName = "/ШаблонДанные.xlsx";
            if (!File.Exists(path + fileName))
            {
                MessageBox.Show("Не удалось найти или открыть файл шаблона!");
                return;
            }
            Excel.Application ex = new Microsoft.Office.Interop.Excel.Application
            {
                Visible = true,
                SheetsInNewWorkbook = 1,
                Interactive = false
            };
            Excel.Workbook workBook = ex.Workbooks.Open(path + fileName);
            ex.DisplayAlerts = false;
            Excel.Worksheet sheet = (Excel.Worksheet)ex.Worksheets.get_Item(1);
            string[] dataFormiObucheniya = LabelFormaObrazovaniya.Text.ToString().Split('.');
            ex.Cells[3, 1] = $"по специальности {'"' + ((TabItem)TabControl.SelectedItem).Header.ToString() +'"'}";
            ex.Cells[4, 1] = $"на {dataFormiObucheniya[0].ToLower().Substring(0,dataFormiObucheniya[0].Length-2) + "ом"} отделении ({dataFormiObucheniya[2].ToLower()}) {dataFormiObucheniya[1]}";
            int reads = dataGridAbiturients.Items.Count;
            for (int i = 0; i < dataGridAbiturients.Items.Count; i++)
            {
                AbiturientDGItem abiturient = (AbiturientDGItem)dataGridAbiturients.Items[i];
                if (abiturient.Hide == true) 
                {
                    reads = i;
                    break;
                }
                ex.Cells[9 + i, 1] = i + 1;
                ex.Cells[9 + i, 18] = Convert.ToBoolean(((CheckBox)dataGridAbiturients.Columns[18].GetCellContent(dataGridAbiturients.Items[i])).IsChecked) ? "+" : "0";
                for (int j = 2; j < 18; j++)
                    ex.Cells[9 + i, j] = dataGridAbiturients.Columns[j].GetCellContent(dataGridAbiturients.Items[i]).GetValue(TextBlock.TextProperty);
                if (abiturient.DifferentAttestat == true)
                {
                    Excel.Range range = ex.Cells[9 + i, 17];
                    range.Font.Bold = true;
                }

            }
            ex.Range["A10","R"+(reads+8)].Cells.Borders.Weight = Excel.XlBorderWeight.xlThin;
            ex.Interactive = true;
            if (((FrameworkElement)sender).Tag != null && ((FrameworkElement)sender).Tag.ToString() == "Print")
            {
                bool userDidntCancel =
            ex.Dialogs[Excel.XlBuiltInDialog.xlDialogPrint].Show(
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
        }

        private void OtchetToExcel_Click(object sender, RoutedEventArgs e)
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = System.IO.Path.GetDirectoryName(location);
            string fileName = "/ШаблонОтчет.xlsx";
            if (!File.Exists(path + fileName))
            {
                MessageBox.Show("Не удалось найти или открыть файл шаблона!");
                return;
            }
            Excel.Application ex = new Microsoft.Office.Interop.Excel.Application
            {
                Visible = true,
                SheetsInNewWorkbook = 1,
                Interactive = false
            };
            Excel.Workbook workBook = ex.Workbooks.Open(path + fileName);
            ex.DisplayAlerts = false;
            Excel.Worksheet sheet = (Excel.Worksheet)ex.Worksheets.get_Item(1);
            string[] dataFormiObucheniya = LabelFormaObrazovaniya.Text.ToString().Split('.');

            ex.Cells[4, 1] = $"Специальность (направление специальности, специализация) {curentPlanPriema.CodeSpec} {curentPlanPriema.NameSpec}";
            ex.Cells[5, 1] = $"Форма получения образования: {curentPlanPriema.NameForm.ToLower()} ({curentPlanPriema.NameFinance.ToLower()})";
            ex.Cells[6, 1] = curentPlanPriema.NameObrazovaie;
            ex.Cells[13, 3] = DateTime.Now.ToString("dd.MM.yyyy");
            int offset = 0;
            bool writeline = false;
            for(int i = 0; i < abiturients.Count; i++)
            {
                if (abiturients[i].DocumentiVidani)
                {
                    offset++;
                    continue;
                }
                ex.Cells[9 + i - offset, 1] = i + 1 - offset;
                ex.Cells[9 + i - offset, 2] = abiturients[i].ExamNum;
                ex.Cells[9 + i - offset, 3] = abiturients[i].FIO;
                ex.Cells[9 + i - offset, 6] = abiturients[i].MarkDecAvg;
                ex.Cells[9 + i - offset, 7] = abiturients[i].MarkDecAvg;
                ex.Cells[9 + i - offset, 9] = abiturients[i].Lgoti;
                ex.Cells[9 + i - offset, 10] = abiturients[i].Status == "Зачислен" ? "зачислен" : "не зачислен";
                Excel.Range cellRange = (Excel.Range)ex.Cells[10 + i - offset, 1];
                Excel.Range rowRange = cellRange.EntireRow;
                rowRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);
                ex.Range[$"A{9 + i - offset}", $"J{9 + i - offset}"].Cells.Borders.Weight = Excel.XlBorderWeight.xlThin;
                if(Convert.ToBoolean(dataGridAbiturients.Tag) == true && abiturients[i].Status != "Зачислен" && !writeline)
                {
                    ex.Range[$"A{9 + i - offset}", $"J{9 + i - offset}"].Cells.Borders.Item[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlMedium;
                    writeline = true;
                }
            }
            ex.Interactive = true;
        }

        private void ImportToAD(object sender, RoutedEventArgs e)
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = System.IO.Path.GetDirectoryName(location);
            string fileName = "/ШаблонAD.xlsx";
            if (!File.Exists(path + fileName))
            {
                MessageBox.Show("Не удалось найти или открыть файл шаблона!");
                return;
            }
            Excel.Application ex = new Microsoft.Office.Interop.Excel.Application
            {
                Visible = true,
                SheetsInNewWorkbook = 1,
                Interactive = false
            };
            Excel.Workbook workBook = ex.Workbooks.Open(path + fileName);
            ex.DisplayAlerts = false;
            Excel.Worksheet sheet = (Excel.Worksheet)ex.Worksheets.get_Item(1);

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("ImportAD", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("id", curentPlanPriema.Id);
                SqlDataReader reader = command.ExecuteReader();
                int readNum = 2;
                while (reader.Read())
                {
                    ex.Cells[readNum, 1] = reader.GetString(0);
                    ex.Cells[readNum, 2] = reader.GetString(1);
                    ex.Cells[readNum, 3] = reader.GetString(2);

                    ex.Cells[readNum, 6] = reader[3] != DBNull.Value ? reader.GetString(3) : "";
                    ex.Cells[readNum, 7] = reader[4] != DBNull.Value ? reader.GetString(4) : "";
                    ex.Cells[readNum, 8] = reader[5] != DBNull.Value ? reader.GetString(3) : "";
                    ex.Cells[readNum, 9] = reader.GetDateTime(6).ToString("dd.MM.yyyy");
                    ex.Cells[readNum, 10] = reader.GetString(7);
                    ex.Cells[readNum, 11] = reader.GetString(8);
                    ex.Cells[readNum, 12] = reader.GetString(9);
                    readNum++;
                }
                reader.Close();
                connection.Close();
            }
            catch(Exception exept)
            {
                MessageBox.Show(exept.Message);
            }

            ex.Interactive = true;
        }
        #endregion
            #region Просмотр информации
        private void Image_BackToAbiturients(object sender, MouseButtonEventArgs e)
        {
            GridInfo.Visibility = Visibility.Hidden;
            AbiturientTableLoad(curentPlanPriema.Id);
        }

        private void Image_AtestatRedakt(object sender, MouseButtonEventArgs e)
        {
            Image_MouseUp_1(sender, e);
            TabControlAddEditForm.SelectedIndex = 2;
        }
        private void MenuItem_DeleteAtestat(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить атестат?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    string sql = $"DELETE FROM Атестат WHERE IDАтестата = {((DataRowView)AtestatGrid.SelectedItem)[0]}";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception) { }
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Get_AbiturientaAttestat", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataGridAbiturients.SelectedItem).ID);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    connection.Open();
                    dataAdapter.Fill(dataTable);
                    AtestatGrid.ItemsSource = dataTable.DefaultView;
                    connection.Close();
                    atestatCount.Text = AtestatGrid.Items.Count.ToString();

                    for (int i = 1; i < AtestatGrid.Columns.Count - 1; i++)
                    {
                        bool isNull = true;
                        for (int j = 0; j < AtestatGrid.Items.Count; j++)
                        {
                            if (((DataRowView)AtestatGrid.Items[j])[i].ToString() != "")
                            {
                                isNull = false;
                            }
                        }
                        if (isNull) AtestatGrid.Columns[i].Visibility = Visibility.Hidden;
                        else AtestatGrid.Columns[i].Visibility = Visibility.Visible;
                    }//скрытие неиспользуемых столбцов
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Image_CTRedakt(object sender, MouseButtonEventArgs e)
        {
            Image_MouseUp_1(sender, e);
            TabControlAddEditForm.SelectedIndex = 3;
        }

        private void Image_KontaktsRedakt(object sender, MouseButtonEventArgs e)
        {
            Image_MouseUp_1(sender, e);
            TabControlAddEditForm.SelectedIndex = 1;
        }

        private void MenuItem_DeleteContact(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить контакт?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    string sql = $"DELETE FROM КонтактныеДанные WHERE IDКонтактныеДанные = {((DataRowView)kontaktnieDannieGrid.SelectedItem)[0]}";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                try//Контактные данные
                {
                    string sql = $"SELECT IDКонтактныеДанные, ROW_NUMbER() OVER(ORDER BY IDКонтактныеДанные) as Num, (SELECT Наименование FROM ТипКонтакта WHERE КонтактныеДанные.IDТипКонтакта = ТипКонтакта.IDТипКонтакта) as [ТипКонтакта], Сведения FROM  КонтактныеДанные WHERE IDАбитуриента = {((AbiturientDGItem)dataGridAbiturients.SelectedItem).ID}";
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    connection.Open();
                    dataAdapter.Fill(dataTable);
                    kontaktnieDannieGrid.ItemsSource = dataTable.DefaultView;
                    connection.Close();
                    contactsCount.Text = kontaktnieDannieGrid.Items.Count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void MenuItem_DeleteCT(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить сертификат ЦТ?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    string sql = $"DELETE FROM СертификатЦТ WHERE IDСертификатаЦТ = {((DataRowView)SertificatiCTGrid.SelectedItem)[0]}";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                try//цт
                {
                    string sql = $"SELECT IDСертификатаЦТ, НомерСерии as num, Дисциплина, ГодПрохождения, Балл, ДесятибальноеЗначение FROM СертификатЦТ WHERE IDАбитуриента = {((AbiturientDGItem)dataGridAbiturients.SelectedItem).ID}";
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    connection.Open();
                    dataAdapter.Fill(dataTable);
                    SertificatiCTGrid.ItemsSource = dataTable.DefaultView;
                    connection.Close();
                    sertificatCount.Text = SertificatiCTGrid.Items.Count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
        #endregion
            #region редактирование записи
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                AddFormGrajdanstvo.Text = "Белорусское";
            }
        }
        private void AddFormGrajdanstvo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text != "")
            {
                ((TextBox)sender).Tag = "";
            }
            if (((TextBox)sender).Text == "Белорусское")
                AddFormChekBoxGrajdanstvo.IsChecked = true;
            else
                AddFormChekBoxGrajdanstvo.IsChecked = false;
        }
        private void DateOfBirth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DateTime.TryParse(((Xceed.Wpf.Toolkit.MaskedTextBox)sender).Text, out _))
            {
                ((Xceed.Wpf.Toolkit.MaskedTextBox)sender).Tag = "";
            }
            else
                ((Xceed.Wpf.Toolkit.MaskedTextBox)sender).Tag = "Error";
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text == "")
                ((TextBox)sender).Tag = "Error";
            else
                ((TextBox)sender).Tag = "";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < AddEditFormContacts.Children.Count; i++)
            {
                if (AddEditFormContacts.Children[i].Visibility == Visibility.Collapsed)
                {
                    try
                    {
                        ComboBox comboBox = (ComboBox)((StackPanel)AddEditFormContacts.Children[i]).Children[3];
                        string sql = "SELECT Наименование FROM ТипКонтакта";
                        SqlConnection connection = new SqlConnection(connectionString);
                        SqlCommand command = new SqlCommand(sql, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        comboBox.Items.Clear();
                        while (reader.Read())
                            comboBox.Items.Add(reader[0]);
                        comboBox.SelectedIndex = 0;
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    AddEditFormContacts.Children[i].Visibility = Visibility.Visible;
                    break;
                }
            }
        }//добавление нового контакта

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Xceed.Wpf.Toolkit.MaskedTextBox textBox = ((StackPanel)((ComboBox)sender).Parent).Children[5] as Xceed.Wpf.Toolkit.MaskedTextBox;
            switch (((ComboBox)sender).SelectedIndex)
            {
                case 0:
                    textBox.Mask = "+0## 00 000-00-00";
                    textBox.Text = "+375";
                    break;
                default:
                    textBox.Mask = "";
                    textBox.Text = "";
                    break;
            }
        }

        private void Button_NewAtestat(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < addEdifFormAtestati.Children.Count; i++)
            {
                if (addEdifFormAtestati.Children[i].Visibility == Visibility.Collapsed)
                {
                    try
                    {
                        ComboBox comboBox = (ComboBox)((StackPanel)addEdifFormAtestati.Children[i]).Children[7];
                        string sql = "SELECT Наименование FROM Шкала";
                        SqlConnection connection = new SqlConnection(connectionString);
                        SqlCommand command = new SqlCommand(sql, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        comboBox.Items.Clear();
                        while (reader.Read())
                            comboBox.Items.Add(reader[0]);
                        comboBox.SelectedIndex = 0;
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    addEdifFormAtestati.Height += 450;
                    addEdifFormAtestati.Children[i].Visibility = Visibility.Visible;
                    break;
                }
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static readonly Regex _regex = new Regex("[^0-9]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void ButtonNewSertificatCT(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < addEdifFormCT.Children.Count; i++)
            {
                if (addEdifFormCT.Children[i].Visibility == Visibility.Collapsed)
                {
                    addEdifFormCT.Height += 257;
                    addEdifFormCT.Children[i].Visibility = Visibility.Visible;
                    break;
                }
            }
        }

        private void Button_PrewPage(object sender, RoutedEventArgs e)
        {
            TabControlAddEditForm.SelectedIndex -= 1;
            if (((TabItem)TabControlAddEditForm.SelectedItem).IsEnabled == false)
                TabControlAddEditForm.SelectedIndex--;
        }

        private void Button_CloseNote(object sender, RoutedEventArgs e)
        {
            ((StackPanel)((Grid)((Button)sender).Parent).Parent).Visibility = Visibility.Collapsed;
        }
        private void Button_NextStep_1(object sender, RoutedEventArgs e)
        {
            if (Correct_1())
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
            }
            else
            {
                ((TabItem)TabControlAddEditForm.Items[0]).Tag = "";
                ScrollAddMain.ScrollToVerticalOffset(0);
            }
        }

        private void Button_NextStep_2(object sender, RoutedEventArgs e)
        {
            if (Correct_2())
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
            }

        }

        private void Button_NextStep_3(object sender, RoutedEventArgs e)
        {
            if (Correct_3())
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
                if (((TabItem)TabControlAddEditForm.SelectedItem).IsEnabled == false)
                    TabControlAddEditForm.SelectedIndex++;
            }
        }

        private void Button_NextStep_4(object sender, RoutedEventArgs e)
        {
            if (Correct_4())
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
            }
        }


        //завершение редактирования
        private void Button_EditEnd(object sender, RoutedEventArgs e)
        {
            if (!EnterIsCorrect()) return;

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Update_MainData", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@surename", addEditFormSurename.Text);
                command.Parameters.AddWithValue("@name", addEditFormName.Text);
                command.Parameters.AddWithValue("@otchestvo", addEditFormOtchestvo.Text);
                command.Parameters.AddWithValue("@shool", addEditFormShool.Text);
                command.Parameters.AddWithValue("@graduationYear", addEditFormGraduationYear.Text);
                command.Parameters.AddWithValue("@grajdaninRB", AddFormChekBoxGrajdanstvo.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@grajdanstvo", AddFormGrajdanstvo.Text);
                command.Parameters.AddWithValue("@obshejitie", addEditFormObshejitie.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@planPriema", PlanPriemaID);
                command.Parameters.AddWithValue("@workPlase", textBoxWorkPlace.Text);
                command.Parameters.AddWithValue("@doljnost", textBoxDoljnost.Text);
                command.Parameters.AddWithValue("@sirota", addEditForm_CheckBox_DetiSiroti.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@dogovor", addEditForm_CheckBox_Dogovor.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@redaktor", UserID);
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                command.Parameters.AddWithValue("@ExamList", addEditFormExamList.Text);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Основные данные");
                return;
            }//Основные данные +-
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sqldel = $"DELETE FROM КонтактныеДанные WHERE IDАбитуриента = {AbiturientID}";
                SqlCommand del = new SqlCommand(sqldel, connection);
                del.ExecuteNonQuery();
                for (int i = 0; i < AddEditFormContacts.Children.Count - 1; i++)
                {
                    if (AddEditFormContacts.Children[i].Visibility == Visibility.Visible && (AddEditFormContacts.Children[i] as StackPanel) != null)
                    {
                        StackPanel stackPanel = AddEditFormContacts.Children[i] as StackPanel;

                        SqlCommand command = new SqlCommand("Add_ContctData", connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        command.Parameters.AddWithValue("@abiturient", AbiturientID);
                        command.Parameters.AddWithValue("@svedeniya", ((TextBox)stackPanel.Children[5]).Text.Replace("_", string.Empty));
                        command.Parameters.AddWithValue("@contactType", ((ComboBox)stackPanel.Children[3]).SelectedItem);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Контактные данные");
            }//Контактные данные* ?
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sqldel = $"DELETE FROM Атестат WHERE IDАбитуриента = {AbiturientID}";
                SqlCommand del = new SqlCommand(sqldel, connection);
                del.ExecuteNonQuery();
                for (int i = 0; i < addEdifFormAtestati.Children.Count - 1; i++)
                {
                    if (addEdifFormAtestati.Children[i].Visibility == Visibility.Visible && (addEdifFormAtestati.Children[i] as StackPanel) != null)
                    {
                        StackPanel stackPanel = addEdifFormAtestati.Children[i] as StackPanel;

                        List<int> marks = new List<int>();
                        List<int> marksDec = new List<int>();
                        Grid grid = stackPanel.Children[4] as Grid;

                        for (int j = 4; j < 32; j += 2)
                        {
                            if (((TextBox)grid.Children[j]).Text != "")
                            {
                                marks.Add(Convert.ToInt16(((TextBox)grid.Children[j]).Text));
                            }
                            else break;
                        }

                        double sum = 0;
                        int col = 0;
                        for (int j = 0; j < marks.Count; j++)
                        {
                            sum += (marks[j]) * (j + 1);
                            col += marks[j];
                        }
                        double markAvg = sum / col;


                        SqlCommand command = new SqlCommand("Add_Atestat", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@abiturient", AbiturientID);
                        command.Parameters.AddWithValue("@scaleName", ((ComboBox)stackPanel.Children[7]).SelectedItem);
                        command.Parameters.AddWithValue("@attestatSeries", ((TextBox)stackPanel.Children[3]).Text);
                        command.Parameters.AddWithValue("@avgMarks", Math.Round(markAvg, 2));
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        int AtestatID = (int)reader[0];
                        reader.Close();
                        for (int j = 0; j < marks.Count; j++)
                        {
                            command = new SqlCommand("Add_Mark", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@attestat", AtestatID);
                            command.Parameters.AddWithValue("@mark", j + 1);
                            command.Parameters.AddWithValue("@colvo", marks[j]);
                            command.ExecuteNonQuery();
                        }

                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Образование");
            }//Образование* ?
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sqldel = $"DELETE FROM СертификатЦТ WHERE IDАбитуриента = {AbiturientID}";
                SqlCommand del = new SqlCommand(sqldel, connection);
                del.ExecuteNonQuery();
                for (int i = 0; i < addEdifFormCT.Children.Count - 1; i++)
                {
                    if (addEdifFormCT.Children[i].Visibility == Visibility.Visible && (addEdifFormCT.Children[i] as StackPanel) != null)
                    {
                        Grid grid = (Grid)(addEdifFormCT.Children[i] as StackPanel).Children[2];

                        SqlCommand command = new SqlCommand("Add_Sertificat", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@sertificat", AbiturientID);
                        command.Parameters.AddWithValue("@disciplin", ((ComboBoxItem)((ComboBox)grid.Children[5]).SelectedItem).Content);
                        command.Parameters.AddWithValue("@mark", ((TextBox)grid.Children[7]).Text);
                        command.Parameters.AddWithValue("@decMark", (Convert.ToDouble(((TextBox)grid.Children[7]).Text) / 10).ToString().Replace(',', '.'));
                        command.Parameters.AddWithValue("@year", ((TextBox)grid.Children[3]).Text);
                        command.Parameters.AddWithValue("@serialNum", ((TextBox)grid.Children[1]).Text);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Сертификаты ЦТ");
            }//Сертификаты ЦТ* ?
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Update_PasportData", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@dateVidachi", PassportDateVidachi.Text);
                command.Parameters.AddWithValue("@dateOfBirth", dateOfBirth.Text);
                command.Parameters.AddWithValue("@seriya", PassportSeriya.Text);
                command.Parameters.AddWithValue("@pasportNum", PassportNomer.Text);
                command.Parameters.AddWithValue("@vidan", PassportVidan.Text);
                command.Parameters.AddWithValue("@identNum", PassportIdentNum.Text);
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Паспортные данные");
            }//Паспортные данные*
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sqldel = $"DELETE FROM СтатьиАбитуриента WHERE IDАбитуриента = {AbiturientID}";
                SqlCommand del = new SqlCommand(sqldel, connection);
                del.ExecuteNonQuery();
                for (int i = 0; i < 2; i++)
                {
                    StackPanel stackPanel = (StackPanel)Stati.Children[i];
                    for (int j = 0; j < 3; j++)
                    {
                        CheckBox checkBox = (CheckBox)stackPanel.Children[j];
                        if (checkBox.IsChecked == true)
                        {
                            string sql1 = $"SELECT IDСтатьи FROM Статьи WHERE ПолноеНаименование LIKE N'{checkBox.Content}'";
                            SqlCommand command = new SqlCommand(sql1, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();

                            command = new SqlCommand("Add_Stati", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@abiturient", AbiturientID);
                            command.Parameters.AddWithValue("@statya", reader[0]);
                            reader.Close();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Статьи");
            }//Статьи* ?
            AbiturientTableLoad(curentPlanPriema.Id);
            addEditForm.Visibility = Visibility.Hidden;
        }

        #region заполнение ComboBoks для формы редактирования
        private void AddEditFormspecialnost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addEditFormspecialnost.SelectedItem == null) return;
            try
            {
                string sql1 = $"SELECT DISTINCT ФормаОбучения.Наименование FROM ПланПриема JOIN Специальность ON(ПланПриема.IDСпециальности = Специальность.IDСпециальность) JOIN ФормаОбучения ON (ПланПриема.IDФормаОбучения = ФормаОбучения.IDФормаОбучения)  WHERE Специальность.Наименование LIKE N'{((ComboBox)sender).SelectedItem}'";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql1, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                addEditFormobushenie.Items.Clear();
                while (reader.Read())
                {
                    addEditFormobushenie.Items.Add(reader[0]);
                }
                reader.Close();
                connection.Close();
                addEditFormobushenie.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddEditFormobushenie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addEditFormobushenie.SelectedItem == null) return;
            try
            {
                string sql1 = $"SELECT DISTINCT Финансирование.Наименование FROM ПланПриема JOIN Специальность ON(ПланПриема.IDСпециальности = Специальность.IDСпециальность) JOIN ФормаОбучения ON (ПланПриема.IDФормаОбучения = ФормаОбучения.IDФормаОбучения) JOIN Финансирование ON (ПланПриема.IDФинансирования = Финансирование.IDФинансирования) WHERE Специальность.Наименование LIKE N'{addEditFormspecialnost.SelectedItem}' AND ФормаОбучения.Наименование LIKE N'{addEditFormobushenie.SelectedItem}'";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql1, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                addEditFormFinansirovanie.Items.Clear();
                while (reader.Read())
                {
                    addEditFormFinansirovanie.Items.Add(reader[0]);
                }
                reader.Close();
                connection.Close();
                addEditFormFinansirovanie.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddEditFormFinansirovanie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addEditFormFinansirovanie.SelectedItem == null) return;
            try
            {
                string sql1 = $"SELECT DISTINCT ФормаОбучения.Образование FROM ПланПриема JOIN Специальность ON(ПланПриема.IDСпециальности = Специальность.IDСпециальность) JOIN ФормаОбучения ON (ПланПриема.IDФормаОбучения = ФормаОбучения.IDФормаОбучения) JOIN Финансирование ON (ПланПриема.IDФинансирования = Финансирование.IDФинансирования) WHERE Специальность.Наименование LIKE N'{addEditFormspecialnost.SelectedItem}' AND ФормаОбучения.Наименование LIKE N'{addEditFormobushenie.SelectedItem}' AND Финансирование.Наименование LIKE N'{addEditFormFinansirovanie.SelectedItem}'";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql1, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                addEditFormobrazovanie.Items.Clear();
                while (reader.Read())
                {
                    addEditFormobrazovanie.Items.Add(reader[0]);
                }
                reader.Close();
                connection.Close();
                addEditFormobrazovanie.SelectedIndex = 0;
                SetExamList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_PlanPriemaID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@speciality", addEditFormspecialnost.SelectedItem);
                command.Parameters.AddWithValue("@formOfEducation", addEditFormobushenie.SelectedItem);
                command.Parameters.AddWithValue("@financing", addEditFormFinansirovanie.SelectedItem);
                command.Parameters.AddWithValue("@education", addEditFormobrazovanie.SelectedItem);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                PlanPriemaID = Convert.ToInt32(reader[0]);
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand($"SELECT ЦТ FROM ПланПриема WHERE IDПланПриема = {PlanPriemaID}", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                TabItemSertificat.IsEnabled = Convert.ToBoolean(reader[0]);
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddEditFormobrazovanie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        #endregion
        private void MaskedTB_IsComplited(object sender, TextChangedEventArgs e)
        {
            Xceed.Wpf.Toolkit.MaskedTextBox maskedText = sender as Xceed.Wpf.Toolkit.MaskedTextBox;
            if (maskedText.IsMaskCompleted)
                maskedText.Tag = "";
            else
                maskedText.Tag = "Error";

        }
        private void TabControlAddEditForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabControlAddEditForm.SelectedItem != null && e.RemovedItems.Count != 0)
            {
                if (!(e.RemovedItems[0] is TabItem tabItem)) return;
                switch (TabControlAddEditForm.Items.IndexOf(tabItem))
                {
                    case 0:
                        if (Correct_1()) ((TabItem)TabControlAddEditForm.Items[0]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[0]).Tag = "";
                        break;

                    case 1:
                        if (Correct_2()) ((TabItem)TabControlAddEditForm.Items[1]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[1]).Tag = "";
                        break;

                    case 2:
                        if (Correct_3()) ((TabItem)TabControlAddEditForm.Items[2]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[2]).Tag = "";
                        break;

                    case 3:
                        if (Correct_4()) ((TabItem)TabControlAddEditForm.Items[3]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[3]).Tag = "";
                        break;
                }
            }
        }
        #endregion
        #endregion
        #region Настройка контрольных цифр приема
        private void TabItem1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PlanPriemaTableLoad(((TabItem)sender).Header.ToString());
            datagridPlanPriemaAdd.Visibility = Visibility.Hidden;
        }

        private void Button_AddPlanPriema(object sender, RoutedEventArgs e)
        {
            datagridPlanPriemaAdd.Visibility = Visibility.Visible;
            ClearData<Grid>(datagridPlanPriemaAdd);
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand comand = new SqlCommand("SELECT Наименование FROM Специальность", connection);
                SqlDataReader reader = comand.ExecuteReader();
                planPriemaADD_Spec.Items.Clear();
                while (reader.Read())
                    planPriemaADD_Spec.Items.Add(reader[0]);
                planPriemaADD_Spec.SelectedIndex = 0;
                reader.Close();

                comand = new SqlCommand("SELECT Наименование, Образование FROM ФормаОбучения", connection);
                reader = comand.ExecuteReader();
                List<string[]> formiObusheniya = new List<string[]>();
                planPriemaADD_ForaObucheniya.Items.Clear();
                while (reader.Read())
                {
                    string[] form = new string[2];
                    form[0] = reader.GetString(0);
                    form[1] = reader.GetString(1);

                    if (!planPriemaADD_ForaObucheniya.Items.Contains(reader[0]))
                    {
                        planPriemaADD_ForaObucheniya.Items.Add(reader[0]);
                    }
                    formiObusheniya.Add(form);
                }
                planPriemaADD_ForaObucheniya.Tag = formiObusheniya;
                planPriemaADD_ForaObucheniya.SelectedIndex = 0;
                reader.Close();

                comand = new SqlCommand("SELECT Наименование FROM Финансирование", connection);
                reader = comand.ExecuteReader();
                planPriemaADD_Finanse.Items.Clear();
                while (reader.Read())
                    planPriemaADD_Finanse.Items.Add(reader[0]);
                planPriemaADD_Finanse.SelectedIndex = 0;
                reader.Close();

                PlanPriemaADD_ForaObucheniya_SelectionChanged(planPriemaADD_ForaObucheniya, null);
                planPriemaADD_Spec.SelectedItem = ((TabItem)TabControl1.SelectedItem).Header;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            planPrieaADD_kolvoCelevihMest.Text = "0";
            planPrieaADD_kolvoMest.Text = "0";
            buttonAdd.Visibility = Visibility.Visible;
            buttonEdit.Visibility = Visibility.Collapsed;
        }

        private void PlanPriemaADD_ForaObucheniya_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (planPriemaADD_ForaObucheniya.Tag == null || planPriemaADD_ForaObucheniya.SelectedValue == null) return;
            string forma = planPriemaADD_ForaObucheniya.SelectedValue.ToString();
            List<string[]> formiObusheniya = (List<string[]>)planPriemaADD_ForaObucheniya.Tag;
            planPriemaADD_Obrazovanie.Items.Clear();
            for (int i = 0; i < formiObusheniya.Count; i++)
            {
                if (formiObusheniya[i][0] == forma)
                    planPriemaADD_Obrazovanie.Items.Add(formiObusheniya[i][1]);
            }
            planPriemaADD_Obrazovanie.SelectedIndex = 0;
        }
        private void PlanPriemaADD_Finanse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((string)planPriemaADD_Finanse.SelectedItem == "Хозрасчет")
            {
                planPrieaADD_kolvoCelevihMest.Tag = planPrieaADD_kolvoCelevihMest.Text;
                planPrieaADD_kolvoCelevihMest.Text = "0";
                planPrieaADD_kolvoCelevihMest.IsEnabled = false;
            }
            else
            {
                if (planPrieaADD_kolvoCelevihMest.Tag != null)
                {
                    planPrieaADD_kolvoCelevihMest.Text = planPrieaADD_kolvoCelevihMest.Tag.ToString();
                }
                planPrieaADD_kolvoCelevihMest.IsEnabled = true;
            }
        }

        private void Button_Add(object sender, RoutedEventArgs e)
        {
            if (planPrieaADD_kod.Text == "" || planPrieaADD_kod.Text.Length > 13)
            {
                planPrieaADD_kod.Tag = "Error";
                return;
            }
            if (planPrieaADD_kolvoMest.Text == "")
            {
                planPrieaADD_kolvoMest.Tag = "Error";
                return;
            }
            if ( Convert.ToInt32(planPrieaADD_kolvoCelevihMest.Text) > Convert.ToInt32(planPrieaADD_kolvoMest.Text))
            {
                planPrieaADD_kolvoCelevihMest.Tag = "Error";
                return;
            }
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Add_PlanPriema", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@year", DateTime.Now.Year);
                command.Parameters.AddWithValue("@spec", planPriemaADD_Spec.SelectedItem);
                command.Parameters.AddWithValue("@form", planPriemaADD_ForaObucheniya.SelectedItem);
                command.Parameters.AddWithValue("@fin", planPriemaADD_Finanse.SelectedItem);
                command.Parameters.AddWithValue("@obr", planPriemaADD_Obrazovanie.SelectedItem);
                command.Parameters.AddWithValue("@kolva", planPrieaADD_kolvoMest.Text);
                command.Parameters.AddWithValue("@kolvaCel", planPrieaADD_kolvoCelevihMest.Text);
                command.Parameters.AddWithValue("@CT", planPrieaADD_CT.IsChecked);
                command.Parameters.AddWithValue("@Code", planPrieaADD_kod.Text);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                datagridPlanPriemaAdd.Visibility = Visibility.Hidden;
                PlanPriemaTableLoad(((TabItem)TabControl1.SelectedItem).Header.ToString());
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Edit(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Update_PlanPriema", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", ((PlanPriema)buttonEdit.Tag).Id);
                command.Parameters.AddWithValue("@spec", planPriemaADD_Spec.SelectedItem);
                command.Parameters.AddWithValue("@form", planPriemaADD_ForaObucheniya.SelectedItem);
                command.Parameters.AddWithValue("@fin", planPriemaADD_Finanse.SelectedItem);
                command.Parameters.AddWithValue("@obr", planPriemaADD_Obrazovanie.SelectedItem);
                command.Parameters.AddWithValue("@kolva", planPrieaADD_kolvoMest.Text);
                command.Parameters.AddWithValue("@kolvaCel", planPrieaADD_kolvoCelevihMest.Text);
                command.Parameters.AddWithValue("@CT", planPrieaADD_CT.IsChecked);
                command.Parameters.AddWithValue("@Code", planPrieaADD_kod.Text);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                datagridPlanPriemaAdd.Visibility = Visibility.Hidden;
                PlanPriemaTableLoad(((TabItem)TabControl1.SelectedItem).Header.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PlanPrieaADD_kolvoCelevihMest_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
                textBox.Text = "0";
        }

        private void ImagecCick_UpdatePlanPriema(object sender, MouseButtonEventArgs e)
        {
            datagridPlanPriemaAdd.Visibility = Visibility.Visible;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand comand = new SqlCommand("SELECT Наименование FROM Специальность", connection);
                SqlDataReader reader = comand.ExecuteReader();
                planPriemaADD_Spec.Items.Clear();
                while (reader.Read())
                    planPriemaADD_Spec.Items.Add(reader[0]);
                reader.Close();

                comand = new SqlCommand("SELECT Наименование, Образование FROM ФормаОбучения", connection);
                reader = comand.ExecuteReader();
                List<string[]> formiObusheniya = new List<string[]>();
                planPriemaADD_ForaObucheniya.Items.Clear();
                while (reader.Read())
                {
                    string[] form = new string[2];
                    form[0] = reader.GetString(0);
                    form[1] = reader.GetString(1);

                    if (!planPriemaADD_ForaObucheniya.Items.Contains(reader[0]))
                    {
                        planPriemaADD_ForaObucheniya.Items.Add(reader[0]);
                    }
                    formiObusheniya.Add(form);
                }
                planPriemaADD_ForaObucheniya.Tag = formiObusheniya;
                reader.Close();

                comand = new SqlCommand("SELECT Наименование FROM Финансирование", connection);
                reader = comand.ExecuteReader();
                planPriemaADD_Finanse.Items.Clear();
                while (reader.Read())
                    planPriemaADD_Finanse.Items.Add(reader[0]);
                reader.Close();

                PlanPriemaADD_ForaObucheniya_SelectionChanged(planPriemaADD_ForaObucheniya, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            PlanPriema planPriema = (PlanPriema)dataGridPlani.SelectedItem;
            planPrieaADD_kod.Text = planPriema.CodeSpec;
            planPriemaADD_Spec.SelectedItem = planPriema.NameSpec;
            planPriemaADD_ForaObucheniya.SelectedItem = planPriema.NameForm;
            planPriemaADD_Finanse.SelectedItem = planPriema.NameFinance;
            planPriemaADD_Obrazovanie.SelectedItem = planPriema.NameObrazovaie;
            planPrieaADD_kolvoCelevihMest.Text = planPriema.CountCelevihMest.ToString();
            planPrieaADD_kolvoMest.Text = planPriema.Count.ToString();
            planPrieaADD_CT.IsChecked = planPriema.Ct;
            buttonEdit.Tag = planPriema;
            buttonAdd.Visibility = Visibility.Collapsed;
            buttonEdit.Visibility = Visibility.Visible;
        }

        private void ImagecCick_DeletePlanPriema(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Удалить план приема?", "Удаление", MessageBoxButton.OKCancel);
            if(result == MessageBoxResult.OK)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM Абитуриент WHERE IDПланаПриема = {((PlanPriema)dataGridPlani.SelectedItem).Id}", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if (reader.GetInt32(0) > 0)
                    {
                        if (MessageBox.Show("В плане приема есть записи о абитуриентах!\nПродолжить?", "Удаление", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }
                    reader.Close();
                    command = new SqlCommand($"DELETE FROM ПланПриема WHERE IDПланПриема ={((PlanPriema)dataGridPlani.SelectedItem).Id}", connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                    PlanPriemaTableLoad(((TabItem)TabControl1.SelectedItem).Header.ToString());
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void DataGridPlani_OpenPlanPriema(object sender, MouseButtonEventArgs e)
        {
            if (dataGridPlani.SelectedItem == null) return;
            TabControl.SelectedIndex = TabControl1.SelectedIndex;
            AbiturientTableLoad(((PlanPriema)dataGridPlani.SelectedItem).Id);
            PlanPriemaTable.Visibility = Visibility.Visible; 
            tabWork.SelectedIndex = 0;
        }
        #endregion Настройка контрольных цифр приема
        #region Статистика подача документов
        private void OpenStats(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (tabWork.SelectedIndex == 2)
            {
                StatsLoad(((TabItem)TabControl2.SelectedItem).Header.ToString());
            }
        }
        private void TabItem2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StatsLoad(((TabItem)sender).Header.ToString());
        }

        private void TbMaskFloat(object sender, TextCompositionEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            string text = textbox.Text;
            text = text.Insert(textbox.SelectionStart, e.Text);
            e.Handled = !Double.TryParse(text, out _);
        }


        private void TbMaskFloat_TextInput(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            while (textbox.Text.Length > 1 && textbox.Text[0] == '0' && textbox.Text[1] != ',')
            {
                textbox.Text = textbox.Text.Substring(1);
            }
            if (textbox.Text != "" && textbox.Text[textbox.Text.Length - 1] == ',')
            {
                textbox.Text += "0";
                textbox.SelectionStart = textbox.Text.Length - 1;
                textbox.SelectionLength = 1;
            }
            if (textbox.Text != "" && textbox.Text[0] == ',')
            {
                textbox.Text = "0" + textbox.Text;
                textbox.SelectionStart = 0;
                textbox.SelectionLength = 1;
            }
            if(textbox.Text == "")
            {
                textbox.Text = "0";
                textbox.SelectionStart = 0;
                textbox.SelectionLength = 1;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            double startValue = Convert.ToDouble(tbStartValue.Text);
            double step = Convert.ToDouble(tbStep.Text);

            bool correctValue = true;
            if(startValue < 0 || startValue > 10)
            {
                tbStartValue.Tag = "Error";
                correctValue = false;
            }
            if(step <= 0 || step >= 10)
            {
                tbStep.Tag = "Error";
                correctValue = false;
            }
            if(correctValue == false)
            {
                return;
            }

            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = System.IO.Path.GetDirectoryName(location);
            string fileName = "/Шаблон для сайта.xlsx";
            if (!File.Exists(path + fileName))
            {
                MessageBox.Show("Не удалось найти или открыть файл шаблона!");
                return;
            }
            Excel.Application ex = new Microsoft.Office.Interop.Excel.Application();
            ex.Visible = true;
            ex.SheetsInNewWorkbook = 1;
            ex.Interactive = false;
            Excel.Workbook workBook = ex.Workbooks.Open(path + fileName);
            ex.DisplayAlerts = false;
            Excel.Worksheet sheet = (Excel.Worksheet)ex.Worksheets.get_Item(1);

            int cellIndex = 7;
            ex.Cells[4, cellIndex] = Math.Round(startValue, 1) + " - " + Math.Round(startValue + step, 1);
            for(double i = startValue + step; i < 9.9; i += step)
            {
                cellIndex++;
                double a = Math.Round(i + 0.1, 1);
                double b = Math.Round(i + step, 1);
                if(b > 10)
                {
                    b = 10;
                }    
                ex.Cells[4, cellIndex] = a + "- " + b;
            }

            List<DocSubmissionStat> stats = (List<DocSubmissionStat>)DGStats.ItemsSource;
            for(int i = 5; i < stats.Count + 5; i++)
            {
                ex.Cells[i, 1] = stats[i - 5].TotalToAdmissionPlan;
                ex.Cells[i, 2] = stats[i - 5].AdmissionPlanDogovor;
                ex.Cells[i, 3] = stats[i - 5].AdmissionPlanPayers;
                ex.Cells[i, 4] = stats[i - 5].TotalToEntrant;
                ex.Cells[i, 5] = stats[i - 5].EntrantDogovor;
                ex.Cells[i, 6] = stats[i - 5].EntrantOutOfCompetition;

                SqlConnection connection = new SqlConnection(connectionString);
                try
                {
                    connection.Open();
                    for (int j = 7; j <= cellIndex; j++)
                    {
                        SqlCommand command = new SqlCommand("GetAbiturientCountForStats",connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IDPlanPriema", stats[i-5].IDAdmissionPlan);
                        string temp = ex.Cells[4, j].Value;
                        string[] headerValue = temp.Split('-');
                        double from = Convert.ToDouble(headerValue[0]);
                        double to = Convert.ToDouble(headerValue[1]);
                        command.Parameters.AddWithValue("@minMark",from);
                        command.Parameters.AddWithValue("@maxMark",to);
                        SqlDataReader reader = command.ExecuteReader();
                        int abiturientCount = 0;
                        while (reader.Read())
                        {
                            abiturientCount++;
                        }
                        ex.Cells[i, j] = abiturientCount;
                        reader.Close();
                    }
                }
                catch(Exception except)
                {
                    ex.Interactive = true;
                    workBook.Close();
                    MessageBox.Show(except.Message);
                    return;
                }
                finally
                {
                    connection.Close();
                }
            }
            ex.Range[ex.Cells[1, 4], ex.Cells[1, cellIndex]].Merge();
            ex.Range[ex.Cells[2, 5], ex.Cells[2, cellIndex]].Merge();
            ex.Range[ex.Cells[3, 7], ex.Cells[3, cellIndex]].Merge();
            ex.Range[ex.Cells[1,1], ex.Cells[4 + stats.Count, cellIndex]].Cells.Borders.Weight = Excel.XlBorderWeight.xlThin;
            ex.Interactive = true;

        }

        private void DataGridStats_OpenPlanPriema(object sender, MouseButtonEventArgs e)
        {
            if (DGStats.SelectedItem == null) return;
            TabControl.SelectedIndex = TabControl2.SelectedIndex;
            AbiturientTableLoad(((DocSubmissionStat)DGStats.SelectedItem).IDAdmissionPlan);
            PlanPriemaTable.Visibility = Visibility.Visible;
            tabWork.SelectedIndex = 0;
        }
        #endregion

        #region Методы
        private void StatsLoad(string specialnost)
        {
            List<DocSubmissionStat> list = new List<DocSubmissionStat>();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("GetStats", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@spec", specialnost);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DocSubmissionStat stat = new DocSubmissionStat(7);
                    stat.IDAdmissionPlan = reader.GetInt32(0);
                    stat.TotalToAdmissionPlan = reader.GetInt32(1);
                    stat.AdmissionPlanDogovor = reader.GetInt32(2);
                    stat.AdmissionPlanPayers = reader.GetInt32(3);
                    stat.TotalToEntrant = reader.GetInt32(4);
                    stat.EntrantDogovor = reader.GetInt32(5);
                    stat.EntrantOutOfCompetition = reader.GetInt32(6);

                    list.Add(stat);
                }
                reader.Close();
                for (int j = 0; j < list.Count; j++)
                {
                    double step = 1;
                    double startValue = 3;
                    int mark = 0;
                    for (double i = startValue; i <= 10 - step; i += step)
                    {
                        SqlCommand command1 = new SqlCommand("GetAbiturientCountForStats", connection);
                        command1.CommandType = CommandType.StoredProcedure;
                        command1.Parameters.AddWithValue("@IDPlanPriema", list[j].IDAdmissionPlan);
                        double from = i + (mark == 0 ? 0 : 0.1);
                        double to = i + step;
                        command1.Parameters.AddWithValue("@minMark", from);
                        command1.Parameters.AddWithValue("@maxMark", to);
                        SqlDataReader reader1 = command1.ExecuteReader();
                        list[j].Marks[mark] = 0;
                        while (reader1.Read()) { list[j].Marks[mark]++; }
                        reader1.Close();
                        mark++;
                    }
                }
                DGStats.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PlanPriemaTableLoad(string specialnost)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("Get_PlanPrieaBySpeciality", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@spec", specialnost);
                SqlDataReader reader = command.ExecuteReader();
                planPriemaDGsource.Clear();
                
                while (reader.Read())
                {
                    planPriemaDGsource.Add(new PlanPriema
                    {
                        Id = reader.GetInt32(0),
                        IdSpec = reader.GetInt32(1),
                        IdForm = reader.GetInt32(2),
                        IdFinance = reader.GetInt32(3),
                        Count = reader.GetInt32(4),
                        CountCelevihMest = reader.GetInt32(5),
                        Year = reader.GetString(6),
                        CodeSpec = reader.GetString(7),
                        NameSpec = reader.GetString(8),
                        NameForm = reader.GetString(9),
                        NameObrazovaie = reader.GetString(10),
                        NameFinance = reader.GetString(11),
                        Ct = reader.GetBoolean(12)
                    });
                }
                dataGridPlani.ItemsSource = null;
                dataGridPlani.ItemsSource = planPriemaDGsource;
                PlaniCountWrite.Text = dataGridPlani.Items.Count.ToString();
            }
            catch
            {

            }
        }
        private void PlaniPriemaLoad(string specialost)
        {
            foreach (Canvas canvas in planPriemaButtons)
            {
                canvas.Visibility = Visibility.Hidden;
            }

            int buttInd = 0;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_PlaniPriema", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@specialost", specialost);
                command.Parameters.AddWithValue("@budjet", CBFinBudjet.IsChecked == true ? "Бюджет" : "");
                command.Parameters.AddWithValue("@hozrash", CBFinHozrach.IsChecked == true ? "Хозрасчет" : "");
                command.Parameters.AddWithValue("@bazovoe", CBObrBaz.IsChecked == true ? "На основе базового образования" : "");
                command.Parameters.AddWithValue("@srednee", CBObrsred.IsChecked == true ? "На основе среднего образования" : "");
                command.Parameters.AddWithValue("@dnevnaya", CBFormDnev.IsChecked == true ? "Дневная" : "");
                command.Parameters.AddWithValue("@zaochnaya", CBformZaoch.IsChecked == true ? "Заочная" : "");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Canvas canvas = planPriemaButtons[buttInd];
                    canvas.Tag = reader[5];
                    canvas.Visibility = Visibility.Visible;
                    canvas.Children[2].SetValue(TextBlock.TextProperty, reader[3].ToString().ToUpper());
                    canvas.Children[3].SetValue(TextBlock.TextProperty, reader[2].ToString().ToUpper() + ". " + reader[4]);
                    canvas.Children[5].SetValue(TextBlock.TextProperty, reader[6].ToString());
                    canvas.Children[2].SetValue(TextBlock.TagProperty, reader[3].ToString() + ". " + reader[2].ToString() + ". " + reader[4].ToString());
                    buttInd++;
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AbiturientTableLoad(int PlanPriemaID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Get_PlanPrieaByID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("id", PlanPriemaID);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                curentPlanPriema = new PlanPriema
                {
                    Id = PlanPriemaID,
                    IdSpec = reader.GetInt32(0),
                    IdForm = reader.GetInt32(1),
                    IdFinance = reader.GetInt32(2),
                    Count = reader.GetInt32(3),
                    CountCelevihMest = reader.GetInt32(4),
                    Year = reader.GetString(5),
                    CodeSpec = reader.GetString(6),
                    NameSpec = reader.GetString(7),
                    NameForm = reader.GetString(8),
                    NameObrazovaie = reader.GetString(9),
                    NameFinance = reader.GetString(10),
                    Ct = reader.GetBoolean(11)
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                connection.Close();
            }

            abiturients = new List<AbiturientDGItem>();
            try
            {
                SqlCommand command = new SqlCommand($"SELECT * FROM GetAbiturientData WHERE (SELECT IDПланаПриема FROM dbo.Абитуриент WHERE(IDАбитуриента = GetAbiturientData.IDАбитуриента)) = {PlanPriemaID}", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string lgoti = "";
                    if (Convert.ToBoolean(reader[19]) == true) { lgoti += "Cирота"; }
                    if (Convert.ToBoolean(reader[20]) == true) { lgoti += (lgoti.Length == 0 ? "" : "\n") + "Договор"; }

                    string status = "";
                    if (Convert.ToBoolean(reader[4]) == true) { status = "Зачислен"; }
                    else if (Convert.ToBoolean(reader[17]) == true) { status = "Отозвано"; }
                    else status = "Принято";

                    abiturients.Add(new AbiturientDGItem(Convert.ToInt32(reader[0]),
                        reader[1].ToString(), reader[2].ToString(),
                        Convert.ToInt32(reader[3]),
                        reader[5] == DBNull.Value ? 0 : Convert.ToInt32(reader[5]),
                        reader[6] == DBNull.Value ? 0 : Convert.ToInt32(reader[6]),
                        reader[7] == DBNull.Value ? 0 : Convert.ToInt32(reader[7]),
                        reader[8] == DBNull.Value ? 0 : Convert.ToInt32(reader[8]),
                        reader[9] == DBNull.Value ? 0 : Convert.ToInt32(reader[9]),
                        reader[10] == DBNull.Value ? 0 : Convert.ToInt32(reader[10]),
                        reader[11] == DBNull.Value ? 0 : Convert.ToInt32(reader[11]),
                        reader[12] == DBNull.Value ? 0 : Convert.ToInt32(reader[12]),
                        reader[13] == DBNull.Value ? 0 : Convert.ToInt32(reader[13]),
                        reader[14] == DBNull.Value ? 0 : Convert.ToInt32(reader[14]),
                        reader[15] == DBNull.Value ? 0 : Convert.ToDouble(reader[15]),
                        reader[16].ToString(),
                        Convert.ToBoolean(reader[17]),
                        reader[18] == DBNull.Value ? 0 : Convert.ToDouble(reader[18]),
                        lgoti, status));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка получения данных!");
            }
            finally
            {
                connection.Close();
            }
            for (int i = 0; i < abiturients.Count; i++)
            {
                List<AbiturientDGItem> list = abiturients.FindAll(x => x.ID == abiturients[i].ID);
                if (list.Count >= 2)
                {
                    abiturients.RemoveAll(x => x.ID == list[0].ID);
                    AbiturientDGItem item = list[0];
                    item.DifferentAttestat = true;
                    for (int j = 1; j < list.Count; j++)
                    {
                        item.MarkDecAvg += list[j].MarkDecAvg;
                    }
                    item.MarkDecAvg = Math.Round(item.MarkDecAvg / list.Count,2);
                    abiturients.Add(item);
                }
            }
            for (int i = 0; i < abiturients.Count; i++)
            {
                abiturients[i].Num = i + 1;

                try
                {
                    SqlCommand command = new SqlCommand("Get_StatiAbiturienta", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", abiturients[i].ID);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    string statyi = "";
                    while (reader.Read())
                    {
                        statyi += reader[0] + " ";
                    }
                    abiturients[i].Stati = statyi;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            GridCountWrite.Text = abiturients.Count.ToString();
            abiturients.Sort((x, y) => x.Num - y.Num);
            dataGridAbiturients.ItemsSource = abiturients;
            dataGridAbiturients.Tag = false;
        }

        private void AbiturientInfoShow()
        {
            if ((AbiturientDGItem)dataGridAbiturients.SelectedItem == null) return;
            GridInfo.Visibility = Visibility.Visible;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaFullInfo", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataGridAbiturients.SelectedItem).ID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();

                InfoFIO.Text = reader[0].ToString();
                infoSchool.Text = reader[1].ToString();
                infoYear.Text = reader[2].ToString();
                infoDate.Text = reader[3].ToString() != "" ? DateTime.Parse(reader[3].ToString()).ToString("D") : "-";
                infoLgoti.Text = ((AbiturientDGItem)dataGridAbiturients.SelectedItem).Lgoti.Replace('\n', ' ');
                if (infoLgoti.Text == "") infoLgotiTB.Visibility = Visibility.Collapsed; else infoLgotiTB.Visibility = Visibility.Visible;
                infoStati.Text = ((AbiturientDGItem)dataGridAbiturients.SelectedItem).Stati.Replace('\n', ' ');
                if (infoStati.Text == "") infoStatiTB.Visibility = Visibility.Collapsed; else infoStatiTB.Visibility = Visibility.Visible;
                infoDateVidoci.Text = reader[4].ToString() != "" ? DateTime.Parse(reader[4].ToString()).ToString("D") : "-";
                infoSeriya.Text = reader[5].ToString();
                infoPassNum.Text = reader[6].ToString();
                infokemvidan.Text = reader[7].ToString();
                infoIdentNum.Text = reader[8].ToString();
                infoGrajdanstvo.Text = reader[9].ToString();
                if (reader[10].ToString() == "")
                {
                    RowInfoWork.Height = new GridLength(0);
                }
                else
                {
                    infoMestoRaboti.Text = reader[10].ToString();
                    infoDoljnost.Text = reader[11].ToString();
                    RowInfoWork.Height = new GridLength(91);
                }
                infoVladelec.Text = reader[12].ToString();
                infoRedaktor.Text = reader[13].ToString();
                if (infoRedaktor.Text == "") infoRedaktorTB.Visibility = Visibility.Hidden; else infoRedaktorTB.Visibility = Visibility.Visible;
                infoDateVvoda.Text = reader[14].ToString();
                infoDateRedact.Text = reader[15].ToString();
                if (infoDateRedact.Text == "") infoDateRedactTB.Visibility = Visibility.Hidden; else infoDateRedactTB.Visibility = Visibility.Visible;
                if ((bool)reader[16]) InfoShow_Status.Text = "Зачислен"; else if ((bool)reader[17]) InfoShow_Status.Text = "Отозвано"; else InfoShow_Status.Text = "Принято";
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try//Атестаты
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaAttestat", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataGridAbiturients.SelectedItem).ID);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                dataAdapter.Fill(dataTable);
                AtestatGrid.ItemsSource = dataTable.DefaultView;
                connection.Close();
                atestatCount.Text = AtestatGrid.Items.Count.ToString();

                for (int i = 1; i < AtestatGrid.Columns.Count - 2; i++)
                {
                    bool isNull = true;
                    for (int j = 0; j < AtestatGrid.Items.Count; j++)
                    {
                        if (((DataRowView)AtestatGrid.Items[j])[i].ToString() != "")
                        {
                            isNull = false;
                        }
                    }
                    if (isNull) AtestatGrid.Columns[i].Visibility = Visibility.Hidden;
                    else AtestatGrid.Columns[i].Visibility = Visibility.Visible;
                }//скрытие неиспользуемых столбцов
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try//цт
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaSertificati", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataGridAbiturients.SelectedItem).ID);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                dataAdapter.Fill(dataTable);
                SertificatiCTGrid.ItemsSource = dataTable.DefaultView;
                connection.Close();
                sertificatCount.Text = SertificatiCTGrid.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try//Контактные данные
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaKontakti", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataGridAbiturients.SelectedItem).ID);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                dataAdapter.Fill(dataTable);
                kontaktnieDannieGrid.ItemsSource = dataTable.DefaultView;
                connection.Close();
                contactsCount.Text = kontaktnieDannieGrid.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } //открытие информации об абитуриенте

        private bool EnterIsCorrect()
        {
            //проверка заполнения паспортных данных
            bool correct = true;
            if (PassportDateVidachi.IsMaskCompleted == false)
            {
                PassportDateVidachi.Tag = "Error";
                correct = false;
            }
            TextBoxCheck(PassportSeriya, ref correct);
            TextBoxCheck(PassportNomer, ref correct);
            TextBoxCheck(PassportVidan, ref correct);
            TextBoxCheck(PassportIdentNum, ref correct);
            if (correct)
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
            }
            //проверка корректности всех вкладок
            foreach (TabItem tabItem in TabControlAddEditForm.Items)
            {
                if (tabItem.Tag.ToString() != "True")
                {
                    TabControlAddEditForm.SelectedItem = tabItem;
                    return false;
                }
            }

            for (int i = 0; i < addEdifFormAtestati.Children.Count - 1; i++)
            {
                if (addEdifFormAtestati.Children[i].Visibility == Visibility.Visible && (addEdifFormAtestati.Children[i] as StackPanel) != null)
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    StackPanel stackPanel = addEdifFormAtestati.Children[i] as StackPanel;
                    Grid grid = stackPanel.Children[4] as Grid;
                    for (int j = 4; j < 32; j += 2)
                    {
                        ((TextBox)grid.Children[j]).Tag = "";
                    }
                    bool error = false;
                    try
                    {
                        SqlCommand comm = new SqlCommand($"SELECT КоличествоБаллов FROM Шкала WHERE Наименование = '{((ComboBox)stackPanel.Children[7]).SelectedItem}'", connection);
                        SqlDataReader reader = comm.ExecuteReader();
                        reader.Read();
                        int count = Convert.ToInt16(reader[0]);
                        reader.Close();
                        for (int j = 4; j < 32; j += 2)
                        {
                            if ((((TextBox)grid.Children[j]).Text == "" && count * 2 + 4 > j) ||
                                (((TextBox)grid.Children[j]).Text != "" && count * 2 + 4 <= j))
                            {
                                error = true;
                                ((TextBox)grid.Children[j]).Tag = "Error";
                            }
                        }
                    }
                    catch { }

                    if (error)
                    {
                        TabControlAddEditForm.SelectedIndex = 2;
                        return false;
                    }
                }
            }
            return true;
            //проверка на корректность ввода оценок
        }

        private void SetExamList()
        {
            if (addEditFormobrazovanie.SelectedItem == null || EditEndButton.Visibility == Visibility.Visible) return;

            string letter;
            int num;
            string additional = "";
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sql1 = $"SELECT Буква FROM Специальность WHERE Наименование = '{addEditFormspecialnost.SelectedValue}'";
                SqlCommand command = new SqlCommand(sql1, connection);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                letter = reader[0].ToString(); ;
                reader.Close();

                command = new SqlCommand("NextExamList", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("id", PlanPriemaID);
                reader = command.ExecuteReader();
                reader.Read();
                if (reader[0] == DBNull.Value)
                    num = 1;
                else
                    num = Convert.ToInt32(reader[0]);
                reader.Close();
                connection.Close();
                if (addEditFormobushenie.SelectedValue.ToString() == "Заочная")
                    additional = "зб";
                else if (addEditFormFinansirovanie.SelectedValue.ToString() == "Хозрасчет")
                    additional = "х/р";
                else if (addEditFormobrazovanie.SelectedValue != null && addEditFormobrazovanie.SelectedValue.ToString() == "На основе среднего образования")
                    additional = "с";
                addEditFormExamList.Text = letter + num + additional;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Номер экзаменационного листа");
            }
        }

        private void ClearData<T>(T obj) where T : Panel
        {
            foreach (object control in obj.Children)
            {
                if (control.GetType() == typeof(CheckBox))
                    ((CheckBox)control).IsChecked = false;
                if (control.GetType() == typeof(TextBox))
                {
                    ((TextBox)control).Text = default;
                    ((TextBox)control).Tag = default;
                }
                if (control.GetType() == typeof(Xceed.Wpf.Toolkit.MaskedTextBox))
                    ((Xceed.Wpf.Toolkit.MaskedTextBox)control).Text = String.Empty;
                if (control.GetType() == typeof(ComboBox))
                    ((ComboBox)control).SelectedIndex = 0;
                if (control.GetType() == typeof(StackPanel))
                {
                    if (((StackPanel)control).Tag != null && ((StackPanel)control).Tag.ToString() == "HIddenField")
                        ((StackPanel)control).Visibility = Visibility.Collapsed;
                    ClearData<StackPanel>((StackPanel)control);
                }
                if (control.GetType() == typeof(Grid))
                {
                    ClearData<Grid>((Grid)control);
                }
            }
        } //очистка текстовых полей чекбоксов и тд

        private bool Correct_1()
        {
            bool correct = true;
            TextBoxCheck(addEditFormSurename, ref correct);
            TextBoxCheck(addEditFormName, ref correct);
            TextBoxCheck(addEditFormOtchestvo, ref correct);
            TextBoxCheck(AddFormGrajdanstvo, ref correct);
            TextBoxCheck(addEditFormShool, ref correct);
            if (!addEditFormGraduationYear.IsMaskCompleted)
            {
                correct = false;
                addEditFormGraduationYear.Tag = "Error";
            }
            if (textBoxWorkPlace.Text != "" && textBoxDoljnost.Text == "")
            {
                correct = false;
                textBoxDoljnost.Tag = "Error";
            }
            if (textBoxWorkPlace.Text == "" && textBoxDoljnost.Text != "")
            {
                correct = false;
                textBoxWorkPlace.Tag = "Error";
            }
            return correct;
        }
        private bool Correct_2()
        {
            bool correct = true;
            for (int i = 0; i < AddEditFormContacts.Children.Count; i++)
            {
                if (AddEditFormContacts.Children[i].Visibility == Visibility.Visible)
                {
                    if (!(AddEditFormContacts.Children[i] is StackPanel stackPanel)) break;
                    if (((Xceed.Wpf.Toolkit.MaskedTextBox)stackPanel.Children[5]).IsMaskCompleted == false || ((Xceed.Wpf.Toolkit.MaskedTextBox)stackPanel.Children[5]).Text == "")
                    {
                        correct = false;
                        ((Xceed.Wpf.Toolkit.MaskedTextBox)stackPanel.Children[5]).Tag = "Error";
                    }
                    else
                        ((Xceed.Wpf.Toolkit.MaskedTextBox)stackPanel.Children[5]).Tag = "";
                }
            }
            return correct;
        }
        private bool Correct_3()
        {
            bool correct = true;
            for (int i = 0; i < addEdifFormAtestati.Children.Count; i++)
            {
                if (addEdifFormAtestati.Children[i].Visibility == Visibility.Visible)
                {
                    if (!(addEdifFormAtestati.Children[i] is StackPanel stackPanel)) break;
                    if (((TextBox)stackPanel.Children[3]).Text == "")
                    {
                        correct = false;
                        ((TextBox)stackPanel.Children[3]).Tag = "Error";
                    }
                    else
                        ((TextBox)stackPanel.Children[3]).Tag = "";
                }
            }
            return correct;
        }
        private bool Correct_4()
        {
            bool correct = true;
            for (int i = 0; i < addEdifFormCT.Children.Count; i++)
            {
                if (addEdifFormCT.Children[i].Visibility == Visibility.Visible)
                {
                    if (addEdifFormCT.Children[i] as StackPanel == null) break;
                    Grid grid = ((StackPanel)addEdifFormCT.Children[i]).Children[2] as Grid;
                    TextBoxCheck((TextBox)grid.Children[1], ref correct);
                    TextBoxCheck((TextBox)grid.Children[7], ref correct);
                    if (((Xceed.Wpf.Toolkit.MaskedTextBox)grid.Children[3]).IsMaskCompleted == false)
                    {
                        ((Xceed.Wpf.Toolkit.MaskedTextBox)grid.Children[3]).Tag = "Error";
                        correct = false;
                    }
                    else
                        ((Xceed.Wpf.Toolkit.MaskedTextBox)grid.Children[3]).Tag = "";
                }
            }
            return correct;
        }
        private void TextBoxCheck(TextBox textBox, ref bool correct)
        {
            if (textBox.Text == "")
            {
                textBox.Tag = "Error";
                correct = false;
            }
            else
            {
                textBox.Tag = "";
            }
        }

        private void ButtonPos(int col) //изменение позиций кнопок под размер экрана
        {
            if (planPriemaColumn == col) return;
            int buttons = 0;
            int row = 1;
            while (buttons < planPriemaButtons.Count)
            {
                for (int i = 1; i <= col && buttons < planPriemaButtons.Count; i++)
                {
                    planPriemaButtons[buttons].SetValue(Grid.RowProperty, row);
                    planPriemaButtons[buttons].SetValue(Grid.ColumnProperty, i);
                    buttons++;
                }
                row++;
            }
            planPriemaColumn = col;
        }
        #endregion
    }
}