using System;
using Android.Hardware.Camera2;

namespace DeviceDemo.Droid
{
    class CameraSessionListener : CameraCaptureSession.StateCallback
    {
        private CameraPageRenderer owner;

        public CameraSessionListener(CameraPageRenderer owner)
        {
            this.owner = owner;
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            System.Diagnostics.Debug.WriteLine("CameraSessionListener.OnConfigured");

            this.owner.session = session;
            // オートフォーカス
            this.owner.previewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Start);
            // プレビューの開始
            session.SetRepeatingRequest(this.owner.previewRequestBuilder.Build(), new CaptureListener(), null);
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            throw new NotImplementedException();
        }
    }
}