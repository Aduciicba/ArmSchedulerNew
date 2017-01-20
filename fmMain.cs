﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;

namespace KodeksArmScheduler
{

    public partial class fmMain : Form
    {
        string _firstShownTabName = "tpMain";


        public fmMain(string TabName)
        {
            InitializeComponent();
            _firstShownTabName = TabName;
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            Icon = new Icon("Images/stack.ico");
            loadControls();
        }

        void loadControls()
        {
            SchedulerEventManager sem = CustomApplicationContext.schedulerManager;
            refreshLog();
            wdImportWeekDays.DataBindings.Add("WeekDays", sem.baseImportEvent, "eventWeekDaysXtra", false, DataSourceUpdateMode.OnPropertyChanged);
            teImportTime.DataBindings.Add("EditValue", sem.baseImportEvent, "EventTime", false, DataSourceUpdateMode.OnPropertyChanged);
            teImportTime.FormatEditValue += TeImportTime_FormatEditValue;
            wdMailWeekDays.DataBindings.Add("WeekDays", sem.baseMailEvent, "eventWeekDaysXtra", false, DataSourceUpdateMode.OnPropertyChanged);
            teMailTime.DataBindings.Add("EditValue", sem.baseMailEvent, "eventTime", false, DataSourceUpdateMode.OnPropertyChanged);
            teFileNameMask.DataBindings.Add("Text", KodeksArmScheduler.Properties.Settings.Default, "ImportFileMask", false, DataSourceUpdateMode.OnPropertyChanged);
            refreshImportPage();
            refreshMailPage();
            selectTabPage(_firstShownTabName);
        }

        private void TeImportTime_FormatEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
        {
            e.Handled = true;
        }

        #region Обновление и загрузка UI

        void refreshImportPage()
        {
            lbImportEmails.DataSource = CustomApplicationContext.schedulerManager.baseImportEvent.emailList;
            lbImportEmails.DisplayMember = "email";
        }
        void refreshMailPage()
        {
            lbMailEmails.DataSource = CustomApplicationContext.schedulerManager.baseMailEvent.emailList;
            lbMailEmails.DisplayMember = "email";
        }

        public void selectTabPage(string TabName)
        {
            try
            {
                tcMain.SelectedTabPage = tcMain.TabPages.Single(t => t.Name == TabName);
            }
            catch
            {

            }
        }

        void refreshLog()
        {
            var loglist = CustomApplicationContext.Db.EventLogs.ToList().OrderByDescending(x => x.event_time);

            gcEventsLog.DataSource = loglist;
        }

        public void refresh()
        {
            refreshLog();
        }

