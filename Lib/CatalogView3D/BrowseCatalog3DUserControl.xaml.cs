//#define KINECT

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using Big3.Hitbase.DataBaseEngine;
using System.IO;
using System.Windows.Media.Animation;
using System.Reflection;
using Big3.Hitbase.Miscellaneous;
using System.ComponentModel;
using Big3.Hitbase.SoundEngineGUI;
using System.Windows.Threading;
using System.Threading;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.CDUtilities;
using System.Windows.Media.Effects;
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.SharedResources;
using System.Collections.ObjectModel;
#if KINECT
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SwipeGestureRecognizer;
using System.Diagnostics;
#endif

namespace Big3.Hitbase.CatalogView3D
{
    /// <summary>
    /// Interaction logic for BrowseCatalogUserControl.xaml
    /// </summary>
    public partial class BrowseCatalog3DUserControl : UserControl, Big3.Hitbase.Controls.DragDrop.IDropTarget
    {
        public class CDModel
        {
            public double XPosition;

            public Model3DGroup ModelCD;
            public Transform3DGroup ModelCDTransformGroup;

            public BitmapImage FrontImage;
            public BitmapImage BackImage;

            public GeometryModel3D FrontImageModel;
            public GeometryModel3D BackImageModel;
            public GeometryModel3D FrontImageModelMirror;
            public GeometryModel3D BackImageModelMirror;

            public CDItem CDItem;

            public int CDID;
        }

        Model3DGroup myModel3DGroup = new Model3DGroup();
        ModelVisual3D myModelVisual3D = new ModelVisual3D();
        PerspectiveCamera myPCamera = new PerspectiveCamera();
        Point pressedPosition;
        Point lastPosition;
        bool zoomActive = false;
        Point3D startCamera = new Point3D(0, 6.2, 30);
        List<CDModel> ModelsCD = new List<CDModel>();
        int currentVisibleCD;
        int lastCurrentVisibleCD = -1;
        int numberOfVisibleCDs = 11;       // Sollte immer ungerade sein
        AmbientLight light = new AmbientLight(Colors.Black);
        DoubleAnimation animMoveFront = new DoubleAnimation();
        
        ImageBrush imgBrushFrontGlass;
        DiffuseMaterial frontGlassMaterial;
        ImageBrush frontLeftBrush;
        DiffuseMaterial frontLeftMaterial;

        bool moveAround = false;
        bool backCoverVisible = false;

        SafeObservableCollection<CDItem> cdItems = new SafeObservableCollection<CDItem>();

        ListCollectionView listCollectionView = null;
        bool searchVisible = false;
        bool scrollingActive = false;
        bool scrollingTooFast = false;

        int rightmostCDIndex;
        GeometryModel3D ground = null;

        private string searchText = "";

        private PlaylistItem lastPlaylistItem = null;
        private bool dataLoaded = false;

        public bool PartyModusActive { get; set; }
        public bool FullScreenActive { get; set; }

        public DataBase DataBase { get; set; }

        private Playlist playlist;
        public Playlist Playlist 
        { 
            get
            {
                return playlist;
            }
            set
            {
                playlist = value;
                listViewPlaylist.ItemsSource = playlist;
            }
        }

        private Wishlist wishlist;
        public Wishlist Wishlist
        {
            get
            {
                return wishlist;
            }
            set
            {
                wishlist = value;
                listViewWishlist.ItemsSource = wishlist;
            }
        }

        private int realVisibleCDIndex
        {
            get
            {
                return rightmostCDIndex - numberOfVisibleCDs + currentVisibleCD + 1;
            }
        }

        DispatcherTimer dtCover = new DispatcherTimer();

        public BrowseCatalog3DUserControl()
        {
            DataContext = this;
            
            InitializeComponent();

            // Erst mal disablen, bis die Daten geladen sind.
            textBoxSearch.IsEnabled = false;

            this.Background = new SolidColorBrush(Colors.Black);
            // Declare scene objects.
            // Defines the camera used to view the 3D object. In order to view the 3D object,
            // the camera must be positioned and pointed such that the object is within view 
            // of the camera.

            // Specify where in the 3D scene the camera is.
            myPCamera.Position = startCamera;

            // Specify the direction that the camera is pointing.
            myPCamera.LookDirection = new Vector3D(0, 0, -1);

            // Define camera's horizontal field of view in degrees.
            myPCamera.FieldOfView = 60;

            // Asign the camera to the viewport
            Viewport3D.Camera = myPCamera;
            Viewport3D.ClipToBounds = false;
            Viewport3D.IsHitTestVisible = false;

            // Add the group of models to the ModelVisual3d.
            BitmapImage bitmapImageFrontGlass = new BitmapImage(new Uri("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/glaseffekt-frontglas.png"));
            imgBrushFrontGlass = new ImageBrush(bitmapImageFrontGlass);
            imgBrushFrontGlass.Opacity = 0.7;

            frontGlassMaterial = new DiffuseMaterial(imgBrushFrontGlass);


            BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/cdblackborder.jpg"));
            frontLeftBrush = new ImageBrush(bitmapImage);
            frontLeftMaterial = new DiffuseMaterial(frontLeftBrush);

            this.grid.MouseWheel += new MouseWheelEventHandler(BrowseCatalog3DUserControl_MouseWheel);
            this.grid.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(BrowseCatalog3D_MouseLeftButtonDown);
            this.grid.MouseMove += new System.Windows.Input.MouseEventHandler(BrowseCatalog3D_MouseMove);
            this.grid.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(BrowseCatalog3D_MouseLeftButtonUp);

            AddWishUserControl.OKClick += new EventHandler(AddWishUserControl_OKClick);
            AddWishUserControl.CancelClick += new EventHandler(AddWishUserControl_CancelClick);

            dtCover.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dtCover.Tick += new EventHandler(dtCover_Tick);
            dtCover.Start();

            //timerSearch.Interval = TimeSpan.FromMilliseconds(500);
            //timerSearch.Tick += new EventHandler(timerSearch_Tick);

            Loaded += new RoutedEventHandler(BrowseCatalog3DUserControl_Loaded);
            Unloaded += new RoutedEventHandler(BrowseCatalog3DUserControl_Unloaded);

            if (!DesignerProperties.GetIsInDesignMode(this))
                VolumeSlider.Value = SoundEngine.SoundEngine.Instance.Volume;

            if (Settings.Current.PartyModePlaylistPinned)
            {
                PlaylistGrid.Margin = new Thickness(0, 22, 22, 10);
                ButtonStickPlaylist.Style = (Style)FindResource("MetroStickyPinButtonStyle");
            }

            if (Settings.Current.PartyModeWishlistPinned)
            {
                WishlistGrid.Margin = new Thickness(0, 22, 0, 10);
                ButtonStickWishlist.Style = (Style)FindResource("MetroStickyPinButtonStyle");
            }
        }

        void BrowseCatalog3DUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync();


            this.Focus();
            Keyboard.Focus(this);
#if KINECT
            InitKinect();
#endif
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            InitializeData();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listCollectionView = new ListCollectionView(cdItems);
            listCollectionView.Filter = FilterRow;

            GridLoadingCircle.Visibility = Visibility.Collapsed;
            ScrollBarCD.Visibility = Visibility.Visible;

            CreateCDs();
            Fade();
            ShowCDTitle();

            dataLoaded = true;

            ColorAnimation anim = new ColorAnimation(Colors.Black, Colors.White, new Duration(new TimeSpan(0, 0, 2)));
            light.BeginAnimation(AmbientLight.ColorProperty, anim);

            textBoxSearch.IsEnabled = true;
        }

