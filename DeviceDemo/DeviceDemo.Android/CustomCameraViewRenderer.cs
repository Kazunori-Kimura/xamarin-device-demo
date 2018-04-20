using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Views;
using Android.Widget;
using DeviceDemo;
using DeviceDemo.Droid;
using Java.Lang;
using Java.Util;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomCameraView), typeof(CustomCameraViewRenderer))]
namespace DeviceDemo.Droid
{
    class CustomCameraViewRenderer : ViewRenderer<CustomCameraView, Android.Views.View>, TextureView.ISurfaceTextureListener, ICameraPreview
    {
        Activity activity;
        Android.Views.View view;

        public CameraDevice Camera { get; set; }
        public CameraCaptureSession Session { get; set; }
        public TextureView TextureView { get; set; }
        public SurfaceTexture SurfaceTexture { get; set; }
        public Android.Util.Size PreviewSize { get; set; }
        public CaptureRequest.Builder PreviewRequestBuilder { get; set; }

        public Surface Surface
        {
            get
            {
                return new Surface(SurfaceTexture);
            }
        }

        public CustomCameraViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CustomCameraView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetupUserInterface();
                AddView(view);
            }
        }

        private void SetupUserInterface()
        {
            activity = (Activity)Context;
            view = activity.LayoutInflater.Inflate(Resource.Layout.CameraLayout, this, false);

            TextureView = view.FindViewById<TextureView>(Resource.Id.textureView);
            TextureView.SurfaceTextureListener = this;
        }


        /// <summary>
        /// 背面カメラを開く
        /// </summary>
        private void OpenBackCamera()
        {
            System.Diagnostics.Debug.WriteLine("CameraPageRenderer.OpenBackCamera");

            string selectedCameraId = string.Empty;
            CameraManager manager = (CameraManager)activity.GetSystemService(Context.CameraService);

            // 背面カメラを探す
            foreach (var cameraId in manager.GetCameraIdList())
            {
                var characteristics = manager.GetCameraCharacteristics(cameraId);
                var facing = (Integer)characteristics.Get(CameraCharacteristics.LensFacing);
                if (facing != null && facing == Integer.ValueOf((int)LensFacing.Back))
                {
                    selectedCameraId = cameraId;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(selectedCameraId))
            {
                // プレビューサイズの取得
                PreviewSize = this.GetPreviewSize(selectedCameraId);
                System.Diagnostics.Debug.WriteLine($"previewSize: w={PreviewSize.Width},h={PreviewSize.Height}");

                // これでいいの？
                TextureView.LayoutParameters = new FrameLayout.LayoutParams(PreviewSize.Width, PreviewSize.Height);
                SurfaceTexture.SetDefaultBufferSize(PreviewSize.Width, PreviewSize.Height);

                // カメラを開く (成功したらコールバックが呼ばれる)
                manager.OpenCamera(selectedCameraId, new CameraStateListener(this), null);
            }
            else
            {
                string msg = "背面カメラが見つけられなかった";
                System.Diagnostics.Debug.WriteLine(msg);
                throw new System.Exception(msg);
            }
        }

        /// <summary>
        /// プレビューの画面サイズを算出
        /// </summary>
        /// <param name="cameraId"></param>
        /// <returns></returns>
        private Android.Util.Size GetPreviewSize(string cameraId)
        {
            CameraManager manager = (CameraManager)activity.GetSystemService(Context.CameraService);
            var characteristics = manager.GetCameraCharacteristics(cameraId);
            StreamConfigurationMap map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
            // 最大サイズからratioを取得
            var maxSize = (Android.Util.Size)Collections.Max(Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg)), new CompareSizesByArea());
            int h = maxSize.Height;
            int w = maxSize.Width;
            System.Diagnostics.Debug.WriteLine($"max size: width={w},height={h}");

            // ディスプレイサイズ
            var displaySize = new Android.Graphics.Point();
            activity.WindowManager.DefaultDisplay.GetSize(displaySize);
            // 横向きに補正
            var maxWidth = displaySize.X > displaySize.Y ? displaySize.X : displaySize.Y;
            var maxHeight = displaySize.Y < displaySize.X ? displaySize.Y : displaySize.X;
            System.Diagnostics.Debug.WriteLine($"display: width={maxWidth},height={maxHeight}");

            // 画面サイズに収まり、アスペクト比が一致するサイズ
            var list = new List<Android.Util.Size>();
            var sizes = map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture)));
            foreach (var size in sizes)
            {
                System.Diagnostics.Debug.WriteLine($"size: width={size.Width},height={size.Height}");

                if ((size.Width <= maxWidth) && (size.Height <= maxHeight) && size.Height == size.Width * h / w)
                {
                    list.Add(size);
                }
            }

            // 最大のやつを取得
            var prevSize = (Android.Util.Size)Collections.Max(list, new CompareSizesByArea());
            // 無理から縦向きに変更
            var ret = new Android.Util.Size(prevSize.Height, prevSize.Width);

            return ret;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            System.Diagnostics.Debug.WriteLine("CameraPageRenderer.OnLayout");

            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            view.Measure(msw, msh);
            view.Layout(0, 0, r - l, b - t);
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            System.Diagnostics.Debug.WriteLine("OnSurfaceTextureAvailable.");
            SurfaceTexture = surface;

            this.OpenBackCamera();
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            System.Diagnostics.Debug.WriteLine("OnSurfaceTextureDestroyed.");

            // カメラプレビューを終了してカメラを開放
            if (Session != null)
            {
                try
                {
                    Session.StopRepeating();
                }
                catch (CameraAccessException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    e.PrintStackTrace();
                }
            }

            if (Camera != null)
            {
                Camera.Close();
            }

            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }
    }
}