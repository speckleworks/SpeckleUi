using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;

namespace SpeckleUiBase
{
  public abstract class SpeckleUIBindings
  {
    public ChromiumWebBrowser Browser { get; set; }

    public SpeckleUIBindings( )
    {
      SpeckleCore.SpeckleInitializer.Initialize();
      SpeckleCore.LocalContext.Init();
    }

    /// <summary>
    /// Sends an event to the UI, bound to the global EventBus.
    /// </summary>
    /// <param name="eventName">The event's name.</param>
    /// <param name="eventInfo">The event args, which will be serialised to a string.</param>
    public void NotifyUi( string eventName, dynamic eventInfo )
    {
      var script = string.Format( "window.EventBus.$emit({0}, {1})", eventName, JsonConvert.SerializeObject( eventInfo ) );
      Browser.GetMainFrame().EvaluateScriptAsync( script );
    }

    /// <summary>
    /// Pops open the dev tools.
    /// </summary>
    public void ShowDev( )
    {
      Browser.ShowDevTools();
    }

    /// <summary>
    /// Gets the current accounts.
    /// </summary>
    /// <returns></returns>
    public string GetAccounts( )
    {
      return JsonConvert.SerializeObject( SpeckleCore.LocalContext.GetAllAccounts() );
    }

    public abstract string GetApplicationHostName( );
    public abstract string GetFileName( );
    public abstract string GetDocumentId( );
    public abstract string GetDocumentLocation( );

    /// <summary>
    /// Returns the serialised clients present in the current open host file.
    /// </summary>
    /// <returns></returns>
    public abstract string GetFileClients( );

    /// <summary>
    /// TODO: Adds a sender and persits the info to the host file
    /// </summary>
    public abstract void AddSender( string args );
    
    /// <summary>
    /// Adds a receiver and persits the info to the host file
    /// </summary>
    public abstract void AddReceiver( string args );

    /// <summary>
    /// Removes a client from the file and persists the info to the host file.
    /// </summary>
    /// <param name="args"></param>
    public abstract void RemoveClient( string args );

    /// <summary>
    /// Bakes the specified client in the host file.
    /// </summary>
    /// <param name="args"></param>
    public abstract void BakeReceiver( string args );

    // TODO: See how we go about this
    public abstract void AddObjectsToSender( string args );
    public abstract void RemoveObjectsFromSender( string args );
  }
}
