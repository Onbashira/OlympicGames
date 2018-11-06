using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GameSettingDataManager
{
    public enum GAME_RULE
    {
        RULE_STOCK = 0,
        RULE_TIME,
        RULE_COMP,
        RULE_MAX,
    }

    [SerializeField]
    private static uint playerNum = 0;

    [SerializeField]
    private static uint gameTime;//これいる？

    //[SerializeField]
    //private static int randSeed;

    [SerializeField]
    private static uint playerStockNum = 3;

    [SerializeField]
    private static GAME_RULE rule;

    public static  GameSettingDataManager.GAME_RULE GetRule()
    {
        return rule;
    }

    public static uint GetPlayerNum()
    {
        return playerNum;
    }

    public static uint GetPlayerStockNum()
    {
        return playerStockNum;
    }

    //public static int GetRandSeed()
    //{
    //    return randSeed;
    //}

    public static void InitGameSetting()
    {
        rule = GAME_RULE.RULE_STOCK;
        playerNum = 0;
        playerStockNum = 3;
    }
}
