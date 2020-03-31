# =================================================================================================================
# Installation of OPC Service (Temporary until installer properly configured and tested once service finalized):
1. Run "Developer Command Prompt for VS" as administrator

2. Navigate to build (Either debug or release, whichever was built) folder for Artc.Harmony.UaWindowsService
E.g. "cd C:\Users\Andrew\Documents\Harmony\src\Edge\OpcUa\Artc.Harmony.UaWindowsService\bin\Debug"

3. Use installutil command to install service.
installutil -i OpcWindowsService.exe

4. Use installutil command to uninstall service once done.
installutil -u OpcWindowsService.exe


# =================================================================================================================
# OpcEdgeClient:
1. Set Opc.EdgeClient as startup project in VS

2. Start once OpcWindowsService has been installed. (Client will not run if service has not been installed or is not running.)


# =================================================================================================================
# Additional Installation / Configuration Notes:
1. OpcWindowsService's MQTT client has been coded to auto-connect to "dev-harmony-01.southeastasia.cloudapp.azure.com:8080/mqtt"
This can be found in line 77 under OpcWindowsService.cs if change required.

2. OpcWindowsService's OPC client has been coded to auto-connect to last connected OPC endpoint (Unless no previous connection)
Retained endpoints will be stored as a .json file in a directory created by the service. (Line 26-29 and 626-650) 

3. Path to folders for storing retained items can be found in OpcWindowsService line 27 - 30.

4. Opc.Ua.WindowsService.Config.xml may need to be manually placed into your machine's system32 or SYSWOW64 folder (depending on whether 32-bit or 64-bit machine)

5. If the EdgeClient is unable to connect to the service, try re-adding the service reference or updating the service reference.

6. Check servicePrincipalName value under UaEdgeClient's App.config file and ensure it's set to your own PC name.


# =================================================================================================================
# Code Notes
These notes highlights the notable functions of key class files as well as descriptions of notable methods and variables for the OpcUa Edge system.
# ============================================================
1. Opc.EdgeClient - GUI for OpcUa Edge system.
SetStartup() - Registers application in registry to have it run on startup.
InitializeClients() - Initializes references to retained item folders, initializes OPC application instance for client and recreates session based on service.
SaveSessionAsync() / LoadSessionAsync() - Retains endpoint URL and loads it back up when service / client restarts.
CheckService() - Prevents client from starting while service is not running.


# ============================================================
2. Opc.Ua.WindowsService - Service implementation for OpcUa Edge system.
mainFolder, itemsFolder, subscriptionsFolder, sessionsFolder - Strings storing paths to folder for storing retained items.
OPCPublish() - Loops through retained items (if any) and recreates the subscriptions if the service has to restart. Note: Any edits to the retained items may affect this.
createDirectories() - Creates directories for storing retained items on service start-up and sets permissions so that EdgeClient can access.


# ============================================================
3. Opc.Ua.WCFInterface - Allows for Interprocess Communication between Opc.Ua.WindowsService and Opc.EdgeClient.


# ============================================================
4. Opc.Ua.ServiceLogic - Business logic for Opc.Ua.WindowsService.
interface IServiceCallback  - Interface to implement callback methods for WindowsService.
static class Host - Static class that implements IServiceCallback interface.
class ServiceLogic - Implements MQTT functionality for WindowsService.


# ============================================================
5. UA Sample Controls - Implements certain OpcUa functionality within WinForms controls implemented in Opc.EdgeClient.
# =======================
# Subscriptions Folder
SubscriptionDlg.cs
EditMI_Click (Line 423 - 464)
-  Event handler for subscription editing and reflects edits in retained subscription files stored.
# =======================
SubscriptionEditDlg.cs
ShowDialog (Line 53 - 88) 
- Shows edit dialog for subscriptions and reflects edits in retained subscription files.
# ============================================================
# Sessions Folder
SessionTreeCtrl.cs
mainFolder, Item, Subscriptions (Line 74-76, 89 - 103)
- Allows referencing of folder where retained files are stored.
Delete (Line 342 - 393)
- Deletes subscription and reflects deletion in retained subscription files.
Delete (Line 398 - 438)
- Deletes monitored item in subscription and reflects deletion in retained subscription and monitored item files.
CreateSubscription(Line 443 - 477)
- Creates new subscription and sets reference to folder where retained files are stored.
SubscriptionMonitorMI_Click
- Event handler for monitoring subscription and sets reference to folder where retained files are stored.
# =======================
BrowseTreeCtrl.cs
mainFolder, Item, Subscriptions (Line 71-73, 81-100)
- Allows referencing of folders where retained files are stored.
SubscribeNewMI_Click (Line 1219 - 1255)
- Event handler for creating new subscription and retains subscription and monitored item.
Subscription_Click (Line 1257 - 1286)
- Event handler for adding monitored item to existing subscription and retains monitored item.
# ============================================================

Note: Remaining class files not indicated above are used as is from their original source (Official Opc Foundation OpcUa .NET Stack - http://opcfoundation.github.io/UA-.NETStandard/)

The full source code can be found at the following link: https://github.com/MrN0ob4Lif3/Edge-Connector- 
# =================================================================================================================