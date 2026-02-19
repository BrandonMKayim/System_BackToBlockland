$BTB::Server = 1;

$BTB::Version = "0.01";
$BTB::Path = "Add-Ons/System_BackToBlockland/";

// Demo check 
if (!isUnlocked())
{
	$BTB::Server = 0;
	echo("\c2ERROR: BTB failed to load because you are in demo mode.");
	return;
}

// Group for keeping server-side objects in
if (!isObject(BTBGroup))
	new SimGroup(BTBGroup);

// Dedicated init (optional)
if ($Server::Dedicated)
{
	if (isFile($BTB::Path @ "dedicated.cs"))
		exec("./dedicated.cs");
}

// Load server modules (optional for now; safe if missing)
if (isFile($BTB::Path @ "modules/server/serverControl.cs"))
	exec("./modules/server/serverControl.cs");

if (isFile($BTB::Path @ "modules/server/guiTransfer.cs"))
	exec("./modules/server/guiTransfer.cs");

// Activate packages only if they exist
if (isPackage(BTB_Modules_Server_GuiTransfer))
	activatePackage(BTB_Modules_Server_GuiTransfer);

if (isPackage(BTB_Modules_Server_ServerControl))
	activatePackage(BTB_Modules_Server_ServerControl);

// Server handshake flag (like RTB hasRTB/rtbVersion)
package BTB_Server
{
	function GameConnection::onConnectRequest(%this,%a,%b,%c,%d,%e,%f,%g,%h,%i,%j,%k,%l,%m,%n,%o,%p)
	{
		// If you later decide to append a BTB tag into the connect string,
		// you can parse it here. For now, keep a placeholder behavior:
		// %g is where RTB used to receive "version ..." when client had RTB.
		if (%g !$= "")
		{
			%this.hasBTB = 1;
			%this.btbVersion = firstWord(%g);
		}
		Parent::onConnectRequest(%this,%a,%b,%c,%d,%e,%f,%g,%h,%i,%j,%k,%l,%m,%n,%o,%p);
	}
};
activatePackage(BTB_Server);
