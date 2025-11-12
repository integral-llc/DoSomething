using System.Collections.Generic;
using System.Drawing;

namespace DoSomething
{
    /// <summary>
    /// Strategy pattern interface for different mouse movement algorithms
    /// </summary>
    public interface IMouseMovementStrategy
    {
        /// <summary>
        /// Generates a path of points from start to end
        /// </summary>
        IEnumerable<Point> GeneratePath(Point start, Point end);
    }
}
