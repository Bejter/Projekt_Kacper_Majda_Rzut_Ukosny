using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Projekt_Kacper_Majda.models;
using SkiaSharp;
using System.Linq;
using System.Reflection.Emit;
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

        public AnaliticSimulation analiticSimulation = new AnaliticSimulation()
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

        
        public double calculateHighestPointEuler()
        {
            // Funkcja obliczająca najwyższy punkt trajektorii dla symulacji Eulera
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

        public double calculateHighestPointAnalitic()
        {
            // Funkcja obliczająca najwyższy punkt trajektorii dla symulacji analitycznej
            TrajectoryPoint tp = new TrajectoryPoint()
            {
                time = 0,
                positionX = 0,
                positionY = 0,
                velocityX = 0,
                velocityY = 0
            };

            foreach (TrajectoryPoint point in trajectoryAnalitic)
            {
                if (point.positionY > tp.positionY)
                {
                    tp = point;
                }
            }

            return tp.positionY;
        }


        public void draw() // Funkcja rysująca trajektorię dla obu symulacji
        {
            // Obliczanie maksymalnego zasięgu i wysokości, aby ustawić odpowiednie limity na wykresie
            double maxX = Math.Max(trajectoryEuler.Max(p => p.positionX), trajectoryAnalitic.Max(p => p.positionX));
            double maxY = Math.Max(trajectoryEuler.Max(p => p.positionY), trajectoryAnalitic.Max(p => p.positionY));
            double maxRange = Math.Max(maxX, maxY);

            // Tworzenie listy punktów do narysowania na wykresie dla obu symulacji
            var pointsEuler = trajectoryEuler.Select(p => new ObservablePoint(p.positionX, p.positionY)).ToList();

            var pointsAnalitic = trajectoryAnalitic.Select(p => new ObservablePoint(p.positionX, p.positionY)).ToList();

            // Ustawianie osi i serii danych na wykresie
            trajectoryChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Zasięg [m]",
                    MinLimit = 0,
                    MaxLimit = maxRange
                }
            };

            trajectoryChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Wysokość [m]",
                    MinLimit = 0,
                    MaxLimit = maxRange
                }
            };

            // Ustawianie serii danych dla obu symulacji
            trajectoryChart.Series = new ISeries[]
            {
                
                new LineSeries<ObservablePoint>
                {
                    Values = pointsEuler,
                    Name = "Euler",
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    Fill = new SolidColorPaint(SKColors.Transparent)
                },

                new LineSeries<ObservablePoint>
                {
                    Values = pointsAnalitic,
                    Name = "Analitic",
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    Fill = new SolidColorPaint(SKColors.Transparent)
                }
            };
        }

        public void draw_error_chart()
        {
            //Ustawienie kroków czasowych do obliczenia błędu zasięgu dla symulacji Eulera
            double[] dtValues =
            {
                1,
                0.5,
                0.2,
                0.1,
                0.05,
                0.01
            };

            List<double> errors = new List<double>();

            // Obliczanie zasięgu dla symulacji analitycznej, który będzie służył jako wartość referencyjna do obliczenia błędu
            double analiticRange = trajectoryAnalitic.Last().positionX;

            foreach (double dtValue in dtValues)
            {
                // Tworzenie tymczasowej symulacji Eulera z aktualnym krokiem czasowym
                EulerSimulation tempEuler = new EulerSimulation()
                {
                    dt = dtValue,
                    G = eulerSimulation.G
                };
                // Obliczanie trajektorii dla tymczasowej symulacji Eulera
                List<TrajectoryPoint> tempTrajectory = tempEuler.Simulate(throwable);

                // Pobieranie zasięgu dla tymczasowej symulacji Eulera
                double eulerRange = tempTrajectory.Last().positionX;

                // Obliczanie błędu jako bezwzględnej różnicy między zasięgiem analitycznym a zasięgiem Eulera
                double error = Math.Abs(analiticRange - eulerRange);

                errors.Add(error);
            }

            // Ustawianie serii danych dla wykresu błędu zasięgu
            trajectoryChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = errors,
                    Name = "Błąd zasięgu",
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    Fill = null
                }
            };

            trajectoryChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Krok czasowy dt [s]",
                    Labels = new[]
                    {
                        "1",
                        "0.5",
                        "0.2",
                        "0.1",
                        "0.05",
                        "0.01"
                    }
                }
            };

            trajectoryChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Błąd zasięgu [m]",
                    MinLimit = 0
                }
            };
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Pobieranie danych z inputów i przypisywanie ich do obiektu throwable oraz symulacji
            throwable.mass = Convert.ToDouble(massInput.Text);
            throwable.height = Convert.ToDouble(heightInput.Text);
            throwable.velocity_start = Convert.ToDouble(startVelocityInput.Text); 
            throwable.angle = Convert.ToDouble(angleInput.Text);

            eulerSimulation.dt = Convert.ToDouble(dtInput.Text);
            eulerSimulation.G = Convert.ToDouble(gravityInput.Text);
            analiticSimulation.dt = Convert.ToDouble(dtInput.Text);
            analiticSimulation.G = Convert.ToDouble(gravityInput.Text);

            Console.WriteLine($"{0}", eulerSimulation.dt);

            // Obliczanie trajektorii dla obu symulacji
            trajectoryEuler = eulerSimulation.Simulate(throwable);
            trajectoryAnalitic = analiticSimulation.Simulate(throwable);

            draw();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e) // Pokazywanie danych z symulacji Eulera
        {
            double farest = trajectoryEuler.Last().positionX;
            double highest = calculateHighestPointEuler();
            double time = trajectoryEuler.Last().time;

            flightTimeLbl.Content = time;
            maksHeightLbl.Content = highest;
            roadLbl.Content = farest;
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e) // Pokazywanie danych z symulacji analitycznej
        {
            double farest = trajectoryAnalitic.Last().positionX;
            double highest = calculateHighestPointAnalitic();
            double time = trajectoryAnalitic.Last().time;

            flightTimeLbl.Content = time;
            maksHeightLbl.Content = highest;
            roadLbl.Content = farest;
        }

        private void RadioButton_Click_2(object sender, RoutedEventArgs e)
        {
            draw_error_chart();
        }

        private void RadioButton_Click_3(object sender, RoutedEventArgs e)
        {
            draw();
        }
    }
}