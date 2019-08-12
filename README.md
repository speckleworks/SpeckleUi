# SpeckleUi
Base speckle ui for embedding in .net apps. It consists of two parts, a .NET scaffold (this repo), and vuejs web app (can be found [here](https://github.com/speckleworks/SpeckleUiApp)). 

### How To

This is the first half of the re-usable speckle web ui. The other half, the actual app itself, is in this repo: https://github.com/speckleworks/SpeckleUiApp. Make sure, when debugging, that you `npm run serve` it beforehand.

Furthermore, make sure you're loading the correct url:

```cs
// SpeckleUiWindow.xaml.cs L34

#if DEBUG
      Browser.Address = @"http://10.4.93.178:8080"; // YMMV: change this to where your node app resides; can be localhost:8080 if developing on the same machine
#else
      Browser.Addrress = @"https://appui.speckle.systems/#/"; // Load the latest CI-ed master in production.
#endif

```

So far, that's about it. The main magic is in the `SpeckleUIBindings` class. When implementing in application of choice, implement this abstract class in order to be able to effectively communicate to the ui. 

More to come. 
