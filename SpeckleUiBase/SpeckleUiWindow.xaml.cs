using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp;

namespace SpeckleUiBase
{
  /// <summary>
  /// Interaction logic for SpeckleUiWindow.xaml
  /// </summary>
  public partial class SpeckleUiWindow : Window
  {
    /// <summary>
    /// Instantiates the ui.
    /// </summary>
    /// <param name="baseBindings">Your implementation of the SpeckleUiBindings class.</param>
    /// <param name="address">Defaults to the master branch release of the web ui app. Change it to where you're running your local server when debugging!</param>
    public SpeckleUiWindow( SpeckleUIBindings baseBindings, string address = "https://appui.speckle.systems/#/" )
    {
      InitializeComponent();

      baseBindings.Browser = Browser;
      baseBindings.Window = this;

      Browser.RegisterAsyncJsObject( "UiBindings", baseBindings );

      Browser.Address = address;
    }

    // Note: Dynamo ships with cefsharp too, so we need to be careful around initialising cefsharp.
    private void InitializeCef( )
    {
      if ( Cef.IsInitialized ) return;

      Cef.EnableHighDPISupport();

      var assemblyLocation = Assembly.GetExecutingAssembly().Location;
      var assemblyPath = System.IO.Path.GetDirectoryName( assemblyLocation );
      var pathSubprocess = System.IO.Path.Combine( assemblyPath, "CefSharp.BrowserSubprocess.exe" );
      var settings = new CefSettings
      {
        BrowserSubprocessPath = pathSubprocess,
      };

      Cef.Initialize( settings );
    }

    // Hides the window rather than closing it, to prevent the browser from going haywire.
    protected override void OnClosing( CancelEventArgs e )
    {
      this.Hide();
      e.Cancel = true;
    }
  }
}
