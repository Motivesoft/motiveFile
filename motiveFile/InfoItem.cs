﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace motiveFile
{
    public abstract class InfoItem
    {
        public abstract string Name
        {
            get;
        }
        public abstract string ModifiedDate
        {
            get;
        }
        public abstract string Type
        {
            get;
        }
        public abstract string Size
        {
            get;
        }
        public abstract string FullName
        {
            get;
        }
        public abstract bool IsTraversible
        {
            get;
        }
        public abstract FileAttributes Attributes
        {
            get;
        }

        public abstract long SortableModifiedDate
        {
            get;
        }
        public abstract long SortableSize
        {
            get;
        }
        public abstract int SortableInfoType
        {
            get;
        }

        public ImageSource Icon
        {
            get
            {
                return Imaging.CreateBitmapSourceFromHBitmap( 
                    Icons.GetSmallIcon( FullName, new System.Drawing.Size( 16, 16 ) ).ToBitmap().GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions() );
            }
        }
        protected string FormatDateTime( DateTime dateTime )
        {
            return $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";
        }

        protected string FormatSize( long size )
        {
            var length = Math.Ceiling( (double) size / 1024 );
            return $"{length:n0} KB";
        }
    }

    class DirectoryInfoItem : InfoItem
    {
        private readonly DirectoryInfo Info;

        public override string Name
        {
            get
            {
                return Info.Name;
            }
        }

        public override string ModifiedDate
        {
            get
            {
                return FormatDateTime( Info.LastWriteTimeUtc );
            }
        }

        public override string Type
        {
            get
            {
                return "File folder";
            }
        }

        public override string Size
        {
            get
            {
                return "";
            }
        }

        public override string FullName
        {
            get
            {
                return Info.FullName;
            }
        }

        public override FileAttributes Attributes
        {
            get
            {
                return Info.Attributes;
            }
        }

        public override bool IsTraversible
        {
            get
            {
                return true;
            }
        }

        public override long SortableModifiedDate
        {
            get
            {
                return Info.LastWriteTime.Ticks;
            }
        }
        public override long SortableSize
        {
            get
            {
                return 0;
            }
        }
        public override int SortableInfoType
        {
            get
            {
                return 0;
            }
        }

        public DirectoryInfoItem( string directory )
        {
            Info = new DirectoryInfo( directory );
        }
    }


    class FileInfoItem : InfoItem
    {
        private readonly FileInfo Info;

        public override string Name
        {
            get
            {
                return Info.Name;
            }
        }

        public override string ModifiedDate
        {
            get
            {
                return FormatDateTime( Info.LastWriteTimeUtc );
            }
        }

        public override string Size
        {
            get
            {
                return FormatSize( Info.Length );
            }
        }

        public override string Type
        {
            get
            {
                return FileTypes.GetFileTypeDescription( FullName );
            }
        }

        public override string FullName
        {
            get
            {
                return Info.FullName;
            }
        }

        public override FileAttributes Attributes
        {
            get
            {
                return Info.Attributes;
            }
        }

        public override bool IsTraversible
        {
            get
            {
                return false;
            }
        }

        public override long SortableModifiedDate
        {
            get
            {
                return Info.LastWriteTime.Ticks;
            }
        }

        public override long SortableSize
        {
            get
            {
                return Info.Length;
            }
        }
        public override int SortableInfoType
        {
            get
            {
                return 1;
            }
        }

        public FileInfoItem( string file )
        {
            Info = new FileInfo( file );
        }
    }

    public class DriveInfoItem : InfoItem
    {
        private readonly DriveInfo Info;

        public override string Name => Info.IsReady && !string.IsNullOrEmpty( Info.VolumeLabel) ? $"{Info.Name} ({Info.VolumeLabel})" : Info.Name;
        public override string FullName => Info.Name;
        public override string Size => Info.IsReady ? $"{Info.TotalSize}" : "";
        public override string Type => Info.DriveType.ToString();
        public override long SortableSize => Info.IsReady ? Info.TotalSize : 0;
        public override bool IsTraversible => true;
        public override string ModifiedDate => "";
        public override long SortableModifiedDate => 0;
        public override int SortableInfoType => 2;
        public override FileAttributes Attributes => FileAttributes.Device;

        public DriveInfoItem( DriveInfo info )
        {
            Info = info;
        }
    }
}
