using System;
using Android.Hardware.Camera2;

namespace DeviceDemo.Droid
{
    class CameraSessionListener : CameraCaptureSession.StateCallback
    {
        private ICameraPreview owner;

        public CameraSessionListener(ICameraPreview owner)
        {
            this.owner = owner;
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            System.Diagnostics.Debug.WriteLine("CameraSessionListener.OnConfigured");

            this.owner.Session = session;
            // オートフォーカス
            this.owner.PreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Start);
            // プレビューの開始
            session.SetRepeatingRequest(this.owner.PreviewRequestBuilder.Build(), new CaptureListener(), null);
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            throw new NotImplementedException();
        }
    }
}