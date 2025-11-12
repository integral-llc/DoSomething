using System;
using System.Collections.Generic;
using System.Drawing;

namespace DoSomething
{
    /// <summary>
    /// Implements human-like mouse movement using cubic Bezier curves with easing
    /// </summary>
    public class HumanLikeMouseMovement : IMouseMovementStrategy
    {
        private readonly Random _random;

        public HumanLikeMouseMovement()
        {
            _random = new Random();
        }

        public IEnumerable<Point> GeneratePath(Point start, Point end)
        {
            double distance = CalculateDistance(start, end);
            int steps = Math.Max(20, (int)(distance / 10));

            Point cp1 = GenerateControlPoint(start, end, 0.33);
            Point cp2 = GenerateControlPoint(start, end, 0.66);

            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double easedT = EaseInOutCubic(t);

                Point p = CalculateBezierPoint(easedT, start, cp1, cp2, end);

                // Add micro-jitter for realism
                if (i > 0 && i < steps)
                {
                    p.X += _random.Next(-1, 2);
                    p.Y += _random.Next(-1, 2);
                }

                yield return p;
            }
        }

        private Point GenerateControlPoint(Point start, Point end, double position)
        {
            int x = (int)(start.X + (end.X - start.X) * position);
            int y = (int)(start.Y + (end.Y - start.Y) * position);

            double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
            double perpAngle = angle + Math.PI / 2;

            double offsetDistance = CalculateDistance(start, end) * 0.2 * (_random.NextDouble() - 0.5);

            x += (int)(Math.Cos(perpAngle) * offsetDistance);
            y += (int)(Math.Sin(perpAngle) * offsetDistance);

            return new Point(x, y);
        }

        private Point CalculateBezierPoint(double t, Point p0, Point p1, Point p2, Point p3)
        {
            double u = 1 - t;
            double tt = t * t;
            double uu = u * u;
            double uuu = uu * u;
            double ttt = tt * t;

            int x = (int)(uuu * p0.X + 3 * uu * t * p1.X + 3 * u * tt * p2.X + ttt * p3.X);
            int y = (int)(uuu * p0.Y + 3 * uu * t * p1.Y + 3 * u * tt * p2.Y + ttt * p3.Y);

            return new Point(x, y);
        }

        private double EaseInOutCubic(double t)
        {
            return t < 0.5
                ? 4 * t * t * t
                : 1 - Math.Pow(-2 * t + 2, 3) / 2;
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            var dy = p2.Y - p1.Y;
            var dx = p2.X - p1.X;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
