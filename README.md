# SpeckleUi
Base speckle ui for embedding in .net apps

### Testing

Make sure you've cloned this repo with its `SpeckleCore` submodule. 

Then, start the web app!

```sh
cd /path/to/solutionfolder/App
npm install
npm run serve
```

Then set your startup project as `SpeckleUiTester` and hit run! If you get errors re sqlite.dll, usually a clean and rebuild solves them. If you get errors about cefsharp, make sure you're running with a specified build architecture (x64). 
