using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return "";
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

        public override bool IsTraversible
        {
            get
            {
                return true;
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
                // TODO finish this
                return Info.Extension;
            }
        }

        public override string FullName
        {
            get
            {
                return Info.FullName;
            }
        }

        public override bool IsTraversible
        {
            get
            {
                return false;
            }
        }

        public FileInfoItem( string file )
        {
            Info = new FileInfo( file );
        }
    }
}
