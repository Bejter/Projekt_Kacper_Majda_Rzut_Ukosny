using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Projekt_Kacper_Majda.models
{
    public class TrajectoryPoint
    {
        //Klasa odpowiadająca za punkt w układzie
        public double time {  get; set; }
        public double positionX { get; set; }
        public double positionY { get; set; }

        public double velocityX { get; set; }
        public double velocityY { get; set; }
    }
}
