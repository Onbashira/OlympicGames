﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;


public class PlayerController : MonoBehaviour
{

    Rigidbody2D rigid;
    [SerializeField]
    float movePower = 1.0f;
    [SerializeField]
    private Vector2 moveVelocity = Vector2.zero;
    [SerializeField]
    private Vector2 velocityClamp;
    [SerializeField]
    float hoverMovePower = 1.0f;
    [SerializeField]
    Vector2 hoverClamp;
    [SerializeField]
    private float angulerVelocity = 0.0f;
    [SerializeField]
    float torquePower = 1.0f;
    [SerializeField]
    Vector2 torqueClamp;
    [SerializeField]
    float maxChargeTime = 2.0f;
    [SerializeField]
    float brakeDump = 0.1f;
    [SerializeField]
    GamepadInput.GamePad.Index playerNo;
    [SerializeField]
    uint playerStock = 0;

    [SerializeField]
    bool isDead = false;
    [SerializeField]
    bool isDown = false;
    [SerializeField]
    bool isInvincible = false;

    [SerializeField]
    Vector2 respownPos;
    [SerializeField]
    float respownRotate;
    [SerializeField]
    float downTime = 3.0f;
    float downtimer = 0.0f;
    [SerializeField]
    float invincibleTime = 3.0f;
    float invincibleTimer = 0.0f;
    [SerializeField]
    private GamepadInput.GamepadState gamepadState;
    [SerializeField]
    private GamepadInput.GamepadState gamepadStateOld;


    [SerializeField]
    private float chargeTime = 0.0f;

    SpriteRenderer spriteRenderer = null;

    System.Action inputUpdater;
    System.Action moveUpdater;
    System.Action stateUpdater;

    public void Initialized()
    {
        moveUpdater = Normal;
        inputUpdater = GamePadStateUpdate;
        playerNo = (GamepadInput.GamePad.Index)0; //test
        chargeTime = 0.0f;
        gamepadStateOld = gamepadState = GamepadInput.GamePad.GetState(playerNo);
        angulerVelocity = 0.0f;
    }

