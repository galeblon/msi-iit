using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MsiImageIndexer.exporters;
using MsiImageIndexer.inputWindow;
using MsiImageIndexer.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        private string lastSaveName = null;
        private int lastSaveFormat = 0;

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
                                Image = new Uri(f.FullName), 
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
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(lastSaveName == null) 
            {
                var fileDialog = new SaveFileDialog();
                fileDialog.Title = "Select output file path";
                fileDialog.Filter = "xml file (*.xml)|*.xml|csv file (*.csv)|*.csv";
                fileDialog.FilterIndex = 1;
                fileDialog.InitialDirectory = Environment.CurrentDirectory;

                bool? res = fileDialog.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    this.lastSaveName = fileDialog.FileName;
                    this.lastSaveFormat = fileDialog.FilterIndex;
                    SaveIndexedImages(this.lastSaveName, this.lastSaveFormat);
                }
            } 
            else 
            {
                SaveIndexedImages(this.lastSaveName, this.lastSaveFormat);
            }

        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new SaveFileDialog();
            fileDialog.Title = "Select output file path";
            fileDialog.Filter = "xml file (*.xml)|*.xml|csv file (*.csv)|*.csv";
            fileDialog.FilterIndex = 1;
            fileDialog.InitialDirectory = Environment.CurrentDirectory;

            bool? res = fileDialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                this.lastSaveName = fileDialog.FileName;
                this.lastSaveFormat = fileDialog.FilterIndex;
                SaveIndexedImages(this.lastSaveName, this.lastSaveFormat);
            }
        }

        private void SaveIndexedImages(string filePath, int mode) 
        {
            try 
            {
                IIndexedImagesExporter exporter = null;
                switch (mode)
                {
                    // XML
                    case 1:
                        exporter = new IndexedImagesXMLExporter();
                        break;
                    // CSV
                    case 2:
                        exporter = new IndexedImagesCSVExporter();
                        break;
                    default:
                        throw new NotImplementedException("Export mode not supported");

                }
                exporter.ExportIndexedImages(filePath, this.viewModel.PointCollection, this.viewModel.IndexedImages);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Image export error");
            }
        }


        private void PrecisionCanvas_Draw() 
        {
            ImageBrush imageBrush = this.viewModel.PrecisionImageBrush;

            if (imageBrush == null)
                return;

            double ScalePercentage = this.viewModel.ZoomLevel / 100.0;
            TransformGroup tg = new TransformGroup
            {
                Children =
                {
                    new ScaleTransform()
                    {
                        CenterX = 0,
                        CenterY = 0,
                        ScaleX = this.viewModel.XScale/PrecisionCanvas.ActualWidth * ScalePercentage,
                        ScaleY = this.viewModel.YScale/PrecisionCanvas.ActualHeight * ScalePercentage,
                    },
                    new TranslateTransform()
                    {
                        X = PrecisionCanvas.ActualWidth/2-this.viewModel.X * ScalePercentage,
                        Y = PrecisionCanvas.ActualHeight/2-this.viewModel.Y * ScalePercentage
                    }
                }
            };
            imageBrush.Transform = tg;
            PrecisionCanvas.Background = imageBrush;
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.viewModel.CurrentIndexedImage == null)
                return;
            double canvas_width = MainCanvas.ActualWidth;
            double canvas_height = MainCanvas.ActualHeight;
            double image_width = this.viewModel.XScale;
            double image_height = this.viewModel.YScale;

            this.viewModel.X = e.GetPosition(MainCanvas).X/canvas_width*image_width;
            this.viewModel.Y = e.GetPosition(MainCanvas).Y/canvas_height*image_height;

            PrecisionCanvas_Draw();
        }

        private void MainCavas_OnClickLeftDown(object sender, MouseEventArgs e) 
        {
            if (this.viewModel.CurrentNamedPoint == null)
                return;

            Bitmap bitMap = new Bitmap(this.viewModel.CurrentIndexedImage.Image.OriginalString);
            System.Drawing.Color pixelColor = bitMap.GetPixel((int)this.viewModel.X, (int)this.viewModel.Y);

            MarkedPoint markedPoint = new MarkedPoint {  };
            this.viewModel.CurrentIndexedImage.MarkedPoints.Add(new MarkedPoint 
            {
                NamedPoint = this.viewModel.CurrentNamedPoint,
                Colour = System.Windows.Media.Color.FromRgb((byte)(255-pixelColor.R), (byte)(255-pixelColor.G), (byte)(255-pixelColor.B)),
                X = this.viewModel.X,
                Y = this.viewModel.Y
            });
            this.viewModel.CurrentIndexedImage.PointsToMark.Remove(this.viewModel.CurrentNamedPoint);
            this.viewModel.CurrentNamedPoint = this.viewModel.CurrentIndexedImage.PointsToMark.FirstOrDefault();
            RefreshAll();
        }

        private void PreviousImageButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.viewModel.IndexedImages != null & this.viewModel.CurrentIndexedImage != null)
            {
                this.viewModel.CurrentIndexedImageIndex = this.viewModel.CurrentIndexedImageIndex == 0 ? this.viewModel.IndexedImages.Count - 1 : this.viewModel.CurrentIndexedImageIndex - 1;
                this.viewModel.CurrentNamedPoint = this.viewModel.CurrentIndexedImage.PointsToMark.FirstOrDefault();
                RefreshAll();
            }
        }

        private void NextImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.viewModel.IndexedImages != null & this.viewModel.CurrentIndexedImage != null)
            {
                this.viewModel.CurrentIndexedImageIndex = this.viewModel.CurrentIndexedImageIndex == this.viewModel.IndexedImages.Count - 1 ? 0 : this.viewModel.CurrentIndexedImageIndex + 1;
                this.viewModel.CurrentNamedPoint = this.viewModel.CurrentIndexedImage.PointsToMark.FirstOrDefault();
                RefreshAll();
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

        private void MarkedPointClear_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            MarkedPoint markedPoint = button.DataContext as MarkedPoint;
            this.viewModel.CurrentIndexedImage.PointsToMark.Add(markedPoint.NamedPoint);
            this.viewModel.CurrentIndexedImage.MarkedPoints.Remove(markedPoint);
            RefreshAll();
        }

        private void DrawPointsMainCanvas(Canvas canvas) 
        {
            canvas.Children.Clear();
            if (this.viewModel.CurrentIndexedImage == null)
                return;
            double canvas_width = MainCanvas.ActualWidth;
            double canvas_height = MainCanvas.ActualHeight;
            double image_width = this.viewModel.XScale;
            double image_height = this.viewModel.YScale;
            foreach (MarkedPoint mp in this.viewModel.CurrentIndexedImage.MarkedPoints) 
            {
                double x = mp.X * canvas_width / image_width;
                double y = mp.Y * canvas_height / image_height;
                Ellipse pt = new Ellipse()
                {
                    Stroke = new SolidColorBrush(mp.Colour),
                    Width = 4,
                    Height = 4,
                    StrokeThickness = 4,
                    Fill = new SolidColorBrush(mp.Colour),
                    Margin = new Thickness(x, y, 0, 0)
                };
                canvas.Children.Add(pt);

                if(MarkedPointsListBox.SelectedItem == mp) 
                {
                    TextBlock textBlock = new TextBlock()
                    {
                        Text = mp.NamedPoint.Name,
                        Foreground = new SolidColorBrush(mp.Colour),
                        FontSize = 16,
                        Margin = new Thickness(x, y-18, 0, 0)
                    };
                    canvas.Children.Add(textBlock);
                }
            }
        }

        private void RefreshAll() 
        {
            NamedPointsComboBox.Items.Refresh();
            MarkedPointsListBox.Items.Refresh();
            DrawPointsMainCanvas(MainCanvas);
            PrecisionCanvas_Draw();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawPointsMainCanvas(MainCanvas);
        }

        private void MarkedPointsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawPointsMainCanvas(MainCanvas);
        }

        private void MarkedPointsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(MarkedPointsListBox.SelectedItem != null) 
            {
                MarkedPoint item = (MarkedPoint)MarkedPointsListBox.SelectedItem;
                InputWindow popup = new InputWindow(item.X, item.Y);
                popup.ShowDialog();
                
                if(!InputWindow.cancelled) 
                {
                    item.X = InputWindow.xEditVal;
                    item.Y = InputWindow.yEditVal;
                    this.RefreshAll();
                }
            }
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(MarkedPoint mp in this.viewModel.CurrentIndexedImage.MarkedPoints) 
            {
                this.viewModel.CurrentIndexedImage.PointsToMark.Add(mp.NamedPoint);
            }
            this.viewModel.CurrentIndexedImage.MarkedPoints.Clear();
            RefreshAll();
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.ZoomLevel += 25;
            PrecisionCanvas_Draw();
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.ZoomLevel -= 25;
            PrecisionCanvas_Draw();
        }
    }
}
