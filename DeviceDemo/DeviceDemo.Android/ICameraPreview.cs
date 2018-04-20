using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Views;

namespace DeviceDemo.Droid
{
    interface ICameraPreview
    {
        CameraDevice Camera { get; set; }
        CameraCaptureSession Session { get; set; }
        TextureView TextureView { get; set; }
        SurfaceTexture SurfaceTexture { get; set; }
        Android.Util.Size PreviewSize { get; set; }
        CaptureRequest.Builder PreviewRequestBuilder { get; set; }
        Surface Surface { get; }
    }
}