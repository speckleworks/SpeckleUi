# SpeckleUi
Base speckle ui for embedding in .net apps. It consists of two parts, a .NET scaffold (this repo), and vuejs web app (can be found [here](https://github.com/speckleworks/SpeckleUiApp)). 

### How To

This is the first half of the re-usable speckle web ui. The other half, the actual app itself, is in this repo: https://github.com/speckleworks/SpeckleUiApp.

So far, that's about it. The main magic is in the `SpeckleUIBindings` class. When implementing in application of choice, implement this abstract class in order to be able to effectively communicate to the ui. 

More to come. 
