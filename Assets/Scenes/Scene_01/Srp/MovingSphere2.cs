using UnityEngine;

public class MovingSphere2 : MonoBehaviour
{


    /*
    如果我们想使用物理引擎，那么我们应该让它控制球体的位置。直接调整位置实际上相当于进行空间传送，这并非我们想要的效果。
    相反，我们必须间接地控制球体，要么施加力，要么调整其速度。
    我们已经实现了对位置的间接控制，因为我们影响的是速度。
    现在，我们想使用物理引擎来控制球体。
    为此，我们需要使用Rigidbody组件。
    我们还需要禁用球体的Transform组件，因为物理引擎会控制它的位置。
    
    */
    [SerializeField]
    Rect allowedArea = new Rect(-4.5f, -4.5f, 9f, 9f);
	[SerializeField, Range(0f, 1f)]
	float bounciness = 0.5f;
    [SerializeField,Range(0f,100f)]
    float maxSpeed = 10;
    [SerializeField,Range(0f,100f)]
    float maxAcceleration = 10;
    Vector3 velocity,desiredVelocity;


    bool onGround;

    [SerializeField]
    float jumpHeight = 2f;
    [SerializeField,Range(0,3)]
    int maxAirJumps = 0;

    int jumpPhase;

    [SerializeField,Range(0f,90f)]
    //最大地面角度
    float maxGroundAngle = 25f;
    float minGroundDotProduct;
    //跳跃
    bool desiredJump;
    //接触点法线
    Vector3 contactNormal;

    Rigidbody rigidbody;
    void OnValidate () {
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
	}
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        OnValidate();
    }

    // Update is called once per frame
    void Update()
    {
        desiredJump = false;

        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        Vector3 acceleration = new Vector3(playerInput.x, 0, playerInput.y) * maxAcceleration;

        desiredVelocity = new Vector3(playerInput.x, 0, playerInput.y) * maxSpeed;

       
        //跳跃
        desiredJump |= Input.GetButtonDown("Jump");
    }
    void FixedUpdate()
    {
        //最大加速度
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
       
        UpdateState();
    
        if(desiredJump && onGround)
        {
            desiredJump = false;
            Jump();
        }
        // 使用Mathf.MoveTowards来限制速度变化
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        if(desiredVelocity.x != 0 || desiredVelocity.z != 0||desiredVelocity.y != 0)
        {
            rigidbody.linearVelocity = velocity;
        }
       
    }

    void UpdateState () {
		velocity = rigidbody.linearVelocity;
		if (onGround) {
			jumpPhase = 0;
		}

	}

    void Jump()
    {
        //在地面或者跳跃次数小于最大跳跃次数，都可以跳跃
        if(onGround || jumpPhase < maxAirJumps)
        {
            jumpPhase++;
            
            // 使用接触点法线方向进行跳跃，而不是仅垂直方向
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            // 计算当前在跳跃方向上的速度分量
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            // 如果已经有向上的速度，只添加差值
            if (alignedSpeed > 0f) {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            // 添加跳跃速度到接触点法线方向
            velocity += contactNormal * jumpSpeed;
        }
    }

    Vector3 ProjectOnContactPlane (Vector3 vector) {
		return vector - contactNormal * Vector3.Dot(vector, contactNormal);
	}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        // 获取矩形的四个角点
        Vector3 bottomLeft = new Vector3(allowedArea.xMin, 0, allowedArea.yMin);
        Vector3 bottomRight = new Vector3(allowedArea.xMax, 0, allowedArea.yMin);
        Vector3 topLeft = new Vector3(allowedArea.xMin, 0, allowedArea.yMax);
        Vector3 topRight = new Vector3(allowedArea.xMax, 0, allowedArea.yMax);
        
        // 绘制四条边
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
        
        // 可选：绘制半透明矩形填充
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawCube(new Vector3(allowedArea.center.x, 0, allowedArea.center.y), 
                        new Vector3(allowedArea.width, 0.1f, allowedArea.height));
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        EvaluateCollision(collision);
    }

    //评估碰撞,通过接触点的Y轴判断是否在地面
    void EvaluateCollision(Collision collision)
    {
        for(int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            //如果法线Y轴大于最大地面角度，则认为在地面,（可以起跳
            if (normal.y >= minGroundDotProduct) {
				onGround = true;
				contactNormal = normal;
			}
            //如果法线Y轴小于最大地面角度，则认为在空中
            else
            {
                contactNormal = Vector3.up;
            }
        }
    }
}
