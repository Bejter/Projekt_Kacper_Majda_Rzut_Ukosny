using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace Projekt_Kacper_Majda.models
{
    public class AnaliticSimulation
    {
        public double dt { get; set; }
        public double G { get; set; }

        public List<TrajectoryPoint> Simulate(ThrowableObject obj) //Obliczanie trajektori metodą Analityczną, zwraca listę obiektów klasy TrajectortyPoint
        {
            List<TrajectoryPoint> trajectory = new List<TrajectoryPoint>();
            
            double t = 0;
            double x = 0;
            double y = obj.height;

            //Przeliczanie z kąta na radiany
            double angle_rad = obj.angle * (Math.PI / 180);

            //Obliczanie prędkości w każdym kierunku
            double vx = obj.velocity_start * Math.Cos(angle_rad);
            double vy = obj.velocity_start * Math.Sin(angle_rad);

            double y0 = obj.height;
            double vy0 = vy;

            //Dodanie pierwszego punktu trajektorii
            trajectory.Add(new TrajectoryPoint()
            {
                time = t,
                positionX = x,
                positionY = y,
                velocityX = vx,
                velocityY = vy
            });

            //zmienne pomocnicze do przechowywania poprzednich wartości
            double oldX = x;
            double oldY = y;
            double oldVy = vy;
            double oldT = t;

            while (true)
            {
                //Aktualizacja poprzednich wartości
                oldX = x;
                oldY = y;
                oldT = t;
                oldVy = vy;

                //Aktualizacja czasu
                t += dt;

                //Obliczanie nowych wartości pozycji i prędkości
                x = vx * t;
                y = y0 + vy0 * t - 0.5 * G * t * t;
                vy = vy0 - G * t; 

                if(y<0) // Obliczanie punktu, w którym obiekt dotyka ziemi, jeśli y jest mniejsze od 0, metodą intepolacji liniowej między ostatnim punktem a aktualnym punktem, gdzie y jest mniejsze od 0
                {
                    double proportion = oldY / (oldY - y);

                    double zeroX = oldX + proportion * (x - oldX);
                    double zeroT = oldT + proportion * (t - oldT);
                    double zeroVy = oldVy + proportion * (vy - oldVy);

                    trajectory.Add(new TrajectoryPoint()
                    {
                        positionX = zeroX,
                        positionY = 0,
                        time = zeroT,
                        velocityX = vx,
                        velocityY = zeroVy,
                    });

                    break;
                }else
                {
                    //Dodanie punktu trajektorii do listy
                    trajectory.Add(new TrajectoryPoint()
                    {
                        time = t,
                        positionX = x,
                        positionY = y,
                        velocityX = vx,
                        velocityY = vy
                    });
                }
            }

            return trajectory;
        }
    }
}
