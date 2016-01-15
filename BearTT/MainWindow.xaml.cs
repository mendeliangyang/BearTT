using BearTT.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml.Serialization;

namespace BearTT
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        ObservableCollection<TTarget> memberData = new ObservableCollection<TTarget>();

        private void init()
        {
            this.ch_AutoSave.IsChecked = Properties.Settings.Default.AutoSaveBearConf;
            memberData = new ObservableCollection<TTarget>();
            Stream fStream = null;
            try
            {
                string fileName = this.txt_TargetFilePath.Text.Trim();
                fStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                fStream.Position = 0;
                XmlSerializer xmlFormat = new XmlSerializer(typeof(ObservableCollection<TTarget>), new Type[] { typeof(TTarget) });
                memberData = (ObservableCollection<TTarget>)xmlFormat.Deserialize(fStream);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\r\n 初始化忽略该提示。");
                //this.Close();
            }
            finally
            {
                if (null != fStream)
                {
                    fStream.Close();
                }
            }
            //memberData.Add(new TTarget() { Name = "n0", Target = "t0", Param = "p0", HttpMethod = "get" });
            dg_tt.DataContext = memberData;
            this.txt_ThreadCount.Text = Properties.Settings.Default.DefaultBearRate.ToString();
            cbAutoStart.IsChecked=Properties.Settings.Default.AutoStartBear;
            if (Properties.Settings.Default.AutoStartBear)
            {
                btn_Start_Click(null,null);
            }
        }

        private void ttData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TTarget drv = dg_tt.SelectedItem as TTarget;
            if (null == drv)
            {
                return;
            }
            TTargetDetail t = new TTargetDetail();
            t.TargetDetail = drv;
            t.ShowDialog();
            if (t.GetModifyFlag())
            {
                //drv = t.TargetDetail;
                //this.memberData.Remove(drv);
                //this.memberData.Add(t.TargetDetail);
                OrderTarget();
            }
        }
        private void OrderTarget()
        {
            List<TTarget> tempTTarget = new List<TTarget>();
            while (0<memberData.Count)
            {
                TTarget target= memberData.First(fm=>fm.Order.Equals(memberData.Min( m=>m.Order)));
                tempTTarget.Add(target);
                memberData.Remove(target);
            }
            memberData.Clear();
            foreach (TTarget item in tempTTarget)
            {
                memberData.Add(item);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.txt_TargetFilePath.Text = baseDirectory + Properties.Settings.Default.DefaultTargetFileName;
            init();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.ch_AutoSave.IsChecked == true)
            {
                SaveTarget();
            }
        }

        private void SaveTarget()
        {
            Stream fStream = null;
            try
            {
                string fileName = this.txt_TargetFilePath.Text.Trim();
                if (File.Exists(this.txt_TargetFilePath.Text.Trim()))
                {
                    File.Delete(this.txt_TargetFilePath.Text.Trim());
                }
                fStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                XmlSerializer xmlFormat = new XmlSerializer(typeof(ObservableCollection<TTarget>), new Type[] { typeof(TTarget) });
                xmlFormat.Serialize(fStream, memberData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (null != fStream)
                {
                    fStream.Close();
                }
            }
        }

        private void btn_AddTarget_Click(object sender, RoutedEventArgs e)
        {
            TTargetDetail t = new TTargetDetail();
            t.ShowDialog();
            if (t.GetModifyFlag())
            {
                this.memberData.Add(t.TargetDetail);
                OrderTarget();
            }
        }

        private void btn_DeleteTarget_Click(object sender, RoutedEventArgs e)
        {
            TTarget drv = dg_tt.SelectedItem as TTarget;
            if (null == drv)
            {
                return;
            }
            this.memberData.Remove(drv);
        }

        private void btn_Reload_Click(object sender, RoutedEventArgs e)
        {
            init();
        }

        private void btn_SaveTarget_Click(object sender, RoutedEventArgs e)
        {
            SaveTarget();
        }

        bool startFlag = false;
        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            startFlag = startFlag == true ? false : true;

            if (startFlag)
            {
                this.btn_Start.Content = "Stop";
                int rate = 0;
                bool rateFlag = int.TryParse(this.txt_ThreadCount.Text.Trim(), out rate);
                rate = rateFlag == true ? rate : Properties.Settings.Default.DefaultBearRate;
                ThreadPool.SetMaxThreads(rate, rate);


                for (int i = 0; i < rate; i++)
                {
                    foreach (TTarget item in memberData)
                    {
                        item.HoldFlag = true;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(item.ToTarget));
                    }
                }
            }
            else
            {
                this.btn_Start.Content = "Start";
                foreach (TTarget item in memberData)
                {
                    item.HoldFlag = false;
                }
            }



        }

        private void cbAutoStart_Click(object sender, RoutedEventArgs e)
        {
           // Properties.Settings.Default.AutoStartBear = cbAutoStart.IsChecked;
        }

    }
}
