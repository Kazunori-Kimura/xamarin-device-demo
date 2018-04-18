using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace DeviceDemo.Droid
{
    class CameraPreview : ViewGroup, ISurfaceHolderCallback
    {
        SurfaceView surfaceView;
        ISurfaceHolder holder;
        IWindowManager windowManager;

        /// <summary>
        /// プレビュー状態
        /// </summary>
        public bool IsPreviewing { get; set; }


        public CameraPreview(Context context) : base(context)
        {
            surfaceView = new SurfaceView(context);
            AddView(surfaceView);

            windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            IsPreviewing = false;
            holder = surfaceView.Holder;
            holder.AddCallback(this);
        }

        /// <summary>
        /// TODO いつ呼ばれるの??
        /// </summary>
        /// <param name="widthMeasureSpec"></param>
        /// <param name="heightMeasureSpec"></param>
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            throw new NotImplementedException();
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Android.Graphics.Format format, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            throw new NotImplementedException();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            throw new NotImplementedException();
        }

    }
}