        void BrowseCatalog3DUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ModelsCD.Clear();
            cdItems.Clear();
            Viewport3D.Children.Clear();
            myModel3DGroup.Children.Clear();
            dtCover.Stop();
        }

        void BrowseCatalog3DUserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                MoveLeft();
            if (e.Delta > 0)
                MoveRight();
        }

        private GeometryModel3D GetGround()
        {
            GeometryModel3D myGeometryModel = new GeometryModel3D();
            MeshGeometry3D myMeshGeometry3D = new MeshGeometry3D();

            // Create a collection of normal vectors for the MeshGeometry3D.
            Vector3DCollection myNormalCollection = new Vector3DCollection();
            myNormalCollection.Add(new Vector3D(0, 1, 0));
            myNormalCollection.Add(new Vector3D(0, 1, 0));
            myNormalCollection.Add(new Vector3D(0, 1, 0));
            myNormalCollection.Add(new Vector3D(0, 1, 0));
            myNormalCollection.Add(new Vector3D(0, 1, 0));
            myNormalCollection.Add(new Vector3D(0, 1, 0));
            myMeshGeometry3D.Normals = myNormalCollection;

            // Create a collection of vertex positions for the MeshGeometry3D. 
            Point3DCollection myPositionCollection = new Point3DCollection();
            myPositionCollection.Add(new Point3D(-10000, 0, 21));
            myPositionCollection.Add(new Point3D(-10000, 0, -18.0));
            myPositionCollection.Add(new Point3D(10000, 0, -18.0));
            myPositionCollection.Add(new Point3D(10000, 0, 21));
            myMeshGeometry3D.Positions = myPositionCollection;

            // Create a collection of texture coordinates for the MeshGeometry3D.
            PointCollection myTextureCoordinatesCollection = new PointCollection();
            myTextureCoordinatesCollection.Add(new Point(0, 1));
            myTextureCoordinatesCollection.Add(new Point(0, 0));
            myTextureCoordinatesCollection.Add(new Point(1, 0));
            myTextureCoordinatesCollection.Add(new Point(1, 1));
            myTextureCoordinatesCollection.Add(new Point(1, 0));
            myTextureCoordinatesCollection.Add(new Point(0, 1));

            myMeshGeometry3D.TextureCoordinates = myTextureCoordinatesCollection;

            // Create a collection of triangle indices for the MeshGeometry3D.
            Int32Collection myTriangleIndicesCollection = new Int32Collection();
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(3);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(0);

            myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

            // Apply the mesh to the geometry model.
            myGeometryModel.Geometry = myMeshGeometry3D;

            LinearGradientBrush brush = new LinearGradientBrush(Colors.Transparent, Colors.Black, 90);
            brush.StartPoint = new Point(0, 0.0);
            brush.EndPoint = new Point(0, 0.9);
            DiffuseMaterial myMaterial = new DiffuseMaterial(brush);
            myGeometryModel.Material = myMaterial;
             
            return myGeometryModel;
        }

        void BrowseCatalog3D_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (zoomActive)
                zoomActive = false;
        }

        void BrowseCatalog3D_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (zoomActive)
            {
                Point pt = e.GetPosition(this);

                Point3D cameraPos = myPCamera.Position;
                cameraPos.X -= ((double)pt.X - (double)lastPosition.X) / 100.0;
                cameraPos.Z -= ((double)pt.Y - (double)lastPosition.Y) / 100.0;
                myPCamera.Position = cameraPos;
                lastPosition = e.GetPosition(this);
            }
        }

        void BrowseCatalog3D_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (moveAround)
            {
                zoomActive = true;
                pressedPosition = e.GetPosition(this);
                lastPosition = e.GetPosition(this);
            }
            else
            {
                if (sender == grid)
                    grid.Focus();

                Point mousePos = e.GetPosition(this);

                if (currentVisibleCD >= 0 && mousePos.X >= ActualWidth / 3 && mousePos.X < ActualWidth / 3 * 2)
                {
                    TurnCover();
                }
            }
        }

        private void TurnCover()
        {
            if (currentVisibleCD >= ModelsCD.Count)
                return;

            if (backCoverVisible)
                gridTrackList.Visibility = Visibility.Hidden;

            DoubleAnimation animRotateLeft = new DoubleAnimation();
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            animRotateLeft.From = 0;
            animRotateLeft.To = 180;
            animRotateLeft.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
            animRotateLeft.AccelerationRatio = 0.8;
            if (!backCoverVisible)
                animRotateLeft.Completed += new EventHandler(animRotateLeft_Completed);

            myAxisAngleRotation3d.Axis = new Vector3D(0, 1, 0);
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;
            
            myRotateTransform3D.CenterX = ModelsCD[currentVisibleCD].XPosition + 0.45;
            myRotateTransform3D.CenterZ = 0;
            //myAxisAngleRotation3d.Angle = 180;
            myAxisAngleRotation3d.BeginAnimation(AxisAngleRotation3D.AngleProperty, animRotateLeft);

            BitmapImage bi = GetBackCover(ModelsCD[currentVisibleCD].CDItem, ModelsCD[currentVisibleCD].FrontImage);
            ImageBrush imgBrushBack = new ImageBrush(bi);
            ModelsCD[currentVisibleCD].BackImageModel.Material = new DiffuseMaterial(imgBrushBack);
            ModelsCD[currentVisibleCD].BackImageModelMirror.Material = new DiffuseMaterial(imgBrushBack);
            ModelsCD[currentVisibleCD].BackImageModelMirror.BackMaterial = new DiffuseMaterial(imgBrushBack); 

            ModelsCD[currentVisibleCD].ModelCDTransformGroup.Children.Add(myRotateTransform3D);

            if (backCoverVisible)
                backCoverVisible = false;
            else
                backCoverVisible = true;
        }

        void animRotateLeft_Completed(object sender, EventArgs e)
        {
            if (backCoverVisible)
            {
                ShowTrackList(ModelsCD[currentVisibleCD].CDID);
            }
        }

        private void ShowTrackList(int CDID)
        {
            CD theCD = DataBase.GetCDById(CDID);

            listBoxTrackList.ItemsSource = theCD.Tracks;
            if (theCD.Tracks.Count > 0)
                listBoxTrackList.ScrollIntoView(theCD.Tracks[0]);
            gridTrackList.Visibility = Visibility.Visible;
            DoubleAnimation anim2 = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            gridTrackList.BeginAnimation(Label.OpacityProperty, anim2);
        }

        private void NowPlaying(PlaylistItem track)
        {
            DoubleAnimation animOut = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 1)));

            animOut.Completed += delegate(object sender, EventArgs e)
            {
                currentCDLine1.Text = track.Info.Artist;
                currentCDLine2.Text = track.Info.Title;

                currentPlayingTrackCover.Source = track.TrackImage;

                DoubleAnimation animIn = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 1)));
                gridCurrentPlayingCD.BeginAnimation(Label.OpacityProperty, animIn);
            };

            gridCurrentPlayingCD.BeginAnimation(Label.OpacityProperty, animOut);
        }

        void track_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid trackGrid = (Grid)sender;
            Big3.Hitbase.DataBaseEngine.Track track = (Big3.Hitbase.DataBaseEngine.Track)trackGrid.Tag;

            Color color = Color.FromArgb(0xB2, 0x8D, 0xBD, 0xFF);

            ColorAnimation anim = new ColorAnimation(color, Colors.Transparent, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            trackGrid.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }

        void track_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid trackGrid = (Grid)sender;
            Color color = Color.FromArgb(0xB2, 0x8D, 0xBD, 0xFF);
            ColorAnimation anim = new ColorAnimation(Colors.Transparent, color, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            trackGrid.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }

        private void CreateCDs()
        {
            lastCurrentVisibleCD = -1;

            Viewport3D.Children.Clear();
            myModel3DGroup.Children.Clear();
            ModelsCD.Clear();

            myModel3DGroup.Children.Add(light);
            rightmostCDIndex = 0;

            for (int i = 0; i < listCollectionView.Count;i++ )
            {
                CDModel cdModel = CreateCD(i);
                ModelsCD.Add(cdModel);

                AddCDModelToSurface(cdModel);

                if (i >= numberOfVisibleCDs - 1)
                    break;
            }

            ScrollBarCD.Maximum = listCollectionView.Count;
            ScrollBarCD.ViewportSize = (double)listCollectionView.Count / 10.0;

            rightmostCDIndex = numberOfVisibleCDs-1;

            // Bodenplatte
            ground = GetGround();
            myModel3DGroup.Children.Add(ground);

            myModelVisual3D.Content = myModel3DGroup;

            this.Viewport3D.Children.Add(myModelVisual3D);

            if (currentVisibleCD < ModelsCD.Count)
                myPCamera.Position = new Point3D(ModelsCD[currentVisibleCD].XPosition, 6.2, 30);
            else
                myPCamera.Position = new Point3D(0, 6.2, 30);
        }

        private void InitializeData()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                searchText = textBoxSearch.HasText ? textBoxSearch.Text : "";
            }));

            SortFieldCollection sfc = new SortFieldCollection();
            sfc.Add(Field.ArtistCDName);
            sfc.Add(Field.Title);
            sfc.Add(Field.CDID);
            sfc.Add(Field.TrackNumber);

            FieldCollection fc = new FieldCollection();
            fc.Add(Field.CDID);
            fc.Add(Field.ArtistCDName);
            fc.Add(Field.Title);
            fc.Add(Field.CDCoverFront);
            fc.Add(Field.CDCoverBack);
            fc.Add(Field.TrackTitle);
            fc.Add(Field.ArtistTrackName);
            fc.Add(Field.ComposerTrackName);
