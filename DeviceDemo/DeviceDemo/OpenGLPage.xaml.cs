
using OpenTK.Graphics.ES30;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DeviceDemo
{
    /// <summary>
    /// OpenGLView Sample
    /// https://developer.xamarin.com/api/type/Xamarin.Forms.OpenGLView/
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OpenGLPage : ContentPage
	{
        float red = 0f;
        float green = 0f;
        float blue = 0f;

		public OpenGLPage ()
		{
			InitializeComponent ();

            var view = new OpenGLView { HasRenderLoop = true };
            var toggle = new Switch { IsToggled = true };
            var button = new Button { Text = "Display" };

            // viewの構築
            view.HeightRequest = 300;
            view.WidthRequest = 300;

            view.OnDisplay = (r) =>
            {
                GL.ClearColor(red, green, blue, 1.0f);
                GL.Clear((ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

                // ちょっとずつ色を変える
                red = incrementColor(red, 0.01f);
                green = incrementColor(green, 0.02f);
                blue = incrementColor(blue, 0.03f);
            };

            // スイッチ
            toggle.Toggled += (s, a) =>
            {
                view.HasRenderLoop = toggle.IsToggled;
            };

            // ボタン
            button.Clicked += (s, a) =>
            {
                view.Display();
            };

            var stack = new StackLayout
            {
                Padding = new Size(20, 20),
                Children =
                {
                    view, toggle, button
                }
            };
            Content = stack;
		}

        /// <summary>
        /// 色をちょっとずつ変更する。1.0fを超えると1.0f未満に修正する
        /// </summary>
        /// <param name="org"></param>
        /// <param name="incr"></param>
        /// <returns></returns>
        private float incrementColor(float org, float incr)
        {
            float color = org + incr;
            if (color >= 1.0f)
            {
                color -= 1.0f;
            }
            return color;
        }
	}
}