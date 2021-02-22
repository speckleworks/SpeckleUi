[![Netlify Status](https://api.netlify.com/api/v1/badges/e4824a5c-a6df-4670-b242-4873d26901ba/deploy-status)](https://app.netlify.com/sites/distracted-jones-770c28/deploys)

# SpeckleUi
Base speckle ui for embedding in .net apps. It consists of two parts, a .NET scaffold (this repo), and vuejs web app (can be found [here](https://github.com/speckleworks/SpeckleUiApp)). 

## ⚠️ IMPORTANT ⚠️

Speckle v1 has reached EOL and is no longer mainatined. Check out our brand new 👉 [Speckle v2](https://github.com/specklesystems)!
Read more about the announcemnt [here](https://speckle.systems/blog/speckle2-vision-and-faq) and check out or new [website](https://speckle.systems).


### How To

This is the first half of the re-usable speckle web ui. The other half, the actual app itself, is in this repo: https://github.com/speckleworks/SpeckleUiApp.

So far, that's about it. The main magic is in the `SpeckleUIBindings` class. When implementing in application of choice, implement this abstract class in order to be able to effectively communicate to the ui. 

More to come. 
