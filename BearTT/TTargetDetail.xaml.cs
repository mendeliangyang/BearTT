using BearTT.Entity;
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
using System.Windows.Shapes;

namespace BearTT
{
    /// <summary>
    /// TTargetDetail.xaml 的交互逻辑
    /// </summary>
    public partial class TTargetDetail : Window
    {
        public TTarget TargetDetail { get; set; }

        private bool modifyFlag = false;

        public bool GetModifyFlag()
        {
            return modifyFlag;
        }

        public TTargetDetail()
        {
            InitializeComponent();


        }



        private void btn_Enter_Click(object sender, RoutedEventArgs e)
        {
            TargetDetail.Name = this.txtName.Text.Trim();
            TargetDetail.Target = this.txtTarget.Text.Trim();
            TargetDetail.HttpMethod = this.txtHttpMethod.Text.Trim();
            TargetDetail.Param = this.txtParam.Text.Trim();
            int tempOrder=0;
            if (int.TryParse(this.txtOrder.Text.Trim(), out tempOrder)) { TargetDetail.Order = tempOrder; } else { TargetDetail.Order = 99; }
            modifyFlag = true;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (null != TargetDetail)
            {
                this.txtName.Text = TargetDetail.Name;
                this.txtTarget.Text = TargetDetail.Target;
                this.txtHttpMethod.Text = TargetDetail.HttpMethod;
                this.txtParam.Text = TargetDetail.Param;
                
                this.txtOrder.Text = TargetDetail.Order.ToString();
            }
            else TargetDetail = new TTarget();

        }
    }
}
