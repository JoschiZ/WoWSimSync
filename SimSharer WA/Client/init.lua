local env = aura_env; -- We need to capture this reference in a local or we cannot access it in callbacks

local ac = LibStub("AceComm-3.0", true);
if ac then ac:Embed(aura_env) end;
aura_env:UnregisterAllComm();

local as = LibStub("AceSerializer-3.0", true)
if as then as:Embed(aura_env) end;

aura_env.messagePrefixes = {};
aura_env.messagePrefixes.getProfile = "msgSharerGet";
aura_env.messagePrefixes.putProfile = "msgSharerPut";

-- Sends a simcString to the server aura
local function OnGetProfile(...)
    local simc = _G.LibStub("AceAddon-3.0"):GetAddon("Simulationcraft");
    local simcString = simc:GetSimcProfile();
    local _, _, sender = select(1, ...);
    local name, realm = UnitName("PLAYER")
    if not realm then
        realm = GetRealmName();
    end

    env:SendCommMessage("msgSharerPut", env:Serialize(name, realm, simcString), "WHISPER", sender)
end
aura_env.OnGetProfile = OnGetProfile;
aura_env:RegisterComm(aura_env.messagePrefixes.getProfile, "OnGetProfile")