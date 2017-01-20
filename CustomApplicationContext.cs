﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using NLog;

namespace KodeksArmScheduler
{
    public delegate void refreshUI();
    public class CustomApplicationContext : ApplicationContext
    {
        private static readonly string IconFileName = "Images/stack.ico";
        private static readonly string DefaultTooltip = "Настройка импорта и отправки уведомлений для Кодекс АРМ";
        private static string connectionStringTemplate = @"data source={0}";

        private static object _lockContext = new object();
        private static SchedulerContext _db;
        public static SchedulerContext Db
        {
            get
            {
                lock(_lockContext)
                {
                    return _db;
                }
            }
        }
        refreshUI _refreshUI;

        static SchedulerEventManager _sem;

        public static SchedulerEventManager schedulerManager
        {
            get
            {
                return _sem;
            }
        }

		public CustomApplicationContext() 
		{
			InitializeContext();

            if (!openDbConnection())
            {
                ExitThread();
            }

            notifyIcon.ContextMenuStrip.Items.Clear();
            notifyIcon.ContextMenuStrip.Items.Add(toolStripMenuItemWithHandler("&Открыть..", showMainItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add(toolStripMenuItemWithHandler("&Настройки..", showSettingsItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add(toolStripMenuItemWithHandler("&Выход", exitItem_Click));
            startScheduler();
            ShowMainForm(); 
		}

        void startScheduler()
        {
            _refreshUI = refreshMainForm;
            _sem = new SchedulerEventManager(_refreshUI);
            _sem.startScheduler();
        }

        bool openDbConnection()
        {
            if (KodeksArmScheduler.Properties.Settings.Default.DbPath == "" ||
                KodeksArmScheduler.Properties.Settings.Default.DbPath.IndexOf(Application.StartupPath) < 0)
            {
                KodeksArmScheduler.Properties.Settings.Default.DbPath = Application.StartupPath + @"\scheduler.sqlite";
                KodeksArmScheduler.Properties.Settings.Default.Save();
            }

            if (!File.Exists(KodeksArmScheduler.Properties.Settings.Default.DbPath))
            {
                MessageBox.Show("Не найден файл базы данных программы или к нему отсутствует доступ"
                               , "Ошибка"
                               , MessageBoxButtons.OK
                               , MessageBoxIcon.Error);
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error("Не найден файл базы данных программы или к нему отсутствует доступ:" + KodeksArmScheduler.Properties.Settings.Default.DbPath);
                KodeksArmScheduler.Properties.Settings.Default.DbPath = "";
                KodeksArmScheduler.Properties.Settings.Default.Save();
                return false;
            }

            try
            {
                var connectionString = String.Format(connectionStringTemplate, KodeksArmScheduler.Properties.Settings.Default.DbPath);
                _db = new SchedulerContext(connectionString);
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show( "Не удалось открыть подключение к БД: " + ex.Message
                               , "Ошибка"
                               , MessageBoxButtons.OK
                               , MessageBoxIcon.Error);
                return false;
            }
        }        

        # region support methods

        public ToolStripMenuItem toolStripMenuItemWithHandler(string displayText, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null) { item.Click += eventHandler; }
            return item;
        }

        # endregion support methods

        # region forms

        private Form mainForm;

        private void ShowMainForm(string TabName = "tpMain")
        {
            if (mainForm == null)
            {
                mainForm = new fmMain(TabName);
                mainForm.Closed += mainForm_Closed;
                mainForm.Show();                
            }
            else 
            { 
                mainForm.Activate();
            }
            
        }

        void refreshMainForm()
        {
            if (mainForm == null)
                return;
            if (mainForm.InvokeRequired)
            {
                mainForm.Invoke(_refreshUI);
                return;
            }
            (mainForm as fmMain).refresh();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e) 
        { 
            ShowMainForm();    
        }

        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }

        private void showMainItem_Click(object sender, EventArgs e) 
        { 
            ShowMainForm(); 
        }

        private void showSettingsItem_Click(object sender, EventArgs e)
        {
            ShowMainForm("tpSettings");
        }

        private void mainForm_Closed(object sender, EventArgs e)        
        {
            mainForm = null;
        }

        private void contextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
        }

        # endregion the forms

        # region generic code 

        private System.ComponentModel.IContainer components;	// a list of components to dispose when the context is disposed
        private NotifyIcon notifyIcon;				            // the icon that sits in the system tray

        private void InitializeContext()
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(components)
                             {
                                 ContextMenuStrip = new ContextMenuStrip(),
                                 Icon = new Icon(IconFileName),
                                 Text = DefaultTooltip,
                                 Visible = true
                             };
            notifyIcon.ContextMenuStrip.Opening += contextMenuStripOpening;
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.MouseUp += notifyIcon_MouseUp;
        }

		protected override void Dispose( bool disposing )
		{
			if( disposing && components != null) { components.Dispose(); }
		}

        private void exitItem_Click(object sender, EventArgs e) 
		{
            closeApp();
		}

        public void closeApp()
        {
            if (
                MessageBox.Show("Вы действительно хотите закрыть программу? Исполнение событий по расписанию будет остановлено."
                               , "Подтверждение"
                               , MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                schedulerManager.stopScheduler();
                ExitThread();

            }
        }

        protected override void ExitThreadCore()
        {
            if (mainForm != null) { mainForm.Close(); }

            notifyIcon.Visible = false; 
            base.ExitThreadCore();
        }

        # endregion generic code

    }
}
