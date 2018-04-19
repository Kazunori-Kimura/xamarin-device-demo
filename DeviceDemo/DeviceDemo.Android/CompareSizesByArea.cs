using Android.Util;
using Java.Lang;
using Java.Util;

namespace DeviceDemo.Droid
{
    class CompareSizesByArea : Java.Lang.Object, IComparator
    {
        public int Compare(Java.Lang.Object o1, Java.Lang.Object o2)
        {
            var o1Size = (Size)o1;
            var o2Size = (Size)o2;

            return Long.Signum((long)o1Size.Width * o1Size.Height - (long)o2Size.Width * o2Size.Height);
        }
    }
}