        private void tcMain_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page.Name == "tpSettings")
            {
                refreshSettings();
            }
        }

        private void tcMain_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            if (
                (e.PrevPage.Name == "tpImport" && CustomApplicationContext.schedulerManager.baseImportEvent.hasUnsavedChanges)
                ||
                (e.PrevPage.Name == "tpMail" && CustomApplicationContext.schedulerManager.baseMailEvent.hasUnsavedChanges)
               )
            {
                var res = MessageBox.Show("На вкладке есть не сохраненные изменения. Сохранить изменения?", "Подтвержление", MessageBoxButtons.YesNoCancel);
                if (res == System.Windows.Forms.DialogResult.Cancel)
                    e.Cancel = true;
                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    if (e.PrevPage.Name == "tpImport")
                        CustomApplicationContext.schedulerManager.baseImportEvent.Save();
                    if (e.PrevPage.Name == "tpMail")
                        CustomApplicationContext.schedulerManager.baseMailEvent.Save();
                }
            }

            if (e.Page.Name == "tpClose")
            {
                Program.applicationContext.closeApp();
                e.Cancel = true;
            }

        }

        #endregion

        #region Общие настройки

        void refreshSettings()
        {
            beImportDirPath.Text = KodeksArmScheduler.Properties.Settings.Default.ImportDirPath;
            beArmDbPath.Text = KodeksArmScheduler.Properties.Settings.Default.KodeksDbPath;
            teEmail.Text = KodeksArmScheduler.Properties.Settings.Default.Email;
            teMailLogin.Text = KodeksArmScheduler.Properties.Settings.Default.EmailLogin;
            teMailPassword.Text = KodeksArmScheduler.Properties.Settings.Default.EmailPassword;
            teMailServer.Text = KodeksArmScheduler.Properties.Settings.Default.SmtpServer;
            teMailServerPort.Text = KodeksArmScheduler.Properties.Settings.Default.SmtpPort;
        }
        private void btnTestMail_Click(object sender, EventArgs e)
        {
            if (validateSettings())
            {
                MailSender ms = new MailSender(
                                                teEmail.Text.Trim()
                                              , Int32.Parse(teMailServerPort.Text.Trim())
                                              , teMailServer.Text.Trim()
                                              , teMailLogin.Text.Trim()
                                              , teMailPassword.Text.Trim());
                string err_msg = ms.SendTestMessage();
                if (err_msg == "")
                {
                    MessageBox.Show("Проверка пройдена, настройки введены корректно", "Уведомление", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Не удалось отпарвить сообщение: " + err_msg, "Ошибка", MessageBoxButtons.OK);
                }
            }
        }

        bool validateSettings()
        {
            if (teEmail.Text.Trim() == "")
            {
                MessageBox.Show("Адрес электронной почты не должен быть пустым", "Ошибка", MessageBoxButtons.OK);
                return false;
            }
            if (teMailLogin.Text.Trim() == "")
            {
                MessageBox.Show("Имя пользователя не должно быть пустым", "Ошибка", MessageBoxButtons.OK);
                return false;
            }
            if (teMailPassword.Text.Trim() == "")
            {
                MessageBox.Show("Пароль не должен быть пустым", "Ошибка", MessageBoxButtons.OK);
                return false;
            }
            if (teMailServer.Text.Trim() == "")
            {
                MessageBox.Show("Адрес сервера исходящей почты не должен быть пустым", "Ошибка", MessageBoxButtons.OK);
                return false;
            }
            if (teMailServerPort.Text.Trim() == "")
            {
                MessageBox.Show("Номер порта почтового сервера не должен быть пустым", "Ошибка", MessageBoxButtons.OK);
                return false;
            }
            else
            {
                int res = 0;
                if (!Int32.TryParse(teMailServerPort.Text.Trim(), out res))
                {
                    MessageBox.Show("Номер порта почтового сервера должен быть числом", "Ошибка", MessageBoxButtons.OK);
                    return false;
                }

            }
            return true;
        }

        private void beImportDirPath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (fbdImportDir.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                beImportDirPath.Text = fbdImportDir.SelectedPath;
            }
        }

        private void beArmDbPath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ofdDbPath.Multiselect = false;

            if (ofdDbPath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                beArmDbPath.Text = ofdDbPath.FileName;
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            KodeksArmScheduler.Properties.Settings.Default.ImportDirPath = beImportDirPath.Text;
            KodeksArmScheduler.Properties.Settings.Default.KodeksDbPath = beArmDbPath.Text;
            KodeksArmScheduler.Properties.Settings.Default.Email = teEmail.Text.Trim();
            KodeksArmScheduler.Properties.Settings.Default.EmailLogin = teMailLogin.Text.Trim();
            KodeksArmScheduler.Properties.Settings.Default.EmailPassword = teMailPassword.Text.Trim();
            KodeksArmScheduler.Properties.Settings.Default.SmtpServer = teMailServer.Text.Trim();
            KodeksArmScheduler.Properties.Settings.Default.SmtpPort = teMailServerPort.Text.Trim();
            KodeksArmScheduler.Properties.Settings.Default.Save();
        }
        #endregion        

        private void btnSaveMail_Click(object sender, EventArgs e)
        {
            CustomApplicationContext.schedulerManager.baseMailEvent.Save();
        }

        private void btnSaveImport_Click(object sender, EventArgs e)
        {
            CustomApplicationContext.schedulerManager.baseImportEvent.Save();
            KodeksArmScheduler.Properties.Settings.Default.Save();
        }

        #region Редактирование Email

        private void btnAddImportEmail_Click(object sender, EventArgs e)
        {
            EventEmail em = new EventEmail()
            {
                fid_event = CustomApplicationContext.schedulerManager.baseImportEvent.sourceEvent.id_event
            };
            editEmail(em);
        }

        private void btnAddMailEmail_Click(object sender, EventArgs e)
        {
            EventEmail em = new EventEmail()
            {
                fid_event = CustomApplicationContext.schedulerManager.baseMailEvent.sourceEvent.id_event
            };
            editEmail(em);
        }

        private void btnEditImportEmail_Click(object sender, EventArgs e)
        {
            EventEmail em = lbImportEmails.SelectedItem as EventEmail;
            editEmail(em);
        }

        private void btnEditMailEmail_Click(object sender, EventArgs e)
        {
            EventEmail em = lbMailEmails.SelectedItem as EventEmail;
            editEmail(em);
        }

        private void btnDelMailEmail_Click(object sender, EventArgs e)
        {
            EventEmail em = lbMailEmails.SelectedItem as EventEmail;
            deleteEmail(em);
            CustomApplicationContext.schedulerManager.refreshEmails(em.fid_event);
            refreshMailPage();
        }

        private void btnDelImportEmail_Click(object sender, EventArgs e)
        {
            EventEmail em = lbImportEmails.SelectedItem as EventEmail;
            deleteEmail(em);
            CustomApplicationContext.schedulerManager.refreshEmails(em.fid_event);
            refreshImportPage();
        }

        void editEmail(EventEmail em)
        {
            if (em == null)
            {
                MessageBox.Show("Выберите email для редактирования", "Ошибка");
                return;
            }
            var fm = new fmEmailEdit();
            fm.FormClosed += fmEmailEditClosed;
            fm.EditingEmail = em;
            fm.ShowDialog();
        }

        void fmEmailEditClosed(object sender, FormClosedEventArgs e)
        {
            fmEmailEdit fm = sender as fmEmailEdit;
            if (fm.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (fm.EditingEmail.id_event_email == 0)
                {
                    CustomApplicationContext.Db.EventEmails.Add(fm.EditingEmail);
                }
                else
                {
                    //var mail = 
                    //CustomApplicationContext.Db.EventEmails.Attach(fm.EditingEmail);
                }
                CustomApplicationContext.Db.SaveChanges();
                CustomApplicationContext.schedulerManager.refreshEmails(fm.EditingEmail.fid_event);
                refreshImportPage();
                refreshMailPage();
            }
        }

        void deleteEmail(EventEmail em)
        {
            if (em == null)
            {
                MessageBox.Show("Выберите email для удаления", "Ошибка");
                return;
            }

            if (
                 MessageBox.Show(
                                  "Вы действительно хотите удалить выбранный email из списка?"
                                , "Подтверждение"
                                , MessageBoxButtons.YesNo
                                ) == System.Windows.Forms.DialogResult.Yes)
            {
                var email = CustomApplicationContext.Db.EventEmails.FirstOrDefault(x => x.id_event_email == em.id_event_email);
                if (email != null)
                {
                    CustomApplicationContext.Db.EventEmails.Remove(email);
                    CustomApplicationContext.Db.SaveChanges();
                }
            }
        }

        #endregion

        private void panel1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(KodeksArmScheduler.Properties.Settings.Default.DbPath);
            MessageBox.Show(KodeksArmScheduler.Properties.Settings.Default.ImportFileMask);
            MessageBox.Show(CustomApplicationContext.Db.Events.Count().ToString());
        }


    }

}
