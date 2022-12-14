using System;
using OpenCvSharp;

namespace EdcHost;

/// <summary>
/// A coordinate converter between coordinate systems
/// </summary>
/// <remarks>
/// This is the transformation between camera frame coordinate, monitor frame coordinate,
/// and the court coordinate.
/// </remarks>
public class CoordinateConverter : IDisposable
{
    #region Public properties

    public Point2f[] CalibrationCorners => this._calibrationCorners;

    #endregion

    #region Private fields

    // The transformation matrices
    private Mat _transformationCameraToCourt;
    private Mat _transformationCourtToCamera;
    private Mat _transformationMonitorToCamera;
    private Mat _transformationCameraToMonitor;

    /// <summary>
    /// The corners of the court in the camera coordinate system
    /// in the last calibration
    /// </summary>
    private Point2f[] _calibrationCorners;

    /// <summary>
    /// The corners of the court in the court coordinate system
    /// </summary>
    private readonly Point2f[] _courtCorners;

    #endregion

    /// <summary>
    /// Construct a coordinate converter.
    /// </summary>
    /// <param name="config">The information of the game</param>
    /// <param name="cameraFrameSize">
    /// The size of the camera frame
    /// </param>
    /// <param name="monitorFrameSize">
    /// The size of the monitor frame
    /// </param>
    /// <param name="courtSize">
    /// The size of the court
    /// </param>
    /// <param name="Point2f[]">
    /// The calibration information to initialize the converter
    /// </param>
    public CoordinateConverter(
        OpenCvSharp.Size cameraFrameSize,
        OpenCvSharp.Size monitorFrameSize,
        OpenCvSharp.Size courtSize,
        Point2f[] calibrationCorners = null
    )
    {
        Point2f[] camCorners = {
            new Point2f(0, 0),
            new Point2f(cameraFrameSize.Width, 0),
            new Point2f(0, cameraFrameSize.Height),
            new Point2f(cameraFrameSize.Width, cameraFrameSize.Height)
        };
        Point2f[] showCorners = {
            new Point2f(0, 0),
            new Point2f(monitorFrameSize.Width, 0),
            new Point2f(0, monitorFrameSize.Height),
            new Point2f(monitorFrameSize.Width, monitorFrameSize.Height)
        };

        this._courtCorners = new Point2f[] {
            new Point2f(0, 0),
            new Point2f(courtSize.Width, 0),
            new Point2f(0, courtSize.Height),
            new Point2f(courtSize.Width, courtSize.Height)
        };

        // By default, the court corners are at the same positions as the camera corners
        // in the camera coordinate system.
        if (calibrationCorners != null)
        {
            this._calibrationCorners = calibrationCorners;
        }
        else
        {
            this._calibrationCorners = camCorners;
        }

        // Get the position transformations between camera frames and monitor frames
        this._transformationCameraToMonitor = Cv2.GetPerspectiveTransform(camCorners, showCorners);
        this._transformationMonitorToCamera = Cv2.GetPerspectiveTransform(showCorners, camCorners);

        // Get the position transformations between camera frames and the court
        this._transformationCameraToCourt = Cv2.GetPerspectiveTransform(this._calibrationCorners, _courtCorners);
        this._transformationCourtToCamera = Cv2.GetPerspectiveTransform(_courtCorners, this._calibrationCorners);
    }

