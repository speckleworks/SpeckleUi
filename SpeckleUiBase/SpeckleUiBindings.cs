extern alias SpeckleNewtonsoft;
using SNJ = SpeckleNewtonsoft.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Wpf;

namespace SpeckleUiBase
{
  public abstract class SpeckleUIBindings
  {
    public ChromiumWebBrowser Browser { get; set; }

    public SpeckleUIBindings()
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
      var script = string.Format( "window.EventBus.$emit('{0}', {1})", eventName, SNJ.JsonConvert.SerializeObject( eventInfo ) );
      Browser.GetMainFrame().EvaluateScriptAsync( script );
    }

    /// <summary>
    /// Dispatches a store action directly. Please note, this will not work for any action.
    /// </summary>
    /// <param name="storeActionName"></param>
    /// <param name="args"></param>
    public void DispatchStoreActionUi( string storeActionName, string args = null )
    {
      var script = string.Format( "window.Store.dispatch('{0}', '{1}')", storeActionName, args );
      Browser.GetMainFrame().EvaluateScriptAsync( script );
    }

    /// <summary>
    /// Pops open the dev tools.
    /// </summary>
    public void ShowDev()
    {
      Browser.ShowDevTools();
    }

    /// <summary>
    /// Gets the current accounts.
    /// </summary>
    /// <returns></returns>
    public string GetAccounts()
    {
      return SNJ.JsonConvert.SerializeObject( SpeckleCore.LocalContext.GetAllAccounts() );
    }

    public abstract string GetApplicationHostName();
    public abstract string GetFileName();
    public abstract string GetDocumentId();
    public abstract string GetDocumentLocation();

    /// <summary>
    /// Returns the serialised clients present in the current open host file.
    /// </summary>
    /// <returns></returns>
    public abstract string GetFileClients();

    /// <summary>
    /// Adds a sender and persits the info to the host file
    /// </summary>
    public abstract void AddSender( string args );

    /// <summary>
    /// Adds the current selection to the provided client.
    /// </summary>
    public abstract void AddSelectionToSender( string args );

    /// <summary>
    /// Removes the current selection from the provided client.
    /// </summary>
    /// <param name="args"></param>
    public abstract void RemoveSelectionFromSender( string args );

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

    public abstract void UpdateSender( string args );

    // TODO: See how we go about this
    public abstract void AddObjectsToSender( string args );
    public abstract void RemoveObjectsFromSender( string args );
  }
}
