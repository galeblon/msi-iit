using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MsiImageIndexer.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using PointCollection = MsiImageIndexer.model.PointCollection;

namespace MsiImageIndexer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel viewModel;
        public MainWindow()
        {
            this.viewModel = new ViewModel();
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void LoadConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select a configuration file";
            fileDialog.Filter = "xml files|*.xml";
            fileDialog.FilterIndex = 0;
            fileDialog.InitialDirectory = Environment.CurrentDirectory;

            bool? res = fileDialog.ShowDialog();
            if (res.HasValue && res.Value) 
            {
                string fileName = fileDialog.FileName;
                XmlSerializer serializer = new XmlSerializer(typeof(PointCollection));

                using (StreamReader reader = new StreamReader(fileName)) 
                {
                    try
                    {
                        this.viewModel.PointCollection = (PointCollection)serializer.Deserialize(reader);
                        this.viewModel.ConfigNameLabel = fileName;
                    } 
                    catch (Exception ex) 
                    {
                        MessageBox.Show(ex.Message, "Config load error");
                    }
                }
            }
        }

        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            var dirDialog = new CommonOpenFileDialog();
            dirDialog.Title = "Select directory with images to index";
            dirDialog.IsFolderPicker = true;
            dirDialog.InitialDirectory = Environment.CurrentDirectory;
            if(dirDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string dirName = dirDialog.FileName;
                if (Directory.Exists(dirName))
                {
                    try 
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(dirName);
                        int count = directoryInfo.GetFiles().Select(f => Regex.Match(f.Extension, "(png)|(jpg)|(jpeg)", RegexOptions.IgnoreCase)).Count();
                        MessageBox.Show($"Loaded {count} images to index", "Data load succes");
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Data load error");
                    }
                }
                else 
                {
                    MessageBox.Show("No such directory", "Data load error");
                }
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // TODO recalculate according to scale of real image

            this.viewModel.X = e.GetPosition(MainCanvas).X;
            this.viewModel.Y = e.GetPosition(MainCanvas).Y;
        }
    }
}
