using System;
using System.Collections.Generic;
using Android.Hardware.Camera2;
using Android.Runtime;
using Android.Views;

namespace DeviceDemo.Droid
{
    /// <summary>
    /// CameraDeviceのコールバックを受けるクラス
    /// </summary>
    class CameraStateListener : CameraDevice.StateCallback
    {
        private ICameraPreview owner;

        public CameraStateListener(ICameraPreview owner)
        {
            this.owner = owner;
        }

        public override void OnDisconnected(CameraDevice camera)
        {
            throw new NotImplementedException();
        }

        public override void OnError(CameraDevice camera, [GeneratedEnum] CameraError error)
        {
            throw new NotImplementedException();
        }

        public override void OnOpened(CameraDevice camera)
        {
            System.Diagnostics.Debug.WriteLine("CameraStateListener.OnOpened");

            owner.Camera = camera;
            var surface = owner.Surface;

            List<Surface> list = new List<Surface>
            {
                surface
            };

            owner.PreviewRequestBuilder = camera.CreateCaptureRequest(CameraTemplate.Preview);
            owner.PreviewRequestBuilder.AddTarget(surface);

            camera.CreateCaptureSession(list, new CameraSessionListener(this.owner), null);
        }
    }
}