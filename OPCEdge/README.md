# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Installation of OPC Service (Temporary until installer properly configured and tested once service finalized):
1. Run "Developer Command Prompt for VS" as administrator

2. Navigate to build (Either debug or release, whichever was built) folder for Artc.Harmony.UaWindowsService
E.g. "cd C:\Users\Andrew\Documents\Harmony\src\Edge\OpcUa\Artc.Harmony.UaWindowsService\bin\Debug"

3. Use installutil command to install service.
installutil -i OpcWindowsService.exe

4. Use installutil command to uninstall service once done.
installutil -u OpcWindowsService.exe

# OpcEdgeClient:
1. Set Opc.EdgeClient as startup project in VS

2. Start once OpcWindowsService has been installed. (Client will not run if service has not been installed or is not running.)

# Notes:
1. OpcWindowsService's MQTT client has been coded to auto-connect to "dev-harmony-01.southeastasia.cloudapp.azure.com:8080/mqtt"
This can be found in line 77 under OpcWindowsService.cs if change required.

2. OpcWindowsService's OPC client has been coded to auto-connect to last connected OPC endpoint (Unless no previous connection)
Retained endpoints will be stored as a .json file in a directory created by the service. (Line 26-29 and 626-650) 




# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)