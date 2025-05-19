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


    //跳跃
    bool desiredJump;

    Rigidbody rigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
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
        if(desiredJump)
        {
                desiredJump = false;
                Jump();
        }
        velocity = rigidbody.linearVelocity;
        // 使用Mathf.MoveTowards来限制速度变化
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        
        rigidbody.linearVelocity = velocity;
        
    }

    void Jump()
    {
        velocity.y +=5f;
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
}
