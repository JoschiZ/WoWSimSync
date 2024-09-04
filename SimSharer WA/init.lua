local env = aura_env;
aura_env.debugPrint = function(key, value)
    if not env.config.isDebug then
        return
    end
    if DevTool then
        DevTool:AddData(key, value)
    else
        print(key, value)
    end
end
aura_env.messagePrefixes = {};
aura_env.messagePrefixes.getProfile = "msgSharerGet";
aura_env.messagePrefixes.putProfile = "msgSharerPut";

local ac = LibStub("AceComm-3.0", true);
if ac then ac:Embed(aura_env) end;
aura_env:UnregisterAllComm();

local as = LibStub("AceSerializer-3.0", true)
if as then as:Embed(aura_env) end;

local function OnPutProfile(...)
    local _, _, message = select(1, ...);
    env:debugPrint("received", message)
    local _, name, simcString = env:Deserialize(message);
    env.db[name] = simcString;
end
aura_env.OnPutProfile = OnPutProfile;
aura_env:RegisterComm(aura_env.messagePrefixes.putProfile, "OnPutProfile")



local function OnGetProfile(...)
    local simc = _G.LibStub("AceAddon-3.0"):GetAddon("Simulationcraft");
    local simcString = simc:GetSimcProfile();

    local name, realm = UnitName("PLAYER")
    if realm then
        name = name.. "-".. realm;
    end

    env:SendCommMessage("msgSharerPut", env:Serialize(name, simcString), "GUILD")
end
aura_env.OnGetProfile = OnGetProfile;
aura_env:RegisterComm(aura_env.messagePrefixes.getProfile, "OnGetProfile")



