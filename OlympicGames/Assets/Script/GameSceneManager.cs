﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField, Tooltip("GameUpdatePreUpdateで要する時間を秒単位で指定")]
    private float maxFadeUpdateTime = 1.0f; //一秒
    [SerializeField, Tooltip("GameUpdatePreUpdateで要する時間を秒単位で指定")]
    private float maxPreUpdateTime = 1.0f; //一秒
    [SerializeField, Tooltip("GameUpdateStartGameで要する時間を秒単位で指定")]
    private float maxStartGameTime = 1.0f; //一秒
    [SerializeField, Tooltip("GameUpdateGameSetで要する時間を秒単位で指定")]
    private float maxGameSetTime = 1.0f; //一秒
    [SerializeField, Tooltip("GameUpdateGameEndで要する時間を秒単位で指定")]
    private float maxGameEndTime = 1.0f; //一秒

    [SerializeField, Tooltip("ゲームが進行した時間")]
    private float gameStepTime = 0.0f; //一秒
    [SerializeField, Tooltip("ゲームの開始フラグ")]
    private bool isPreUpdatePassed = false; //一秒
    [SerializeField, Tooltip("ゲームの開始フラグ")]
    private bool isGameStartPassed = false; //一秒
    [SerializeField, Tooltip("ゲームの開始フラグ")]
    private bool isGameSetPassed = false; //一秒
    [SerializeField, Tooltip("ゲームの開始フラグ")]
    private bool isGameEndPassed = false; //一秒

    public struct RespawnParamater
    {
        public Vector2 pos { get; set; }
        public float rotation { get; set; }
        public RespawnParamater(Vector2 pos, float rotation)
        {
            this.pos = pos;
            this.rotation = rotation;
        }
    };

    [SerializeField]
    RespawnParamater[] respawnParamaters = {
        new RespawnParamater(new Vector2(  -10.0f,  10.0f),-135.0f),
        new RespawnParamater(new Vector2(   10.0f,  10.0f), 135.0f),
        new RespawnParamater(new Vector2(   0.0f,  -10.0f),-45.0f),
        new RespawnParamater(new Vector2(   0.0f,  -10.0f), 45.0f)
    };

    [SerializeField]
    List<Sprite> characterTextures;


    System.Action gameUpdater;
    Camera mainCamera;
    List<PlayerController> players;
    CanvasGroup UIcanvas;
    [SerializeField]
    private Fade fade = null;

    // Use this for initialization
    void Start()
    {
        Initialize();

    }

    // Update is called once per frame
    void Update()
    {
        gameUpdater();

    }

    void Initialize()
    {
        isPreUpdatePassed = false; //一秒
        isGameStartPassed = false; //一秒
        isGameSetPassed = false; //一秒
        isGameEndPassed = false; //一秒
        gameStepTime = 0.0f;
        //接続されているコントローラーからプレイヤーを生成する
        //Lutが必要
        //Test
        if (ModeSeting.player_data != null)
        {
            ModeSeting.player_data[0] = new ModeSeting.PlayerData();
        }
        foreach (var pl in ModeSeting.player_data)
        {
            GameObject obj = (GameObject)Resources.Load("Player");
            GameObject playerObj = Instantiate(obj, respawnParamaters[pl.player_number].pos, Quaternion.AngleAxis(respawnParamaters[pl.player_number].rotation, Vector3.forward));
            PlayerController player = playerObj.GetComponent<PlayerController>();
            SpriteRenderer spriteRendere = playerObj.GetComponent<SpriteRenderer>();

            player.Initialized();
            player.SetPlayerNO(pl.player_number);
            player.SetRespownParamater(respawnParamaters[pl.player_number].pos, respawnParamaters[pl.player_number].rotation);
            player.SetUpdaterToWait();
            spriteRendere.sprite = this.characterTextures[pl.color];

        }
        gameUpdater = GameUpdateFadeIn;
        fade.FadeIn(maxFadeUpdateTime, () => {
            gameUpdater = GameUpdatePreUpdate;

        });
    }

    void GameUpdateFadeIn()
    {

    }

    //ゲームが始まる前までの処理
    void GameUpdatePreUpdate()
    {
        gameStepTime += Time.deltaTime;

        if (gameStepTime >= maxPreUpdateTime)
        {
            gameStepTime = 0.0f;
            isPreUpdatePassed = true;
            gameUpdater = GameUpdateStartGame;
        }
    }
    //ゲームのスタートを告げる（カットインとか）処理
    void GameUpdateStartGame()
    {
        gameStepTime += Time.deltaTime;
        if (gameStepTime >= maxStartGameTime)
        {
            gameStepTime = 0.0f;
            isGameStartPassed = true;

            gameUpdater = GameUpdateNormal;
        }
    }
    //ゲームの通常アップデート　もしもこのアップデート中にプレイヤーの残機がゼロになるなどでゲームセットフェーズに移行
    void GameUpdateNormal()
    {

    }

    //ゲームの終了を告げるカットイン
    void GameUpdateGameSet()
    {
        gameStepTime += Time.deltaTime;
        if (gameStepTime >= maxGameSetTime)
        {
            gameStepTime = 0.0f;
            isGameSetPassed = true;

            gameUpdater = GameUpdateNormal;
        }
    }

    //全ての入力を切ってフェードフラグをたたせる
    void GameUpdateGameEnd()
    {
        gameStepTime += Time.deltaTime;
        if (gameStepTime >= maxGameEndTime)
        {
            gameStepTime = 0.0f;
            isGameEndPassed = true;

            gameUpdater = GameUpdateNormal;

            fade.FadeOut(maxFadeUpdateTime, () => {
                gameUpdater = GameUpdateFadeOut;

            });
        }
    }
    void GameUpdateFadeOut()
    {
        if (fade.IsFadeOutCompleted())
        {
            //次のシーンの読み込み

        }
    }
}
