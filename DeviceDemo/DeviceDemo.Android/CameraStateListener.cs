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
        private CameraPageRenderer owner;

        public CameraStateListener(CameraPageRenderer owner)
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

            owner.camera = camera;
            var surface = owner.Surface;

            List<Surface> list = new List<Surface>
            {
                surface
            };

            owner.previewRequestBuilder = camera.CreateCaptureRequest(CameraTemplate.Preview);
            owner.previewRequestBuilder.AddTarget(surface);

            camera.CreateCaptureSession(list, new CameraSessionListener(this.owner), null);
        }
    }
}