﻿using PriyemnayaKomissiya_TechnicalSecretary_.Controls;
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

namespace PriyemnayaKomissiya_TechnicalSecretary_
{
    /// <summary>
    /// Файл для работы с базой данных
    /// </summary>
    class DB
    {
        static public readonly string connectionString;

        static DB()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
        /// <summary>
        /// Добавление статьи
        /// </summary>
        /// <param name="AbiturientID">ИД абитуриента</param>
        /// <param name="ArticleName">название статьи</param>
        public static void InsertArticles(int AbiturientID, string ArticleName)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sql1 = $"SELECT IDСтатьи FROM Статьи WHERE ПолноеНаименование LIKE N'{ArticleName}'";
                SqlCommand command = new SqlCommand(sql1, connection);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                command = new SqlCommand("Add_Stati", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                command.Parameters.AddWithValue("@statya", reader[0]);
                reader.Close();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Статьи");
            }
        }
        /// <summary>
        /// Добавление контактных данных
        /// </summary>
        /// <param name="contactData">форма добавления контактных данных</param>
        /// <param name="AbiturientID">ИД абитуриента</param>
        public static void InsertContactData(ContactData contactData, int AbiturientID)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Add_ContctData", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                command.Parameters.AddWithValue("@svedeniya", contactData.mtbData.Text.Replace("_", string.Empty));
                command.Parameters.AddWithValue("@contactType", contactData.cbContactType.SelectedItem);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Контактные данные");
            }
        }
        /// <summary>
        /// Добавление аттестата
        /// </summary>
        /// <param name="certificate">форма добавления аттестата</param>
        /// <param name="AbiturientID">ИД абитуриента</param>
        public static void InsertCertificate(Certificate certificate, int AbiturientID)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                double sum = 0;
                int col = 0;
                int MaxMarkInScale = Convert.ToInt32(((ComboBoxItem)certificate.cbScaleType.SelectedItem).Tag);
                for (int j = 0; j < MaxMarkInScale; j++)
                {
                    int mark = Convert.ToInt32(certificate.Marks[j].Text);
                    sum += mark * (j + 1);
                    col += mark;
                }
                double markAvg = sum / col;

                SqlCommand command = new SqlCommand("Add_Atestat", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                command.Parameters.AddWithValue("@scaleName", ((ComboBoxItem)certificate.cbScaleType.SelectedItem).Content);
                command.Parameters.AddWithValue("@attestatSeries", certificate.tbSeries.Text);
                command.Parameters.AddWithValue("@avgMarks", markAvg.ToString().Replace(',', '.'));
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                int AtestatID = (int)reader[0];
                reader.Close();
                for (int j = 0; j < MaxMarkInScale; j++)
                {
                    command = new SqlCommand("Add_Mark", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@mark", j + 1);
                    command.Parameters.AddWithValue("@colvo", Convert.ToInt32(certificate.Marks[j].Text));
                    command.Parameters.AddWithValue("@attestat", AtestatID);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Образование");
            }
        }
        /// <summary>
        /// Добавление сертификата ЦТ
        /// </summary>
        /// <param name="ctCertificate">форма добавления сертификата ЦТ</param>
        /// <param name="AbiturientID">ИД абитуриента</param>
        public static void InsertCtCertificate(CtCertificate ctCertificate, int AbiturientID)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("Add_Sertificat", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@sertificat", AbiturientID);
                command.Parameters.AddWithValue("@disciplin", ((ComboBoxItem)ctCertificate.cbDisciplin.SelectedItem).Content);
                command.Parameters.AddWithValue("@mark", ctCertificate.tbScore.Text);
                command.Parameters.AddWithValue("@decMark", (Convert.ToDouble(ctCertificate.tbScore.Text) / 10).ToString().Replace(',', '.'));
                command.Parameters.AddWithValue("@year", ctCertificate.mtbYear.Text);
                command.Parameters.AddWithValue("@serialNum", ctCertificate.tbSeries.Text);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Сертификаты ЦТ");
            }
        }
        /// <summary>
        /// Добавление паспортных данных
        /// </summary>
        /// <param name="AbiturientID">ИД Абитуриента</param>
        /// <param name="dateOfIssue">Дата выдачи</param>
        /// <param name="dateOfBirth">Дата рождения</param>
        /// <param name="series">Серия паспорта</param>
        /// <param name="PasspornNum">Номер паспорта</param>
        /// <param name="name">Кем выдан пасспорт</param>
        /// <param name="identNum">Идентификационных номер</param>
        public static void InsertPasportData(int AbiturientID, string dateOfIssue, string dateOfBirth, string series, string PasspornNum, string name, string identNum)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Add_PassportData", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                command.Parameters.AddWithValue("@dateIssue", dateOfIssue);
                command.Parameters.AddWithValue("@dateOfBirth", dateOfBirth);
                command.Parameters.AddWithValue("@series", series);
                command.Parameters.AddWithValue("@PasspornNum", PasspornNum);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@identNum", identNum);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Паспортные данные");
            }
        }
        /// <summary>
        /// Добавление основной информации об абитуриенте
        /// </summary>
        /// <param name="surename">Фамилия</param>
        /// <param name="name">Имя</param>
        /// <param name="otchestvo">Отчество</param>
        /// <param name="shool">Школа</param>
        /// <param name="graduationYear">Год окончания школы</param>
        /// <param name="grajdanstvoRB">является ли гражданином РБ</param>
        /// <param name="grajdanstvo">Гражданство</param>
        /// <param name="obshejitie">Нужно ли общежитие</param>
        /// <param name="planPriema">План приема</param>
        /// <param name="workPlace">Место работы</param>
        /// <param name="doljnost">Должность</param>
        /// <param name="sirota">Является ли сиротой</param>
        /// <param name="dogovor">Поступает ли по целевому договору</param>
        /// <param name="user">Пользователь</param>
        /// <param name="ExamList">Экзаменационных лист</param>
        /// <returns>ИД Абитуриента</returns>
        public static int InsertAbiturientMainData(string surename, string name, string otchestvo, string shool, string graduationYear, bool grajdanstvoRB, string grajdanstvo, bool obshejitie, int planPriema, string workPlace, string doljnost, bool sirota, bool dogovor, int user, string ExamList)
        {
            int id = 0;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Add_Abiturient", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@surename", surename);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@otchestvo", otchestvo);
                command.Parameters.AddWithValue("@shool", shool);
                command.Parameters.AddWithValue("@graduationYear", graduationYear);
                command.Parameters.AddWithValue("@grajdanstvoRB", grajdanstvoRB);
                command.Parameters.AddWithValue("@grajdanstvo", grajdanstvo);
                command.Parameters.AddWithValue("@obshejitie", obshejitie);
                command.Parameters.AddWithValue("@planPriema", planPriema);
                command.Parameters.AddWithValue("@workPlace", workPlace);
                command.Parameters.AddWithValue("@doljnost", doljnost);
                command.Parameters.AddWithValue("@sirota", sirota);
                command.Parameters.AddWithValue("@dogovor", dogovor);
                command.Parameters.AddWithValue("@user", user);
                command.Parameters.AddWithValue("@ExamList", ExamList);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                id = Convert.ToInt32(reader[0]);
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Основные данные");
            }
            return id;
        }
        /// <summary>
        /// Редактирование основной информации об абитуриенте
        /// </summary>
        /// <param name="surename">Фамилия</param>
        /// <param name="name">Имя</param>
        /// <param name="otchestvo">Отчество</param>
        /// <param name="shool">Школа</param>
        /// <param name="graduationYear">Год окончания школы</param>
        /// <param name="grajdanstvoRB">является ли гражданином РБ</param>
        /// <param name="grajdanstvo">Гражданство</param>
        /// <param name="obshejitie">Нужно ли общежитие</param>
        /// <param name="planPriema">План приема</param>
        /// <param name="workPlace">Место работы</param>
        /// <param name="doljnost">Должность</param>
        /// <param name="sirota">Является ли сиротой</param>
        /// <param name="dogovor">Поступает ли по целевому договору</param>
        /// <param name="redaktor">Пользователь</param>
        /// <param name="ExamList">Экзаменационных лист</param>
        public static void UpdateAbiturientMainData(int AbiturientID, string surename, string name, string otchestvo, string shool, string graduationYear, bool grajdanstvoRB, string grajdanstvo, bool obshejitie, int planPriema, string workPlace, string doljnost, bool sirota, bool dogovor, int redaktor, string ExamList)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Update_MainData", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@surename", surename);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@otchestvo", otchestvo);
                command.Parameters.AddWithValue("@shool", shool);
                command.Parameters.AddWithValue("@graduationYear", graduationYear);
                command.Parameters.AddWithValue("@grajdaninRB", grajdanstvoRB);
                command.Parameters.AddWithValue("@grajdanstvo", grajdanstvo);
                command.Parameters.AddWithValue("@obshejitie", obshejitie);
                command.Parameters.AddWithValue("@planPriema", planPriema);
                command.Parameters.AddWithValue("@workPlase", workPlace);
                command.Parameters.AddWithValue("@doljnost", doljnost);
                command.Parameters.AddWithValue("@sirota", sirota);
                command.Parameters.AddWithValue("@dogovor", dogovor);
                command.Parameters.AddWithValue("@redaktor", redaktor);
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                command.Parameters.AddWithValue("@ExamList", ExamList);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Основные данные");
            }
        }
        /// <summary>
        /// Удаление записей в таблице связанной с абитуриентом
        /// </summary>
        /// <param name="AbiturientID">ИД Абитуриента</param>
        /// <param name="TableName">Имя таблицы</param>
        public static void DeleteAllAbiturientDataInTable(int AbiturientID, string TableName)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sqldel = $"DELETE FROM {TableName} WHERE IDАбитуриента = {AbiturientID}";
                SqlCommand del = new SqlCommand(sqldel, connection);
                del.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Обновление паспортных данных
        /// </summary>
        /// <param name="AbiturientID">ИД абитуриента</param>
        /// <param name="dateOfIssue">Дата выдачи</param>
        /// <param name="dateOfBirth">Дата рождения</param>
        /// <param name="series">Серия паспорта</param>
        /// <param name="PasspornNum">Номер паспорта</param>
        /// <param name="name">Кем выданны документы</param>
        /// <param name="identNum">Идентификационный номер</param>
        public static void UpdatePasportData(int AbiturientID, string dateOfIssue, string dateOfBirth, string series, string PasspornNum, string name, string identNum)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Update_PasportData", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@dateVidachi", dateOfIssue);
                command.Parameters.AddWithValue("@dateOfBirth", dateOfBirth);
                command.Parameters.AddWithValue("@seriya", series);
                command.Parameters.AddWithValue("@pasportNum", PasspornNum);
                command.Parameters.AddWithValue("@vidan", name);
                command.Parameters.AddWithValue("@identNum", identNum);
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Паспортные данные");
            }
        }
        /// <summary>
        /// Получение списка спецальностей
        /// </summary>
        /// <param name="OnlyWithAdmissionsPlans">Только специальности где есть планы приема</param>
        /// <returns>Список масивов типа (краткое наименование, наименование)</returns>
        public static List<string[]> Get_SpecialnostiName(bool OnlyWithAdmissionsPlans)
        {
            List<string[]> Result = new List<string[]>();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_SpecialnostiName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@useFilter", OnlyWithAdmissionsPlans ? 1 : 0);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string[] arr = { reader.GetString(0), reader.GetString(1) };
                    Result.Add(arr);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return Result;
        }
        /// <summary>
        /// Плоучение списка планаов приема
        /// </summary>
        /// <param name="specialost">Краткое наименование специальности</param>
        /// <param name="budjet">Бюджет</param>
        /// <param name="hozrash">Хлзрасчет</param>
        /// <param name="bazovoe">Базовое</param>
        /// <param name="srednee">среднее</param>
        /// <param name="dnevnaya">Дневная</param>
        /// <param name="zaochnaya">Заочная</param>
        /// <returns>Список планов пиема</returns>
        public static List<PlanPriema> Get_PlaniPriema(string specialost, bool? budjet, bool? hozrash, bool? bazovoe, bool? srednee, bool? dnevnaya, bool? zaochnaya)
        {
            List<PlanPriema> AdmissionsPlans = new List<PlanPriema>();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_PlaniPriema", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@specialost", specialost);
                command.Parameters.AddWithValue("@budjet", budjet == true ? "Б%" : "");
                command.Parameters.AddWithValue("@hozrash", hozrash == true ? "Х%" : "");
                command.Parameters.AddWithValue("@bazovoe", bazovoe == true ? "%баз%" : "");
                command.Parameters.AddWithValue("@srednee", srednee == true ? "%сред%" : "");
                command.Parameters.AddWithValue("@dnevnaya", dnevnaya == true ? "Д%" : "");
                command.Parameters.AddWithValue("@zaochnaya", zaochnaya == true ? "З%" : "");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    AdmissionsPlans.Add(new PlanPriema
                    {
                        Year = reader.GetString(0),
                        NameSpec = reader.GetString(1),
                        NameFinance = reader.GetString(2),
                        NameForm = reader.GetString(3),
                        NameObrazovaie = reader.GetString(4),
                        Id = reader.GetInt32(5),
                        Writes = reader.GetInt32(6),
                        IdSpec = reader.GetInt32(8),
                        IdFinance = reader.GetInt32(9),
                        Count = reader.GetInt32(10),
                        CountCelevihMest = reader.GetInt32(11),
                        Ct = reader.GetBoolean(12),
                        IdForm = reader.GetInt32(13),
                        CodeSpec = reader.GetString(7)
                    });
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return AdmissionsPlans;
        }
        /// <summary>
        /// Получение плана приема по ИД
        /// </summary>
        /// <param name="id">ИД плана приема</param>
        /// <returns>План приема</returns>
        public static PlanPriema Get_PlanPriemaByID(int id)
        {
            PlanPriema planPriema = new PlanPriema();
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Get_PlanPrieaByID", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("id", id);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                planPriema = new PlanPriema
                {
                    Id = id,
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
            }
            finally
            {
                connection.Close();
            }
            return planPriema;
        }
        /// <summary>
        /// Получение статей абитуриента
        /// </summary>
        /// <param name="abiturientId">ИД абитуриента</param>
        /// <returns>Статьи</returns>
        public static string Get_StatiAbiturienta(int abiturientId)
        {
            string statyi = "";
            try
            {
                SqlConnection connection1 = new SqlConnection(connectionString);
                SqlCommand command1 = new SqlCommand("Get_StatiAbiturienta", connection1);
                command1.CommandType = CommandType.StoredProcedure;
                command1.Parameters.AddWithValue("@abiturient", abiturientId);
                connection1.Open();
                SqlDataReader reader1 = command1.ExecuteReader();

                while (reader1.Read())
                {
                    statyi += reader1[0] + " ";
                }
                connection1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return statyi;
        }
        /// <summary>
        /// Получение буквы специальности
        /// </summary>
        /// <param name="specialityName">Краткое наименование специальности</param>
        /// <returns>буква</returns>
        public static string Get_SpecialtyLetter(string specialityName)
        {
            string letter = "";
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sql1 = $"SELECT Буква FROM Специальность WHERE КраткоеНаименование = '{specialityName}'";
                SqlCommand command = new SqlCommand(sql1, connection);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                letter = reader.GetString(0);
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return letter;
        }
        /// <summary>
        /// Получение следующего номера экзаменационного листа
        /// </summary>
        /// <param name="AdmissionPlanID">Ид плана приема</param>
        /// <returns>намер экзаменационного листа</returns>
        public static int Get_NextExamList(int AdmissionPlanID)
        {
            int value = 1;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("NextExamList", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("id", AdmissionPlanID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader[0] != DBNull.Value)
                {
                    value = reader.GetInt32(0);
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return value;
        }
        /// <summary>
        /// Получение основных и пасспортных данных об абитуриенте
        /// </summary>
        /// <param name="AbiturientID">ИД абитуриента</param>
        /// <returns>данные об абитуриенте</returns>
        public static Abiturient Get_AbiturientFullInfo(int AbiturientID)
        {
            Abiturient abiturient = new Abiturient();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaFullInfo", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();

                abiturient.FIO = reader[0].ToString();
                abiturient.Shool = reader[1].ToString();
                abiturient.YearOfGraduation = reader[2].ToString();
                abiturient.BirthDate = reader[3].ToString() != "" ? DateTime.Parse(reader[3].ToString()).ToString("D") : "-";
                abiturient.PassportDateIssued = reader[4].ToString() != "" ? DateTime.Parse(reader[4].ToString()).ToString("D") : "-";
                abiturient.PassportSeries = reader[5].ToString();
                abiturient.PassportNum = reader[6].ToString();
                abiturient.PassportIssuedBy = reader[7].ToString();
                abiturient.PassportIdentnum = reader[8].ToString();
                abiturient.Сitizenship = reader[9].ToString();

                abiturient.WorkPlase = reader[10].ToString();
                abiturient.Position = reader[11].ToString();

                abiturient.Vladelec = reader[12].ToString();
                abiturient.Editor = reader[13].ToString();
                abiturient.Date = reader[14].ToString();
                abiturient.EditDate = reader[15].ToString();
                if ((bool)reader[16])
                    abiturient.Status = "Зачислен";
                else if ((bool)reader[17])
                    abiturient.Status = "Документы выданы";
                else
                    abiturient.Status = "Документы приняты";
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return abiturient;
        }
        /// <summary>
        /// Получение специальностей
        /// </summary>
        /// <returns>список специальностей</returns>
        public static List<Speciality> GetSpecialityTable() 
        {
            List<Speciality> list = new List<Speciality>();
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("SELECT * FROM Специальность", sqlConnection);
                sqlConnection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Speciality speciality = new Speciality
                    {
                        Num = reader.GetInt32(reader.GetOrdinal("IDСпециальность")),
                        Title = reader.GetString(reader.GetOrdinal("Наименование")),
                        Letter = reader.GetString(reader.GetOrdinal("Буква")),
                        ShortTitle = reader.GetString(reader.GetOrdinal("КраткоеНаименование")),
                        Code = reader.GetString(reader.GetOrdinal("Код"))
                    };
                    list.Add(speciality);
                }
                sqlConnection.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка получения специальностей");
            }
            return list;
        }
        /// <summary>
        /// Добавление специальности
        /// </summary>
        /// <param name="speciality">Специаьность</param>
        public static void InsertSpeciality(Speciality speciality)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("InsertSpeciality", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Title", speciality.Title);
                command.Parameters.AddWithValue("@ShortTitle", speciality.ShortTitle);
                command.Parameters.AddWithValue("@Letter", speciality.Letter);
                command.Parameters.AddWithValue("@Code", speciality.Code);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка создания специальности");
            }
        }
        /// <summary>
        /// Обновление специальности
        /// </summary>
        /// <param name="speciality">Специаьность</param>
        public static void UpdateSpeciality(Speciality speciality)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("UpdateSpeciality", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", speciality.Num);
                command.Parameters.AddWithValue("@Title", speciality.Title);
                command.Parameters.AddWithValue("@ShortTitle", speciality.ShortTitle);
                command.Parameters.AddWithValue("@Letter", speciality.Letter);
                command.Parameters.AddWithValue("@Code", speciality.Code);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка редактирования специальности");
            }
        }
        /// <summary>
        /// Удаление специальности
        /// </summary>
        /// <param name="id">ИД специаьности</param>
        public static void DeleteSpeciality(int id)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand($"DELETE Специальность WHERE IDСпециальность = {id}", connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления специальности");
            }
        }
    }
}
