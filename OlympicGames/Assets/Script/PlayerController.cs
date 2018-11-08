using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;


public class PlayerController : MonoBehaviour
{

    Rigidbody2D rigid;
    [SerializeField]
    float movePower = 1.0f;
    [SerializeField]
    float boostPower = 2.0f;
    [SerializeField]
    float AttackPower = 5.0f;
    [SerializeField]
    private Vector2 moveVelocity = Vector2.zero;
    [SerializeField]
    private float velocityClamp;
    [SerializeField]
    float hoverMovePower = 1.0f;
    [SerializeField]
    private float hoverClamp;
    [SerializeField]
    private float angulerVelocity = 0.0f;
    [SerializeField]
    float torquePower = 1.0f;
    [SerializeField]
    float torqueClamp;
    [SerializeField]
    float brakeDump = 0.1f;
    [SerializeField]
    float brakeAngulerDump = 0.1f;
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
    bool isCollided = false;

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
    float blinkDownTime = 0.1f;
    [SerializeField]
    float blinkInvincibleTime = 0.4f;
    float blinkTimer = 0.0f;
    [SerializeField]
    float attackTime = 1.0f;
    float attackTimer = 0.0f;
    [SerializeField]
    private GamepadInput.GamepadState gamepadState;
    [SerializeField]
    private GamepadInput.GamepadState gamepadStateOld;
    [SerializeField]
    AudioSource deadSE;
    [SerializeField]
    AudioSource boostSE;
    [SerializeField]
    AudioSource moveSE;
    [SerializeField]
    AudioSource downSE;
    [SerializeField]
    AudioSource slashSE;

    SpriteRenderer spriteRenderer = null;

    System.Action inputUpdater;
    System.Action moveUpdater;
    System.Action stateUpdater;
    System.Action attackSeq;

    [SerializeField]
    CapsuleCollider2D slasherCollider = null;

    public void Initialized()
    {
        isCollided = false;
        stateUpdater = Normal;
        moveVelocity = Vector2.zero;
        angulerVelocity = 0.0f;
        inputUpdater = GamePadStateUpdate;
        gamepadStateOld = gamepadState = GamepadInput.GamePad.GetState(playerNo);
        angulerVelocity = 0.0f;
    }