    // Use this for initialization
    void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        rigid = this.GetComponent<Rigidbody2D>();
        Initialized();
    }

    // Update is called once per frame
    void Update()
    {
        inputUpdater();
        moveUpdater();
        RotatePlayer();
        ClampVelocity();
        ClapmAngluerVelocity();
    }

    private void LateUpdate()
    {

    }

    private void FixedUpdate()
    {

    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool IsDown()
    {
        return isDown;
    }

    public void Kill()
    {
        isDead = true;
    }

    public void SetPlayerNO(int index)
    {
        playerNo = (GamepadInput.GamePad.Index)index;
    }

    public void SetRespownParamater(Vector2 pos , float rotate)
    {

        this.respownPos = pos;
        this.respownRotate = rotate;

    }

    public void SetUpdaterToWait()
    {
        moveUpdater = Wait;
    }

    public void SetUpdaterToNormal()
    {
        moveUpdater = Normal;
    }


    void ClampVelocity()
    {
        moveVelocity = rigid.velocity;
        moveVelocity.x = Mathf.Clamp(moveVelocity.x, velocityClamp.x, velocityClamp.y);
        moveVelocity.y = Mathf.Clamp(moveVelocity.y, velocityClamp.x, velocityClamp.y);
        rigid.velocity = moveVelocity;
    }

    void ClapmAngluerVelocity()
    {
        rigid.angularVelocity = Mathf.Clamp(rigid.angularVelocity, torqueClamp.x, torqueClamp.y);
        this.angulerVelocity = rigid.angularVelocity;

    }

    void Brake()
    {
        Vector2 rvelo = rigid.velocity;

        if (!(rigid.velocity.SqrMagnitude().Equals(0.0f)) && gamepadState.B)
        {
            Vector2 dump = rvelo * brakeDump;
            rigid.velocity -= dump * Time.deltaTime;
            if (rigid.velocity.SqrMagnitude() <= 0.001f)
            {
                rigid.velocity = Vector2.zero;
            }
        }
        else if (!gamepadState.B)
        {
            moveUpdater = Normal;
            return;
        }

        Vector2 tempVelo = Vector2.zero;

        if (gamepadState.Left)
        {
            tempVelo += Vector2.left;
        }
        if (gamepadState.Up)
        {
            tempVelo += Vector2.up;
        }
        if (gamepadState.Right)
        {
            tempVelo += Vector2.right;
        }
        if (gamepadState.Down)
        {
            tempVelo += Vector2.down;
        }

        tempVelo.Normalize();
        tempVelo *= hoverMovePower * Time.deltaTime;
        tempVelo.x = Mathf.Clamp(tempVelo.x, hoverClamp.x, hoverClamp.y);
        tempVelo.y = Mathf.Clamp(tempVelo.y, hoverClamp.x, hoverClamp.y);

        rigid.AddRelativeForce((tempVelo * movePower), ForceMode2D.Force);

    }

    void Charge()
    {
        if (gamepadState.A)
        {
            chargeTime += 1.0f * Time.deltaTime;
            if (chargeTime >= maxChargeTime)
            {
                chargeTime = maxChargeTime;
            }
        }


        if (!gamepadState.A)
        {
            Vector2 tempVelo = (Vector2.up * (float)chargeTime * movePower);
            if (chargeTime >= maxChargeTime)
            {
                rigid.velocity = Vector2.zero;
            }

            rigid.AddRelativeForce(tempVelo, ForceMode2D.Impulse);

            chargeTime = 0.0f;
            moveUpdater = Normal;
            return;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //他プレイヤーとの衝突時
        if (collision.gameObject.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.moveVelocity.SqrMagnitude() >= moveVelocity.SqrMagnitude())
            {
                isDown = true;
                moveUpdater = Downed;
                chargeTime = 0.0f;

            }

        }
        else if (collision.gameObject.tag == "BlackHole") //ギミックとの衝突時
        {
            Vector3 hole = collision.transform.position;
            Vector3 vec = hole - this.transform.position;
        }
        else if (collision.gameObject.tag == "Wall") //壁との衝突時
        {

        }
    }

    void Wait()
    {

    }

    void Normal()
    {
        if (gamepadState.A)
        {
            moveUpdater = Charge;
            return;
        }
        else if (gamepadState.B)
        {
            moveUpdater = Brake;
            return;
        }
    }

    void RotatePlayer()
    {
        if (gamepadState.LeftShoulder)
        {
            //rigid.AddTorque(torquePower * Time.deltaTime, ForceMode2D.Impulse);
            this.transform.Rotate(Vector3.forward, torquePower * Time.deltaTime);

        }
        if (gamepadState.RightShoulder)
        {
            //rigid.AddTorque(-torquePower * Time.deltaTime, ForceMode2D.Impulse);
            this.transform.Rotate(Vector3.forward, -torquePower * Time.deltaTime);
        }

    }

    void Downed()
    {
        downtimer += Time.deltaTime;

        if (downtimer >= downTime)
        {
            isDown = false;
            isInvincible = true;
            downtimer = 0.0f;
            moveUpdater = Invincible;
            return;
        }
        {
            //二倍の減衰でブレーキ
            Vector2 dump = rigid.velocity * brakeDump * 2.0f;
            rigid.velocity -= dump * Time.deltaTime;
            if (rigid.velocity.SqrMagnitude() <= 0.001f)
            {
                rigid.velocity = Vector2.zero;
            }

            Vector2 tempVelo = Vector2.zero;
            if (gamepadState.Left)
            {
                tempVelo += Vector2.left;
            }
            if (gamepadState.Up)
            {
                tempVelo += Vector2.up;
            }
            if (gamepadState.Right)
            {
                tempVelo += Vector2.right;
            }
            if (gamepadState.Down)
            {
                tempVelo += Vector2.down;
            }
            tempVelo.Normalize();
            tempVelo *= hoverMovePower * Time.deltaTime;
            tempVelo.x = Mathf.Clamp(tempVelo.x, hoverClamp.x, hoverClamp.y);
            tempVelo.y = Mathf.Clamp(tempVelo.y, hoverClamp.x, hoverClamp.y);

            rigid.AddRelativeForce((tempVelo * movePower), ForceMode2D.Force);
        }
    }

    void Invincible()
    {
        invincibleTimer += Time.deltaTime;
        if (invincibleTimer >= invincibleTime)
        {
            isInvincible = false;
            invincibleTimer = 0.0f;
            moveUpdater = Normal;
        }
    }

    void GamePadStateUpdate()
    {
        gamepadStateOld = gamepadState;

        gamepadState = GamepadInput.GamePad.GetState(playerNo);

    }


}
