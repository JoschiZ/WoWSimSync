-- /run WeakAuras.ScanEvents("SHARER_START_COLLECTION")
-- SHARER_START_COLLECTION
function(event)
    if event == "OPTIONS" then return end
    if not event == "SHARER_START_COLLECTION" then return end

    aura_env.isCollecting = true;
    aura_env:debugPrint("start collecting", aura_env.db);
    aura_env.displayText = "Started Collection"
    local sender, _ = UnitName("PLAYER");

    -- prevents multiple registration
    if not aura_env.OnPutProfile then
        local env = aura_env;
        local function OnPutProfile(...)
            if not env.isCollecting then return end

            local _, _, message = select(1, ...);
            env:debugPrint("received", message)
            local _, name, realm, simcString = env:Deserialize(message);
            local storageModel = {};
            storageModel.CharacterName = name;
            storageModel.Realm = realm;
            storageModel.SimCString = simcString;
            env.db[name.. realm] = storageModel;
            local textItteration = "Received From: \n"

            for _, stored in pairs(env.db) do
                textItteration = textItteration.. stored.CharacterName.. "-" .. stored.Realm.. "\n";
            end

            env.displayText = textItteration;
            env:debugPrint("received end", message)
        end
        aura_env.OnPutProfile = OnPutProfile;
        aura_env:RegisterComm(aura_env.messagePrefixes.putProfile, "OnPutProfile")
    end

    if aura_env.config.isDebug then
        aura_env:SendCommMessage(aura_env.messagePrefixes.getProfile, sender, "GUILD");
    else
        aura_env:SendCommMessage(aura_env.messagePrefixes.getProfile, sender, "RAID");
    end
    
end