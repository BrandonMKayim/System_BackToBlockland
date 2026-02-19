// client.cs
// Add-Ons/System_BackToBlockland/

// --------------------
// Paths / defaults
// --------------------
if ($BTB::Root $= "")
{
	$BTB::Root      = "Add-Ons/System_BackToBlockland/";
	$BTB::Images    = $BTB::Root @ "images/";
	$BTB::Interface = $BTB::Root @ "interface/";
	$BTB::Sounds    = $BTB::Root @ "sounds/";
}

if ($BTB::Version $= "")
	$BTB::Version = "0.01";

// --------------------
// Overlay loading / open / close / toggle
// --------------------
function BTB_LoadOverlay()
{
	if (isFile($BTB::Interface @ "overlay.gui"))
		exec($BTB::Interface @ "overlay.gui");
	else if (isFile($BTB::Interface @ "overlay.cs"))
		exec($BTB::Interface @ "overlay.cs");
}

function BTB_OpenOverlay()
{
	if (!isObject(BTB_Overlay))
		BTB_LoadOverlay();

	if (isObject(BTB_Overlay) && !BTB_Overlay.isAwake())
		canvas.pushDialog(BTB_Overlay);

	if (isObject(BTB_OverlayChatInput))
		canvas.setFirstResponder(BTB_OverlayChatInput);
}

function BTB_CloseOverlay()
{
	if (isObject(BTB_Overlay) && BTB_Overlay.isAwake())
		canvas.popDialog(BTB_Overlay);
}

function BTB_ToggleOverlay()
{
	if (isObject(BTB_Overlay) && BTB_Overlay.isAwake())
		BTB_CloseOverlay();
	else
		BTB_OpenOverlay();
}

function BTB_Overlay::fadeOut(%this) { BTB_CloseOverlay(); }
function BTB_Overlay::toggle(%this, %target) { BTB_OpenOverlay(); }

// --------------------
// Main menu label
// --------------------
function BTB_Interface_InstallMainMenu()
{
	if (!isObject(MainMenuButtonsGui))
	{
		cancel($BTB::MMInstallSch);
		$BTB::MMInstallSch = schedule(250, 0, "BTB_Interface_InstallMainMenu");
		return;
	}

	if (isObject(BTB_MMVersionText))
		BTB_MMVersionText.delete();

	%t = new GuiMLTextCtrl(BTB_MMVersionText)
	{
		profile = "GuiTextProfile";
		horizSizing = "left";
		vertSizing = "bottom";
		position = "469 1";
		extent = "220 16";
		visible = "1";
		allowColorChars = 1;
		text = "Back to Blockland v" @ $BTB::Version;
		tooltip = "Click to open Back to Blockland";
	};

	MainMenuButtonsGui.add(%t);
}

package BTB_MainMenuClick
{
	function GuiMLTextCtrl::onMouseDown(%this, %pos, %btn)
	{
		if (isObject(BTB_MMVersionText) && %this.getId() == BTB_MMVersionText.getId())
		{
			BTB_ToggleOverlay();
			return;
		}
		Parent::onMouseDown(%this, %pos, %btn);
	}

	function GuiTextCtrl::onMouseDown(%this, %pos, %btn)
	{
		if (isObject(BTB_MMVersionText) && %this.getId() == BTB_MMVersionText.getId())
		{
			BTB_ToggleOverlay();
			return;
		}
		Parent::onMouseDown(%this, %pos, %btn);
	}
};
deactivatePackage(BTB_MainMenuClick);
activatePackage(BTB_MainMenuClick);

// --------------------
// Keybind (rebindable)
// --------------------
function BTB_ToggleGuiBind(%val)
{
	if (!%val) return;
	BTB_ToggleOverlay();
}

function BTB_InstallBind()
{
	if (!isObject(moveMap))
	{
		cancel($BTB::BindInstallSch);
		$BTB::BindInstallSch = schedule(250, 0, "BTB_InstallBind");
		return;
	}

	if (!$BTB::BoundDefault)
	{
		$BTB::BoundDefault = 1;

		// todo: in options in overlay, make this able to be changed by rebinding it (unbind -> bind)
		globalActionMap.bind(keyboard, "f6", BTB_ToggleGuiBind);

		$RemapDivision[$RemapCount]  = "Back to Blockland";
		$RemapName[$RemapCount]      = "In-Game Overlay";
		$RemapCmd[$RemapCount]       = "BTB_ToggleGuiBind";
		$RemapCount++;
	}
}

// --------------------
// Chat (local-only for now)
// --------------------
function BTB_Chat_Ensure()
{
	if (!isObject(BTB_OverlayChatText) || !isObject(BTB_OverlayChatScroll))
		return 0;
	return 1;
}

function BTB_Chat_Append(%line)
{
	if (!BTB_Chat_Ensure())
		return;

	%old = BTB_OverlayChatText.getText();
	if (%old $= "")
		%new = %line;
	else
		%new = %old @ "<br>" @ %line;

	BTB_OverlayChatText.setText(%new);
	BTB_OverlayChatText.forceReflow();
	BTB_OverlayChatScroll.scrollToBottom();
}

function BTB_Chat_OnMessage(%room, %from, %msg)
{
	if (%room $= "") %room = "Lobby";
	if (%from $= "") %from = "Server";
	BTB_Chat_Append("<color:130 190 255 255>[" @ %room @ "]<color:255 255 255 255> " @ %from @ ": " @ %msg);
}

function BTB_Chat_onEnter()
{
	if (!isObject(BTB_OverlayChatInput))
		return;

	%msg = trim(BTB_OverlayChatInput.getValue());
	if (%msg $= "")
		return;

	BTB_OverlayChatInput.setValue("");
	BTB_Chat_OnMessage("Lobby", $Pref::Player::NetName, %msg);
}

// --------------------
// Slash command
// --------------------
function clientCmdBTB()
{
	BTB_ToggleOverlay();
}

// --------------------
// BOOT
// --------------------
function BTB_Boot()
{
	BTB_LoadOverlay();
	BTB_Interface_InstallMainMenu();
	BTB_InstallBind();
}

// Only guard the boot call (NOT the function/package definitions)
if ($BTB::Booted)
{
	echo("System_BackToBlockland: client.cs already booted");
}
else
{
	$BTB::Booted = 1;
	BTB_Boot();
}