    /// <summary>
    /// Dispose the converter.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
    }

    /// <summary>
    /// Dispose the converter.
    /// </summary>
    /// <param name="disposing">
    /// True if to dispose
    /// </param>
    public void Dispose(bool disposing)
    {
        if (disposing)
        {
            _transformationCameraToCourt.Dispose();
            _transformationCourtToCamera.Dispose();
            _transformationMonitorToCamera.Dispose();
            _transformationCameraToMonitor.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Calibrate the coordinate transformation.
    /// </summary>
    /// <param name="corners">The four corners of the court in the monitor coordinate system</param>
    public void Calibrate(Point2f[] corners)
    {
        // Return if the corner list is invalid
        if (corners == null)
        {
            return;
        }
        if (corners.Length != 4)
        {
            return;
        }

        corners = Cv2.PerspectiveTransform(corners, _transformationMonitorToCamera);

        // Get the position transformations between camera frames and the court
        this._transformationCameraToCourt = Cv2.GetPerspectiveTransform(corners, this._courtCorners);
        this._transformationCourtToCamera = Cv2.GetPerspectiveTransform(this._courtCorners, corners);

        this._calibrationCorners = corners;
    }

    #region Transformations

    /// <summary>
    /// Convert the coordinates of a point from the monitor coordinate system to the camera coordinate system.
    /// </summary>
    /// <param name="points">The point in the monitor coordinate system</param>
    /// <returns>The point in the camera coordinate system</returns>
    public Point2f MonitorToCamera(Point2f point)
    {
        return Cv2.PerspectiveTransform(new Point2f[1] { point }, _transformationMonitorToCamera)[0];
    }

    /// <summary>
    /// Convert the coordinates of points from the monitor coordinate system to the camera coordinate system.
    /// </summary>
    /// <param name="points">The points in the monitor coordinate system</param>
    /// <returns>The points in the camera coordinate system</returns>
    public Point2f[] MonitorToCamera(Point2f[] points)
    {
        return Cv2.PerspectiveTransform(points, _transformationMonitorToCamera);
    }

    /// <summary>
    /// Convert the coordinates of a point from the camera coordinate system to the monitor coordinate system.
    /// </summary>
    /// <param name="points">The point in the camera coordinate system</param>
    /// <returns>The point in the monitor coordinate system</returns>
    public Point2f CameraToMonitor(Point2f point)
    {
        return Cv2.PerspectiveTransform(new Point2f[1] { point }, _transformationCameraToMonitor)[0];
    }

    /// <summary>
    /// Convert the coordinates of points from the camera coordinate system to the monitor coordinate system.
    /// </summary>
    /// <param name="points">The points in the camera coordinate system</param>
    /// <returns>The points in the monitor coordinate system</returns>
    public Point2f[] CameraToMonitor(Point2f[] points)
    {
        return Cv2.PerspectiveTransform(points, _transformationCameraToMonitor);
    }

    /// <summary>
    /// Convert the coordinates of a point from the camera coordinate system to the court coordinate system.
    /// </summary>
    /// <param name="points">The point in the camera coordinate system</param>
    /// <returns>The point in the court coordinate system</returns>
    public Point2f CameraToCourt(Point2f point)
    {
        return Cv2.PerspectiveTransform(new Point2f[1] { point }, _transformationCameraToCourt)[0];
    }

    /// <summary>
    /// Convert the coordinates of points from the camera coordinate system to the court coordinate system.
    /// </summary>
    /// <param name="points">The points in the camera coordinate system</param>
    /// <returns>The points in the court coordinate system</returns>
    public Point2f[] CameraToCourt(Point2f[] points)
    {
        return Cv2.PerspectiveTransform(points, _transformationCameraToCourt);
    }

    /// <summary>
    /// Convert the coordinates of a point from the court coordinate system to the camera coordinate system.
    /// </summary>
    /// <param name="points">The point in the court coordinate system</param>
    /// <returns>The point in the camera coordinate system</returns>
    public Point2f CourtToCamera(Point2f point)
    {
        return Cv2.PerspectiveTransform(new Point2f[1] { point }, _transformationCourtToCamera)[0];
    }

    /// <summary>
    /// Convert the coordinates of points from the court coordinate system to the camera coordinate system.
    /// </summary>
    /// <param name="points">The points in the court coordinate system</param>
    /// <returns>The points in the camera coordinate system</returns>
    public Point2f[] CourtToCamera(Point2f[] points)
    {
        return Cv2.PerspectiveTransform(points, _transformationCourtToCamera);
    }

    /// <summary>
    /// Convert the coordinates of a point from the monitor coordinate system to the court coordinate system.
    /// </summary>
    /// <param name="points">The point in the monitor coordinate system</param>
    /// <returns>The point in the court coordinate system</returns>
    public Point2f MonitorToCourt(Point2f point)
    {
        return this.CameraToCourt(this.MonitorToCamera(point));
    }

    /// <summary>
    /// Convert the coordinates of points from the monitor coordinate system to the court coordinate system.
    /// </summary>
    /// <param name="points">The points in the monitor coordinate system</param>
    /// <returns>The points in the court coordinate system</returns>
    public Point2f[] MonitorToCourt(Point2f[] points)
    {
        return this.CameraToCourt(this.MonitorToCamera(points));
    }

    /// <summary>
    /// Convert the coordinates of a point from the court coordinate system to the monitor coordinate system.
    /// </summary>
    /// <param name="points">The point in the court coordinate system</param>
    /// <returns>The point in the monitor coordinate system</returns>
    public Point2f CourtToMonitor(Point2f point)
    {
        return this.CameraToMonitor(this.CourtToCamera(point));
    }

    /// <summary>
    /// Convert the coordinates of points from the court coordinate system to the monitor coordinate system.
    /// </summary>
    /// <param name="points">The points in the court coordinate system</param>
    /// <returns>The points in the monitor coordinate system</returns>
    public Point2f[] CourtToMonitor(Point2f[] points)
    {
        return this.CameraToMonitor(this.CourtToCamera(points));
    }

    #endregion
}