using System;
using System.Windows;

namespace MsiImageIndexer.inputWindow
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public static double xEditVal;
        public static double yEditVal;
        public static bool cancelled = false;

        public InputWindow()
        {
            InitializeComponent();
        }

        public InputWindow(double x, double y)
        {
            InitializeComponent();
            this.textEditX.Text = x.ToString();
            this.textEditY.Text = y.ToString();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textEditX.Text) && !string.IsNullOrEmpty(this.textEditY.Text)) 
            {
                try 
                {
                    xEditVal = double.Parse(this.textEditX.Text);
                    yEditVal = double.Parse(this.textEditY.Text);
                    cancelled = false;
                    this.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Invalid coordinates value!");
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            cancelled = true;
            this.Close();
        }
    }
}
