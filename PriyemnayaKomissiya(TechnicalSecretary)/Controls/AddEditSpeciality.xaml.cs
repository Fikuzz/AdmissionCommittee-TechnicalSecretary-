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

namespace PriyemnayaKomissiya_TechnicalSecretary_.Controls
{
    /// <summary>
    /// Логика взаимодействия для формы добавления и редактирования спейиальности
    /// </summary>
    public partial class AddEditSpeciality : IDataForm
    {
        public event RoutedEventHandler EndEdit; //Команда завершения редактирования
        private int ID = -1; //ИД редактируемой записи
        public AddEditSpeciality()
        {
            InitializeComponent();
            btnSave.Tag = true; //тег определяет какое действие будет выполнять кнопка true:добавление false:редактирование
        }
        /// <summary>
        /// Редактирование записи
        /// </summary>
        /// <param name="speciality">Редактируемая запись</param>
        public void Edit(Speciality speciality)
        {
            tbTitle.Text = speciality.Title;
            tbShortTitle.Text = speciality.ShortTitle;
            tbCode.Text = speciality.Code;
            tbLetter.Text = speciality.Letter;
            ID = speciality.Num;
            btnClose.Visibility = Visibility.Visible;
            btnSave.Content = "Сохранить";
            btnSave.Tag = false;
        }
        public bool Validate()
        {
            bool corect = true;
            PLib.CorrectData(tbTitle,ref corect);
            PLib.CorrectData(tbShortTitle, ref corect);
            PLib.CorrectData(tbLetter, ref corect);
            PLib.CorrectData(tbCode, ref corect);
            return corect;
        }
        /// <summary>
        /// Нажатие кнопки отме ны редактирования
        /// </summary>
        private void CloseEdit(object sender, RoutedEventArgs e)
        {
            tbTitle.Text = "";
            tbShortTitle.Text = "";
            tbCode.Text = "";
            tbLetter.Text = "";
            btnClose.Visibility = Visibility.Hidden;
            btnSave.Content = "Добавить";
            btnSave.Tag = true;

            EndEdit(sender, e);
        }
        /// <summary>
        /// Нажатие кнопки сохранения
        /// </summary>
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                if ((bool)(sender as Button).Tag == true)
                {
                    Speciality speciality = new Speciality
                    {
                        Title = tbTitle.Text,
                        ShortTitle = tbShortTitle.Text,
                        Letter = tbLetter.Text,
                        Code = tbCode.Text
                    };
                    DB.InsertSpeciality(speciality);
                }
                else
                {
                    Speciality speciality = new Speciality
                    {
                        Num = ID,
                        Title = tbTitle.Text,
                        ShortTitle = tbShortTitle.Text,
                        Letter = tbLetter.Text,
                        Code = tbCode.Text
                    };
                    DB.UpdateSpeciality(speciality);
                }
                CloseEdit(sender, e);
            }
        }
        /// <summary>
        /// установка верхнего регистра для текста
        /// </summary>
        private void tbLetter_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).Text.ToUpper();
        }
    }
}
