This application uses async Task Main(string[] args) so it requires the C# version to be >=7.1. To select the correct version do the following:-
In Solution explorer select properties
from popup select Build
click on 'Advanced.." button
from the popup Language Version drop down list select "C# latest minor version(latest)"  It needs to be >=7.1

In debug mode with the option 'Just My Code' enabled', Visual Studio will break on a line that throws an exception and display an error message that says "exception not handled by user code." This will happen when an async method is cancelled. To prevent this behaviour, uncheck the 'Enable Just My Code' checkbox under Tools, Options, Debugging, General.