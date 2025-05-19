using UnityEngine;

public class MovingSphere2 : MonoBehaviour
{


    [SerializeField]
    Rect allowedArea = new Rect(-4.5f, -4.5f, 9f, 9f);
	[SerializeField, Range(0f, 1f)]
	float bounciness = 0.5f;
    [SerializeField,Range(0f,100f)]
    float maxSpeed = 10;
    [SerializeField,Range(0f,100f)]
    float maxAcceleration = 10;
    Vector3 velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        Vector3 acceleration = new Vector3(playerInput.x, 0, playerInput.y) * maxAcceleration;

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0, playerInput.y) * maxSpeed;

        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        //移动的距离
        Vector3 displacement = velocity * Time.deltaTime;
        //移动
        Vector3 newPosition = transform.localPosition + displacement;
        //限制移动范围
        if(newPosition.x < allowedArea.xMin)
        {
            newPosition.x = allowedArea.xMin;
            velocity.x = -velocity.x*bounciness;
        }
        else if(newPosition.x > allowedArea.xMax)
        {
            newPosition.x = allowedArea.xMax;
            velocity.x = -velocity.x*bounciness;
        }
        if(newPosition.z < allowedArea.yMin)
        {
            newPosition.z = allowedArea.yMin;
            velocity.z = -velocity.z*bounciness;
        }
        else if(newPosition.z > allowedArea.yMax)
        {
            newPosition.z = allowedArea.yMax;
            velocity.z = -velocity.z*bounciness;
        }
        transform.localPosition = newPosition;

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
