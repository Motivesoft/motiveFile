using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace motiveFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Initialisation
        private readonly string defaultPath = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );

        private string[] arguments;

        // State variables
        private string currentPath;

        private bool altKeyDown = false;

        public MainWindow( string[] args )
        {
            arguments = args;

            InitializeComponent();
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            var initialPath = defaultPath;

            if ( arguments.Length > 0 )
            {
                var arg0 = arguments[ 0 ];

                // Allow a path on the command line, but validate it before we use it
                if ( Directory.Exists( arg0 ) )
                {
                    initialPath = arg0;
                }
                else
                {
                    MessageBox.Show( $"{arg0} is not a valid path" );
                }
            }

            UpdateView( initialPath );
        }

        private void UpdateView( string newPath )
        {
            if ( Directory.Exists( newPath ) )
            {
                //...
                var fullPath = newPath;
                try
                {
                    // Add a little convenience thing of ~ for home directory - make it an option?
                    if ( fullPath.StartsWith( "~" ) )
                    {
                        // Prune following slashes - but only in the case of ~
                        var remnant = fullPath.Substring( 1 );
                        while ( remnant.StartsWith( "/" ) || remnant.StartsWith( @"\" ) )
                        {
                            remnant = remnant.Substring( 1 );
                        }
                        fullPath = System.IO.Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.UserProfile ), remnant );
                    }

                    // Do we really want to do this? I guess so - make it an option?
                    fullPath = fullPath.Replace( "/", @"\" );

                    var items = new List<InfoItem>();

                    foreach ( var dir in Directory.GetDirectories( fullPath ) )
                    {
                        items.Add( new DirectoryInfoItem( dir ) );
                    }

                    foreach ( var file in Directory.GetFiles( fullPath ) )
                    {
                        items.Add( new FileInfoItem( file ) );
                    }

                    Display( newPath, items );
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( $"Failed: {ex.Message}" );

                    // Make sure we go back to last good value...
                    if ( listView.Tag is string )
                    {
                        // ...unless we were there already
                        if ( ( listView.Tag as string ) != newPath )
                        {
                            UpdateView( listView.Tag as string );
                        }
                    }
                }
            }
            else if( !newPath.Equals( currentPath ) )
            {
                UpdateView( currentPath );
            }
            else if ( !newPath.Equals( defaultPath ) )
            {
                UpdateView( defaultPath );
            }
        }

        private void Display( string path, List<InfoItem> items )
        {
            try
            {
                currentPath = path;

                var from = listView.Tag as string;
                var focused = false;
                //var smallIconSize = smallImageList.ImageSize;

                textBox.Text = path;
                listView.Tag = path;

                listView.ItemsSource = items;

                {
                    /*
                    if ( !smallImageList.Images.ContainsKey( dir.FullName ) )
                    {
                        smallImageList.Images.Add( dir.FullName, Icons.GetSmallIcon( dir.FullName, smallIconSize ) );
                    }
                    lvi.ImageKey = dir.FullName;
                    lvi.Tag = dir;

                    lvi.SubItems.Add( GetDateString( dir.LastWriteTime ) );
                    lvi.SubItems.Add( "" ); // Size
                    lvi.SubItems.Add( "File folder" );
                    */
                    /*
                    listView.Items.Add( lvi );

                    if ( dir.FullName == from )
                    {
                        focused = true;
                        lvi.Focus();

                        listView.SelectedItem = lvi;
                    }
                    */
                }

                /*
                foreach ( var file in files )
                {
                    var lvi = new ListViewItem( file.Name );

                    if ( !smallImageList.Images.ContainsKey( file.FullName ) )
                    {
                        smallImageList.Images.Add( file.FullName, Icons.GetSmallIcon( file.FullName, smallIconSize ) );
                    }
                    lvi.ImageKey = file.FullName;
                    lvi.Tag = file;

                    lvi.SubItems.Add( GetDateString( file.LastWriteTime ) );
                    lvi.SubItems.Add( GetSizeString( file.Length ) ); // Size
                    lvi.SubItems.Add( GetTypeString( file.Extension ) );

                    listView1.Items.Add( lvi );
                }
                */
                if ( !focused && listView.Items.Count > 0 && listView.Items[ 0 ] is ListViewItem )
                {
                    (listView.Items[ 0 ] as ListViewItem).Focus();
                }
            }
            finally
            {
                listView.Focus();
            }
        }

        private void listView_PreviewKeyDown( object sender, KeyEventArgs e )
        {
            if( e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Right )
            {
                if ( listView.SelectedItems.Count > 0 )
                {
                    var lvi0 = listView.SelectedItems[ 0 ] as InfoItem;
                    
                    if ( lvi0.IsTraversible )
                    {
                        UpdateView( lvi0.FullName );
                    }
                    else if ( e.Key == Key.Enter || e.Key == Key.Return )
                    {
                        foreach ( InfoItem lvi in listView.SelectedItems )
                        {
                            if ( !lvi.IsTraversible )
                            {
                                System.Diagnostics.Process.Start( lvi.FullName );
                            }
                        }
                    }
                }
            }
            else if ( e.Key == Key.Back || e.Key == Key.Left )
            {
                var path = listView.Tag as string;
                var parent = Directory.GetParent( path );
                if ( parent != null )
                {
                    UpdateView( parent.FullName );
                }
                else
                {
                    // Get roots?
                }
            }
            else if ( altKeyDown )
            {
                altKeyDown = false;
                if ( e.SystemKey == Key.D )
                {
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }

            if ( e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt )
            {
                altKeyDown = true;
            }
        }

        private void textBox_PreviewKeyDown( object sender, KeyEventArgs e )
        {
            if ( altKeyDown )
            {
                altKeyDown = false;
                if ( e.SystemKey == Key.D )
                {
                    textBox.SelectAll();
                }
            }
            else if ( e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt )
            {
                altKeyDown = true;
            }
            else if ( e.Key == Key.Down )
            {
                listView.Focus();
            }
            else if ( e.Key == Key.Enter || e.Key == Key.Return )
            {
                UpdateView( textBox.Text );
            }
        }
    }
}
