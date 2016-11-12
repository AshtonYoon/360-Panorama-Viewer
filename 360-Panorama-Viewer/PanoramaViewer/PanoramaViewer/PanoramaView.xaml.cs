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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PanoramaViewer
{
    /// <summary>
    /// PanoramaView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PanoramaView : UserControl
    {
        public static Geometry3D SphereModel = CreateGeometry();

        public ImageSource PanoramaImage
        {
            get { return (ImageSource)GetValue(PanoramaImageProperty); }
            set { SetValue(PanoramaImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanoramaImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanoramaImageProperty = DependencyProperty.Register("PanoramaImage", typeof(ImageSource), typeof(PanoramaView), new PropertyMetadata(null));

        public PanoramaView()
        {
            InitializeComponent();
        }
    
        private static Geometry3D CreateGeometry()
        {
            int tDiv = 64;
            int yDiv = 64;
            double maxTheta = (360.0 / 180.0) * Math.PI;
            double minY = -1.0;
            double maxY = 1.0;

            double dt = maxTheta / tDiv;
            double dy = (maxY - minY) / yDiv;

            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int yi = 0; yi <= yDiv; yi++)
            {
                double y = minY + yi * dy;

                for (int ti = 0; ti <= tDiv; ti++)
                {
                    double t = ti * dt;

                    mesh.Positions.Add(GetPosition(t, y));
                    mesh.Normals.Add(GetNormal(t, y));
                    mesh.TextureCoordinates.Add(GetTextureCoordinate(t, y));
                }
            }

            for (int yi = 0; yi < yDiv; yi++)
            {
                for (int ti = 0; ti < tDiv; ti++)
                {
                    int x0 = ti;
                    int x1 = (ti + 1);
                    int y0 = yi * (tDiv + 1);
                    int y1 = (yi + 1) * (tDiv + 1);

                    mesh.TriangleIndices.Add(x0 + y0);
                    mesh.TriangleIndices.Add(x0 + y1);
                    mesh.TriangleIndices.Add(x1 + y0);

                    mesh.TriangleIndices.Add(x1 + y0);
                    mesh.TriangleIndices.Add(x0 + y1);
                    mesh.TriangleIndices.Add(x1 + y1);
                }
            }

            mesh.Freeze();
            return mesh;
        }

        internal static Point3D GetPosition(double t, double y)
        {
            double r = Math.Sqrt(1 - y * y);
            double x = r * Math.Cos(t);
            double z = r * Math.Sin(t);

            return new Point3D(x, y, z);
        }

        private static Vector3D GetNormal(double t, double y)
        {
            return (Vector3D)GetPosition(t, y);
        }

        private static Point GetTextureCoordinate(double t, double y)
        {
            Matrix TYtoUV = new Matrix();
            TYtoUV.Scale(1 / (2 * Math.PI), -0.5);

            Point p = new Point(t, y);
            p = p * TYtoUV;

            return p;
        }

        public double RotateX
        {
            get { return (double)GetValue(RotateXProperty); }
            set { SetValue(RotateXProperty, value); }
        }
        
        public static readonly DependencyProperty RotateXProperty = DependencyProperty.Register("RotateX", typeof(double), typeof(PanoramaView), new PropertyMetadata(0.0));

        public double RotateY
        {
            get { return (double)GetValue(RotateYProperty); }
            set { SetValue(RotateYProperty, value); }
        }
        
        public static readonly DependencyProperty RotateYProperty = DependencyProperty.Register("RotateY", typeof(double), typeof(PanoramaView), new PropertyMetadata(0.0));

        public static readonly DependencyProperty dependencyProperty;

        private bool _isOnDrag = false;
        private Point _startPoiint = new Point();
        private double _startRotateX = 0.0;
        private double _startRotateY = 0.0;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this._isOnDrag = true;
            this._startPoiint = e.GetPosition(this);
            this._startRotateX = this.RotateX;
            this._startRotateY = this.RotateY;
            base.OnMouseLeftButtonDown(e);
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this._isOnDrag == true && e.LeftButton == MouseButtonState.Pressed)
            {
                Vector delta = this._startPoiint - e.GetPosition(this);

                this.RotateX = this._startRotateX + (delta.X / this.ActualWidth * 360);
                this.RotateY = this._startRotateY + (delta.Y / this.ActualHeight * 360);
            }
            base.OnMouseMove(e);           
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isOnDrag = false;
            base.OnMouseLeftButtonUp(e);
        }
    }
}
