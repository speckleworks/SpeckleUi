using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using SpeckleUiBase;

namespace SpeckleUiTester
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    SpeckleUiWindow UiWindow;
    private void Application_Startup( object sender, StartupEventArgs e )
    {

#if DEBUG
      UiWindow = new SpeckleUiWindow( new TestBindings(), @"http://localhost:8080/#/");
#else
      UiWindow = new SpeckleUiWindow( new TestBindings() ); // On release, default to the latest ci-ed version from https://appui.speckle.systems
#endif

      UiWindow.Show();
    }
  }

  /// <summary>
  /// Test bindings class, void
  /// </summary>
  public class TestBindings : SpeckleUIBindings
  {

    public List<dynamic> myClients = new List<dynamic>();

    public TestBindings( ) : base()
    {
    }

    public override void AddObjectsToSender( string args )
    {
      throw new NotImplementedException();
    }

    public override void AddReceiver( string _args )
    {
      dynamic args = JsonConvert.DeserializeObject( _args );
      //var copy = args;
      myClients.Add( args );
    }

    public override void AddSender( string args )
    {
      //throw new NotImplementedException();
    }

    public override void UpdateSender(string args)
    {
      //throw new NotImplementedException();
    }

    public override void BakeReceiver( string args )
    {
      //throw new NotImplementedException();
    }

    public override string GetApplicationHostName( )
    {
      return "UI Tester";
    }

    public override string GetFileName( )
    {
      return "Somewhere in Memory. Not implemented :)";
    }

    public override string GetDocumentId( )
    {
      return "In memory testing!";
    }

    public override string GetDocumentLocation( )
    {
      return "RAM or SWAP";
    }


    public override string GetFileClients( )
    {
      return JsonConvert.SerializeObject( myClients );
    }


    public override void RemoveObjectsFromSender( string args )
    {
      throw new NotImplementedException();
    }

    public override void RemoveClient( string args )
    {
      var client = JsonConvert.DeserializeObject<dynamic>( args );
      try
      {
        var index = myClients.FindIndex( acc => acc._id == client._id );
        myClients.RemoveAt( index );
      }
      catch ( Exception e ) { }
    }

    public override void AddSelectionToSender( string args )
    {
      throw new NotImplementedException();
    }

    public override void RemoveSelectionFromSender( string args )
    {
      throw new NotImplementedException();
    }

    public override void SelectClientObjects( string args )
    {
      throw new NotImplementedException();
    }

    public override List<ISelectionFilter> GetSelectionFilters()
    {
      return new List<ISelectionFilter>
      {
        new ElementsSelectionFilter
        {
          Name = "Selection",
          Icon = "mouse",
          Count = 99
        },
        new ListSelectionFilter
        {
          Name = "Category",
          Icon = "category",
          Values = new List<string>
          {
            "Walls",
            "Doors",
            "Floors",
            "Structural Elements",
            "Dimitries",
            "Lols"
          }
        },
        new CustomSelectionFilter
        {
          Name = "Property",
          Icon = "filter_list",
          Values = new List<string>
          {
            "Family Name",
            "Family Type",
            "Custom",
          }
        }
      };
    }
  }

}
