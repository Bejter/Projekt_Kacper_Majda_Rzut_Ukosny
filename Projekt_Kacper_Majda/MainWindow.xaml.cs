using Projekt_Kacper_Majda.models;
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
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using System.Linq;
using LiveChartsCore.Painting;

namespace Projekt_Kacper_Majda
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ThrowableObject throwable = new ThrowableObject()
        {
            mass = 0,
            height = 0,
            velocity_start = 0,
            angle = 0
        };

        public EulerSimulation eulerSimulation = new EulerSimulation()
        {
            dt = 0,
            G = 0
        };

        public EulerSimulation analiticSimulation = new EulerSimulation()
        {
            dt = 0,
            G = 0
        };

        public List<TrajectoryPoint> trajectoryEuler = new List<TrajectoryPoint>();
        public List<TrajectoryPoint> trajectoryAnalitic = new List<TrajectoryPoint>();


        public MainWindow()
        {
            InitializeComponent();

            trajectoryChart.BorderThickness = new Thickness(3);
        }

        
        public double calculateHighestPoint()
        {
            TrajectoryPoint tp = new TrajectoryPoint()
            {
                time = 0,
                positionX = 0,
                positionY = 0,
                velocityX = 0,
                velocityY = 0
            };

            foreach(TrajectoryPoint point in trajectoryEuler)
            {
                if(point.positionY > tp.positionY)
                {
                    tp = point;
                }
            }

            return tp.positionY;
        }


        public void draw()
        {
            var points = trajectoryEuler.Select(p => new ObservablePoint(p.positionX, p.positionY)).ToList();

            

            points = trajectoryAnalitic.Select(p => new ObservablePoint(p.positionX, p.positionY)).ToList();

            trajectoryChart.Series = new ISeries[]
            {
                
                new LineSeries<ObservablePoint>
                {
                    Values = points,
                    Name = "Euler",
                    GeometrySize = 0,
                    LineSmoothness = 0
                }
            };
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            throwable.mass = Convert.ToDouble(massInput.Text);
            throwable.height = Convert.ToDouble(heightInput.Text);
            throwable.velocity_start = Convert.ToDouble(startVelocityInput.Text); 
            throwable.angle = Convert.ToDouble(angleInput.Text);

            eulerSimulation.dt = Convert.ToDouble(dtInput.Text);
            eulerSimulation.G = Convert.ToDouble(gravityInput.Text);
            analiticSimulation.dt = Convert.ToDouble(dtInput.Text);
            analiticSimulation.G = Convert.ToDouble(gravityInput.Text);

            Console.WriteLine($"{0}", eulerSimulation.dt);

            trajectoryEuler = eulerSimulation.Simulate(throwable);
            trajectoryAnalitic = analiticSimulation.Simulate(throwable);

            double farest = trajectoryEuler.Last().positionX;
            double highest = calculateHighestPoint();
            double time = trajectoryEuler.Last().time;

            flightTimeLbl.Content = time;
            maksHeightLbl.Content = highest;
            roadLbl.Content = farest;

            draw();
        }
    }
}