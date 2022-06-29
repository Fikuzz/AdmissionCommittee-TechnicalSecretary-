using PriyemnayaKomissiya_TechnicalSecretary_.Controls;
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using Excel = Microsoft.Office.Interop.Excel;


namespace PriyemnayaKomissiya_TechnicalSecretary_.View
{
    /// <summary>
    /// Логика взаимодействия для основного рабочего окна
    /// </summary>
    public partial class MainWorkingWindow : Window
    {
        private readonly string connectionString;
        /// <summary>
        /// Количество столбцов для кнопок плана приема
        /// (для позиционирования кнопок под размер окна)
        /// </summary>
        private int planPriemaColumn = 0;
        /// <summary>
        /// Текущий используемый план приема
        /// </summary>
        private PlanPriema curentPlanPriema = null;
        /// <summary>
        /// ИД редактируемого абитуриента
        /// </summary>
        private int AbiturientID = 0;
        /// <summary>
        /// Пользователь под которым осуществлен вход
        /// </summary>
        private readonly User user = new User();
        /// <summary>
        /// Список абтуриентов для таблицы
        /// </summary>
        private List<AbiturientDGItem> abiturients;
        /// <summary>
        /// Список кнопок плана приема
        /// </summary>
        private readonly List<Button> planPriemaButtons = new List<Button>();
        /// <summary>
        /// Список планов приема для таблицы
        /// </summary>
        private readonly List<PlanPriema> planPriemaDGsource = new List<PlanPriema>();

