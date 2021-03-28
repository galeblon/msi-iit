using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MsiImageIndexer.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                        var images = directoryInfo.GetFiles().Where(f => Regex.Match(f.Extension, "(png)|(jpg)|(jpeg)", RegexOptions.IgnoreCase).Success);
                        var imagesToIndex = images.Select(f => 
                            new IndexedImage 
                            {
                                Image = new Uri(f.ToString()), 
                                MarkedPoints = new List<MarkedPoint>(), 
                                PointsToMark = new ObservableCollection<NamedPoint>(this.viewModel.PointCollection.Points.ToList()) 
                            })
                            .ToList();
                        this.viewModel.IndexedImages = imagesToIndex;
                        this.viewModel.CurrentIndexedImage = imagesToIndex.FirstOrDefault();
                        NamedPointsComboBox.SelectedIndex = 0;
                        MessageBox.Show($"Loaded {imagesToIndex.Count()} images to index", "Data load succes");
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

        private void MainCavas_OnClickLeftDown(object sender, MouseEventArgs e) 
        {
            // TODO unique colour
            MarkedPoint markedPoint = new MarkedPoint {  };
            this.viewModel.CurrentIndexedImage.MarkedPoints.Add(new MarkedPoint 
            {
                NamedPoint = this.viewModel.CurrentNamedPoint,
                Colour = Colors.Red,
                X = this.viewModel.X,
                Y = this.viewModel.Y
            });
            this.viewModel.CurrentIndexedImage.PointsToMark.Remove(this.viewModel.CurrentNamedPoint);
            this.viewModel.CurrentNamedPoint = this.viewModel.CurrentIndexedImage.PointsToMark.FirstOrDefault();
            NamedPointsComboBox.Items.Refresh();
            MarkedPointsListBox.Items.Refresh();
        }

        private void PreviousImageButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.viewModel.IndexedImages != null & this.viewModel.CurrentIndexedImage != null)
            {
                this.viewModel.CurrentIndexedImageIndex = this.viewModel.CurrentIndexedImageIndex == 0 ? this.viewModel.IndexedImages.Count - 1 : this.viewModel.CurrentIndexedImageIndex - 1;
                this.viewModel.CurrentNamedPoint = this.viewModel.CurrentIndexedImage.PointsToMark.FirstOrDefault();
                NamedPointsComboBox.Items.Refresh();
                MarkedPointsListBox.Items.Refresh();
            }
        }

        private void NextImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.viewModel.IndexedImages != null & this.viewModel.CurrentIndexedImage != null)
            {
                this.viewModel.CurrentIndexedImageIndex = this.viewModel.CurrentIndexedImageIndex == this.viewModel.IndexedImages.Count - 1 ? 0 : this.viewModel.CurrentIndexedImageIndex + 1;
                this.viewModel.CurrentNamedPoint = this.viewModel.CurrentIndexedImage.PointsToMark.FirstOrDefault();
                NamedPointsComboBox.Items.Refresh();
                MarkedPointsListBox.Items.Refresh();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                NamedPoint newSelectedNamedPoint = (NamedPoint)e.AddedItems[0];
                this.viewModel.CurrentNamedPoint = newSelectedNamedPoint;
            }
            else 
            {
                NamedPointsComboBox.SelectedIndex = 0;
            }
        }
    }
}