//            fc.Add(Field.TrackSoundFile);
//            fc.Add(Field.TrackLength);
//            fc.Add(Field.TrackBpm);

            cdItems.ClearFromThread();

            using (DataBaseView view = TrackView.CreateView(DataBase, fc, sfc))
            { 
                lastCurrentVisibleCD = -1;
                currentVisibleCD = 0;

                int lastCDID = 0;
                CDItem newItem = null;

                object[] values;

                while ((values = view.Read()) != null)
                {
                    string artist = values[2] as string;
                    string title = values[3] as string;
                    string trackTitle = values[6] as string;
                    int CDID = (int)values[1];

                    /*if (!string.IsNullOrEmpty(searchString) && textBoxSearch.HasText)
                    {
                        bool found = false;
                        if (artist.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            found = true;
                        if (title.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            found = true;
                        if (trackTitle.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            found = true;

                        if (!found)
                            continue;
                    }*/

                    if (lastCDID != CDID)
                    {
                        if (newItem != null)
                            cdItems.AddItemFromThread(newItem);

                        newItem = new CDItem();
                        newItem.ID = CDID;
                        newItem.Artist = artist;
                        newItem.Title = title;
                        newItem.FrontCover = values[4] as string;
                        newItem.BackCover = values[5] as string;

                        newItem.Tracks = new List<TrackItem>();
                        lastCDID = CDID;
                    }

                    if (newItem != null)
                    {
                        TrackItem track = new TrackItem();
                        track.Title = trackTitle;
                        track.Artist = values[7] as string;
                        track.Composer = values[8] as string;
                        //track.Soundfile = values[9] as string;
                        //track.Length = (int)values[10];
                        //track.Bpm = (int)values[11];
                        track.ID = (int)values[0];

                        newItem.Tracks.Add(track);
                    }
                }
        
                if (newItem != null)
                    cdItems.AddItemFromThread(newItem);
            }
        }

        private void AddCDModelToSurface(CDModel cdModel)
        {
            myModel3DGroup.Children.Add(cdModel.ModelCD);
        }

        private void RemoveModelFromSurface(CDModel cdModel)
        {
            myModel3DGroup.Children.Remove(cdModel.ModelCD);

            ModelsCD.Remove(cdModel);
        }

        private CDModel CreateCD(int index)
        {
            CDItem cdItem = listCollectionView.GetItemAt(index) as CDItem;
            double x = -1.2 + index * 3.0;

            BitmapImage biFront = GetFrontCover(cdItem, false);

            // Back-Cover erst laden, wenn CD umgedreht wird
            BitmapImage biBack = null;// GetBackCover(cdItem, biFront);

            CDModel cdModel = GetCDModel(biFront, biBack, x, cdItem);
            cdModel.CDID = cdItem.ID;
            cdModel.CDItem = cdItem;

            // Die CDs rechts der aktuellen werden auf links gedreht
            if (index > 0 && index >= this.realVisibleCDIndex)
            {
                RotateTransform3D myRotateTransform3D = new RotateTransform3D();
                AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
                myAxisAngleRotation3d.Axis = new Vector3D(0, 1, 0);
                myRotateTransform3D.Rotation = myAxisAngleRotation3d;
                myRotateTransform3D.CenterX = x + 0.45;
                myRotateTransform3D.CenterZ = -7.0;
                myAxisAngleRotation3d.Angle = 180;

                cdModel.ModelCDTransformGroup.Children.Add(myRotateTransform3D);
            }
            else
            {
            }

            cdModel.XPosition = x;

            return cdModel;
        }

        private BitmapImage GetBackCover(CDItem cdItem, BitmapImage biFront)
        {
            string backCover = Misc.FindCover(cdItem.BackCover);
            BitmapImage biBack = null;

            if (string.IsNullOrEmpty(backCover) ||
                !File.Exists(backCover))
            {
                biBack = biFront;
            }
            else
            {
                // Front-Cover
                try
                {
                    biBack = new BitmapImage();
                    biBack.BeginInit();
                    biBack.DecodePixelWidth = 500;
                    biBack.CacheOption = BitmapCacheOption.OnLoad;
                    biBack.UriSource = new Uri(backCover);
                    biBack.EndInit();
                }
                catch
                {
                    biBack = biFront;
                }
            }
            return biBack;
        }

        private BitmapImage GetFrontCover(CDItem cdItem, bool forceGet)
        {
            string frontCover = Misc.FindCover(cdItem.FrontCover);
            BitmapImage biFront = null;
            if (string.IsNullOrEmpty(frontCover) ||
                !File.Exists(frontCover))
            {
                // Erst mal nicht mehr!! biFront = GetCoverFromCDID(cdItem, forceGet);
            }
            else
            {
                try
                {
                    // Front-Cover
                    biFront = new BitmapImage();
                    biFront.BeginInit();
                    biFront.DecodePixelWidth = 500;
                    biFront.CacheOption = BitmapCacheOption.OnLoad;
                    biFront.UriSource = new Uri(frontCover);
                    biFront.EndInit();
                }
                catch
                {
                    biFront = null;
                }
            }

            if (biFront == null)
                biFront = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/NoCover.png"));
            
            return biFront;
        }

        private BitmapImage GetCoverFromCDID(CDItem cdItem, bool forceGet)
        {
            //CD cd = DataBase.GetCDById(cdid);

            foreach (Track track in cdItem.Tracks)
            {
                if (!scrollingTooFast || forceGet)
                {
                    SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(track.Soundfile);
                    if (sfi.Images != null && sfi.Images.Count > 0)
                    {
                        try
                        {
                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            MemoryStream ms = new MemoryStream(sfi.Images[0]);
                            image.DecodePixelWidth = 500;
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = ms;
                            image.EndInit();
                            return image;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return null;
        }

        private CDModel GetCDModel(BitmapImage biFront, BitmapImage biBack, double x, CDItem cdItem)
        {
            CDModel cdModel = new CDModel();

            Transform3DGroup transformGroupCD = new Transform3DGroup();
            Model3DGroup myGeometryModel = Get3DModelForCD(cdModel, biFront, biBack, x, cdItem);
            myGeometryModel.Transform = transformGroupCD;

            cdModel.ModelCD = new Model3DGroup();
            cdModel.ModelCD.Children.Add(myGeometryModel);
            cdModel.ModelCDTransformGroup = transformGroupCD;

            cdModel.FrontImage = biFront;
            cdModel.BackImage = biBack;

            return cdModel;
        }

        const double CDWidth = 0.7;

        Model3DGroup Get3DModelForCD(CDModel model, BitmapImage biFront, BitmapImage biBack, double xposition, CDItem cdItem)
        {
            ImageBrush imgBrushFront = new ImageBrush(biFront);
            DiffuseMaterial frontMaterial = new DiffuseMaterial(imgBrushFront);

            TextBlock tb = new TextBlock();
            tb.Width = 500;
            tb.Height = 30;
            tb.Padding = new Thickness(10, 6, 0, 0);
            tb.Text = cdItem.Artist + " - " + cdItem.Title;
            tb.FontWeight = FontWeights.Bold;
            RotateTransform rt = new RotateTransform(-90);
            tb.LayoutTransform = rt;
            tb.Background = new SolidColorBrush(Color.FromRgb(0, 30, 30));
            tb.Foreground = Brushes.White;

            VisualBrush vb = new VisualBrush(tb);
            RenderOptions.SetCachingHint(vb, CachingHint.Cache);
            SolidColorBrush leftBrush = new SolidColorBrush(Color.FromRgb(0, 30, 30));

            DiffuseMaterial leftMaterial = new DiffuseMaterial(vb);


            TextBlock tb2 = new TextBlock();
            tb2.Width = 500;
            tb2.Height = 30;
            tb2.Padding = new Thickness(10, 6, 0, 0);
            tb2.Text = cdItem.Artist + " - " + cdItem.Title;
            tb2.FontWeight = FontWeights.Bold;
            TransformGroup tg = new TransformGroup();
            RotateTransform rt2 = new RotateTransform(-90);
            tg.Children.Add(rt2);
            ScaleTransform st = new ScaleTransform(-1, 1);
            tg.Children.Add(st);
            tb2.LayoutTransform = tg;
            tb2.Background = new SolidColorBrush(Color.FromRgb(0, 30, 30));
            tb2.Foreground = Brushes.White;
            SolidColorBrush rightBrush = new SolidColorBrush(Color.FromRgb(0, 30, 30));
            //rightBrush.Opacity = 0.8;

            VisualBrush vb2 = new VisualBrush(tb2);
            RenderOptions.SetCachingHint(vb2, CachingHint.Cache);
            DiffuseMaterial rightMaterial = new DiffuseMaterial(vb2);

            ImageBrush imgBrushBack = new ImageBrush(biBack);

            DiffuseMaterial backMaterial = new DiffuseMaterial(imgBrushBack);

            Model3DGroup cdModel = new Model3DGroup();

            // Front-Cover
            model.FrontImageModel = Create3DRectangle(
                new Point3D(CDWidth + xposition, 12.4, -1.2),
                new Point3D(CDWidth + xposition, 12.4, -14.0),
                new Point3D(CDWidth + xposition, 0, -1.2),
                new Point3D(CDWidth + xposition, 0, -14.0),
                frontMaterial);

            cdModel.Children.Add(model.FrontImageModel);

            // Front-Cover glass
            cdModel.Children.Add(Create3DRectangle(
                new Point3D(CDWidth + xposition + 0.01, 12.4, -1.2),
                new Point3D(CDWidth + xposition + 0.01, 12.4, -14.0),
                new Point3D(CDWidth + xposition + 0.01, 0, -1.2),
                new Point3D(CDWidth + xposition + 0.01, 0, -14.0),
                frontGlassMaterial));


            // Front-Cover (schwarze linke seite)
            cdModel.Children.Add(Create3DRectangle(
                new Point3D(CDWidth + xposition, 12.4, 0),
                new Point3D(CDWidth + xposition, 12.4, -1.2),
                new Point3D(CDWidth + xposition, 0, 0),
                new Point3D(CDWidth + xposition, 0, -1.2),
                frontLeftMaterial));

            // Linke schmale seite
            cdModel.Children.Add(Create3DRectangle(
                new Point3D(CDWidth + xposition, 0, 0),
                new Point3D(0 + xposition, 0, 0),
                new Point3D(CDWidth + xposition, 12.4, 0),
                new Point3D(0 + xposition, 12.4, 0),
                leftMaterial));

            // Back-Cover
            model.BackImageModel = Create3DRectangle(
                new Point3D(0 + xposition, 12.4, -14.0),
                new Point3D(0 + xposition, 12.4, 0),
                new Point3D(0 + xposition, 0, -14.0),
                new Point3D(0 + xposition, 0, 0),
                backMaterial);
            cdModel.Children.Add(model.BackImageModel);

            // Rechte schmale seite
            cdModel.Children.Add(Create3DRectangle(
                new Point3D(0 + xposition, 12.4, -14.0),
                new Point3D(CDWidth + xposition, 12.4, -14.0),
                new Point3D(0 + xposition, 0, -14.0),
                new Point3D(CDWidth + xposition, 0, -14.0),
                rightMaterial));

            // ---------------------------Das Spiegelbild---------------------------------


            // Front-Cover
            model.FrontImageModelMirror = Create3DRectangle(
                new Point3D(CDWidth + xposition, -12.4, -1.2),
                new Point3D(CDWidth + xposition, -12.4, -14.0),
                new Point3D(CDWidth + xposition, 0, -1.2),
                new Point3D(CDWidth + xposition, 0, -14.0),
                frontMaterial);
            cdModel.Children.Add(model.FrontImageModelMirror);

            // Front-Cover glass
            cdModel.Children.Add(Create3DRectangle(
                new Point3D(CDWidth + xposition + 0.01, -12.4, -1.2),
                new Point3D(CDWidth + xposition + 0.01, -12.4, -14.0),
                new Point3D(CDWidth + xposition + 0.01, 0, -1.2),
                new Point3D(CDWidth + xposition + 0.01, 0, -14.0),
                frontGlassMaterial));

            // Front-Cover (schwarze linke seite)
            cdModel.Children.Add(Create3DRectangle(
                new Point3D(CDWidth + xposition, -12.4, 0),
                new Point3D(CDWidth + xposition, -12.4, -1.2),
                new Point3D(CDWidth + xposition, 0, 0),
                new Point3D(CDWidth + xposition, 0, -1.2),
                frontLeftMaterial));

            // Linke schmale seite
            cdModel.Children.Add(Create3DRectangle(
                new Point3D(CDWidth + xposition, 0, 0),
                new Point3D(0 + xposition, 0, 0),
                new Point3D(CDWidth + xposition, -12.4, 0),
                new Point3D(0 + xposition, -12.4, 0),
                leftMaterial));

            // Back-Cover
            model.BackImageModelMirror = Create3DRectangle(
                new Point3D(0 + xposition, -12.4, -14.0),
                new Point3D(0 + xposition, -12.4, 0),
                new Point3D(0 + xposition, 0, -14.0),
                new Point3D(0 + xposition, 0, 0),
                backMaterial);
            cdModel.Children.Add(model.BackImageModelMirror);

            // Rechte schmale seite
            cdModel.Children.Add(Create3DRectangle(
                new Point3D(0 + xposition, -12.4, -14.0),
                new Point3D(CDWidth + xposition, -12.4, -14.0),
                new Point3D(0 + xposition, 0, -14.0),
                new Point3D(CDWidth + xposition, 0, -14.0),
                rightMaterial));

            return cdModel;
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

        private void Fade()
        {
            if (lastCurrentVisibleCD == -1)
                FadeIn(currentVisibleCD, false);
            else
                FadeOut(lastCurrentVisibleCD, false);
        }

        private void FadeIn(int indexCD, bool right)
        {
            Fade(true, indexCD, right);

            lastCurrentVisibleCD = indexCD;
        }

        private void FadeOut(int indexCD, bool right)
        {
            Fade(false, indexCD, right);

            lastCurrentVisibleCD = -1;
        }

        private void Fade(bool fadeIn, int indexCD, bool right)
        {
            if (indexCD >= ModelsCD.Count)
                return;

            if (backCoverVisible)
            {
                gridTrackList.Visibility = Visibility.Hidden;
                backCoverVisible = false;
            }

            TimeSpan speed;
            
            if (scrollingActive)
                speed = new TimeSpan(0, 0, 0, 0, 100);
            else
                speed = new TimeSpan(0, 0, 0, 0, 500);
            animMoveFront.From = 0;
            animMoveFront.To = fadeIn ? 7 : -7;
            animMoveFront.Duration = new Duration(speed);
            //animRotateLeft.FillBehavior = FillBehavior.Stop;

            DoubleAnimation animRotateLeft = new DoubleAnimation();
            animRotateLeft.From = 0;
            animRotateLeft.To = fadeIn ? -90 : 90;
            animRotateLeft.Duration = new Duration(speed);
            //animRotateLeft.AccelerationRatio = 0.8;
            //animRotateLeft.FillBehavior = FillBehavior.Stop;

            DoubleAnimation animRotateRight = new DoubleAnimation();
            animRotateRight.From = 0;
            animRotateRight.To = fadeIn ? 90 : -90;
            animRotateRight.Duration = new Duration(speed);
            //animRotateRight.AccelerationRatio = 0.8;
            //animRotateRight.FillBehavior = FillBehavior.Stop;

            DoubleAnimation animMoveLeft = new DoubleAnimation();
            animMoveLeft.From = 0;
            animMoveLeft.To = fadeIn ? -10 : 10;
            animMoveLeft.Duration = new Duration(speed);
            //animMoveLeft.FillBehavior = FillBehavior.Stop;

            DoubleAnimation animMoveRight = new DoubleAnimation();
            animMoveRight.From = 0;
            animMoveRight.To = fadeIn ? 10 : -10;
            animMoveRight.Duration = new Duration(speed);
            //animMoveRight.FillBehavior = FillBehavior.Stop;

            Point3DAnimation animCamera = new Point3DAnimation();
            animCamera.From = myPCamera.Position;
            animCamera.To = new Point3D(ModelsCD[indexCD].XPosition, 6.2, 30);
            animCamera.Duration = new Duration(speed);
            //animCamera.FillBehavior = FillBehavior.Stop;
            //myPCamera.BeginAnimation(PerspectiveCamera.PositionProperty, null);
            myPCamera.BeginAnimation(PerspectiveCamera.PositionProperty, animCamera);

            int count = 0;
            foreach (CDModel cdModel in ModelsCD)
            {
                if (count < indexCD)
                {
                    TranslateTransform3D translate3D = new TranslateTransform3D();
                    translate3D.BeginAnimation(TranslateTransform3D.OffsetXProperty, animMoveLeft);

                    cdModel.ModelCDTransformGroup.Children.Add(translate3D);
                }

                if (count == indexCD)
                {
                    RotateTransform3D myRotateTransform3D = new RotateTransform3D();
                    AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
                    myAxisAngleRotation3d.Axis = new Vector3D(0, 1, 0);
                    myRotateTransform3D.Rotation = myAxisAngleRotation3d;
                    myRotateTransform3D.CenterX = cdModel.XPosition + 0.45;
                    myRotateTransform3D.CenterZ = fadeIn ? -7.0 : 0;
                    myAxisAngleRotation3d.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
                    myAxisAngleRotation3d.BeginAnimation(AxisAngleRotation3D.AngleProperty, right ? animRotateRight : animRotateLeft);

                    cdModel.ModelCDTransformGroup.Children.Add(myRotateTransform3D);

                    TranslateTransform3D translate3D = new TranslateTransform3D();
                    translate3D.BeginAnimation(TranslateTransform3D.OffsetZProperty, animMoveFront);
                    cdModel.ModelCDTransformGroup.Children.Add(translate3D);
                }

                if (count > indexCD)
                {
                    TranslateTransform3D translate3D = new TranslateTransform3D();
                    translate3D.BeginAnimation(TranslateTransform3D.OffsetXProperty, null);
                    translate3D.BeginAnimation(TranslateTransform3D.OffsetXProperty, animMoveRight);

                    cdModel.ModelCDTransformGroup.Children.Add(translate3D);
                }

                count++;
            }
        }

        private void StartPartyModus()
        {
            this.buttonPartyMode.Source = new BitmapImage(new Uri("/CatalogView3D;component/Images/PartyModusOn.png", UriKind.RelativeOrAbsolute));

            PartyModusActive = true;

            this.listViewPlaylist.SetValue(Hitbase.Controls.DragDrop.DragDrop.IsDropTargetProperty, false);
            
        }

        private void EndPartyModus()
        {
            this.buttonPartyMode.Source = new BitmapImage(new Uri("/CatalogView3D;component/Images/PartyModusOff.png", UriKind.RelativeOrAbsolute));
            PartyModusActive = false;
            this.listViewPlaylist.SetValue(Hitbase.Controls.DragDrop.DragDrop.IsDropTargetProperty, true);
            
        }

        public void UnlockFeatures()
        {
            PasswordBoxExitPartyModus.Password = "";
            GridPasswordBox.Visibility = Visibility.Visible;
            PasswordBoxExitPartyModus.Focus();
        }

        private void MoveLeft()
        {
            if (currentVisibleCD < 1)
                return;

            if (backCoverVisible)
                TurnCover();

            if (currentVisibleCD == numberOfVisibleCDs / 2 && rightmostCDIndex-numberOfVisibleCDs >= 0)
            {
                RemoveModelFromSurface(ModelsCD[numberOfVisibleCDs-1]);

                CDModel model = CreateCD(rightmostCDIndex - numberOfVisibleCDs);

                rightmostCDIndex--;

                DoubleAnimation animMoveLeft = new DoubleAnimation();
                animMoveLeft.From = 0;
                animMoveLeft.To = -10;
                animMoveLeft.Duration = new Duration(new TimeSpan(0));

                TranslateTransform3D translate3D = new TranslateTransform3D();
                translate3D.BeginAnimation(TranslateTransform3D.OffsetXProperty, null);
                translate3D.BeginAnimation(TranslateTransform3D.OffsetXProperty, animMoveLeft);

                model.ModelCDTransformGroup.Children.Add(translate3D);

                ModelsCD.Insert(0, model);
                AddCDModelToSurface(model);

                myModel3DGroup.Children.Remove(ground);
                myModel3DGroup.Children.Add(ground);

                lastCurrentVisibleCD++;
                currentVisibleCD++;
            }

            if (lastCurrentVisibleCD != -1)
                FadeOut(lastCurrentVisibleCD, true);

            currentVisibleCD--;

            FadeIn(currentVisibleCD, false);

            ShowCDTitle();
        }

        private void MoveRight()
        {
            if (!dataLoaded || this.realVisibleCDIndex >= listCollectionView.Count - 1)
                return;

            if (backCoverVisible)
                TurnCover();

            if (currentVisibleCD == numberOfVisibleCDs / 2 && rightmostCDIndex < listCollectionView.Count - 1)
            {
                RemoveModelFromSurface(ModelsCD[0]);

                rightmostCDIndex++;
                
                CDModel model = CreateCD(rightmostCDIndex);
                DoubleAnimation animMoveRight = new DoubleAnimation();
                animMoveRight.From = 0;
                animMoveRight.To = 10;
                animMoveRight.Duration = new Duration(new TimeSpan(0));

                TranslateTransform3D translate3D = new TranslateTransform3D();
                translate3D.BeginAnimation(TranslateTransform3D.OffsetXProperty, null);
                translate3D.BeginAnimation(TranslateTransform3D.OffsetXProperty, animMoveRight);

                model.ModelCDTransformGroup.Children.Add(translate3D);

                ModelsCD.Add(model);
                AddCDModelToSurface(model);

                myModel3DGroup.Children.Remove(ground);
                myModel3DGroup.Children.Add(ground);

                lastCurrentVisibleCD--;
                currentVisibleCD--;
            }

            if (lastCurrentVisibleCD != -1)
                FadeOut(lastCurrentVisibleCD, false);

            if (currentVisibleCD < ModelsCD.Count - 1)
            {
                currentVisibleCD++;

                FadeIn(currentVisibleCD, true);
            }

            ShowCDTitle();
        }


        private void MoveTo(int absoluteIndex)
        {
            for (int i=0;i<ModelsCD.Count;i++)
                RemoveModelFromSurface(ModelsCD[i]);

            int j = absoluteIndex - numberOfVisibleCDs / 2;
            if (j < 0)
                j = 0;
            for (; j < numberOfVisibleCDs && j < ModelsCD.Count; j++)
            {
                CDModel model = CreateCD(j);

                //model.ModelCDTransformGroup.Children.Add(translate3D);
                ModelsCD.Add(model);
                AddCDModelToSurface(model);
            }

            myModel3DGroup.Children.Remove(ground);
            myModel3DGroup.Children.Add(ground);

            lastCurrentVisibleCD = j;
            currentVisibleCD = absoluteIndex;

            if (lastCurrentVisibleCD != -1)
                FadeOut(lastCurrentVisibleCD, false);

            FadeIn(currentVisibleCD, true);

            ShowCDTitle();
        }

        private void ShowCDTitle()
        {
            if (realVisibleCDIndex >= listCollectionView.Count)
            {
                textBlockArtist.Text = "";
                textBlockTitle.Text = "";
                return;
            }

            ScrollBarCD.Value = realVisibleCDIndex;

            if (!scrollingActive)
            {
                DoubleAnimation anim = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 1)));
                gridCurrentCD.BeginAnimation(Grid.OpacityProperty, anim);
            }
            else
            {
                gridCurrentCD.Opacity = 1;
            }

            textBlockArtist.Text = ((CDItem)listCollectionView.GetItemAt(realVisibleCDIndex)).Artist;
            textBlockTitle.Text = ((CDItem)listCollectionView.GetItemAt(realVisibleCDIndex)).Title;
        }

        private void ShowWishlist()
        {
            if (!Settings.Current.PartyModeWishlistPinned)
            {
                Storyboard sb = new Storyboard();

                ThicknessAnimation ta = new ThicknessAnimation(new Thickness(0, 22, 0, 10), new Duration(new TimeSpan(0, 0, 0, 0, 200)));
                Storyboard.SetTargetName(ta, "WishlistGrid");
                Storyboard.SetTargetProperty(ta, new PropertyPath(Grid.MarginProperty));

                ThicknessAnimation ta1 = new ThicknessAnimation(new Thickness(0, 22, -310, 10), new Duration(new TimeSpan(0, 0, 0, 0, 200)));
                ta1.BeginTime = new TimeSpan(0, 0, 5);
                Storyboard.SetTargetName(ta1, "WishlistGrid");
                Storyboard.SetTargetProperty(ta1, new PropertyPath(Grid.MarginProperty));

                sb.Children.Add(ta);
                sb.Children.Add(ta1);

                sb.Begin(this);
            }

        }

        public void UpdatePlaylist()
        {
            if (Playlist == null)
                return;

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(UpdatePlaylist));
                return;
            }


        }

        public void SetCurrentTrackInfo(int index, int trackPlayOffsetInMs)
        {
            if (Playlist != null && lastPlaylistItem != Playlist[index])
            {
                if (index >= 0 && index < Playlist.Count)
                {
                    NowPlaying(Playlist[index]);
                }

                lastPlaylistItem = Playlist[index];
                progressBar.Maximum = Playlist[index].Info.Length;
            }

            progressBar.Value = trackPlayOffsetInMs;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ColorAnimation anim = new ColorAnimation(Colors.Black, Colors.White, new Duration(new TimeSpan(0, 0, 2)));
            light.BeginAnimation(AmbientLight.ColorProperty, anim);
        }

        private void buttonMoveAround_Click(object sender, RoutedEventArgs e)
        {
            moveAround = true;
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri("background.jpg", UriKind.RelativeOrAbsolute));
            ImageBrush brush = new ImageBrush(bitmapImage);
            grid.Background = brush;
        }

        private void listViewPlaylist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!PartyModusActive)
            {
                Playlist.CurrentTrack = listViewPlaylist.SelectedIndex;
            }
            /*PlaylistItem pli = listViewPlaylist.SelectedItem as PlaylistItem;
            
            string filename = pli.Info.Filename;

            AddTracksToPlaylistParameter addTracks = new AddTracksToPlaylistParameter();
            addTracks.AddTracksType = AddTracksToPlaylistType.Now;
            addTracks.Filenames.Add(filename);
            HitbaseCommands.AddTracksToPlaylist.Execute(addTracks, System.Windows.Application.Current.MainWindow);*/
        }

        private void imageScrollLeft_MouseEnter(object sender, MouseEventArgs e)
        {
            /*DoubleAnimation anim = new DoubleAnimation(0.3, 0.8, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            imageScrollLeft.BeginAnimation(Image.OpacityProperty, anim);*/
        }

        private void imageScrollLeft_MouseLeave(object sender, MouseEventArgs e)
        {
            /*DoubleAnimation anim = new DoubleAnimation(0.8, 0.3, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            imageScrollLeft.BeginAnimation(Image.OpacityProperty, anim);*/
        }

        private void imageScrollRight_MouseEnter(object sender, MouseEventArgs e)
        {
            //DoubleAnimation anim = new DoubleAnimation(0.3, 0.8, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            //imageScrollRight.BeginAnimation(Image.OpacityProperty, anim);
        }

        private void imageScrollRight_MouseLeave(object sender, MouseEventArgs e)
        {
            //DoubleAnimation anim = new DoubleAnimation(0.8, 0.3, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            //imageScrollRight.BeginAnimation(Image.OpacityProperty, anim);
        }

