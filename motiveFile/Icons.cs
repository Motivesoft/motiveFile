using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace motiveFile
{
    public class Icons
    {
        [StructLayout( LayoutKind.Sequential )]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
            public string szDisplayName;
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 80 )]
            public string szTypeName;
        };

        class Win32
        {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

            [DllImport( "shell32.dll" )]
            public static extern IntPtr SHGetFileInfo( string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags );

            [DllImport( "User32.dll" )]
            public static extern int DestroyIcon( IntPtr hIcon );

        }

        static Icons()
        {
            // Do nothing
        }

        private static Icon GetPaddedIcon( Icon ico, int xpad, int ypad )
        {
            var bmp = new Bitmap( ico.Width + xpad + xpad, ico.Height + ypad + ypad );

            var iconBmp = ico.ToBitmap();

            for ( int x = 0; x < ico.Width; x++ )
            {
                for ( int y = 0; y < ico.Height; y++ )
                {
                    bmp.SetPixel( x + xpad, y + ypad, iconBmp.GetPixel( x, y ) );
                }
            }

            var hIcon = bmp.GetHicon();
            Icon icon = (Icon) Icon.FromHandle( hIcon ).Clone();
            Win32.DestroyIcon( hIcon );

            return icon;
        }

        public static Icon GetSmallIcon( string fileName, Size size )
        {
            var smallIcon = GetIcon( fileName, Win32.SHGFI_SMALLICON );

            if ( !size.IsEmpty )
            {
                var xpad = ( size.Width - 16 ) / 2;
                var ypad = ( size.Height - 16 ) / 2;
                smallIcon = GetPaddedIcon( smallIcon, xpad, ypad );
            }

            return smallIcon;
        }

        public static Icon GetLargeIcon( string fileName )
        {
            return GetIcon( fileName, Win32.SHGFI_LARGEICON );
        }

        private static Icon GetIcon( string fileName, uint flags )
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImgSmall = Win32.SHGetFileInfo( fileName, 0, ref shinfo, (uint) Marshal.SizeOf( shinfo ), Win32.SHGFI_ICON | flags );

            Icon icon = (Icon) Icon.FromHandle( shinfo.hIcon ).Clone();
            Win32.DestroyIcon( shinfo.hIcon );
            return icon;
        }
    }
}
