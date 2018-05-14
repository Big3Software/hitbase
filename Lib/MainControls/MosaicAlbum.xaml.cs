using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Big3.Hitbase.DataBaseEngine;
using System.IO;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Big3.Hitbase.SoundEngine;
using System.Windows.Media.Media3D;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for MosaicAlbum.xaml
    /// </summary>
    public partial class MosaicAlbum : Window
    {
        private DataBase dataBase;

        private DispatcherTimer dt = new DispatcherTimer();
        DispatcherTimer timer = new DispatcherTimer();

        private Random random = new Random();

        public Playlist Playlist { get; private set; }

        private string[] allFiles;

        // Für 3D
        const int imagesHorizontal = 21;      // Sollte ungerade sein
        const int imagesVertical = 7;         // Sollte ungerade sein

        bool modus3D = false;

        public MosaicAlbum(DataBase dataBase, Playlist playlist, bool modus3D)
        {
            this.modus3D = modus3D;
            this.Playlist = playlist;
            this.DataContext = Playlist;

            InitializeComponent();

            this.WaveUserCtrl.Playlist = playlist;

            this.dataBase = dataBase;

            if (!modus3D)
            {
                for (int i = 0; i < ThumbnailGrid.Rows * ThumbnailGrid.Columns; i++)
                {
                    Image img = new Image();
                    img.Stretch = Stretch.Fill;
                    ThumbnailGrid.Children.Add(img);
                }
            }

            dt.Interval = TimeSpan.FromMilliseconds(100);
            dt.Tick += new EventHandler(dt_Tick);

            if (!modus3D)
            {
                DoubleAnimation daOldMovie = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(20).Duration());
                daOldMovie.AutoReverse = true;
                daOldMovie.RepeatBehavior = RepeatBehavior.Forever;
                OldMoviePixelShader.BeginAnimation(Big3.Hitbase.SharedResources.OldMovie.ScratchAmountProperty, daOldMovie);
            }

            if (!playlist.IsPlaying)
                playlist.Play();

            DoubleAnimation daCurrentTrack = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1).Duration());
            daCurrentTrack.BeginTime = TimeSpan.FromSeconds(1);
            StackPanelCurrentTrack.BeginAnimation(StackPanel.OpacityProperty, daCurrentTrack);

            DoubleAnimation daNextTrack = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1).Duration());
            daNextTrack.BeginTime = TimeSpan.FromSeconds(2);
            GridNextTrack.BeginAnimation(Grid.OpacityProperty, daNextTrack);

            Update3DCover();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
        }

        public System.Windows.Shapes.Path PlayPausePath
        {
            get
            {
                if (Playlist.IsPaused)
                    return FindResource("PlayPath") as System.Windows.Shapes.Path;
                else
                    return FindResource("PausePath") as System.Windows.Shapes.Path;
            }
        }

        public System.Windows.Shapes.Path PlayPausePathHover
        {
            get
            {
                if (Playlist.IsPaused)
                    return FindResource("PlayPathHover") as System.Windows.Shapes.Path;
                else
                    return FindResource("PausePathHover") as System.Windows.Shapes.Path;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (Playlist.IsPlaying)
            {
                int position = Playlist.CurrentTrackPlayPosition;
                TextBlockStatus.Text = Misc.GetShortTimeString(position);
                ProgressBar.Value = position;
                ProgressBar.Maximum = Playlist.CurrentPlaylistItem.Info.Length;

                // Folgender Algorithmu
                // Es wird die Hälfte der Tracklänge genommen
                // d.h. nach dem ersten Viertel des Tracks wird angefangen zu Scrollen, dann bis zu 3/4 des tracks
                // Das Ganze linear

                double trackLength = ProgressBar.Maximum / 2;
                double percent = 100 / trackLength * (ProgressBar.Value - trackLength/2);
                double scrollOffset = ScrollViewerLyrics.ScrollableHeight * (percent / 100.0);
                if (scrollOffset < 0)
                    ScrollViewerLyrics.ScrollToVerticalOffset(0);
                else
                    ScrollViewerLyrics.ScrollToVerticalOffset(scrollOffset);
            }
            else
            {
                //Visibility = Visibility.Hidden;
                TextBlockStatus.Text = "";
                //ImageCover.Source = null;
                ProgressBar.Value = 0;
            }
        }


        private void Update3DCover()
        {
            Material coverMaterial = FindResource("CoverMaterial") as Material;

            Model3DGroup myModel3DGroup = new Model3DGroup();
            ModelVisual3D myModelVisual3D = new ModelVisual3D();

            AmbientLight light = new AmbientLight(Colors.White);
            myModel3DGroup.Children.Add(light);

            myModel3DGroup.Children.Add(Create3DRectangle(new Point3D(-10, 10, 0), new Point3D(10, 10, 0), new Point3D(-10, -10, 0), new Point3D(10, -10, 0), coverMaterial));
            myModelVisual3D.Content = myModel3DGroup;
            this.ViewPort3D.Children.Add(myModelVisual3D);
            
            // Y-Rotation
            Transform3DGroup transformGroup = new Transform3DGroup();
            AxisAngleRotation3D rotationY = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            transformGroup.Children.Add(new RotateTransform3D(rotationY));

            DoubleAnimation daY = new DoubleAnimation(-10, 10, TimeSpan.FromSeconds(5).Duration());
            daY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut };
            daY.AutoReverse = true;
            daY.RepeatBehavior = RepeatBehavior.Forever;
            rotationY.BeginAnimation(AxisAngleRotation3D.AngleProperty, daY);


            // X-Rotation
            AxisAngleRotation3D rotationX = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
            transformGroup.Children.Add(new RotateTransform3D(rotationX));

            DoubleAnimation daX = new DoubleAnimation(-5, 5, TimeSpan.FromSeconds(10).Duration());
            daX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut };
            daX.AutoReverse = true;
            daX.RepeatBehavior = RepeatBehavior.Forever;
            rotationX.BeginAnimation(AxisAngleRotation3D.AngleProperty, daX);

            myModel3DGroup.Transform = transformGroup;

        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (random.Next(0, 10) == 1)
            {
                int index = random.Next(0, ThumbnailGrid.Columns * ThumbnailGrid.Rows);
                ChangeImage(index);
            }
        }

        private void ChangeImage(int index)
        {
            if (allFiles.Length == 0)
                return;

            Image img = this.ThumbnailGrid.Children[index] as Image;
            DoubleAnimation da = new DoubleAnimation(0, TimeSpan.FromMilliseconds(500).Duration());

            da.Completed += delegate
            {
                int imageIndex = random.Next(0, allFiles.Length);
                BitmapImage bi = ImageLoader.GetThumbnailImage(allFiles[imageIndex]);
                img.Source = bi;
                DoubleAnimation da2 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500).Duration());
                img.BeginAnimation(Image.OpacityProperty, da2);
            };

            img.BeginAnimation(Image.OpacityProperty, da);
        }

        private void LoadImagesFromDatabase()
        {
            allFiles = this.dataBase.GetAllFrontCovers(true);

            if (allFiles.Length == 0)
                return;

            // Shuffle
            allFiles = allFiles.OrderBy(x => random.Next()).ToArray();

            int rowCount = ThumbnailGrid.Rows;
            int columnCount = ThumbnailGrid.Columns;
            List<BitmapImage> thumbnails = new List<BitmapImage>();
            int imageIndex = 0;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {

                for (int i = 0; i < rowCount * columnCount; i++)
                {
                    if (imageIndex >= allFiles.Length)
                        imageIndex = 0;

                    BitmapImage bi = ImageLoader.GetThumbnailImage(allFiles[imageIndex]);

                    if (bi == null)
                    {
                        i--;
                        imageIndex++;
                        continue;
                    }

                    thumbnails.Add(bi);

                    imageIndex++;
                }
            };
            bw.RunWorkerCompleted += delegate
            {
                imageIndex = 0;
                for (int i = 0; i < ThumbnailGrid.Rows * ThumbnailGrid.Columns; i++)
                {
                    Image img = ThumbnailGrid.Children[i] as Image;
                    img.Source = thumbnails[imageIndex];
                    DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500).Duration());
                    img.BeginAnimation(Image.OpacityProperty, da);

                    imageIndex++;
                    if (imageIndex >= thumbnails.Count)
                        imageIndex = 0;
                }
            };
            bw.RunWorkerAsync();
        }

        private void LoadImagesFromDisk()
        {
            allFiles = Directory.GetFiles(Big3.Hitbase.Miscellaneous.Misc.GetCDCoverDirectory());

            if (allFiles.Length == 0)
                return;

            // Shuffle
            allFiles = allFiles.OrderBy(x => random.Next()).ToArray();

            int imageIndex = 0;

            List<BitmapImage> thumbnails = new List<BitmapImage>();

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {

                for (int i = 0; i < ThumbnailGrid.Rows * ThumbnailGrid.Columns; i++)
                {
                    if (imageIndex >= allFiles.Length)
                        imageIndex = 0;

                    BitmapImage bi = ImageLoader.GetThumbnailImage(allFiles[imageIndex]);

                    if (bi == null)
                    {
                        i--;
                        imageIndex++;
                        continue;
                    }

                    thumbnails.Add(bi);

                    imageIndex++;
                }
            };
            bw.RunWorkerCompleted += delegate
            {
                imageIndex = 0;
                for (int i = 0; i < ThumbnailGrid.Rows * ThumbnailGrid.Columns; i++)
                {
                    Image img = ThumbnailGrid.Children[i] as Image;
                    img.Source = thumbnails[i];
                    DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500).Duration());
                    img.BeginAnimation(Image.OpacityProperty, da);
                }
            };
            bw.RunWorkerAsync();
        }

        private void Load3DImages()
        {
            Model3DGroup myModel3DGroup = new Model3DGroup();
            ModelVisual3D myModelVisual3D = new ModelVisual3D();

            AmbientLight light = new AmbientLight(Color.FromRgb(96,96,96));
            myModel3DGroup.Children.Add(light);

            /*PointLight light2 = new PointLight(Colors.White, new Point3D(0, 0, 0.3));
            myModel3DGroup.Children.Add(light2);
            Point3DAnimation daLight = new Point3DAnimation(new Point3D(-10, 0, 0.3), new Point3D(10, 0, 0.3), TimeSpan.FromMilliseconds(25000).Duration());
            light2.BeginAnimation(PointLight.PositionProperty, daLight);*/

            SpotLight light3 = new SpotLight(Colors.Red, new Point3D(0, 0, 10), new Vector3D(0, 0, -1), 90, 0);
            myModel3DGroup.Children.Add(light3);
            Vector3DAnimation daLight = new Vector3DAnimation(new Vector3D(-1, 0, -0.3), new Vector3D(1, 0, -0.3), TimeSpan.FromMilliseconds(15000).Duration());
            daLight.RepeatBehavior = RepeatBehavior.Forever;
            daLight.AutoReverse = true;
            light3.BeginAnimation(SpotLight.DirectionProperty, daLight);

            /*ColorAnimation colorAnimation = new ColorAnimation(Colors.Red, Color.FromRgb(0, 0, 255), TimeSpan.FromSeconds(10).Duration());
            colorAnimation.RepeatBehavior = RepeatBehavior.Forever;
            colorAnimation.AutoReverse = true;
            light3.BeginAnimation(SpotLight.ColorProperty, colorAnimation);*/

            myModelVisual3D.Content = myModel3DGroup;
            this.ImageWall3D.Children.Add(myModelVisual3D);

            allFiles = dataBase.GetAllFrontCovers(true);

            if (allFiles.Length == 0)
                return;

            // Shuffle
            allFiles = allFiles.OrderBy(file => random.Next()).ToArray();

            int imageIndex = 0;
            double xPosStart = -(double)imagesHorizontal / 2.0 - 31.415;
            double xPos = xPosStart;
            double yPos = (double)imagesVertical / 2.0;

            int xIndex = 0;
            int yIndex = 0;

            double centerX = 0;
            double centerZ = 20;
            double radius = 20;

            List<BitmapImage> thumbnails = new List<BitmapImage>();

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {

                for (int i = 0; i < imagesHorizontal * imagesVertical; i++)
                {
                    if (imageIndex >= allFiles.Length)
                        imageIndex = 0;

                    BitmapImage bi = ImageLoader.GetThumbnailImage(allFiles[imageIndex]);

                    if (bi == null)
                    {
                        i--;
                        imageIndex++;
                        continue;
                    }

                    thumbnails.Add(bi);

                    imageIndex++;
                }
            };
            bw.RunWorkerCompleted += delegate
            {
                imageIndex = 0;
                for (int i = 0; i < imagesHorizontal * imagesVertical; i++)
                {
                    double circleXPos = xPos / 20;
                    double circleXPos2 = xPos / 20 + 0.048;

                    double x1 = centerX + radius * Math.Cos(circleXPos);
                    double z1 = centerZ + radius * Math.Sin(circleXPos);

                    double x2 = centerX + radius * Math.Cos(circleXPos2);
                    double z2 = centerZ + radius * Math.Sin(circleXPos2);

                    Point3D leftTop = new Point3D(x1, yPos, z1);
                    Point3D rightTop = new Point3D(x2, yPos, z2);
                    Point3D leftBottom = new Point3D(x1, yPos - 0.9, z1);
                    Point3D rightBottom = new Point3D(x2, yPos - 0.9, z2);

                    /*                Point3D leftTop = new Point3D(xPos, yPos, 0);
                    Point3D rightTop = new Point3D(xPos+0.9, yPos, 0);
                    Point3D leftBottom = new Point3D(xPos, yPos-0.9, 0);
                    Point3D rightBottom = new Point3D(xPos+0.9, yPos-0.9, 0);*/
                    BitmapImage bi = thumbnails[imageIndex];

                    DiffuseMaterial material = new DiffuseMaterial(new ImageBrush(bi));

                    GeometryModel3D coverPlate = Create3DRectangle(leftTop, rightTop, leftBottom, rightBottom, material);

                    myModel3DGroup.Children.Add(coverPlate);

                    Transform3DGroup transformGroup = new Transform3DGroup();

                    TranslateTransform3D trans = new TranslateTransform3D();

                    DoubleAnimation daZ = new DoubleAnimation(30, 0, TimeSpan.FromMilliseconds(random.NextDouble() * 3000.0 + 5000.0).Duration());

                    daZ.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut };
                    trans.BeginAnimation(TranslateTransform3D.OffsetZProperty, daZ);

                    transformGroup.Children.Add(trans);

                    coverPlate.Transform = transformGroup;

                    xPos += 1;
                    xIndex++;

                    if (xIndex >= imagesHorizontal)
                    {
                        xIndex = 0;
                        xPos = xPosStart;

                        yIndex++;
                        yPos -= 1;
                    }

                    imageIndex++;
                    if (imageIndex >= thumbnails.Count)
                        imageIndex = 0;
                }
            };

            bw.RunWorkerAsync();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (modus3D)
            {
                Load3DImages();
                Grid3D.Visibility = System.Windows.Visibility.Visible;
                ThumbnailGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                LoadImagesFromDatabase();

                dt.Start();
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            dt.Stop();
            timer.Stop();
        }

        /*int lastChangeIndex = -1;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            int column = (int)(Mouse.GetPosition(ThumbnailGrid).X / (ThumbnailGrid.ActualWidth / ThumbnailGrid.Columns));
            int row = (int)(Mouse.GetPosition(ThumbnailGrid).Y / (ThumbnailGrid.ActualHeight/ ThumbnailGrid.Rows));

            int changeIndex = row * ThumbnailGrid.Columns + column;

            if (changeIndex != lastChangeIndex)
                this.ChangeImage(changeIndex);

            lastChangeIndex = changeIndex;
        }*/

        double lastMouseXPos = 0;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (modus3D && e.LeftButton == MouseButtonState.Pressed)
            {
            /*    double mouseXPos = Mouse.GetPosition(e.OriginalSource as IInputElement).X;

                if (mouseXPos > lastMouseXPos)
                {
                    MyCamera.LookDirection = new Vector3D(MyCamera.LookDirection.X + 0.01, MyCamera.LookDirection.Y, MyCamera.LookDirection.Z);
                }
                else
                {
                    MyCamera.LookDirection = new Vector3D(MyCamera.LookDirection.X - 0.01, MyCamera.LookDirection.Y, MyCamera.LookDirection.Z);
                }

                lastMouseXPos = mouseXPos;*/
            }
        }


        private GeometryModel3D Create3DRectangle(Point3D leftTop, Point3D rightTop, Point3D leftBottom, Point3D rightBottom, Material material)
        {
            GeometryModel3D geometryModel3D = new GeometryModel3D();

            MeshGeometry3D myMeshGeometry3D = new MeshGeometry3D();

            Point3DCollection myPositionCollection = new Point3DCollection();
            myPositionCollection.Add(leftTop);
            myPositionCollection.Add(rightTop);
            myPositionCollection.Add(leftBottom);
            myPositionCollection.Add(rightBottom);

            myMeshGeometry3D.Positions = myPositionCollection;

            // Create a collection of texture coordinates for the MeshGeometry3D.
            PointCollection myTextureCoordinatesCollection = new PointCollection();
            myTextureCoordinatesCollection.Add(new Point(0, 0));
            myTextureCoordinatesCollection.Add(new Point(1, 0));
            myTextureCoordinatesCollection.Add(new Point(0, 1));
            myTextureCoordinatesCollection.Add(new Point(1, 1));

            myMeshGeometry3D.TextureCoordinates = myTextureCoordinatesCollection;

            // Create a collection of triangle indices for the MeshGeometry3D.
            Int32Collection myTriangleIndicesCollection = new Int32Collection();
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(3);

            myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;
            geometryModel3D.Geometry = myMeshGeometry3D;
            geometryModel3D.BackMaterial = material;
            geometryModel3D.Material = material;
            return geometryModel3D;
        }

        private void ImageClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void MosaicAlbumWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }

        private void ProgressBar_PreviewMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double percentage = 100 / ProgressBar.ActualWidth * e.GetPosition(ProgressBar).X;

                Playlist.CurrentTrackPlayPosition = (int)(ProgressBar.Maximum / 100.0 * percentage);

                e.Handled = true;
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PauseButton.Visibility = System.Windows.Visibility.Visible;
            PlayButton.Visibility = System.Windows.Visibility.Collapsed;

            Playlist.Pause(false);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            PauseButton.Visibility = System.Windows.Visibility.Collapsed;
            PlayButton.Visibility = System.Windows.Visibility.Visible;

            Playlist.Pause(true);
        }

        private void PrevTrackButton_Click(object sender, RoutedEventArgs e)
        {
            Playlist.PlayPrev();
        }

        private void NextTrackButton_Click(object sender, RoutedEventArgs e)
        {
            Playlist.PlayNext();

        }

    }
}