/*        private void imageScrollLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MoveLeft();
            e.Handled = true;
        }*/

        private void imageScrollRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MoveRight();
            e.Handled = true;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (GridPasswordBox.Visibility == Visibility.Visible)
                return;

            if (e.Key == Key.Left)
            {
                MoveLeft();
                e.Handled = true;
            }
            if (e.Key == Key.Right)
            {
                MoveRight();
                e.Handled = true;
            }
            if (e.Key == Key.Up)
            {
                TurnCover();
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                TurnCover();
                e.Handled = true;
            }
        }

        //private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    timerSearch.Stop();
        //    timerSearch.Start();
        //}

        void Search()
        {
            if (DataBase == null || !textBoxSearch.HasText)
                return;

            double parentHeight = this.ActualHeight;

            if (!searchVisible)
            {
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                DoubleAnimation fadeInSize = new DoubleAnimation(0, parentHeight * 0.4 - 50, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                SearchResultGrid.BeginAnimation(ListView.OpacityProperty, fadeIn);
                SearchResultGrid.BeginAnimation(ListView.HeightProperty, fadeInSize);
                DoubleAnimation fadeInSizeImage = new DoubleAnimation(parentHeight * 0.4, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                imageSearchArea.BeginAnimation(Image.HeightProperty, fadeInSizeImage);
                searchVisible = true;

                //SearchImage.Source = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/DeleteSmall.png"));
            }

            this.searchText = textBoxSearch.HasText ? textBoxSearch.Text : "";

            listCollectionView.Refresh();

            ObservableCollection<TrackItem> foundTracks = new ObservableCollection<TrackItem>();
            // Flache Track-Liste erzeigen
            foreach (CDItem cdItem in listCollectionView)
            {
                bool cdFound = false;

                if (cdItem.Artist != null && cdItem.Artist.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    cdFound = true;
                if (cdItem.Title != null && cdItem.Title.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    cdFound = true;

                foreach (TrackItem track in cdItem.Tracks)
                {
                    bool trackFound = false;

                    if (track.Title != null && track.Title.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        trackFound = true;

                    if (cdFound || trackFound)
                        foundTracks.Add(track);
                }
            }

            listViewSearchResults.ItemsSource = foundTracks;

            TextBlockFoundElements.Text = foundTracks.Count.ToString() + " Track(s) gefunden";


            if (listViewSearchResults.Items.Count > 0)
                listViewSearchResults.ScrollIntoView(listViewSearchResults.Items[0]);

            currentVisibleCD = 0;

            CreateCDs();
            Fade();
            ShowCDTitle();
        }

        private void HideTrackSearch()
        {
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            DoubleAnimation fadeOutSize = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            SearchResultGrid.BeginAnimation(ListView.OpacityProperty, fadeOut);
            SearchResultGrid.BeginAnimation(ListView.HeightProperty, fadeOutSize);
            DoubleAnimation fadeOutSizeImage = new DoubleAnimation(45, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            imageSearchArea.BeginAnimation(Image.HeightProperty, fadeOutSizeImage);

            searchVisible = false;
        }

        private bool FilterRow(object row)
        {
            if (string.IsNullOrEmpty(this.searchText))
                return true;

            CDItem cdItem = row as CDItem;

            if (cdItem.Artist != null && cdItem.Artist.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;
            if (cdItem.Title != null && cdItem.Title.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;

            foreach (Track track in cdItem.Tracks)
            {
                if (track.Title != null && track.Title.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        private void listViewSearchResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Track item = (Track)listViewSearchResults.SelectedItem;

            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is ListBoxItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep != null && item != null)
            {
                if (!PartyModusActive)
                {
                    Big3.Hitbase.DataBaseEngine.Track track = DataBase.GetTrackById(item.ID);

                    AddTrackToPlaylist(track.Soundfile, AddTracksToPlaylistType.Now);
                }
            }
        }

        private void AddTrackToPlaylist(string filename, AddTracksToPlaylistType addToPlaylistType)
        {
            if (!Settings.Current.PartyModePlaylistPinned)
            {
                Storyboard sb = new Storyboard();

                ThicknessAnimation ta = new ThicknessAnimation(new Thickness(0, 22, 22, 10), new Duration(new TimeSpan(0, 0, 0, 0, 200)));
                Storyboard.SetTargetName(ta, "PlaylistGrid");
                Storyboard.SetTargetProperty(ta, new PropertyPath(Grid.MarginProperty));

                ThicknessAnimation ta1 = new ThicknessAnimation(new Thickness(-310, 22, 22, 10), new Duration(new TimeSpan(0, 0, 0, 0, 200)));
                ta1.BeginTime = new TimeSpan(0, 0, 5);
                Storyboard.SetTargetName(ta1, "PlaylistGrid");
                Storyboard.SetTargetProperty(ta1, new PropertyPath(Grid.MarginProperty));

                sb.Children.Add(ta);
                sb.Children.Add(ta1);

                sb.Begin(this);
            }

            ThicknessAnimation ta2 = new ThicknessAnimation(new Thickness(120, 0, 0, 0), new Thickness(-100, 0, 0, 0), new Duration(new TimeSpan(0, 0, 10)));
            ta2.AutoReverse = false;
            ta2.RepeatBehavior = RepeatBehavior.Forever;
            TextBlockNowPlaying.BeginAnimation(TextBlock.MarginProperty, ta2);

            string[] filenames = new string[1];
            filenames[0] = filename;

            AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
            addTracksParams.Filenames = filenames.ToList();
            addTracksParams.AddTracksType = addToPlaylistType;
            HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, System.Windows.Application.Current.MainWindow);
        }

        private void ImagePlayNow_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayNowHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayNow_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayNow.png"));
            img.Source = bmp;
        }

        private void ImagePlayNext_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayNextHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayNext_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayNext.png"));
            img.Source = bmp;
        }

        private void ImagePlayLast_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayLastHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayLast_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayLast.png"));
            img.Source = bmp;
        }

        private void ImagePlayPreListen_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayPreListenHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayPreListen_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayPreListen.png"));
            img.Source = bmp;
        }

        private void ImageAddToWishlist_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/AddWishHover.png"));
            img.Source = bmp;
        }

        private void ImageAddToWishlist_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/AddWish.png"));
            img.Source = bmp;
        }

        private void ImagePlayNow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!PartyModusActive)
            {
                AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.Now);
            }
        }

        private void ImagePlayNext_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!PartyModusActive || Settings.Current.PartyModeAllowPlayNext)
            {
                AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.Next);
            }
        }

        private void ImagePlayLast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!PartyModusActive || Settings.Current.PartyModeAllowPlayLast)
            {
                AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.End);
            }
        }

        private void ImagePlayPreListen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Big3.Hitbase.DataBaseEngine.Track track = GetTrack((DependencyObject)e.OriginalSource);

            if (track != null)
            {
                HitbaseCommands.PreListenTrack.Execute(track, System.Windows.Application.Current.MainWindow);

                e.Handled = true;
            }
        }

        private void ImageAddToWishlist_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        void AddWishUserControl_CancelClick(object sender, EventArgs e)
        {
            HideWishlistUserControl();
        }

        void AddWishUserControl_OKClick(object sender, EventArgs e)
        {
            HideWishlistUserControl();

            WishlistItem wishlistItem = AddWishUserControl.GetWishlistItem();

            if (AddWishUserControl.NewWish)
            {
                HitbaseCommands.AddToWishlist.Execute(wishlistItem, System.Windows.Application.Current.MainWindow);
            }

            ShowWishlist();
        }

        private void ShowWishlistUserControl()
        {
            GridAddWishUserControl.Visibility = Visibility.Visible;
            DoubleAnimation daHide = new DoubleAnimation(0.2, TimeSpan.FromMilliseconds(500).Duration());
            grid.BeginAnimation(Viewport3D.OpacityProperty, daHide);
            grid.IsEnabled = false;
            DoubleAnimation daShow = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200).Duration());
            GridAddWishUserControl.BeginAnimation(UserControl.OpacityProperty, daShow);
        }


        private void HideWishlistUserControl()
        {
            DoubleAnimation daShow = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500).Duration());
            grid.BeginAnimation(Viewport3D.OpacityProperty, daShow);
            grid.IsEnabled = true;
            DoubleAnimation daHide = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200).Duration());
            daHide.Completed += delegate
            {
                GridAddWishUserControl.Visibility = Visibility.Collapsed;
            };
            GridAddWishUserControl.BeginAnimation(UserControl.OpacityProperty, daHide);
        }

        private void AddTrackFromSearchResult(DependencyObject originalSource, AddTracksToPlaylistType addToPlaylistType)
        {
         //   if (Settings.Current.PartyModusPassword)
            Big3.Hitbase.DataBaseEngine.Track track = GetTrack(originalSource);

            if (track != null)
            {
                AddTrackToPlaylist(track.Soundfile, addToPlaylistType);
            }
        }

        Big3.Hitbase.DataBaseEngine.Track GetTrack(DependencyObject dep)
        {
            while ((dep != null) && !(dep is ListBoxItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep != null)
            {
                ListBoxItem lbi = dep as ListBoxItem;
                ListBox listBox = VisualTreeExtensions.FindParent<ListBox>(dep);

                if (listBox == null)
                    return null;

                Track item = listBox.ItemContainerGenerator.ItemFromContainer(lbi) as Track;

                if (item != null)
                {
                    Big3.Hitbase.DataBaseEngine.Track track = DataBase.GetTrackById(item.ID);

                    return track;
                }

                WishlistItem wishItem = listBox.ItemContainerGenerator.ItemFromContainer(lbi) as WishlistItem;
                if (wishItem != null)
                {
                    Big3.Hitbase.DataBaseEngine.Track track = DataBase.GetTrackById(wishItem.TrackID);

                    return track;
                }
            }

            return null;
        }

        public new void DragOver(Controls.DragDrop.IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = Big3.Hitbase.Controls.DragDrop.DropTargetAdorners.Insert;

            if (dropInfo.Data is PlaylistItem)
            {
                dropInfo.Effects = DragDropEffects.Move;
            }

            if (dropInfo.Data is Big3.Hitbase.CDUtilities.WishlistItem)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
            if (dropInfo.Data is Track || dropInfo.Data is List<Track>)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public new void Drop(Controls.DragDrop.IDropInfo dropInfo)
        {
            if (dropInfo.VisualTarget == this.listViewWishlist)
            {
                List<WishlistItem> wishlistItems = new List<WishlistItem>();

                if (dropInfo.Data is Track || dropInfo.Data is List<Track>)
                {
                    // Prüfen, ob ein Track einer CD gedropped wurde
                    Track track = dropInfo.Data as Track;
                    List<Track> trackList = dropInfo.Data as List<Track>;

                    if (track != null)
                    {
                        WishlistItem item = GetWishlistItemByTrack(track);
                        wishlistItems.Add(item);
                    }

                    if (trackList != null)
                    {
                        foreach (Track trackItem in trackList)
                        {
                            WishlistItem item = GetWishlistItemByTrack(track);
                            wishlistItems.Add(item);
                        }
                    }
                }

                if (wishlistItems.Count > 0)
                {
                    HitbaseCommands.AddToWishlist.Execute(wishlistItems, System.Windows.Application.Current.MainWindow);
                }
                
                return;
            }

            if (dropInfo.Data is PlaylistItem)
            {
                Controls.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }

            if (dropInfo.Data is Big3.Hitbase.CDUtilities.WishlistItem)
            {
                Big3.Hitbase.CDUtilities.WishlistItem wishlistItem = dropInfo.Data as Big3.Hitbase.CDUtilities.WishlistItem;

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                addTracksParams.InsertIndex = dropInfo.InsertIndex;

                addTracksParams.TrackIds.Add(wishlistItem.TrackID);
                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, Application.Current.MainWindow);
            }

            if (dropInfo.Data is Track || dropInfo.Data is List<Track>)
            {
                Track track = dropInfo.Data as Track;
                List<Track> trackList = dropInfo.Data as List<Track>;

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                addTracksParams.InsertIndex = dropInfo.InsertIndex;

                if (track != null)
                {
                    addTracksParams.TrackIds.Add(track.ID);
                }

                if (trackList != null)
                {
                    foreach (Track trackItem in trackList)
                        addTracksParams.TrackIds.Add(trackItem.ID);
                }

                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, Application.Current.MainWindow);
            }


        }


        private WishlistItem GetWishlistItemByTrack(Track track)
        {
            WishlistItem item = new WishlistItem();

            item.TrackID = track.ID;
            item.Title = track.Title;
            item.Artist = track.Artist;
            //??            item.ImageFilename = playlistItem.TrackImage;

            return item;
        }


        private void SearchImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (searchVisible)
            {
                textBoxSearch.Text = "";
                //HideTrackSearch();
            }

            e.Handled = true;
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!PartyModusActive)
            {
                if (listViewPlaylist.SelectedIndex < 0)
                    Playlist.Play();
                else
                    Playlist.CurrentTrack = listViewPlaylist.SelectedIndex;
            }
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (!PartyModusActive)
            {
                Playlist.Stop();
            }
        }

        private void ButtonPrevTrack_Click(object sender, RoutedEventArgs e)
        {
            if (!PartyModusActive)
            {
                Playlist.CurrentTrack--;
            }
        }

        private void ButtonNextTrack_Click(object sender, RoutedEventArgs e)
        {
            if (!PartyModusActive)
            {
                Playlist.CurrentTrack++;
            }
        }

        private void ButtonMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (!PartyModusActive)
            {
                if (listViewPlaylist.SelectedIndex > 0)
                {
                    Playlist.Move(listViewPlaylist.SelectedIndex, listViewPlaylist.SelectedIndex - 1);
                }
            }
        }

        private void ButtonMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (!PartyModusActive)
            {
                if (listViewPlaylist.SelectedIndex >= 0 && listViewPlaylist.SelectedIndex < Playlist.Count - 1)
                {
                    Playlist.Move(listViewPlaylist.SelectedIndex, listViewPlaylist.SelectedIndex + 1);
                }
            }
        }

        private void ButtonStickPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Current.PartyModePlaylistPinned)
            {
                ButtonStickPlaylist.Style = (Style)FindResource("MetroStickyUnpinButtonStyle");
                Settings.Current.PartyModePlaylistPinned = false;
            }
            else
            {
                ButtonStickPlaylist.Style = (Style)FindResource("MetroStickyPinButtonStyle");
                Settings.Current.PartyModePlaylistPinned = true;
            }
        }

        private void ScrollBarCD_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (e.ScrollEventType == System.Windows.Controls.Primitives.ScrollEventType.SmallIncrement)
                MoveRight();
            if (e.ScrollEventType == System.Windows.Controls.Primitives.ScrollEventType.SmallDecrement)
                MoveLeft();
            if (e.ScrollEventType == System.Windows.Controls.Primitives.ScrollEventType.LargeIncrement)
            {
                for (int i=0;i<5;i++)
                    MoveRight();
            }
            if (e.ScrollEventType == System.Windows.Controls.Primitives.ScrollEventType.LargeDecrement)
            {
                for (int i = 0; i < 5; i++)
                    MoveLeft();
            }
            if (e.ScrollEventType == System.Windows.Controls.Primitives.ScrollEventType.ThumbTrack)
            {
                scrollingActive = true;

                // Wenn wir zu schnell scollen, laden wir zunächst keine Cover
                scrollingTooFast = Math.Abs(realVisibleCDIndex - (int)e.NewValue) > 1;

                //MoveTo((int)e.NewValue);
                while (realVisibleCDIndex != (int)e.NewValue)
                {
                    int oldrealVisibleCDIndex = realVisibleCDIndex;

                    if (realVisibleCDIndex > (int)e.NewValue)
                        MoveLeft();
                    else
                        MoveRight();

                    if (oldrealVisibleCDIndex == realVisibleCDIndex)
                    {
                        // Hm.... darf eigentlich nicht passieren, aber sonst hängen wir hier fest....
                        // Knoten lösen
                        break;
                    }
                }
            }
            if (e.ScrollEventType == System.Windows.Controls.Primitives.ScrollEventType.EndScroll)
            {
                scrollingActive = false;
                scrollingTooFast = false;
                UpdateCovers();
                // Jetzt nochmal für alle sichtbaren CDs die Cover einblenden
            }
        }

        private int lastScrollPosition = -1;
        void dtCover_Tick(object sender, EventArgs e)
        {
            // Prüfen, ob die Scrollposition eine Sekunde "still" steht
            if (scrollingActive)
            {
                if (lastScrollPosition != -1 && lastScrollPosition != (int)ScrollBarCD.Value)
                {
                    //UpdateCovers();
                }

                lastScrollPosition = (int)ScrollBarCD.Value;
            }

            if (Playlist.IsPlaying)
            {
                this.SetCurrentTrackInfo(Playlist.CurrentTrack, Playlist.CurrentTrackPlayPosition);
            }
            else
            {
                DoubleAnimation animIn = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 1)));
                gridCurrentPlayingCD.BeginAnimation(Grid.OpacityProperty, animIn);
                lastPlaylistItem = null;
            }
        }

        private void UpdateCovers()
        {
            for (int i = 0; i < ModelsCD.Count; i++)
            {
                CDModel model = ModelsCD[i];

                model.FrontImage = GetFrontCover(model.CDItem, true);
                ImageBrush imgBrushFront = new ImageBrush(model.FrontImage);
                model.FrontImageModel.Material = new DiffuseMaterial(imgBrushFront);
                model.FrontImageModel.BackMaterial = model.FrontImageModel.Material;
                model.FrontImageModelMirror.Material = model.FrontImageModel.Material;
                model.FrontImageModelMirror.BackMaterial = model.FrontImageModel.Material;
            }
        }

        private void repeatButtonScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            MoveLeft();
            e.Handled = true;
        }

        private void repeatButtonScrollRight_Click(object sender, RoutedEventArgs e)
        {
            MoveRight();
            e.Handled = true;
        }

        private void GridPasswordBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ButtonPasswordCancel_Click(object sender, RoutedEventArgs e)
        {
            GridPasswordBox.Visibility = Visibility.Hidden;
        }

        private void ButtonPasswordOK_Click(object sender, RoutedEventArgs e)
        {
            CheckPassword();
        }

        private void CheckPassword()
        {
            if (PasswordBoxExitPartyModus.Password == Settings.Current.PartyModusPassword)
            {
                GridPasswordBox.Visibility = Visibility.Hidden;
                EndPartyModus();
            }
            else
            {
                MessageBox.Show("Das Passwort ist falsch!", System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
                PasswordBoxExitPartyModus.Password = "";
            }
        }

        private void buttonPartyMode_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!PartyModusActive)
            {
                StartPartyModus();
            }
            else
            {
                if (Settings.Current.PartyModusEnablePassword)
                    UnlockFeatures();
                else
                    EndPartyModus();
            }

            e.Handled = true;
        }

        private void buttonPartyMode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void progressBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double percentage = 1 / progressBar.Width * e.GetPosition(progressBar).X;

            // Zur Position springen (seek)
            Playlist.CurrentTrackPlayPosition = (int)(progressBar.Maximum * percentage);

            e.Handled = true;
        }

        private void PlaylistGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Settings.Current.PartyModePlaylistPinned)
            {
                ThicknessAnimation fadeIn = new ThicknessAnimation(new Thickness(0, 22, 22, 10), new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                fadeIn.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                PlaylistGrid.BeginAnimation(Grid.MarginProperty, fadeIn);
            }
        }

        private void PlaylistGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Settings.Current.PartyModePlaylistPinned)
            {
                ThicknessAnimation fadeIn = new ThicknessAnimation(new Thickness(-310, 22, 22, 10), new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                fadeIn.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                PlaylistGrid.BeginAnimation(Grid.MarginProperty, fadeIn);
            }
        }

        private void WishlistGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Settings.Current.PartyModeWishlistPinned)
            {
                ThicknessAnimation fadeIn = new ThicknessAnimation(new Thickness(0, 22, 0, 10), new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                fadeIn.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                WishlistGrid.BeginAnimation(Grid.MarginProperty, fadeIn);
            }
        }

        private void WishlistGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Settings.Current.PartyModeWishlistPinned)
            {
                ThicknessAnimation fadeIn = new ThicknessAnimation(new Thickness(22, 22, -310, 10), new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                fadeIn.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                WishlistGrid.BeginAnimation(Grid.MarginProperty, fadeIn);
            }
        }

        private void listViewWishlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WishlistItem wishlistItem = listViewWishlist.SelectedItem as WishlistItem;

            if (wishlistItem != null)
            {
                ShowWishlistUserControl();
                AddWishUserControl.SetWishlistItem(wishlistItem);
            }
        }

        private void ButtonStickWishlist_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Current.PartyModeWishlistPinned)
            {
                ButtonStickWishlist.Style = (Style)FindResource("MetroStickyUnpinButtonStyle");
                Settings.Current.PartyModeWishlistPinned = false;
            }
            else
            {
                ButtonStickWishlist.Style = (Style)FindResource("MetroStickyPinButtonStyle");
                Settings.Current.PartyModeWishlistPinned = true;
            }
        }

        private void SearchTextBox_Search(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void textBoxSearch_Clear(object sender, RoutedEventArgs e)
        {
            HideTrackSearch();
            //InitializeData();
            this.searchText = textBoxSearch.HasText ? textBoxSearch.Text : "";

            listCollectionView.Refresh();

            currentVisibleCD = 0;
            CreateCDs();
            Fade();
            ShowCDTitle();
        }

        private void AddToPlaylistUserControl_AddToWishlist(object sender, RoutedEventArgs e)
        {
            Big3.Hitbase.DataBaseEngine.Track track = GetTrack((DependencyObject)e.OriginalSource);

            if (track != null)
            {
                ShowWishlistUserControl();
                AddWishUserControl.SetTrack(track);
            }
        }

        private void AddToPlaylistUserControl_PlayNow(object sender, RoutedEventArgs e)
        {
            if (!PartyModusActive)
            {
                AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.Now);
            }
        }

        private void AddToPlaylistUserControl_PlayNext(object sender, RoutedEventArgs e)
        {
            if (!PartyModusActive || Settings.Current.PartyModeAllowPlayNext)
            {
                AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.Next);
            }
        }

        private void AddToPlaylistUserControl_PlayLast(object sender, RoutedEventArgs e)
        {
            if (!PartyModusActive || Settings.Current.PartyModeAllowPlayLast)
            {
                AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.End);
            }
        }

        bool preListenVisible = false;
        private void AddToPlaylistUserControl_PreListen(object sender, RoutedEventArgs e)
        {
            // Nur, wenn konfiguriert
            if (Settings.Current.PreListenVirgin || Settings.Current.OutputDevicePreListen < 0)
                return;

            if (!preListenVisible)
            {
                PreListenUserControl.Visibility = System.Windows.Visibility.Visible;
                DoubleAnimation opacityAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500).Duration());
                PreListenUserControl.BeginAnimation(UserControl.OpacityProperty, opacityAnim);
                
                TranslateTransform translate = new TranslateTransform();

                DoubleAnimation moveInAnim = new DoubleAnimation(-20, 0, TimeSpan.FromMilliseconds(500).Duration());
                moveInAnim.EasingFunction = new QuadraticEase();
                translate.BeginAnimation(TranslateTransform.YProperty, moveInAnim);
                PreListenUserControl.RenderTransform = translate;
            }

            preListenVisible = true;

            Big3.Hitbase.DataBaseEngine.Track track = GetTrack(e.OriginalSource as DependencyObject);
            HitbaseCommands.PreListenTrack.Execute(track, System.Windows.Application.Current.MainWindow);

            e.Handled = true;
        }

        private void PreListenUserControl_Closed(object sender, EventArgs e)
        {
            PreListenUserControl.Visibility = System.Windows.Visibility.Visible;
            DoubleAnimation opacityAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500).Duration());
            PreListenUserControl.BeginAnimation(UserControl.OpacityProperty, opacityAnim);

            TranslateTransform translate = new TranslateTransform();

            DoubleAnimation moveInAnim = new DoubleAnimation(0, -20, TimeSpan.FromMilliseconds(500).Duration());
            moveInAnim.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseIn };
            translate.BeginAnimation(TranslateTransform.YProperty, moveInAnim);
            PreListenUserControl.RenderTransform = translate;

            preListenVisible = false;
        }

        private void ButtonDeleteTrack_Click(object sender, RoutedEventArgs e)
        {
            DeleteTrack();
        }

        private void listViewPlaylist_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteTrack();
                e.Handled = true;
            }
        }

        private void DeleteTrack()
        {
            if (PartyModusActive)
                return;

            if (listViewPlaylist.SelectedIndex >= 0)
            {
                int oldSelectedIndex = listViewPlaylist.SelectedIndex;
                int nextFocusIndex;
                ListBoxItem lbi = null;

                for (int i = listViewPlaylist.SelectedItems.Count - 1; i >= 0; i--)
                {
                    // Das aktuell spielende Lied darf nicht entfernt werden.
                    if (Playlist.CurrentTrack < 0 || listViewPlaylist.SelectedItems[i] != Playlist[Playlist.CurrentTrack])
                        Playlist.Remove((PlaylistItem)listViewPlaylist.SelectedItems[i]);
                }

                if (oldSelectedIndex >= listViewPlaylist.Items.Count)
                    nextFocusIndex = listViewPlaylist.Items.Count - 1;
                else
                    nextFocusIndex = oldSelectedIndex;
                if (nextFocusIndex >= 0 && nextFocusIndex < listViewPlaylist.Items.Count)
                    lbi = listViewPlaylist.ItemContainerGenerator.ContainerFromIndex(nextFocusIndex) as ListBoxItem;

                listViewPlaylist.SelectedIndex = oldSelectedIndex;
                if (lbi != null)
                    lbi.Focus();
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SoundEngine.SoundEngine.Instance.Volume = (float)e.NewValue;
        }

        private void ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            ToolTip toolTip = sender as ToolTip;

            TrackItem trackItem = toolTip.DataContext as TrackItem;

            if (trackItem != null)
            {
                Track track = this.DataBase.GetTrackById(trackItem.ID);

                //SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(trackItem.Soundfile);

                if (track != null)
                    toolTip.DataContext = track;
            }
        }

        private void listViewWishlist_KeyDown(object sender, KeyEventArgs e)
        {
            if (!PartyModusActive)
            {
                if (e.Key == Key.Delete)
                {
                    this.Wishlist.Remove((WishlistItem)this.listViewWishlist.SelectedItem);
                }
            }
        }


        #region Kinect
