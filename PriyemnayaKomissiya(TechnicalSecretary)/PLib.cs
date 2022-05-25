using PriyemnayaKomissiya_TechnicalSecretary_.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PriyemnayaKomissiya_TechnicalSecretary_
{
    static class PLib
    {
        public static void ClearError(object sender)
        {
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).Tag = "Error";
            }
            else
            {
                ((TextBox)sender).Tag = "";
            }
        }
        public static void SetStartPosition(object sender)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "") return;
            char[] arr = textBox.Text.ToCharArray();
            if (arr[0] == '_')
            {
                textBox.SelectionStart = 0;
            }
            else if (textBox.SelectionStart == textBox.Text.Length)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] == '_')
                    {
                        textBox.SelectionStart = i;
                        break;
                    }
                }
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9]+");
        public static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public static void CorrectData(object value, ref bool result)
        {
            if (value is Xceed.Wpf.Toolkit.MaskedTextBox textBox)
            {
                if (textBox.IsMaskCompleted == false || (string)textBox.Tag == "Error")
                {
                    result = false;
                    textBox.Tag = "Error";
                }
            }
            else if (value is TextBox tb)
            {
                if (tb.Text == "" || (string)tb.Tag == "Error")
                {
                    result = false;
                    tb.Tag = "Error";
                }
            }
        }

        public static bool FormIsCorrect<T>(Panel panel) where T : IDataForm
        {
            bool correct = true;
            for (int i = 0; i < panel.Children.Count; i++)
            {
                if (panel.Children[i] is T ctCertificate)
                {
                    if (!ctCertificate.Validate())
                    {
                        correct = false;
                    }
                }
            }
            return correct;
        }
        public static void ButtonPos(int coluns, int curentColumns, List<Button> planPriemaButtons) //изменение позиций кнопок под размер экрана
        {
            if (curentColumns == coluns) return;
            int buttons = 0;
            int row = 1;
            while (buttons < planPriemaButtons.Count)
            {
                for (int i = 1; i <= coluns && buttons < planPriemaButtons.Count; i++)
                {
                    planPriemaButtons[buttons].SetValue(Grid.RowProperty, row);
                    planPriemaButtons[buttons].SetValue(Grid.ColumnProperty, i);
                    buttons++;
                }
                row++;
            }
            curentColumns = coluns;
        }
        public static void ClearData<T>(T obj) where T : Panel
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
    }
}
