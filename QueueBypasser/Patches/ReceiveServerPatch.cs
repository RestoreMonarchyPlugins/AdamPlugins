using HarmonyLib;
using Rocket.API;
using SDG.Framework.Modules;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Types = SDG.Unturned.Types;

namespace Adam.QueueBypasser.Patches
{
    [HarmonyPatch(typeof(Provider), "receiveServer", null, null, null)]
    public static class ReceiveServerPatch
    {
        private static List<Module> critMods = (List<Module>)typeof(Provider).GetField("critMods", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);

        private static byte[] _serverPasswordHash = (byte[])typeof(Provider).GetField(nameof(_serverPasswordHash), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);

        [HarmonyPrefix]
        public static bool Prefix(CSteamID steamID, byte[] packet, int offset, int size, int channel)
        {
            if (!Dedicator.isDedicated)
                return true;
            byte index1 = packet[offset];
            if (!(index1 < (byte)26))
            {
                UnityEngine.Debug.LogWarning((object)("Received invalid packet index from " + (object)steamID + ", so we're refusing them"));
                SDG.Unturned.Provider.refuseGarbageConnection(steamID, "sv invalid packet index");
            }
            else
            {
                ESteamPacket packet1 = (ESteamPacket)index1;
                if (packet1 == ESteamPacket.AUTHENTICATE || packet1 == ESteamPacket.CONNECT)
                    Console.WriteLine(packet1);
                if (packet1 == ESteamPacket.CONNECT)
                {
                    RocketPlayer player = new RocketPlayer(steamID.ToString());
                    bool result = player.HasPermission("QueueBypasser.Skip");
                    Console.WriteLine(result);
                    if (!result)
                        return true; //We don't need to do jack shit because he doesn't have the right permission.

                    for (int index2 = 0; index2 < SDG.Unturned.Provider.pending.Count; ++index2)
                    {
                        if (SDG.Unturned.Provider.pending[index2].playerID.steamID == steamID)
                        {
                            Console.WriteLine("line 1");
                            //SDG.Unturned.Provider.reject(steamID, ESteamRejection.ALREADY_PENDING);
                            return true;
                        }
                    }
                    for (int index2 = 0; index2 < SDG.Unturned.Provider.clients.Count; ++index2)
                    {
                        if (SDG.Unturned.Provider.clients[index2].playerID.steamID == steamID)
                        {
                            Console.WriteLine("line 2");
                            //SDG.Unturned.Provider.reject(steamID, ESteamRejection.ALREADY_CONNECTED);
                            return true;
                        }
                    }
                    object[] objects = SteamPacker.getObjects(steamID, offset, 0, packet, Types.BYTE_TYPE, Types.BYTE_TYPE, Types.STRING_TYPE, Types.STRING_TYPE, Types.BYTE_ARRAY_TYPE, Types.BYTE_ARRAY_TYPE, Types.BYTE_ARRAY_TYPE, Types.BYTE_TYPE, Types.UINT32_TYPE, Types.BOOLEAN_TYPE, Types.SINGLE_TYPE, Types.STRING_TYPE, Types.STEAM_ID_TYPE, Types.BYTE_TYPE, Types.BYTE_TYPE, Types.BYTE_TYPE, Types.COLOR_TYPE, Types.COLOR_TYPE, Types.COLOR_TYPE, Types.BOOLEAN_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_ARRAY_TYPE, Types.BYTE_TYPE, Types.STRING_TYPE, Types.STRING_TYPE, Types.STEAM_ID_TYPE, Types.UINT32_TYPE);
                    SteamPlayerID newPlayerID = new SteamPlayerID(steamID, (byte)objects[1], (string)objects[2], (string)objects[3], (string)objects[11], (CSteamID)objects[12]);
                    if (objects[8].ToString() != SDG.Unturned.Provider.APP_VERSION && (int)(uint)objects[8] != Parser.getUInt32FromIP(SDG.Unturned.Provider.APP_VERSION))
                    {
                        Console.WriteLine("line 3");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_VERSION); 
                        return true;
                    }
                    if (newPlayerID.playerName.Length < 2)
                    {
                        Console.WriteLine("line 4");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_PLAYER_SHORT);
                        return true;
                    }
                    if (newPlayerID.characterName.Length < 2)
                    {
                        Console.WriteLine("line 5");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_CHARACTER_SHORT);
                        return true;
                    }
                    if (newPlayerID.playerName.Length > 32)
                    {
                        Console.WriteLine("line 6");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_PLAYER_LONG);
                        return true;
                    }
                    if (newPlayerID.characterName.Length > 32)
                    {
                        Console.WriteLine("line 7");
                        //Provider.reject(steamID, ESteamRejection.NAME_CHARACTER_LONG);
                        return true;
                    }
                    long result1;
                    double result2;
                    if (long.TryParse(newPlayerID.playerName, out result1) || double.TryParse(newPlayerID.playerName, out result2))
                    {
                        Console.WriteLine("line 8");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_PLAYER_NUMBER);
                        return true;
                    }
                    long result3;
                    double result4;
                    if (long.TryParse(newPlayerID.characterName, out result3) || double.TryParse(newPlayerID.characterName, out result4))
                    {
                        Console.WriteLine("line 9");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_CHARACTER_NUMBER);
                        return true;
                    }
                    if (SDG.Unturned.Provider.filterName)
                    {
                        if (!NameTool.isValid(newPlayerID.playerName))
                        {
                            Console.WriteLine("line 10");
                            //SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_PLAYER_INVALID);
                            return true;
                        }
                        if (!NameTool.isValid(newPlayerID.characterName))
                        {
                            Console.WriteLine("line 11");
                            //SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_CHARACTER_INVALID);
                            return true;
                        }
                    }
                    if (NameTool.containsRichText(newPlayerID.playerName))
                    {
                        Console.WriteLine("line 12");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_PLAYER_INVALID);
                        return true;
                    }
                    if (NameTool.containsRichText(newPlayerID.characterName))
                    {
                        Console.WriteLine("line 13");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_CHARACTER_INVALID);
                        return true;
                    }
                    P2PSessionState_t pConnectionState;
                    uint ip = !SteamGameServerNetworking.GetP2PSessionState(steamID, out pConnectionState) ? 0U : pConnectionState.m_nRemoteIP;
                    SteamBlacklistID blacklistID;
                    if (SteamBlacklist.checkBanned(steamID, ip, out blacklistID))
                    {
                        //int size1;
                        //byte[] bytes = SteamPacker.getBytes(0, out size1, (object)(byte)9, (object)blacklistID.reason, (object)blacklistID.getTime());
                        //SDG.Unturned.Provider.send(steamID, ESteamPacket.BANNED, bytes, size1, 0);
                        Console.WriteLine("line 15");
                        return true;
                    }
                    bool flag3 = SteamWhitelist.checkWhitelisted(steamID);
                    if (SDG.Unturned.Provider.isWhitelisted && !flag3)
                    {
                        Console.WriteLine("line 16");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.WHITELISTED);
                        return true;
                    }

                    byte[] hash_0_1 = (byte[])objects[4];
                    if (hash_0_1.Length != 20)
                    {
                        Console.WriteLine("line 17");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_PASSWORD);
                        return true;
                    }
                    byte[] hash_0_2 = (byte[])objects[5];
                    if (hash_0_2.Length != 20)
                    {
                        Console.WriteLine("line 18");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_HASH_LEVEL);
                        return true;
                    }
                    byte[] h = (byte[])objects[6];
                    if (h.Length != 20)
                    {
                        Console.WriteLine("line 19");
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_HASH_ASSEMBLY);
                        return true;
                    }

                    string str1 = (string)objects[29];
                    ModuleDependency[] moduleDependencyArray;
                    if (string.IsNullOrEmpty(str1))
                    {
                        moduleDependencyArray = new ModuleDependency[0];
                    }
                    else
                    {
                        string[] strArray1 = str1.Split(';');
                        moduleDependencyArray = new ModuleDependency[strArray1.Length];
                        for (int index2 = 0; index2 < moduleDependencyArray.Length; ++index2)
                        {
                            string[] strArray2 = strArray1[index2].Split(',');
                            if (strArray2.Length == 2)
                            {
                                moduleDependencyArray[index2] = new ModuleDependency();
                                moduleDependencyArray[index2].Name = strArray2[0];
                                uint.TryParse(strArray2[1], out moduleDependencyArray[index2].Version_Internal);
                            }
                        }
                    }
                    critMods.Clear();
                    ModuleHook.getRequiredModules(critMods);
                    bool flag4 = true;
                    for (int index2 = 0; index2 < moduleDependencyArray.Length; ++index2)
                    {
                        bool flag2 = false;
                        if (moduleDependencyArray[index2] != null)
                        {
                            for (int index3 = 0; index3 < critMods.Count; ++index3)
                            {
                                if (critMods[index3] != null && critMods[index3].config != null && (critMods[index3].config.Name == moduleDependencyArray[index2].Name && critMods[index3].config.Version_Internal >= moduleDependencyArray[index2].Version_Internal))
                                {
                                    flag2 = true;
                                    return true;
                                }
                            }
                        }
                        if (!flag2)
                        {
                            flag4 = false;
                            return true;
                        }
                    }
                    if (!flag4)
                    {
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.CLIENT_MODULE_DESYNC);
                        Console.WriteLine("line 20");
                        return true;
                    }
                    bool flag5 = true;
                    for (int index2 = 0; index2 < critMods.Count; ++index2)
                    {
                        bool flag2 = false;
                        if (critMods[index2] != null && critMods[index2].config != null)
                        {
                            for (int index3 = 0; index3 < moduleDependencyArray.Length; ++index3)
                            {
                                if (moduleDependencyArray[index3] != null && moduleDependencyArray[index3].Name == critMods[index2].config.Name && moduleDependencyArray[index3].Version_Internal >= critMods[index2].config.Version_Internal)
                                {
                                    flag2 = true;
                                    return true;
                                }
                            }
                        }
                        if (!flag2)
                        {
                            flag5 = false;
                            return true;
                        }
                    }
                    if (!flag5)
                    {
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.SERVER_MODULE_DESYNC);
                        Console.WriteLine("line 21");
                        return true;
                    }

                    Console.WriteLine("section 1");

                    if (SDG.Unturned.Provider.serverPassword == string.Empty || Hash.verifyHash(hash_0_1, _serverPasswordHash))
                    {
                        Console.WriteLine("section 2");
                        if (Hash.verifyHash(hash_0_2, Level.hash))
                        {
                            Console.WriteLine("section 3");
                            if (ReadWrite.appIn(h, (byte)objects[7]))
                            {
                                Console.WriteLine("section 4");
                                if ((double)(float)objects[10] < (double)SDG.Unturned.Provider.configData.Server.Max_Ping_Milliseconds / 1000.0)
                                {
                                    Console.WriteLine("section 5");
                                    //var pending = new SteamPending(newPlayerID, (bool)objects[9], (byte)objects[13], (byte)objects[14], (byte)objects[15], (Color)objects[16], (Color)objects[17], (Color)objects[18], (bool)objects[19], (ulong)objects[20], (ulong)objects[21], (ulong)objects[22], (ulong)objects[23], (ulong)objects[24], (ulong)objects[25], (ulong)objects[26], (ulong[])objects[27], (EPlayerSkillset)(byte)objects[28], (string)objects[30], (CSteamID)objects[31]);

                                    //Provider.pending.Insert(0, pending);
                                    //pending.sendVerifyPacket();
                                    //Console.WriteLine($"Accepting {pending.playerID.steamID}.");
                                    return false;
                                }
                                //SDG.Unturned.Provider.reject(steamID, ESteamRejection.PING);
                                Console.WriteLine("line 22");
                                return true;
                            }
                            //SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_HASH_ASSEMBLY);
                            Console.WriteLine("line 23");
                            return true;
                        }
                        //SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_HASH_LEVEL);
                        Console.WriteLine("line 24");
                        return true;
                    }
                    //SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_PASSWORD);
                    Console.WriteLine("line 25");
                    return true;
                }
            }
            return true;
        }
    }
}
