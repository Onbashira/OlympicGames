using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.SceneManagement;

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
        new RespawnParamater(new Vector2(    0.0f,  00f),-0.0f),
        new RespawnParamater(new Vector2(  -1.0f,  0.5f),-135.0f),
        new RespawnParamater(new Vector2(   1.0f,  0.5f), 135.0f),
        new RespawnParamater(new Vector2(  -1.0f,  -0.5f),-45.0f),
        new RespawnParamater(new Vector2(   1.0f,  -0.5f), 45.0f)
    };

    [SerializeField]
    List<Sprite> characterTextures;


    System.Action gameUpdater;
    List<GameObject> players = new List<GameObject>();
    CanvasGroup UIcanvas;
    [SerializeField]
    private Fade fade = null;
    [SerializeField]
    CameraShaker shaker = null;
    [SerializeField]
    private uint maxPlayer;
    private uint deadCount;

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

        foreach (var pl in ModeSetting.player_data)
        {
            players.Add(Instantiate((GameObject)Resources.Load("Player"), respawnParamaters[pl.player_number + 1].pos, Quaternion.AngleAxis(respawnParamaters[pl.player_number + 1].rotation, Vector3.forward)));

            players[players.Count - 1].GetComponent<PlayerController>().Initialized();
            players[players.Count - 1].GetComponent<PlayerController>().SetPlayerNO(pl.player_number + 1);
            players[players.Count - 1].GetComponent<PlayerController>().SetRespownParamater(respawnParamaters[pl.player_number + 1].pos, respawnParamaters[pl.player_number + 1].rotation);
            players[players.Count - 1].GetComponent<PlayerController>().SetUpdaterToWait();
            players[players.Count - 1].GetComponent<SpriteRenderer>().sprite = this.characterTextures[(int)pl.color];
        }
        maxPlayer = (uint)players.Count;
        deadCount = 0;
        gameUpdater = GameUpdateFadeIn;
        fade.FadeIn(maxFadeUpdateTime, () =>
        {
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
            AllPlayerActive();
            gameUpdater = GameUpdateNormal;
        }
    }

    void AllPlayerActive()
    {
        foreach (var pl in players)
        {
            pl.GetComponent<PlayerController>().SetUpdaterToNormal();
        }
    }
    
    //ゲームの通常アップデート　もしもこのアップデート中にプレイヤーの残機がゼロになるなどでゲームセットフェーズに移行
    void GameUpdateNormal()
    {
        foreach (var player in players)
        {
            var p = player.GetComponent<PlayerController>();
            if (p.IsColl())
            {
                shaker.Shake(0.2f, 0.1f);
                p.CollReset();
                break;
            }
            if (p.IsDead())
            {
                GameResultManager.GetPlayerRank().Add((int)p.GetPlayerNO());
                p.DeadPlayer();

                ++deadCount;

            }
            if (deadCount == maxPlayer - 1)
            {
                gameUpdater = GameUpdateGameSet;
                return;
            }
        }
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

            fade.FadeOut(maxFadeUpdateTime, () =>
            {
                gameUpdater = GameUpdateFadeOut;

            });
        }
    }
    void GameUpdateFadeOut()
    {
        if (fade.IsFadeOutCompleted())
        {
            GameResultManager.GetPlayerRank().Reverse();
            SceneManager.LoadScene("Result");

        }
    }
}