        #region Общее
        /// <summary>
        /// Конструктор для основной рабочей формы
        /// </summary>
        /// <param name="id">Ид пользователя</param>
        /// <param name="login">Логин пользователя</param>
        /// <param name="name">Имя пользователя</param>
        public MainWorkingWindow(int id, string login, string name)
        {
            InitializeComponent();
            user.ID = id;
            user.Login = login;
            user.Name = name;
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            lUser_FIO.Text = user.Name;

            ucSpeciality.EndEdit += Speciality_EndEdit;
            ucArticles.BlockCheckBox += BlockCheckBox;
        }
        /// <summary>
        /// Выход из формы
        /// </summary>
        private void TextBlock_Exit(object sender, MouseButtonEventArgs e)
        {
            Autorization authorization = new Autorization();
            authorization.tbLogin.Text = user.Login;
            authorization.Show();
            authorization.tbPassword.Focus();
            this.Close();
        }
        /// <summary>
        /// Завершение загрузки формы
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddEditFormContacts.Tag = 0;
            addEdifFormAtestati.Tag = 0;
            addEdifFormCT.Tag = 0;
            var date = new StringBuilder(DateTime.Now.ToString("dddd, d MMMM"));
            date[0] = char.ToUpper(date[0]);
            lDate.Content = date.ToString();
            lbPlanPriemaYear.Content = "ПЛАН ПРИЁМА " + DateTime.Now.Year;
            UpdateSpeciality();
            //Заполнение специальностей
        }
        /// <summary>
        /// Обновление вкладок специальностей
        /// </summary>
        private void UpdateSpeciality()
        {
            List<string[]> specialty = DB.Get_SpecialnostiName(false);
            TabControl.Items.Clear();
            TabControl1.Items.Clear();
            TabControl2.Items.Clear();
            foreach (string[] names in specialty)
            {
                {
                    TabItem tabItem = new TabItem
                    {
                        Style = (Style)FindResource("TabItemStyle"),
                        Header = names[0],
                        Tag = names[1]
                    };
                    tabItem.PreviewMouseDown += new MouseButtonEventHandler(TabItem_MouseDown);
                    TabControl.Items.Add(tabItem);

                    TabItem tabItem1 = new TabItem
                    {
                        Style = (Style)FindResource("TabItemStyle"),
                        Header = names[0],
                        Tag = names[1]
                    };
                    tabItem1.PreviewMouseDown += new MouseButtonEventHandler(TabItem1_MouseDown);
                    TabControl1.Items.Add(tabItem1);

                    TabItem tabItem2 = new TabItem
                    {
                        Style = (Style)FindResource("TabItemStyle"),
                        Header = names[0],
                        Tag = names[1]
                    };
                    tabItem2.PreviewMouseDown += new MouseButtonEventHandler(TabItem2_MouseDown);
                    TabControl2.Items.Add(tabItem2);
                }
                TabControl.SelectedItem = TabControl.Items[0];
                PlaniPriemaLoad(((TabItem)TabControl.SelectedItem).Header.ToString());
            }
        }
        /// <summary>
        /// Обновление данных в таблице абитуриентов прие переходе на форму статистики подачи документов
        /// </summary>
        private void TabItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(tabWork.SelectedIndex == 1 &&  TabControl1.SelectedItem != null)
                PlanPriemaTableLoad(((TabItem)TabControl1.SelectedItem).Header.ToString());
        }
        #endregion

        #region Работа со сводными ведомостями
        /// <summary>
        /// Выбор специальности на форме  работы со сводными ведомостями
        /// </summary>
        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PlaniPriemaLoad(((TabItem)sender).Header.ToString());
            PlanPriemaTable.Visibility = Visibility.Hidden;
            GridInfo.Visibility = Visibility.Hidden;
            addEditForm.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// обновление данных при переходе на вкладку Работа со сводными ведомостями
        /// </summary>
        private void TabItem_IsVisibleChangedVedomosti(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (tabWork.SelectedIndex == 0 && TabControl.SelectedItem != null)
                PlaniPriemaLoad(((TabItem)TabControl.SelectedItem).Header.ToString());
        }
            #region Выбор плана приема
        /// <summary>
        /// Обработчик нахотия на кнопку плана приема
        /// </summary>
        private void PlanPriema_MouseDown(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            curentPlanPriema = (PlanPriema)button.Tag;
            PlanPriemaTable.Visibility = Visibility.Visible;
            LabelFormaObrazovaniya.Text = curentPlanPriema.NameForm + ". " + curentPlanPriema.NameFinance + ".\n" + curentPlanPriema.NameObrazovaie;
            AbiturientTableLoad(curentPlanPriema.Id);
            filterCB.SelectedIndex = 0;
        }
        /// <summary>
        /// изменение фильтра
        /// </summary>
        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            PlaniPriemaLoad(((TabItem)TabControl.SelectedItem).Header.ToString());
        }
        /// <summary>
        /// обработчик изменения размера окна для изменения позиций кнопок плана приема
        /// </summary>
        private void MainWorkingWindowForm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                if (System.Windows.SystemParameters.PrimaryScreenWidth < 1300)
                {
                    ButtonPos(2);
                }
                else if (System.Windows.SystemParameters.PrimaryScreenWidth < 1600)
                {
                    ButtonPos(3);
                }
                else
                {
                    ButtonPos(4);
                }
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
            #region Таблица абитуриентов
        /// <summary>
        /// Выдача документов абитуриенту
        /// </summary>
        private void Abiturient_IssueDocuments(object sender, RoutedEventArgs e)
        {
            if ((AbiturientDGItem)dataGridAbiturients.SelectedItem == null) return;
            if (MessageBox.Show($"Отметить запись '{((AbiturientDGItem)dataGridAbiturients.SelectedItem).FIO}' как документы выданы?", "Выдать документы", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Del_AbiturientMarks", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataGridAbiturients.SelectedItem).ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    AbiturientTableLoad(curentPlanPriema.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Выдача документов");
                }
            }
        }
        /// <summary>
        /// Открытие формы просмотра информации об абитуриенте
        /// </summary>
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AbiturientInfoShow();
        }
        /// <summary>
        /// Открытие формы для Ррдактирования данных об абитуриенте
        /// </summary>
        private void Image_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            ScrollAddMain.ScrollToHome();
            PlanPriema temp = curentPlanPriema.Clone();

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
                PLib.ClearData<StackPanel>(AddEditMainData);
                PLib.ClearData<StackPanel>(AddEditFormContacts);
                PLib.ClearData<StackPanel>(addEdifFormAtestati);
                PLib.ClearData<StackPanel>(addEdifFormCT);
                PLib.ClearData<StackPanel>(AddEditFormPassport);

                List<string[]> spec = DB.Get_SpecialnostiName(true);
                addEditFormspecialnost.SelectedItem = 0;
                addEditFormspecialnost.Items.Clear();
                foreach (string[] names in spec)
                {
                    addEditFormspecialnost.Items.Add(names[1]);
                }
                addEditFormspecialnost.SelectedItem = temp.NameSpec;
                addEditFormobushenie.SelectedItem = temp.NameForm;
                addEditFormFinansirovanie.SelectedItem = temp.NameFinance;
                addEditFormobrazovanie.SelectedItem = temp.NameObrazovaie;
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
                    foreach (CheckBox checkBox in ucArticles.checkBoxes)
                    {
                        SqlCommand command1 = new SqlCommand("HasStatya", con);
                        command1.CommandType = CommandType.StoredProcedure;
                        command1.Parameters.AddWithValue("@abiturient", abiturient.ID);
                        command1.Parameters.AddWithValue("@statya", checkBox.Content);
                        SqlDataReader reader1 = command1.ExecuteReader();
                        checkBox.IsChecked = reader1.HasRows;
                        reader1.Close();
                    }
                    con.Close();
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message, "Заполнение основных и паспортных данных"); 
                }//основные данные и паспортные данные
                try
                {
                    AddEditFormContacts.Children.RemoveRange(0, (int)AddEditFormContacts.Tag);
                    AddEditFormContacts.Tag = 0;
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand("Get_AbiturientaKontakti", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@abiturient", abiturient.ID);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Visibility btClose = (int)AddEditFormContacts.Tag == 0 ? Visibility.Hidden : Visibility.Visible;
                        ContactData contact = new ContactData(btClose, (int)AddEditFormContacts.Tag + 1);
                        AddEditFormContacts.Children.Insert((int)AddEditFormContacts.Tag, contact);
                        AddEditFormContacts.Tag = (int)AddEditFormContacts.Tag + 1;

                        contact.cbContactType.SelectedItem = reader.GetString(2);
                        contact.mtbData.Text = reader.GetString(3);
                        break;
                    }
                    if ((int)AddEditFormContacts.Tag == 0)
                    {
                        ContactData contact = new ContactData(Visibility.Hidden, 1);
                        AddEditFormContacts.Children.Insert(0, contact);
                        AddEditFormContacts.Tag = 1;
                    }
                    connection.Close();
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message, "Заполнение контактных данных"); 
                }//контактные данные
                try
                {
                    addEdifFormAtestati.Children.RemoveRange(0, (int)addEdifFormAtestati.Tag);
                    addEdifFormAtestati.Tag = 0;
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand("Get_AbiturientaAttestat", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@abiturient", abiturient.ID);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Visibility btClose = (int)addEdifFormAtestati.Tag == 0 ? Visibility.Hidden : Visibility.Visible;
                        Certificate certificate = new Certificate(btClose, (int)addEdifFormAtestati.Tag + 1);
                        addEdifFormAtestati.Children.Insert((int)addEdifFormAtestati.Tag, certificate);
                        addEdifFormAtestati.Tag = (int)addEdifFormAtestati.Tag + 1;

                        certificate.tbSeries.Text = reader.GetString(reader.GetOrdinal("Num"));
                        string scaleName = reader.GetString(reader.GetOrdinal("Scale")); ;
                        foreach (ComboBoxItem item in certificate.cbScaleType.Items)
                        {
                            if(item.Content.ToString() == scaleName)
                            {
                                certificate.cbScaleType.SelectedItem = item;
                                break;
                            }
                        }
                        for (int i = 0; i < certificate.Marks.Count; i++)
                        {
                            if (reader[reader.GetOrdinal("n" + (i + 1))] == DBNull.Value)
                                break;
                            certificate.Marks[i].Text = reader.GetInt32(reader.GetOrdinal("n" + (i + 1))).ToString();
                        }
                    }
                    reader.Close();
                    if ((int)addEdifFormAtestati.Tag == 0)
                    {
                        Certificate certificate = new Certificate(Visibility.Hidden, 1);
                        addEdifFormAtestati.Children.Insert(0, certificate);
                        addEdifFormAtestati.Tag = 1;
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Заполнение аттестатов");
                }//Аттестаты
                try
                {
                    addEdifFormCT.Children.RemoveRange(0, (int)addEdifFormCT.Tag);
                    addEdifFormCT.Tag = 0;
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
                        Visibility btClose = (int)addEdifFormCT.Tag == 0 ? Visibility.Hidden : Visibility.Visible;
                        CtCertificate ct = new CtCertificate((int)addEdifFormCT.Tag + 1);
                        addEdifFormCT.Children.Insert((int)addEdifFormCT.Tag, ct);
                        addEdifFormCT.Tag = (int)addEdifFormCT.Tag + 1;

                        ct.tbSeries.Text = reader.GetString(reader.GetOrdinal("num"));
                        string disciplin = reader.GetString(reader.GetOrdinal("Дисциплина"));
                        bool hasDisc = false;
                        foreach (ComboBoxItem item in ct.cbDisciplin.Items)
                        {
                            if (item.Content.ToString() == disciplin)
                            {
                                hasDisc = true;
                                ct.cbDisciplin.SelectedItem = item;
                                return;
                            }
                        }
                        if (hasDisc == false)
                        {
                            ComboBoxItem item = new ComboBoxItem()
                            {
                                Content = disciplin
                            };
                            ct.cbDisciplin.Items.Add(item);
                            ct.cbDisciplin.SelectedItem = item;
                        }
                        ct.cbDisciplin.SelectedItem =
                        ct.mtbYear.Text = reader.GetInt32(reader.GetOrdinal("ГодПрохождения")).ToString();
                        ct.tbScore.Text = reader.GetInt32(reader.GetOrdinal("Балл")).ToString();
                        break;

                    }
                    connection.Close();
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message, "Заполнение сертификатов ЦТ"); 
                }//сертификаты цт
            }
        } //нажатие кнопки редактирования
        /// <summary>
        /// Открытие контекстного меню в таблице абитуриентов
        /// </summary>
        private void Image_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            ((Image)sender).ContextMenu.IsOpen = true;
        }
        /// <summary>
        /// Обработка нажатия клавиши Delete для удаления абитуриентов
        /// </summary>
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
                    if (dataGridAbiturients.SelectedItems.Count > 3)
                        delItemsName += $"И еще {dataGridAbiturients.SelectedItems.Count - 3} запись(-ей)";
                }

                if (MessageBox.Show($"Удалить выбранные записи?\n\n {delItemsName}", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (AbiturientDGItem abiturient in dataGridAbiturients.SelectedItems)
                    {
                        DB.DeleteAllAbiturientDataInTable(abiturient.ID, "Абитуриент");
                    }
                    AbiturientTableLoad(curentPlanPriema.Id);
                }
            }
        }
        /// <summary>
        /// Открытие информации об абитуриенте по двойному нажатию
        /// </summary>
        private void DataGridAbiturients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AbiturientInfoShow();
        }
        /// <summary>
        /// Изменение статуса абитуиента
        /// </summary>
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
                MessageBox.Show(ex.Message, "Изменение статуса абитуриента");
            }
        }
        /// <summary>
        /// Удаление записи об абитуриенте
        /// </summary>
        private void Abiturient_Delete(object sender, RoutedEventArgs e)
        {
            AbiturientDGItem abiturient = (AbiturientDGItem)dataGridAbiturients.SelectedItem;
            MessageBoxResult acceptDeletion = MessageBox.Show("Удалить выбранную запись?\n" + abiturient.FIO, "Удаление", MessageBoxButton.YesNo);
            if (acceptDeletion == MessageBoxResult.Yes)
            {
                DB.DeleteAllAbiturientDataInTable(abiturient.ID, "Абитуриент");
                AbiturientTableLoad(curentPlanPriema.Id);
                GridInfo.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// Поиск абитуриентов
        /// </summary>
        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<AbiturientDGItem> newabiturients = new List<AbiturientDGItem>();

            newabiturients = abiturients.FindAll(x => Regex.IsMatch(x.FIO.ToLower(), $@"{textBoxSearch.Text.ToLower()}"));

            dataGridAbiturients.ItemsSource = newabiturients;
        }
        /// <summary>
        /// фильтр по целевому договору
        /// </summary>
        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (abiturients == null) return;
            if (((ComboBox)sender).SelectedIndex == 1)
            {
                foreach (AbiturientDGItem item in abiturients)
                {
                    if (!Regex.IsMatch(item.Lgoti, $@"Договор"))
                        item.Hide = true;
                }
            }
            else
            {
                foreach (AbiturientDGItem item in abiturients)
                    item.Hide = false;
            }
            TextBoxSearch_TextChanged(sender, null);
        }
        /// <summary>
        /// Закрытие формы таблицы абитуриентов
        /// </summary>
        private void CloseButtonUp(object sender, MouseButtonEventArgs e)
        {
            Panel controlElement = (Panel)((Image)sender).Tag;
            controlElement.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// Сортировка абитуриентов по приоритету
        /// </summary>
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
                    list.Add(abiturients.Find(_ => _.ID == abiturientID));
                }
                reader.Close();
                abiturients = list;
                dataGridAbiturients.ItemsSource = abiturients;
                dataGridAbiturients.Tag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Сортировка по приоритету");
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Открыие контекстного меню с отчетами
        /// </summary>
        private void ExcelContextMenuOpen(object sender, MouseButtonEventArgs e)
        {
            ((Image)sender).ContextMenu.IsOpen = true;
        }
        /// <summary>
        /// Обработчик сортировки таблицы по одному из полей
        /// </summary>
        private void DataGridAbiturients_Sorting(object sender, DataGridSortingEventArgs e)
        {
            dataGridAbiturients.Tag = false; //тег обозначает отсортирована ли таблица по приоритету
        }
        /// <summary>
        /// Нажатие кнопки печати
        /// </summary>
        private void PrintButton_Click(object sender, MouseButtonEventArgs e)
        {
            ToExcelButton_Click(sender, e);
        }
        /// <summary>
        /// формирование отчета по текущему состоянию таблицы
        /// </summary>
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
            try
            {
                ex.Cells[3, 1] = $"по специальности {'"' + curentPlanPriema.NameSpec + '"'}";
                ex.Cells[4, 1] = $"на {curentPlanPriema.NameForm.ToLower().Substring(0, curentPlanPriema.NameForm.Length - 2) + "ом"} отделении ({curentPlanPriema.NameObrazovaie.ToLower()}) {curentPlanPriema.NameFinance.ToLower()}";
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
                    ex.Cells[9 + i, 2] = abiturient.FIO;
                    ex.Cells[9 + i, 3] = abiturient.Shool;
                    ex.Cells[9 + i, 4] = abiturient.Year;
                    ex.Cells[9 + i, 5] = abiturient.Lgoti;
                    ex.Cells[9 + i, 6] = abiturient.Stati;
                    for (int j = 0; j < abiturient.Marks.Length && j < 10; j++)
                    {
                        ex.Cells[9 + i, 7 + j] = abiturient.Marks[j];
                    }
                    ex.Cells[9 + i, 17] = abiturient.MarkDecAvg;
                    ex.Cells[9 + i, 18] = abiturient.DocumentiVidani ? "+" : "0";
                    if (abiturient.DifferentAttestat == true) //Если у абитуриента 2 или более аттестата с разными шкалами
                    {
                        Excel.Range range = ex.Cells[9 + i, 17];
                        range.Font.Bold = true; //Выделение среднего балла жирным
                    }

                }
                ex.Range["A10", "R" + (reads + 8)].Cells.Borders.Weight = Excel.XlBorderWeight.xlThin;
                ex.Interactive = true;
                if (((FrameworkElement)sender).Tag != null && ((FrameworkElement)sender).Tag.ToString() == "Print") //Если обработчик был вызван из кнопки печати
                {
                    bool userDidntCancel =
                ex.Dialogs[Excel.XlBuiltInDialog.xlDialogPrint].Show(
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing); //Открытие формы печати
                }
            }
            catch
            {
                MessageBox.Show("Ошибка формированя отчета");
                ex.Interactive = true;
            }
        }
        /// <summary>
        /// Отчет для исполкома
        /// </summary>
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
            try
            {
                string[] dataFormiObucheniya = LabelFormaObrazovaniya.Text.ToString().Split('.');

                ex.Cells[4, 1] = $"Специальность (направление специальности, специализация) {curentPlanPriema.CodeSpec} {curentPlanPriema.NameSpec}";
                ex.Cells[5, 1] = $"Форма получения образования: {curentPlanPriema.NameForm.ToLower()} ({curentPlanPriema.NameFinance.ToLower()})";
                ex.Cells[6, 1] = curentPlanPriema.NameObrazovaie;
                ex.Cells[13, 3] = DateTime.Now.ToString("dd.MM.yyyy");
                int offset = 0;
                bool writeline = false;
                for (int i = 0; i < abiturients.Count; i++)
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
                    if (!writeline && Convert.ToBoolean(dataGridAbiturients.Tag) == true && abiturients[i].Status != "Зачислен")
                    {
                        ex.Range[$"A{9 + i - offset}", $"J{9 + i - offset}"].Cells.Borders.Item[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlMedium;
                        writeline = true;
                    }
                }
                ex.Interactive = true;
            }
            catch (Exception exept)
            {
                MessageBox.Show(exept.Message, "Ошибка формированя отчета");
                ex.Interactive = true;
            }
        }
        /// <summary>
        /// Формирование файла для импорта в AD
        /// </summary>
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

                    ex.Cells[readNum, 6] = reader[3].ToString();
                    ex.Cells[readNum, 7] = reader[4].ToString();
                    ex.Cells[readNum, 8] = reader[5].ToString();
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
        /// <summary>
        /// вызод из формы просмотра
        /// </summary>
        private void Image_BackToAbiturients(object sender, MouseButtonEventArgs e)
        {
            GridInfo.Visibility = Visibility.Hidden;
            AbiturientTableLoad(curentPlanPriema.Id);
        }
        /// <summary>
        /// Редактирование аттестата
        /// </summary>
        private void Image_AtestatRedakt(object sender, MouseButtonEventArgs e)
        {
            Image_MouseUp_1(sender, e);
            TabControlAddEditForm.SelectedIndex = 2;
        }
        /// <summary>
        /// Удаление аттестата
        /// </summary>
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
                try //Обновление аттестатов битуриента
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Get_AbiturientaAttestat", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
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
                    MessageBox.Show(ex.Message, "Скрытие неиспользуемых столбцов таблицы");
                }
            }
        }
        /// <summary>
        /// Редактирование ЦТ
        /// </summary>
        private void Image_CTRedakt(object sender, MouseButtonEventArgs e)
        {
            Image_MouseUp_1(sender, e);
            TabControlAddEditForm.SelectedIndex = 3;
        }
        /// <summary>
        /// Редактирование контактных данных
        /// </summary>
        private void Image_KontaktsRedakt(object sender, MouseButtonEventArgs e)
        {
            Image_MouseUp_1(sender, e);
            TabControlAddEditForm.SelectedIndex = 1;
        }
        /// <summary>
        /// Удаление контакта
        /// </summary>
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
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message, "Удаление контактных данных"); 
                }
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
                    MessageBox.Show(ex.Message, "Запись контактных данных абитуриента");
                }
            }
        }
        /// <summary>
        /// Удаление ЦТ
        /// </summary>
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
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message); 
                }
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
        /// <summary>
        /// Обработчик для установки первой буквы заглавной
        /// </summary>
        private void InUpperLetter(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = ((TextBox)sender);
            if (textBox.Text.Length == 0)
            {
                textBox.Text = e.Text.ToUpper();
                textBox.SelectionStart = 1;
                e.Handled = true;
            }
        }
        /// <summary>
        /// Установка гражданства на Республика Беларусь при активации checkBox
        /// </summary>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                AddFormGrajdanstvo.Text = "Республика Беларусь";
            }
        }
        /// <summary>
        /// Активация CheckBox При вводе гражданства "Республика Беларусь"
        /// </summary>
        private void AddFormGrajdanstvo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text != "")
            {
                ((TextBox)sender).Tag = "";
            }
            if (((TextBox)sender).Text == "Республика Беларусь")
                AddFormChekBoxGrajdanstvo.IsChecked = true;
            else
                AddFormChekBoxGrajdanstvo.IsChecked = false;
        }
        /// <summary>
        /// Проверка корректности ввода даты рождения
        /// </summary>
        private void DateOfBirth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DateTime.TryParse(((Xceed.Wpf.Toolkit.MaskedTextBox)sender).Text, out _))
            {
                ((Xceed.Wpf.Toolkit.MaskedTextBox)sender).Tag = "";
            }
            else
                ((Xceed.Wpf.Toolkit.MaskedTextBox)sender).Tag = "Error";
        }
        /// <summary>
        /// добавление нового контакта
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ContactData contact = new ContactData(Visibility.Visible, (int)AddEditFormContacts.Tag + 1);
            AddEditFormContacts.Children.Insert((int)AddEditFormContacts.Tag, contact);
            AddEditFormContacts.Tag = (int)AddEditFormContacts.Tag + 1;
        }
        /// <summary>
        /// Добавление нового аттестата
        /// </summary>
        private void Button_NewAtestat(object sender, RoutedEventArgs e)
        {
            Certificate certificate = new Certificate(Visibility.Visible, (int)addEdifFormAtestati.Tag + 1);
            addEdifFormAtestati.Children.Insert((int)addEdifFormAtestati.Tag, certificate);
            addEdifFormAtestati.Tag = (int)addEdifFormAtestati.Tag + 1;
        }
        /// <summary>
        /// Проверка ввода и идентификационный номер только цифр и латинских букв 
        /// </summary>
        private void Tb_IdentNuber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9a-zA-Z]+");
            bool isMatch = regex.IsMatch(e.Text);
            ttpIdentNum.PlacementTarget = (UIElement)sender;
            ttpIdentNum.IsOpen = !isMatch;
            e.Handled = !isMatch;
        }
        /// <summary>
        /// Проверка ввода и идентификационный номер только латинских букв 
        /// </summary>
        private void Tb_SeriyaPasporta_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[a-zA-Z]+$");
            bool isMatch = regex.IsMatch(e.Text);
            ttpSerya.PlacementTarget = (UIElement)sender;
            ttpSerya.IsOpen = !isMatch;
            e.Handled = !isMatch;
        }
        /// <summary>
        /// Установка заглавных букв в серии аттестата 
        /// </summary>
        private void PassportSeriya_TextInput(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int selStart = tb.SelectionStart;
            tb.Text = tb.Text.ToUpper();
            tb.SelectionStart = selStart;
        }
        /// <summary>
        /// Проверка ввода только цифр
        /// </summary>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !PLib.IsTextAllowed(e.Text);
        }
        /// <summary>
        /// Добавление нового сертификата ЦТ
        /// </summary>
        private void ButtonNewSertificatCT(object sender, RoutedEventArgs e)
        {
            CtCertificate ct = new CtCertificate((int)addEdifFormCT.Tag + 1);
            addEdifFormCT.Children.Insert((int)addEdifFormCT.Tag, ct);
            addEdifFormCT.Tag = (int)addEdifFormCT.Tag + 1;
        }
        /// <summary>
        /// Переход на прдыдущую  вкладку
        /// </summary>
        private void Button_PrewPage(object sender, RoutedEventArgs e)
        {
            TabControlAddEditForm.SelectedIndex -= 1;
            if (((TabItem)TabControlAddEditForm.SelectedItem).IsEnabled == false)
                TabControlAddEditForm.SelectedIndex--;
        }
        /// <summary>
        /// Переход на 2 этап
        /// </summary>
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
        /// <summary>
        /// Переход на 3 этап
        /// </summary>
        private void Button_NextStep_2(object sender, RoutedEventArgs e)
        {
            if (PLib.FormIsCorrect<ContactData>(AddEditFormContacts))
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
            }

        }
        /// <summary>
        /// Переход на 4 этап
        /// </summary>
        private void Button_NextStep_3(object sender, RoutedEventArgs e)
        {
            if ((PLib.FormIsCorrect<Certificate>(addEdifFormAtestati)))
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
                if (((TabItem)TabControlAddEditForm.SelectedItem).IsEnabled == false)
                    TabControlAddEditForm.SelectedIndex++;
            }
        }
        /// <summary>
        /// Переход на 5 этап
        /// </summary>
        private void Button_NextStep_4(object sender, RoutedEventArgs e)
        {
            if (PLib.FormIsCorrect<CtCertificate>(addEdifFormCT))
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
            }
        }
        /// <summary>
        /// завершение редактирования
        /// </summary>
        private void Button_EditEnd(object sender, RoutedEventArgs e)
        {
            if (!InputIsCorrect())
            {
                return;
            }
            DB.UpdateAbiturientMainData(AbiturientID,
                addEditFormSurename.Text,
                addEditFormName.Text,
                addEditFormOtchestvo.Text,
                addEditFormShool.Text,
                addEditFormGraduationYear.Text,
                AddFormChekBoxGrajdanstvo.IsChecked == true,
                AddFormGrajdanstvo.Text,
                addEditFormObshejitie.IsChecked == true,
                curentPlanPriema.Id,
                textBoxWorkPlace.Text,
                textBoxDoljnost.Text,
                addEditForm_CheckBox_DetiSiroti.IsChecked == true,
                addEditForm_CheckBox_Dogovor.IsChecked == true,
                user.ID,
                addEditFormExamList.Text);
            //Основные данные 

            DB.DeleteAllAbiturientDataInTable(AbiturientID, "КонтактныеДанные");
            for (int i = 0; i < (int)AddEditFormContacts.Tag; i++)
            {
                if (AddEditFormContacts.Children[i] is ContactData contactData)
                {
                    DB.InsertContactData(contactData, AbiturientID);
                }
            } //Контактные данные* ?

            DB.DeleteAllAbiturientDataInTable(AbiturientID, "Атестат");
            for (int i = 0; i < (int)addEdifFormAtestati.Tag; i++)
            {
                if (addEdifFormAtestati.Children[i] is Certificate certificate)
                {
                    DB.InsertCertificate(certificate, AbiturientID);
                }
            } //Образование* ?

            DB.DeleteAllAbiturientDataInTable(AbiturientID, "СертификатЦТ");
            for (int i = 0; i < (int)addEdifFormCT.Tag; i++)
            {
                if (addEdifFormCT.Children[i] is CtCertificate ct)
                {
                    DB.InsertCtCertificate(ct, AbiturientID);
                }
            } //Сертификаты ЦТ* ?

            DB.UpdatePasportData(AbiturientID, PassportDateVidachi.Text, dateOfBirth.Text, PassportSeriya.Text, PassportNomer.Text, PassportVidan.Text, PassportIdentNum.Text);
            //Паспортные данные*

            DB.DeleteAllAbiturientDataInTable(AbiturientID, "СтатьиАбитуриента");

            foreach (CheckBox checkBox in ucArticles.checkBoxes)
            {
                if (checkBox.IsChecked == true)
                {
                    DB.InsertArticles(AbiturientID, (string)checkBox.Content);
                }
            }
            //Статьи* ?
            AbiturientTableLoad(curentPlanPriema.Id);
            addEditForm.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// Блокирование льготы Сирота
        /// </summary>
        private void BlockCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;
            if(checkBox.IsChecked == true)
            {
                addEditForm_CheckBox_DetiSiroti.IsChecked = false;
                addEditForm_CheckBox_DetiSiroti.IsEnabled = false;
            }
            else
            {
                addEditForm_CheckBox_DetiSiroti.IsEnabled = true;
            }
        }
        /// <summary>
        /// Блокирование статьи Сирота
        /// </summary>
        private void BlockCheckBox2(object sender, RoutedEventArgs e)
        {
            foreach(CheckBox checkBox in ucArticles.checkBoxes)
            {
                if(checkBox.Content.ToString() == "Сирота")
                {
                    if (addEditForm_CheckBox_DetiSiroti.IsChecked == true)
                    {
                        checkBox.IsChecked = false;
                        checkBox.IsEnabled = false;
                    }
                    else
                    {
                        checkBox.IsEnabled = true;
                    }
                    return;
                }
            }
        }
        /// <summary>
        /// Закрытие формы редактирование/добавления
        /// </summary>
        private void Image_MouseUp_3(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Данные не будут сохранены!", "Закрыт форму?", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                addEditForm.Visibility = Visibility.Hidden;
            }
        }
        #region заполнение ComboBoks для формы редактирования
        /// <summary>
        /// Изменение специальности
        /// </summary>
        private void AddEditFormspecialnost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addEditFormspecialnost.SelectedItem == null) return;
            try //заполнение списка форм обучения
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
        /// <summary>
        /// Изменение формы обучения
        /// </summary>
        private void AddEditFormobushenie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addEditFormobushenie.SelectedItem == null) return;
            try //Заполнение списка фнанирования
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
            if(addEditFormobushenie.SelectedItem.ToString() == "Дневная")
            {
                AddEditWork.Visibility = Visibility.Collapsed;
                textBoxWorkPlace.Text = "";
                textBoxDoljnost.Text = "";
            }
            else
            {
                AddEditWork.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Изменение финансирования
        /// </summary>
        private void AddEditFormFinansirovanie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addEditFormFinansirovanie.SelectedItem == null) return;
            try //заполнение формы обучения
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Заполнение списка форм образования");
            }
            try //получение плана приема из выбранных данных
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_PlanPriemaID", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@speciality", addEditFormspecialnost.SelectedItem);
                command.Parameters.AddWithValue("@formOfEducation", addEditFormobushenie.SelectedItem);
                command.Parameters.AddWithValue("@financing", addEditFormFinansirovanie.SelectedItem);
                command.Parameters.AddWithValue("@education", addEditFormobrazovanie.SelectedItem);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    curentPlanPriema = DB.Get_PlanPriemaByID(reader.GetInt32(0));
                    SetExamList();
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Получение плана приема");
            }
            try // блокирование формы сертификатов ЦТ
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand($"SELECT ЦТ FROM ПланПриема WHERE IDПланПриема = {curentPlanPriema.Id}", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                TabItemSertificat.IsEnabled = Convert.ToBoolean(reader[0]);
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Активация формы сертификатов ЦТ");
            }
        }
        /// <summary>
        /// Изменение формы обучения
        /// </summary>
        private void AddEditFormobrazovanie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addEditFormobrazovanie.SelectedItem == null) return;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_PlanPriemaID", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@speciality", addEditFormspecialnost.SelectedItem);
                command.Parameters.AddWithValue("@formOfEducation", addEditFormobushenie.SelectedItem);
                command.Parameters.AddWithValue("@financing", addEditFormFinansirovanie.SelectedItem);
                command.Parameters.AddWithValue("@education", addEditFormobrazovanie.SelectedItem);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    curentPlanPriema = DB.Get_PlanPriemaByID(reader.GetInt32(0));
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Получение плана приема");
            }
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
                        if (PLib.FormIsCorrect<ContactData>(AddEditFormContacts)) ((TabItem)TabControlAddEditForm.Items[1]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[1]).Tag = "";
                        break;

                    case 2:
                        if (PLib.FormIsCorrect<Certificate>(addEdifFormAtestati)) ((TabItem)TabControlAddEditForm.Items[2]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[2]).Tag = "";
                        break;

                    case 3:
                        if (PLib.FormIsCorrect<CtCertificate>(addEdifFormCT)) ((TabItem)TabControlAddEditForm.Items[3]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[3]).Tag = "";
                        break;
                }
            }
        }
        #endregion
        #endregion
        #region Настройка контрольных цифр приема
        /// <summary>
        /// загрузка планов приема при выборе специальости
        /// </summary>
        private void TabItem1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddAdmissionPlan.Children.Clear();
            PlanPriemaTableLoad(((TabItem)sender).Header.ToString());
            labelSpec.Content = (sender as TabItem).Tag;
        }
        /// <summary>
        /// открытие формы добавление плана приема
        /// </summary>
        private void Button_AddPlanPriema(object sender, RoutedEventArgs e)
        {
            AddAdissionPlanControl addAdissionPlan = new AddAdissionPlanControl(((TabItem)TabControl1.SelectedItem).Tag.ToString());
            addAdissionPlan.CloseControl += CloseAdmissionControl;   
            AddAdmissionPlan.Children.Add(addAdissionPlan);
        }
        /// <summary>
        /// Обработка закытия формы добавления/редктирования плана приема
        /// </summary>
        private void CloseAdmissionControl(object sender, RoutedEventArgs e)
        {
            PlanPriemaTableLoad(((TabItem)TabControl1.SelectedItem).Header.ToString());
        }
        /// <summary>
        /// Открытие формы редактирования плана приема
        /// </summary>
        private void ImagecCick_UpdatePlanPriema(object sender, MouseButtonEventArgs e)
        {
            AddAdissionPlanControl addAdissionPlan = new AddAdissionPlanControl((PlanPriema)dataGridPlani.SelectedItem);
            AddAdmissionPlan.Children.Add(addAdissionPlan);
        }
        /// <summary>
        /// Удаление плана приема
        /// </summary>
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
                        if (MessageBox.Show("В плане приема есть записи о абитуриентах!\n(Они будут удалены!)\nПродолжить?", "Удаление", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
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
                    MessageBox.Show(ex.Message, "Удаление плана приема");
                }
            }
        }
        /// <summary>
        /// Открытие таблицы плана приема по двойному нажатию
        /// </summary>
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
        /// <summary>
        /// Обработчик открытия формы статистики подачи документов для обновления данных
        /// </summary>
        private void OpenStats(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (tabWork.SelectedIndex == 2 && TabControl2.SelectedItem != null)
            {
                StatsLoad(((TabItem)TabControl2.SelectedItem).Header.ToString());
            }
        }
        /// <summary>
        /// Обновление данных при воборе специальности
        /// </summary>
        private void TabItem2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StatsLoad(((TabItem)sender).Header.ToString());
        }
        /// <summary>
        /// Проверка на ввод только значений с плавующей запятой
        /// </summary>
        private void TbMaskFloat(object sender, TextCompositionEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            string text = textbox.Text;
            text = text.Insert(textbox.SelectionStart, e.Text);
            e.Handled = !Double.TryParse(text, out _);
        }
        /// <summary>
        /// Маска ввода для значений с плавующей запятой
        /// </summary>
        private void TbMaskFloat_TextInput(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            textbox.Tag = "";
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
        /// <summary>
        /// Формирование отчета по подаче документов
        /// </summary>
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
            Excel.Application ex = new Microsoft.Office.Interop.Excel.Application
            {
                Visible = true,
                SheetsInNewWorkbook = 1,
                Interactive = false
            };
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
                try //Получение количества абитуриентов с средним баллом в заданном диапазоне
                {
                    connection.Open();
                    for (int j = 7; j <= cellIndex; j++)
                    {
                        SqlCommand command = new SqlCommand("GetAbiturientCountForStats", connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
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
        /// <summary>
        /// Открытие плана приема двойным щелчком по элементу таблицы
        /// </summary>
        private void DataGridStats_OpenPlanPriema(object sender, MouseButtonEventArgs e)
        {
            if (DGStats.SelectedItem == null) return;
            TabControl.SelectedIndex = TabControl2.SelectedIndex;
            AbiturientTableLoad(((DocSubmissionStat)DGStats.SelectedItem).IDAdmissionPlan);
            PlanPriemaTable.Visibility = Visibility.Visible;
            tabWork.SelectedIndex = 0;
        }
        #endregion
        #region перечень специальностей
        /// <summary>
        /// Обновление данных прие переходе на форму
        /// </summary>
        private void TabItemClick_LoadSpecialityTable(object sender, MouseButtonEventArgs e)
        {
            dgSpeciality.ItemsSource = DB.GetSpecialityTable();
        }
        /// <summary>
        /// редактирование Специальности
        /// </summary>
        private void MouseUp_SpecialityEdit(object sender, MouseButtonEventArgs e)
        {
            if (dgSpeciality.SelectedItem == null) return;
            ucSpeciality.Edit((Speciality)dgSpeciality.SelectedItem);
            dgSpeciality.IsEnabled = false;
        }
        /// <summary>
        /// Удаление специальности
        /// </summary>
        private void MouseUp_SpecialityDelete(object sender, MouseButtonEventArgs e)
        {
            if (dgSpeciality.SelectedItem == null) return;
            MessageBoxResult result = MessageBox.Show($"Удалить выбранную специальность\n{((Speciality)dgSpeciality.SelectedItem).Title}", "Удаление", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand($"Select IDПланПриема FROM ПланПриема where IDСпециальности = {(dgSpeciality.SelectedItem as Speciality).Num}", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    result = MessageBox.Show("Существуют планы приема с выбранной специальностью которые также будут удалены\nПродолжить?", "Удаление", MessageBoxButton.YesNo);
                    if (result != MessageBoxResult.Yes)
                    {
                        connection.Close();
                        return;
                    }
                }
                connection.Close();
                DB.DeleteSpeciality(((Speciality)dgSpeciality.SelectedItem).Num);
                dgSpeciality.ItemsSource = DB.GetSpecialityTable();
                UpdateSpeciality();
            }
        }
        /// <summary>
        /// Обработчик завершения редактирования и добавления
        /// </summary>
        private void Speciality_EndEdit(object sender, RoutedEventArgs e)
        {
            dgSpeciality.IsEnabled = true;
            dgSpeciality.ItemsSource = DB.GetSpecialityTable();
            if (((Button)sender).Name == "btnSave")
            {
                UpdateSpeciality();
            }
        }
        #endregion
        #region Методы
        /// <summary>
        /// Изменение позиций кнопок под размер окна
        /// </summary>
        /// <param name="col">количество столбцов</param>
        private void ButtonPos(int col)
        {
            if (planPriemaColumn == col) return;

            double x = colButtonsize.Width.Value;
            double y = rowButtonsize.Height.Value;

            int buttons = 0;
            int row = 1;
            while (buttons < planPriemaButtons.Count)
            {
                for (int i = 1; i <= col && buttons < planPriemaButtons.Count; i++)
                {
                    Button button = planPriemaButtons[buttons];
                    int curRow = (int)button.GetValue(Grid.RowProperty);
                    int curCol = (int)button.GetValue(Grid.ColumnProperty);

                    ThicknessAnimation animation = new ThicknessAnimation
                    {
                        From = button.Margin,
                        To = new Thickness((i - curCol - 1) * x, (row - curRow - 1) * y, 0, 0),
                        Duration = TimeSpan.FromSeconds(0.2)
                    };
                    planPriemaButtons[buttons].BeginAnimation(Button.MarginProperty, animation);
                    buttons++;
                }
                row++;
            }
            grdAdmissionPlans.Height = (row-1) * y;
            planPriemaColumn = col;
        }
        /// <summary>
        /// Загрузка данных о статистике подачи документов
        /// </summary>
        /// <param name="specialnost">Краткое наименование специальности</param>
        private void StatsLoad(string specialnost)
        {
            List<DocSubmissionStat> list = new List<DocSubmissionStat>();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("GetStats", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@spec", specialnost);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DocSubmissionStat stat = new DocSubmissionStat(7)
                    {
                        IDAdmissionPlan = reader.GetInt32(0),
                        TotalToAdmissionPlan = reader.GetInt32(1),
                        AdmissionPlanDogovor = reader.GetInt32(2),
                        AdmissionPlanPayers = reader.GetInt32(3),
                        TotalToEntrant = reader.GetInt32(4),
                        EntrantDogovor = reader.GetInt32(5),
                        EntrantOutOfCompetition = reader.GetInt32(6),
                        AdmissionBased = reader.GetString(7)
                    };

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
                        SqlCommand command1 = new SqlCommand("GetAbiturientCountForStats", connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
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
                MessageBox.Show(ex.Message, "Получение количества абитуриентов для статистики");
            }
        }
        /// <summary>
        /// Загрузка данных о планах приема
        /// </summary>
        /// <param name="specialnost">Краткое наименование специальности</param>
        private void PlanPriemaTableLoad(string specialnost)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("Get_PlanPrieaBySpeciality", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
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
        /// <summary>
        /// Загрузка кнопок планов приема
        /// </summary>
        /// <param name="specialost">Краткое наименование специальности</param>
        private void PlaniPriemaLoad(string specialost)
        {
            Brush[] colors = { 
                new SolidColorBrush(Color.FromRgb(255, 87, 107)), 
                new SolidColorBrush(Color.FromRgb(26, 149, 176)), 
                new SolidColorBrush(Color.FromRgb(68, 166, 212)), 
                new SolidColorBrush(Color.FromRgb(220, 136, 51)), 
                new SolidColorBrush(Color.FromRgb(93, 79, 236)),
                new SolidColorBrush(Color.FromRgb(87, 154, 255)),
                new SolidColorBrush(Color.FromRgb(255, 222, 87))}; //сюда можно добавить цвета для кнопок плана приема
            int i = 0;
            planPriemaButtons.Clear();
            grdAdmissionPlans.Children.Clear();

            List<PlanPriema> AdmissionsPlans = DB.Get_PlaniPriema(specialost, CBFinBudjet.IsChecked, CBFinHozrach.IsChecked, CBObrBaz.IsChecked, CBObrsred.IsChecked, CBFormDnev.IsChecked, CBformZaoch.IsChecked);
            foreach (PlanPriema plan in AdmissionsPlans)
            {
                Button button = new Button()
                {
                    Style = (Style)FindResource("AdmissionPlan"),
                };
                button.Click += PlanPriema_MouseDown;
                planPriemaButtons.Add(button);
                button.Tag = plan;
                ButtonAdmissionPlanThemeProperties.SetFundingType(button, plan.NameForm.ToUpper());
                ButtonAdmissionPlanThemeProperties.SetStudyType(button, plan.NameFinance + ". " + plan.NameObrazovaie);
                ButtonAdmissionPlanThemeProperties.SetWritesCount(button, plan.Writes.ToString());
                ButtonAdmissionPlanThemeProperties.SetTickBrush(button, colors[i]);
                grdAdmissionPlans.Children.Add(button);
                i++;
                if (i == colors.Length-1) i = 0;
            }
            planPriemaColumn = 0;
            MainWorkingWindowForm_SizeChanged(null, null);
        }
        /// <summary>
        /// Очистка тега Error при изменении текста
        /// </summary>
        private void ClearError(object sender, TextChangedEventArgs e)
        {
            PLib.ClearError(sender);
        }
        /// <summary>
        /// Загрузка таблицы абитуриентов
        /// </summary>
        /// <param name="PlanPriemaID">ИД плана приема</param>
        private void AbiturientTableLoad(int PlanPriemaID)
        {
            curentPlanPriema = DB.Get_PlanPriemaByID(PlanPriemaID);
            if ((TabControl.SelectedItem as TabItem).Header.ToString() != curentPlanPriema.NameSpec || (TabControl.SelectedItem as TabItem).Tag.ToString() != curentPlanPriema.NameSpec)
            {
                foreach (TabItem tabItem in TabControl.Items)
                {
                    if (tabItem.Header.ToString() == curentPlanPriema.NameSpec || tabItem.Tag.ToString() == curentPlanPriema.NameSpec)
                    {
                        TabControl.SelectedItem = tabItem;
                        break;
                    }
                }
            }
            SqlConnection connection = new SqlConnection(connectionString);
            abiturients = new List<AbiturientDGItem>();
            try
            {
                SqlCommand command = new SqlCommand($"SELECT * FROM GetAbiturientData WHERE (SELECT IDПланаПриема FROM dbo.Абитуриент WHERE(IDАбитуриента = GetAbiturientData.IDАбитуриента)) = {PlanPriemaID}", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string lgoti = "";
                    if (Convert.ToBoolean(reader[24]) == true) { lgoti += "Cирота"; }
                    if (Convert.ToBoolean(reader[25]) == true) { lgoti += (lgoti.Length == 0 ? "" : "\n") + "Договор"; }

                    string status = "";
                    if (Convert.ToBoolean(reader[4]) == true) { status = "Зачислен"; }
                    else if (Convert.ToBoolean(reader[22]) == true) { status = "Документы выданы"; }
                    else status = "Документы приняты";

                    int[] marks = new int[15];
                    for (int i = 0; i < 15; i++)
                        marks[i] = reader[i+5] == DBNull.Value ? 0 : Convert.ToInt32(reader[i + 5]);

                    AbiturientDGItem abiturient = new AbiturientDGItem(Convert.ToInt32(reader[0]),
                        reader[1].ToString(), reader[2].ToString(),
                        Convert.ToInt32(reader[3]),
                        marks,
                        reader[15] == DBNull.Value ? 0 : Convert.ToDouble(reader[15]),
                        reader[21].ToString(),
                        Convert.ToBoolean(reader[22]),
                        reader[23] == DBNull.Value ? 0 : Convert.ToDouble(reader[23]),
                        lgoti, status);
                    abiturient.ScaleSize = reader.GetInt32(26);

                    abiturients.Add(abiturient);
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
                string statyi = DB.Get_StatiAbiturienta(abiturients[i].ID);
                abiturients[i].Stati = statyi;
            }
            GridCountWrite.Text = abiturients.Count.ToString();
            abiturients.Sort((x, y) => x.Num - y.Num);
            dataGridAbiturients.ItemsSource = abiturients;
            dataGridAbiturients.Tag = false;

            int scaleSizeMax = 0;
            foreach(AbiturientDGItem abitur in abiturients)
            {
                if (abitur.ScaleSize > scaleSizeMax)
                    scaleSizeMax = abitur.ScaleSize;
            }
            for(int i = 0; i <= 15; i++)
            {
                if(i > scaleSizeMax)
                {
                    dataGridAbiturients.Columns[i + 6].Visibility = Visibility.Hidden;
                }
                else
                {
                    dataGridAbiturients.Columns[i + 6].Visibility = Visibility.Visible;
                }
            }
        }
        /// <summary>
        /// Открытие формы просмитра информации для выделенного абитуриента
        /// </summary>
        private void AbiturientInfoShow()
        {
            if ((AbiturientDGItem)dataGridAbiturients.SelectedItem == null) return;
            GridInfo.Visibility = Visibility.Visible;
            try
            {
                Abiturient abiturient = DB.Get_AbiturientFullInfo(((AbiturientDGItem)dataGridAbiturients.SelectedItem).ID);

                InfoFIO.Text = abiturient.FIO;
                infoSchool.Text = abiturient.Shool;
                infoYear.Text = abiturient.YearOfGraduation;
                infoDate.Text = abiturient.BirthDate;
                infoLgoti.Text = ((AbiturientDGItem)dataGridAbiturients.SelectedItem).Lgoti.Replace('\n', ' ');
                if (infoLgoti.Text == "")
                    infoLgotiTB.Visibility = Visibility.Collapsed;
                else
                    infoLgotiTB.Visibility = Visibility.Visible;
                infoStati.Text = ((AbiturientDGItem)dataGridAbiturients.SelectedItem).Stati.Replace('\n', ' ');
                if (infoStati.Text == "")
                    infoStatiTB.Visibility = Visibility.Collapsed;
                else
                    infoStatiTB.Visibility = Visibility.Visible;
                infoDateVidoci.Text = abiturient.PassportDateIssued;
                infoSeriya.Text = abiturient.PassportSeries;
                infoPassNum.Text = abiturient.PassportNum;
                infokemvidan.Text = abiturient.PassportIssuedBy;
                infoIdentNum.Text = abiturient.PassportIdentnum;
                infoGrajdanstvo.Text = abiturient.Сitizenship;
                if (abiturient.WorkPlase == "")
                {
                    RowInfoWork.Height = new GridLength(0);
                }
                else
                {
                    infoMestoRaboti.Text = abiturient.WorkPlase;
                    infoDoljnost.Text = abiturient.Position;
                    RowInfoWork.Height = new GridLength(91);
                }
                infoVladelec.Text = abiturient.Vladelec;
                infoRedaktor.Text = abiturient.Editor;
                if (infoRedaktor.Text == "")
                    infoRedaktorTB.Visibility = Visibility.Hidden;
                else
                    infoRedaktorTB.Visibility = Visibility.Visible;
                infoDateVvoda.Text = abiturient.Date;
                infoDateRedact.Text = abiturient.EditDate;
                if (infoDateRedact.Text == "")
                    infoDateRedactTB.Visibility = Visibility.Hidden;
                else
                    infoDateRedactTB.Visibility = Visibility.Visible;
                InfoShow_Status.Text = abiturient.Status;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Заполнение формы информации об абитуриенте");
            }
            try//Атестаты
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaAttestat", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
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
                MessageBox.Show(ex.Message, "Заполнение таблицы аттестатов абитуиента");
            }
            try//цт
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaSertificati", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
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
                MessageBox.Show(ex.Message, "Заполнение таблицы сертификатов ЦТ абитуиента");
            }
            try//Контактные данные
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaKontakti", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
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
                MessageBox.Show(ex.Message, "Заполнение таблицы контактных данных абитуиента");
            }
        } 
        /// <summary>
        /// Проверка корректности ввода всех полей при добавлении/редактировании
        /// </summary>
        /// <returns>Все ли данные корректны</returns>
        private bool InputIsCorrect()
        {
            //проверка заполнения паспортных данных
            bool correct = true;
            if (PassportDateVidachi.IsMaskCompleted == false)
            {
                PassportDateVidachi.Tag = "Error";
                correct = false;
            }
            PLib.CorrectData(PassportSeriya, ref correct);
            PLib.CorrectData(PassportNomer, ref correct);
            PLib.CorrectData(PassportVidan, ref correct);
            PLib.CorrectData(PassportIdentNum, ref correct);
            if (correct)
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
            }
            //проверка корректности всех вкладок
            foreach (TabItem tabItem in TabControlAddEditForm.Items)
            {
                if (tabItem.Tag.ToString() != "True" && tabItem.IsEnabled == true)
                {
                    TabControlAddEditForm.SelectedItem = tabItem;
                    return false;
                }
            }
            return true;
            //проверка на корректность ввода оценок
        }
        /// <summary>
        /// запись экзаменационного листа
        /// </summary>
        private void SetExamList()
        {
            if (addEditFormobrazovanie.SelectedItem == null || EditEndButton.Visibility == Visibility.Visible) return;

            string letter;
            int num;
            string additional = "";
            try
            {
                letter = DB.Get_SpecialtyLetter((string)addEditFormspecialnost.SelectedValue);
                num = DB.Get_NextExamList(curentPlanPriema.Id);
                if (addEditFormobushenie.SelectedValue.ToString() == "Заочная")
                    additional = "зб";
                else if (addEditFormFinansirovanie.SelectedValue.ToString() == "Хозрасчет")
                    additional = "х/р";
                else if (addEditFormobrazovanie.SelectedValue != null && addEditFormobrazovanie.SelectedValue.ToString() == "На основе среднего образования")
                    additional = "с";
                addEditFormExamList.Text = num + letter + additional;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Номер экзаменационного листа");
            }
        }
        /// <summary>
        /// Проверка корректности первого этапа
        /// </summary>
        /// <returns>все ли ванные корректны</returns>
        private bool Correct_1()
        {
            bool correct = true;
            PLib.CorrectData(addEditFormSurename, ref correct);
            PLib.CorrectData(addEditFormName, ref correct);
            PLib.CorrectData(addEditFormOtchestvo, ref correct);
            PLib.CorrectData(AddFormGrajdanstvo, ref correct);
            PLib.CorrectData(addEditFormShool, ref correct);
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
        #endregion
    }
}