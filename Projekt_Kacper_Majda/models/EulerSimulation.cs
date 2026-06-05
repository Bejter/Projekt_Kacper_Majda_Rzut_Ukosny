using System;
using System.Collections.Generic;
using System.Text;

namespace Projekt_Kacper_Majda.models
{
    public class EulerSimulation
    {
        //Klasa odpowiadająca za obliczanie trajektorii metodą Eulera
        public double dt { get; set; }
        public double G { get; set; }

        public List<TrajectoryPoint> Simulate(ThrowableObject obj) // Obliczanie trajektori metodą Eulera, zwraca listę obiektów klasy TrajectortyPoint
        {
            List<TrajectoryPoint> trajectory = new List<TrajectoryPoint>();

            double t = 0;
            double x = 0;
            double y = obj.height;

            double oldT = 0;
            double oldX = 0;
            double oldY = 0;
            double oldVy = 0;

            //Przeliczanie z kąta na radiany
            double angle_rad = obj.angle * (Math.PI / 180);

            //Obliczanie prędkości w każdym kierunku
            double vx = obj.velocity_start * Math.Cos(angle_rad);
            double vy = obj.velocity_start * Math.Sin(angle_rad);

            while(true)
            {
                //Dodanie punktu do listy
                trajectory.Add(new TrajectoryPoint()
                {
                    positionX = x,
                    positionY = y,
                    time = t,
                    velocityX = vx,
                    velocityY = vy
                });

                //Zapisywanie starych współrzędnych
                oldX = x;
                oldY = y;
                oldT = t;
                oldVy = vy;

                //aktualizacja współrzędnych, czasu oraz prędkości
                x = x + vx * dt;
                y = y + vy * dt;
                vy = vy - G * dt;
                t = t + dt;

                if (y < 0) // przerywanie pętli jeżeli y < 0 oraz obliczanie punktu w którym y = 0 
                { 
                    double proportion = oldY / (oldY -  y);

                    double zeroX = oldX + proportion * (x - oldX);
                    double zeroT = oldT + proportion * (t - oldT);
                    double zeroVy = oldVy + proportion * (vy - oldVy);

                    //Dodanie punktu w miejscu y = 0
                    trajectory.Add(new TrajectoryPoint()
                    {
                        positionX = zeroX,
                        positionY = 0,
                        time = zeroT,
                        velocityX = vx,
                        velocityY = zeroVy
                    });
                    break; 
                } 
            }
            return trajectory; 
        }
    }
}
