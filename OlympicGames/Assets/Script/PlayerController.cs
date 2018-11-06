using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    Rigidbody2D rigid;
    [SerializeField]
    float movePower = 1.0f;
    [SerializeField]
    float hoverMovePower = 1.0f;
    [SerializeField]
    Vector2 hoverClamp;
    [SerializeField]
    float torquePower = 1.0f;
    [SerializeField]
    float maxChargeTime = 2.0f;
    [SerializeField]
    Vector2 torqueClamp;
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
    bool isMassive = false;
    [SerializeField]
    System.Action inputUpdater;
    [SerializeField]
    System.Action moveUpdater;
    [SerializeField]
    Vector2 respownPos;
    [SerializeField]
    float respownRotate;
    [SerializeField]
    float downTime = 3.0f;
    [SerializeField]
    private GamepadInput.GamepadState gamepadState;
    [SerializeField]
    private GamepadInput.GamepadState gamepadStateOld;
    [SerializeField]
    private Vector2 moveVelocity = Vector2.zero;
    [SerializeField]
    private Vector2 velocityClamp;
    [SerializeField]
    private float angulerVelocity = 0.0f;
    [SerializeField]
    private float chargeTime = 0.0f;

    private void Initialized()
    {
        moveUpdater = Normal;
        inputUpdater = GamePadStateUpdate;
        playerNo = (GamepadInput.GamePad.Index)1; //test
        chargeTime = 0.0f;
        gamepadStateOld = gamepadState = GamepadInput.GamePad.GetState(playerNo);
        angulerVelocity = 0.0f;
    }

    // Use this for initialization
    void Start()
    {
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

    private void FixedUpdate()
    {

    }

    private void LateUpdate()
    {

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
            rigid.velocity -= dump * Time.fixedDeltaTime;
            if (rigid.velocity.SqrMagnitude().Equals(0.0f))
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
        tempVelo *= hoverMovePower * Time.fixedDeltaTime;
        tempVelo.x = Mathf.Clamp(tempVelo.x, hoverClamp.x, hoverClamp.y);
        tempVelo.y = Mathf.Clamp(tempVelo.y, hoverClamp.x, hoverClamp.y);

        rigid.AddRelativeForce((tempVelo * movePower), ForceMode2D.Force);

    }

    void Charge()
    {
        if (gamepadState.A) {
            chargeTime += 1.0f * Time.fixedDeltaTime;
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

    void Wait()
    {

    }

    void Normal()
    {
        if (gamepadState.A && !gamepadStateOld.A)
        {
            moveUpdater = Charge;
            return;
        }
        else if (gamepadState.B && !gamepadStateOld.B)
        {
            moveUpdater = Brake;
            return;
        }
    }

    void RotatePlayer()
    {
        if (gamepadState.LeftShoulder )
        {
            //rigid.AddTorque(torquePower * Time.fixedDeltaTime, ForceMode2D.Impulse);
            this.transform.Rotate(Vector3.forward, torquePower * Time.fixedDeltaTime);

        }
        if (gamepadState.RightShoulder)
        {
            //rigid.AddTorque(-torquePower * Time.fixedDeltaTime, ForceMode2D.Impulse);
            this.transform.Rotate(Vector3.forward, -torquePower * Time.fixedDeltaTime);
        }

    }

    void Downed()
    {

    }

    void Massive()
    {

    }

    void GamePadStateUpdate()
    {
        gamepadStateOld = gamepadState;

        gamepadState = GamepadInput.GamePad.GetState(playerNo);

    }

}