#if KINECT
        /// <summary>
        /// The recognizer being used.
        /// </summary>
        private Recognizer activeRecognizer;

        /// <summary>
        /// Array of arrays of contiguous line segements that represent a skeleton.
        /// </summary>
        private static readonly JointType[][] SkeletonSegmentRuns = new JointType[][]
        {
            new JointType[] 
            { 
                JointType.Head, JointType.ShoulderCenter, JointType.HipCenter 
            },
            new JointType[] 
            { 
                JointType.HandLeft, JointType.WristLeft, JointType.ElbowLeft, JointType.ShoulderLeft,
                JointType.ShoulderCenter,
                JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight
            },
            new JointType[]
            {
                JointType.FootLeft, JointType.AnkleLeft, JointType.KneeLeft, JointType.HipLeft,
                JointType.HipCenter,
                JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight
            }
        };

        /// <summary>
        /// The sensor we're currently tracking.
        /// </summary>
        private KinectSensor nui;

        /// <summary>
        /// There is currently no connected sensor.
        /// </summary>
        private bool isDisconnectedField = true;

        /// <summary>
        /// Any message associated with a failure to connect.
        /// </summary>
        private string disconnectedReasonField;

        /// <summary>
        /// Array to receive skeletons from sensor, resize when needed.
        /// </summary>
        private Skeleton[] skeletons = new Skeleton[0];

        /// <summary>
        /// Time until skeleton ceases to be highlighted.
        /// </summary>
        private DateTime highlightTime = DateTime.MinValue;

        /// <summary>
        /// The ID of the skeleton to highlight.
        /// </summary>
        private int highlightId = -1;

        /// <summary>
        /// The ID if the skeleton to be tracked.
        /// </summary>
        private int nearestId = -1;

        /// <summary>
        /// Event implementing INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a value indicating whether no Kinect is currently connected.
        /// </summary>
        public bool IsDisconnected
        {
            get
            {
                return this.isDisconnectedField;
            }

            private set
            {
                if (this.isDisconnectedField != value)
                {
                    this.isDisconnectedField = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("IsDisconnected"));
                    }
                }
            }
        }

        /// <summary>
        /// Gets any message associated with a failure to connect.
        /// </summary>
        public string DisconnectedReason
        {
            get
            {
                return this.disconnectedReasonField;
            }

            private set
            {
                if (this.disconnectedReasonField != value)
                {
                    this.disconnectedReasonField = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("DisconnectedReason"));
                    }
                }
            }
        }


        private void InitKinect()
        {
            // Create the gesture recognizer.
            this.activeRecognizer = this.CreateRecognizer();

            // Start the Kinect system, this will cause StatusChanged events to be queued.
            this.InitializeNui();

            // Handle StatusChange events to pick the first sensor that connects.
            KinectSensor.KinectSensors.StatusChanged += (s, ee) =>
            {
                switch (ee.Status)
                {
                    case KinectStatus.Connected:
                        if (nui == null)
                        {
                            Debug.WriteLine("New Kinect connected");

                            InitializeNui();
                        }
                        else
                        {
                            Debug.WriteLine("Existing Kinect signalled connection");
                        }

                        break;
                    default:
                        if (ee.Sensor == nui)
                        {
                            Debug.WriteLine("Existing Kinect disconnected");

                            UninitializeNui();
                        }
                        else
                        {
                            Debug.WriteLine("Other Kinect event occurred");
                        }

                        break;
                }
            };
        }

        /// <summary>
        /// Handle insertion of Kinect sensor.
        /// </summary>
        private void InitializeNui()
        {
            this.UninitializeNui();

            var index = 0;
            while (this.nui == null && index < KinectSensor.KinectSensors.Count)
            {
                try
                {
                    this.nui = KinectSensor.KinectSensors[index];

                    this.nui.Start();

                    this.IsDisconnected = false;
                    this.DisconnectedReason = null;
                }
                catch (IOException ex)
                {
                    this.nui = null;

                    this.DisconnectedReason = ex.Message;
                }
                catch (InvalidOperationException ex)
                {
                    this.nui = null;

                    this.DisconnectedReason = ex.Message;
                }

                index++;
            }

            if (this.nui != null)
            {
                this.nui.SkeletonStream.Enable();

                this.nui.SkeletonFrameReady += this.OnSkeletonFrameReady;
            }
        }

        /// <summary>
        /// Handle removal of Kinect sensor.
        /// </summary>
        private void UninitializeNui()
        {
            if (this.nui != null)
            {
                this.nui.SkeletonFrameReady -= this.OnSkeletonFrameReady;

                this.nui.Stop();

                this.nui = null;
            }

            this.IsDisconnected = true;
            this.DisconnectedReason = null;
        }

        /// <summary>
        /// Create a wired-up recognizer for running the slideshow.
        /// </summary>
        /// <returns>The wired-up recognizer.</returns>
        private Recognizer CreateRecognizer()
        {
            // Instantiate a recognizer.
            var recognizer = new Recognizer();

            // Wire-up swipe right to manually advance picture.
            recognizer.SwipeRightDetected += (s, e) =>
            {
                if (e.Skeleton.TrackingId == nearestId)
                {
                    MoveRight();

                    HighlightSkeleton(e.Skeleton);
                }
            };

            // Wire-up swipe left to manually reverse picture.
            recognizer.SwipeLeftDetected += (s, e) =>
            {
                if (e.Skeleton.TrackingId == nearestId)
                {
                    MoveLeft();

                    HighlightSkeleton(e.Skeleton);
                }
            };

            return recognizer;
        }


        /// <summary>
        /// Handler for skeleton ready handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Get the frame.
            using (var frame = e.OpenSkeletonFrame())
            {
                // Ensure we have a frame.
                if (frame != null)
                {
                    // Resize the skeletons array if a new size (normally only on first call).
                    if (this.skeletons.Length != frame.SkeletonArrayLength)
                    {
                        this.skeletons = new Skeleton[frame.SkeletonArrayLength];
                    }

                    // Get the skeletons.
                    frame.CopySkeletonDataTo(this.skeletons);

                    // Assume no nearest skeleton and that the nearest skeleton is a long way away.
                    var newNearestId = -1;
                    var nearestDistance2 = double.MaxValue;

                    // Look through the skeletons.
                    foreach (var skeleton in this.skeletons)
                    {
                        // Only consider tracked skeletons.
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            // Find the distance squared.
                            var distance2 = (skeleton.Position.X * skeleton.Position.X) +
                                (skeleton.Position.Y * skeleton.Position.Y) +
                                (skeleton.Position.Z * skeleton.Position.Z);

                            // Is the new distance squared closer than the nearest so far?
                            if (distance2 < nearestDistance2)
                            {
                                // Use the new values.
                                newNearestId = skeleton.TrackingId;
                                nearestDistance2 = distance2;
                            }
                        }
                    }

                    if (this.nearestId != newNearestId)
                    {
                        this.nearestId = newNearestId;
                    }

                    // Pass skeletons to recognizer.
                    this.activeRecognizer.Recognize(sender, frame, this.skeletons);

                    this.DrawStickMen(this.skeletons);
                }
            }
        }
        /// <summary>
        /// Select a skeleton to be highlighted.
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        private void HighlightSkeleton(Skeleton skeleton)
        {
            // Set the highlight time to be a short time from now.
            this.highlightTime = DateTime.UtcNow + TimeSpan.FromSeconds(0.5);

            // Record the ID of the skeleton.
            this.highlightId = skeleton.TrackingId;
        }

        /// <summary>
        /// Draw stick men for all the tracked skeletons.
        /// </summary>
        /// <param name="skeletons">The skeletons to draw.</param>
        private void DrawStickMen(Skeleton[] skeletons)
        {
            // Remove any previous skeletons.
            StickMen.Children.Clear();

            foreach (var skeleton in skeletons)
            {
                // Only draw tracked skeletons.
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    // Draw a background for the next pass.
                    this.DrawStickMan(skeleton, Brushes.WhiteSmoke, 7);
                }
            }

            foreach (var skeleton in skeletons)
            {
                // Only draw tracked skeletons.
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    // Pick a brush, Red for a skeleton that has recently gestures, black for the nearest, gray otherwise.
                    var brush = DateTime.UtcNow < this.highlightTime && skeleton.TrackingId == this.highlightId ? Brushes.Red :
                        skeleton.TrackingId == this.nearestId ? Brushes.Black : Brushes.Gray;

                    // Draw the individual skeleton.
                    this.DrawStickMan(skeleton, brush, 3);
                }
            }
        }


        /// <summary>
        /// Draw an individual skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to draw.</param>
        /// <param name="brush">The brush to use.</param>
        /// <param name="thickness">This thickness of the stroke.</param>
        private void DrawStickMan(Skeleton skeleton, Brush brush, int thickness)
        {
            Debug.Assert(skeleton.TrackingState == SkeletonTrackingState.Tracked, "The skeleton is being tracked.");

            foreach (var run in SkeletonSegmentRuns)
            {
                var next = this.GetJointPoint(skeleton, run[0]);
                for (var i = 1; i < run.Length; i++)
                {
                    var prev = next;
                    next = this.GetJointPoint(skeleton, run[i]);

                    var line = new Line
                    {
                        Stroke = brush,
                        StrokeThickness = thickness,
                        X1 = prev.X,
                        Y1 = prev.Y,
                        X2 = next.X,
                        Y2 = next.Y,
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    };

                    StickMen.Children.Add(line);
                }
            }
        }
        
        /// <summary>
        /// Convert skeleton joint to a point on the StickMen canvas.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="jointType">The joint to project.</param>
        /// <returns>The projected point.</returns>
        private Point GetJointPoint(Skeleton skeleton, JointType jointType)
        {
            var joint = skeleton.Joints[jointType];

            // Points are centered on the StickMen canvas and scaled according to its height allowing
            // approximately +/- 1.5m from center line.
            var point = new Point
            {
                X = (StickMen.Width / 2) + (StickMen.Height * joint.Position.X / 3),
                Y = (StickMen.Width / 2) - (StickMen.Height * joint.Position.Y / 3)
            };

            return point;
        }
#endif

        #endregion

    }

    public class CDItem : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string FrontCover { get; set; }
        public string BackCover { get; set; }

        public List<TrackItem> Tracks { get; set; }

        public override string ToString()
        {
            return Artist + " - " + Title;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class TrackItem : Track, INotifyPropertyChanged
    {
        public override string ToString()
        {
            return Artist + " - " + Title;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class LengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return Miscellaneous.Misc.GetShortTimeString((int)value);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }
    }

    public class Mp3ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(value as string);

            if (sfi.Images != null && sfi.Images.Count > 0)
            {
                MemoryStream ms = new MemoryStream(sfi.Images[0]);
                return ImageLoader.GetBitmapImageFromMemoryStream(ms);
            }

            return new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/CDCover.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TrackTextConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Track track = value as Track;

            if (track == null)
                return "";

            return string.Format("{0} - {1}", track.Artist, track.Title);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
