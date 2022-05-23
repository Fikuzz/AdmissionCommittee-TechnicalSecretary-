using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    /// Логика взаимодействия для AddAdissionPlanControl.xaml
    /// </summary>
    public partial class AddAdissionPlanControl : UserControl
    {
        private readonly string connectionString;
        public AddAdissionPlanControl(string specName)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand comand = new SqlCommand("SELECT Наименование FROM Специальность", connection);
                SqlDataReader reader = comand.ExecuteReader();
                Spec.Items.Clear();
                while (reader.Read())
                    Spec.Items.Add(reader[0]);
                Spec.SelectedIndex = 0;
                reader.Close();

                comand = new SqlCommand("SELECT Наименование, Образование FROM ФормаОбучения", connection);
                reader = comand.ExecuteReader();
                List<string[]> formiObusheniya = new List<string[]>();
                FormaObucheniya.Items.Clear();
                while (reader.Read())
                {
                    string[] form = new string[2];
                    form[0] = reader.GetString(0);
                    form[1] = reader.GetString(1);

                    if (!FormaObucheniya.Items.Contains(reader[0]))
                    {
                        FormaObucheniya.Items.Add(reader[0]);
                    }
                    formiObusheniya.Add(form);
                }
                FormaObucheniya.Tag = formiObusheniya;
                FormaObucheniya.SelectedIndex = 0;
                reader.Close();

                comand = new SqlCommand("SELECT Наименование FROM Финансирование", connection);
                reader = comand.ExecuteReader();
                Finanse.Items.Clear();
                while (reader.Read())
                    Finanse.Items.Add(reader[0]);
                Finanse.SelectedIndex = 0;
                reader.Close();

                ForaObucheniya_SelectionChanged(FormaObucheniya, null);
                Spec.SelectedItem = specName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            kolvoCelevihMest.Text = "0";
            kolvoMest.Text = "0";
            buttonAdd.Visibility = Visibility.Visible;
            buttonEdit.Visibility = Visibility.Collapsed;
        }
        public AddAdissionPlanControl(PlanPriema planPriema)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand comand = new SqlCommand("SELECT Наименование FROM Специальность", connection);
                SqlDataReader reader = comand.ExecuteReader();
                Spec.Items.Clear();
                while (reader.Read())
                    Spec.Items.Add(reader[0]);
                reader.Close();

                comand = new SqlCommand("SELECT Наименование, Образование FROM ФормаОбучения", connection);
                reader = comand.ExecuteReader();
                List<string[]> formiObusheniya = new List<string[]>();
                FormaObucheniya.Items.Clear();
                while (reader.Read())
                {
                    string[] form = new string[2];
                    form[0] = reader.GetString(0);
                    form[1] = reader.GetString(1);

                    if (!FormaObucheniya.Items.Contains(reader[0]))
                    {
                        FormaObucheniya.Items.Add(reader[0]);
                    }
                    formiObusheniya.Add(form);
                }
                FormaObucheniya.Tag = formiObusheniya;
                reader.Close();

                comand = new SqlCommand("SELECT Наименование FROM Финансирование", connection);
                reader = comand.ExecuteReader();
                Finanse.Items.Clear();
                while (reader.Read())
                    Finanse.Items.Add(reader[0]);
                reader.Close();

                ForaObucheniya_SelectionChanged(FormaObucheniya, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            kod.Text = planPriema.CodeSpec;
            Spec.SelectedItem = planPriema.NameSpec;
            FormaObucheniya.SelectedItem = planPriema.NameForm;
            Finanse.SelectedItem = planPriema.NameFinance;
            Obrazovanie.SelectedItem = planPriema.NameObrazovaie;
            kolvoCelevihMest.Text = planPriema.CountCelevihMest.ToString();
            kolvoMest.Text = planPriema.Count.ToString();
            CT.IsChecked = planPriema.Ct;
            buttonEdit.Tag = planPriema;
            buttonAdd.Visibility = Visibility.Collapsed;
            buttonEdit.Visibility = Visibility.Visible;
        }
        private void ForaObucheniya_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormaObucheniya.Tag == null || FormaObucheniya.SelectedValue == null) return;
            string forma = FormaObucheniya.SelectedValue.ToString();
            List<string[]> formiObusheniya = (List<string[]>)FormaObucheniya.Tag;
            Obrazovanie.Items.Clear();
            for (int i = 0; i < formiObusheniya.Count; i++)
            {
                if (formiObusheniya[i][0] == forma)
                    Obrazovanie.Items.Add(formiObusheniya[i][1]);
            }
            Obrazovanie.SelectedIndex = 0;
        }
        private void Finanse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)Finanse.SelectedItem == "Хозрасчет")
            {
                kolvoCelevihMest.Tag = kolvoCelevihMest.Text;
                kolvoCelevihMest.Text = "0";
                kolvoCelevihMest.IsEnabled = false;
            }
            else
            {
                if (kolvoCelevihMest.Tag != null)
                {
                    kolvoCelevihMest.Text = kolvoCelevihMest.Tag.ToString();
                }
                kolvoCelevihMest.IsEnabled = true;
            }
        }

        private void Button_Add(object sender, RoutedEventArgs e)
        {
            if (kod.Text == "" || kod.Text.Length > 13)
            {
                kod.Tag = "Error";
                return;
            }
            if (kolvoMest.Text == "")
            {
                kolvoMest.Tag = "Error";
                return;
            }
            if (Convert.ToInt32(kolvoCelevihMest.Text) > Convert.ToInt32(kolvoMest.Text))
            {
                kolvoCelevihMest.Tag = "Error";
                return;
            }
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Add_PlanPriema", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@year", DateTime.Now.Year);
                command.Parameters.AddWithValue("@spec", Spec.SelectedItem);
                command.Parameters.AddWithValue("@form", FormaObucheniya.SelectedItem);
                command.Parameters.AddWithValue("@fin", Finanse.SelectedItem);
                command.Parameters.AddWithValue("@obr", Obrazovanie.SelectedItem);
                command.Parameters.AddWithValue("@kolva", kolvoMest.Text);
                command.Parameters.AddWithValue("@kolvaCel", kolvoCelevihMest.Text);
                command.Parameters.AddWithValue("@CT", CT.IsChecked);
                command.Parameters.AddWithValue("@Code", kod.Text);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                this.Visibility = Visibility.Hidden;
                CloseControl(sender, e);

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
                SqlCommand command = new SqlCommand("Update_PlanPriema", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@id", ((PlanPriema)buttonEdit.Tag).Id);
                command.Parameters.AddWithValue("@spec", Spec.SelectedItem);
                command.Parameters.AddWithValue("@form", FormaObucheniya.SelectedItem);
                command.Parameters.AddWithValue("@fin", Finanse.SelectedItem);
                command.Parameters.AddWithValue("@obr", Obrazovanie.SelectedItem);
                command.Parameters.AddWithValue("@kolva", kolvoMest.Text);
                command.Parameters.AddWithValue("@kolvaCel", kolvoCelevihMest.Text);
                command.Parameters.AddWithValue("@CT", CT.IsChecked);
                command.Parameters.AddWithValue("@Code", kod.Text);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                this.Visibility = Visibility.Hidden;
                CloseControl(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public RoutedEventHandler CloseControl;

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !PLib.IsTextAllowed(e.Text);
        }
        private void PlanPriemaADD_ForaObucheniya_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormaObucheniya.Tag == null || FormaObucheniya.SelectedValue == null) return;
            string forma = FormaObucheniya.SelectedValue.ToString();
            List<string[]> formiObusheniya = (List<string[]>)FormaObucheniya.Tag;
            Obrazovanie.Items.Clear();
            for (int i = 0; i < formiObusheniya.Count; i++)
            {
                if (formiObusheniya[i][0] == forma)
                    Obrazovanie.Items.Add(formiObusheniya[i][1]);
            }
            Obrazovanie.SelectedIndex = 0;
        }
        private void PlanPriemaADD_Finanse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)Finanse.SelectedItem == "Хозрасчет")
            {
                kolvoCelevihMest.Tag = kolvoCelevihMest.Text;
                kolvoCelevihMest.Text = "0";
                kolvoCelevihMest.IsEnabled = false;
            }
            else
            {
                if (kolvoCelevihMest.Tag != null)
                {
                    kolvoCelevihMest.Text = kolvoCelevihMest.Tag.ToString();
                }
                kolvoCelevihMest.IsEnabled = true;
            }
        }
        private void PlanPrieaADD_kolvoCelevihMest_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
                textBox.Text = "0";
        }
        private void CloseButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid grid = (Grid)this.Parent;
            grid.Children.Remove(this);
        }
    }
}
