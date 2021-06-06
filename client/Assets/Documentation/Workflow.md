# Workflow Suggestions

Here are some suggestions while working on the project.

**Project Structure**
* `README.md` - This README file
* `client/` - Open this folder in the Unity Editor
* `client/Assets/` - Core files of the project
* `client/Assets/Scenes/` - **Open a scene** in the Unity Editor to play the game!
* `client/Assets/3rdParty/` - Dependency asset files for the project
* `client/Packages/` - Dependency package files for the project

# Workflow: Committing To Master Branch?

## Check Stability
 1. Run each scene directly
 1. No bugs?
 1. No warnings?
 1. No errors?
 
 ## Check Logging
 1. Set Configuration.asset's DebugLogLevel to Disable
 1. Zero runtime logging? Good
 1. Set Configuration.asset's DebugLogLevel to Simple. This is the default.