    // Use this for initialization
    void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        rigid = this.GetComponent<Rigidbody2D>();
        //slasherCollider = this.GetComponentInChildren<CapsuleCollider2D>();
        slasherCollider.enabled = false;
        Initialized();
    }

    // Update is called once per frame
    void Update()
    {
        inputUpdater();
        stateUpdater();
        //RotatePlayer();
        ClampVelocity();
        ClapmAngluerVelocity();
    }

    private void LateUpdate()
    {

    }

    private void FixedUpdate()
    {

    }

    public bool IsColl()
    {
        return isCollided;
    }

    public void CollReset()
    {
        isCollided = false;
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

    public void SetRespownParamater(Vector2 pos, float rotate)
    {

        this.respownPos = pos;
        this.respownRotate = rotate;

    }

    public void SetUpdaterToWait()
    {
        stateUpdater = Wait;
    }

    public void SetUpdaterToNormal()
    {
        stateUpdater = Normal;
    }

    void ClampVelocity()
    {
        moveVelocity = rigid.velocity;
        moveVelocity.x = Mathf.Clamp(moveVelocity.x, -velocityClamp, velocityClamp);
        moveVelocity.y = Mathf.Clamp(moveVelocity.y, -velocityClamp, velocityClamp);
        rigid.velocity = moveVelocity;
    }

    void ClapmAngluerVelocity()
    {
        rigid.angularVelocity = Mathf.Clamp(rigid.angularVelocity, -torqueClamp, torqueClamp);
        this.angulerVelocity = rigid.angularVelocity;

    }

    void Brake()
    {
        DumpBrake();

        if (!gamepadState.B)
        {
            stateUpdater = Normal;
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
        tempVelo = tempVelo * hoverMovePower * Time.deltaTime;
        tempVelo.x = Mathf.Clamp(tempVelo.x, -hoverClamp, hoverClamp);
        tempVelo.y = Mathf.Clamp(tempVelo.y, -hoverClamp, hoverClamp);

        rigid.AddRelativeForce((tempVelo), ForceMode2D.Force);
        RotatePlayer();
    }

    void Move()
    {
        Vector2 tempVelo = (Vector2.up * movePower);
        rigid.AddRelativeForce(tempVelo, ForceMode2D.Impulse);
        moveVelocity = rigid.velocity;
        stateUpdater = Normal;
    }

    void DumpBrake(float amount = 1.0f)
    {
        if (!(rigid.velocity.SqrMagnitude().Equals(0.0f)))
        {
            Vector2 dump = rigid.velocity * brakeDump * amount* Time.deltaTime;
            rigid.velocity -= dump;
        }
        if (rigid.velocity.SqrMagnitude() < 0.00001f)
        {
            rigid.velocity = Vector2.zero;
        }
    }

    void DumpAngleBrake(float amount = 1.0f)
    {
        if (!(rigid.angularVelocity.Equals(0.0f)))
        {
            float dump = rigid.angularVelocity * brakeAngulerDump * amount * Time.deltaTime;
            rigid.angularVelocity -= dump;
        }
        if (rigid.angularVelocity < 0.0001f)
        {
            rigid.angularVelocity = 0.0f;
        }
    }

    void Attack()
    {
        Vector2 boostVelo = (Vector2.up * boostPower);

        rigid.AddRelativeForce(boostVelo, ForceMode2D.Impulse);

        // StartCoroutine(AttakerUpdate(1.0f));
        stateUpdater = AttackAction1;
        RotatePlayer();
    }

    //急速前進減衰あり
    void AttackAction1()
    {
        DumpBrake(2.0f);
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackTime)
        {
            rigid.angularVelocity = 0.0f;


            slasherCollider.enabled = true;
            attackTimer = 0.0f;
            stateUpdater = AttackAction2;
            return;
        }
    }

    //切りつけ回転
    void AttackAction2()
    {
        DumpBrake(2.0f);
        attackTimer += Time.deltaTime;
        transform.Rotate(new Vector3( 0.0f,0.0f , 12.0f ));

        if (attackTimer >= attackTime)
        {
            slasherCollider.enabled = false;
            attackTimer = 0.0f;
            stateUpdater = Normal;
        }
    }

    private IEnumerator AttakerUpdate(float duration)
    {
        var elapsed = 0.0f;

        while (elapsed < duration)
        {


            elapsed += Time.deltaTime;
            if (this.isDown)
            {
                slasherCollider.enabled = false;

                yield return null;
            }
            yield return null;
        }
        slasherCollider.enabled = false;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //他プレイヤーとの衝突時
        if (collision.gameObject.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.moveVelocity.SqrMagnitude() >= moveVelocity.SqrMagnitude())
            {

                rigid.velocity += player.rigid.velocity / 2.0f;
                player.rigid.velocity /= 2.0f;
            }

        }

        else if (collision.gameObject.tag == "BlackHole") //ギミックとの衝突時
        {
            //Vector3 hole = collision.transform.position;
            //Vector3 vec = hole - this.transform.position;
        }
        else if (collision.gameObject.tag == "Wall") //壁との衝突時
        {
            slasherCollider.enabled = false;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //他プレイヤー攻撃との衝突時
        if (collision.gameObject.tag == "AttackColl")
        {
            var player = collision.gameObject.GetComponentInParent<PlayerController>();
            var vec = -(player.transform.position - this.transform.position).normalized;
            this.rigid.velocity = vec * AttackPower;
            isCollided = true;
            stateUpdater = Downed;
            slasherCollider.enabled = false;
            Downed();
            attackTime = 0.0f;
        }
    }

    void Wait()
    {

    }

    void Normal()
    {
        if (gamepadState.A && !gamepadStateOld.A)
        {
            stateUpdater = Move;
            return;
        }
        else if (gamepadState.Y && !gamepadStateOld.Y)
        {
            stateUpdater = Attack;
            return;
        }
        else if (gamepadState.B && !gamepadStateOld.B)
        {
            stateUpdater = Brake;
            return;
        }
        RotatePlayer();
    }

    void RotatePlayer()
    {
        //mode1
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
        //mode2
        //{
        //    Vector2 rotateVector = gamepadState.LeftStickAxis;
        //    if (!rotateVector.SqrMagnitude().Equals(0.0f))
        //    {
        //        float angle = Mathf.Rad2Deg * Mathf.Atan2(rotateVector.y, rotateVector.x) - 90.0f;
        //        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        //    }
        //}

    }

    void Downed()
    {
        downtimer += Time.deltaTime;
        blinkTimer += downTime;
        if (blinkTimer >= blinkDownTime)
        {
            blinkTimer = 0.0f;
            bool flg = spriteRenderer.enabled;
            spriteRenderer.enabled = !flg;
        }
        if (downtimer >= downTime)
        {
            isDown = false;
            isInvincible = true;
            spriteRenderer.enabled = true;
            blinkTimer = 0.0f;

            downtimer = 0.0f;
            stateUpdater = Invincible;
            return;
        }
        {
            DumpBrake(1.0f);

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
            tempVelo.x = Mathf.Clamp(tempVelo.x, -hoverClamp, hoverClamp);
            tempVelo.y = Mathf.Clamp(tempVelo.y, -hoverClamp, hoverClamp);

            rigid.AddRelativeForce((tempVelo * movePower), ForceMode2D.Force);
        }
    }

    void Invincible()
    {
        invincibleTimer += Time.deltaTime;
        blinkTimer += invincibleTimer;
        if (blinkTimer >= blinkInvincibleTime)
        {
            blinkTimer = 0.0f;
            bool flg = spriteRenderer.enabled;
            spriteRenderer.enabled = !flg;
        }
        if (invincibleTimer >= invincibleTime)
        {
            spriteRenderer.enabled = true;
            blinkTimer = 0.0f;
            isInvincible = false;
            invincibleTimer = 0.0f;
            stateUpdater = Normal;
        }
    }

    void GamePadStateUpdate()
    {
        gamepadStateOld = gamepadState;

        gamepadState = GamepadInput.GamePad.GetState(playerNo, true);

    }


}
