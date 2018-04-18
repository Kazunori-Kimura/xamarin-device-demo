using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DeviceDemo
{
    public partial class MainPage : ContentPage
	{
        IDeviceMotion motion = CrossDeviceMotion.Current;

        private bool capturingAccelerometer = false;
        private bool capturingGyroscope = false;
        private bool capturingCompass = false;

        public MainPage()
		{
			InitializeComponent();

            // 現在位置
            CurrentLocation.Clicked += async (object s, EventArgs e) =>
            {
                var position = await this.GetCurrentLocation();
                if (position != null)
                {
                    LatLng.Text = $"lat:{position.Latitude} lng:{position.Longitude}";
                }
            };

            // 加速度センサー
            ToggleAccelerometer.Clicked += (object s, EventArgs e) =>
            {
                // フラグを反転
                capturingAccelerometer = !capturingAccelerometer;

                if (capturingAccelerometer)
                {
                    StartAccelerometerCapture();
                }
                else
                {
                    StopAccelerometerCapture();
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    ToggleAccelerometer.Text = capturingAccelerometer ? "停止" : "開始";
                });
            };

            // ジャイロ
            ToggleGyroscope.Clicked += (object s, EventArgs e) =>
            {
                // フラグを反転
                capturingGyroscope = !capturingGyroscope;

                if (capturingGyroscope)
                {
                    StartGyroscopeCapture();
                }
                else
                {
                    StopGyroscopeCapture();
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    ToggleGyroscope.Text = capturingGyroscope ? "停止" : "開始";
                });
            };

            // コンパス
            ToggleCompass.Clicked += (object s, EventArgs e) =>
            {
                capturingCompass = !capturingCompass;

                if (capturingCompass)
                {
                    StartCompassCapture();
                }
                else
                {
                    StopCompassCapture();
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    ToggleCompass.Text = capturingCompass ? "停止" : "開始";
                });
            };

            // preview表示
            PreviewCamera.Clicked += async (object s, EventArgs e) =>
            {
                await Navigation.PushAsync(new CameraPage());
            };

            // 確認
            CheckLocationPermissionStatusAsync().Wait();
		}

        /// <summary>
        /// 位置情報の取得
        /// </summary>
        /// <returns></returns>
        private async Task<Position> GetCurrentLocation()
        {
            try
            {
                var location = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(10));
                if (location != null)
                {
                    return new Position(location.Latitude, location.Longitude);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        /// 位置情報の取得権限確認
        /// </summary>
        /// <returns></returns>
        private async Task CheckLocationPermissionStatusAsync()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                // リクエストを行う
                status = (await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location))[Permission.Location];
            }
        }

        /// <summary>
        /// 加速度センサーの値を取得
        /// </summary>
        private void StartAccelerometerCapture()
        {
            motion.Start(MotionSensorType.Accelerometer, MotionSensorDelay.Default);
            if (motion.IsActive(MotionSensorType.Accelerometer))
            {
                motion.SensorValueChanged += (object s, SensorValueChangedEventArgs e) =>
                {
                    if (!capturingAccelerometer)
                    {
                        return;
                    }

                    var value = (MotionVector)e.Value;
                    Debug.WriteLine($"accelerometer => X:{value.X} Y:{value.Y} Z:{value.Z}");
                    // Formに描画する
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        AccelerometerValue.Text = $"X:{value.X} Y:{value.Y} Z:{value.Z}";
                    });
                };
            }
        }

        /// <summary>
        /// 加速度センサーの取得を停止
        /// </summary>
        private void StopAccelerometerCapture()
        {
            motion.Stop(MotionSensorType.Accelerometer);
        }

        /// <summary>
        /// ジャイロの取得を開始
        /// </summary>
        private void StartGyroscopeCapture()
        {
            motion.Start(MotionSensorType.Gyroscope, MotionSensorDelay.Default);
            if (motion.IsActive(MotionSensorType.Gyroscope))
            {
                motion.SensorValueChanged += (object s, SensorValueChangedEventArgs e) =>
                {
                    if (!capturingGyroscope)
                    {
                        return;
                    }

                    var value = (MotionVector)e.Value;
                    Debug.WriteLine($"gyroscope => X:{value.X} Y:{value.Y} Z:{value.Z}");
                    // Formに描画する
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        GyroscopeValue.Text = $"X:{value.X} Y:{value.Y} Z:{value.Z}";
                    });
                };
            }
        }

        /// <summary>
        /// ジャイロの取得を停止
        /// </summary>
        private void StopGyroscopeCapture()
        {
            motion.Stop(MotionSensorType.Gyroscope);
        }

        /// <summary>
        /// コンパスの取得を開始
        /// </summary>
        private void StartCompassCapture()
        {
            motion.Start(MotionSensorType.Compass, MotionSensorDelay.Default);
            if (motion.IsActive(MotionSensorType.Compass))
            {
                motion.SensorValueChanged += (object s, SensorValueChangedEventArgs e) =>
                {
                    if (!capturingCompass)
                    {
                        return;
                    }

                    var value = e.Value.Value;
                    if (value.HasValue)
                    {
                        double v = value.Value;
                        Debug.WriteLine($"compass => value:{v}");
                        // Formに描画する
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            CompassValue.Text = $"value:{v}";
                        });
                    }
                };
            }
        }

        /// <summary>
        /// コンパスの取得を停止
        /// </summary>
        private void StopCompassCapture()
        {
            motion.Stop(MotionSensorType.Compass);
        }
	}
}
