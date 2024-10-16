using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayInterface;

public class Light : MonoBehaviour,IRay
{
    public float MoveSeppd = 5f;//移动速度
    private Rigidbody2D rb;//获取刚体 


    public LayerMask collisionLayer; // 射线碰撞的图层
    public float rayLength = 10f; // 限制射线长度，
    public float reflectionAngle; // 反射角度
  
    private Vector2 startPoint; // 射线起始点
    private Vector2 endPoint; // 射线终点
    private RaycastHit2D hit; // 射线碰撞信息
    private Vector2 direction = Vector2.right; // 射线初始方向
    private float angle =0f; // 当前角度

    LineRenderer lineRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        // 确保 LineRenderer 有材质和宽度
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        // 计算射线起始点和终点
        startPoint = rb.position;
        endPoint = startPoint + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * rayLength;

        // 发射射线并检测碰撞
        hit = Physics2D.Raycast(startPoint, endPoint - startPoint, rayLength, collisionLayer);
        //如果没有检测到碰撞
        // 计算射线起始点和终点
        if (hit.collider != null)
        {
            Vector2 normal = hit.normal;
            Vector2 reflection = Vector2.Reflect(direction, normal); // 反射向量

            
            // 计算反射角度
            float reflectionAngleRad = Mathf.Atan2(reflection.y, reflection.x);
            float reflectionAngle = 2 * Mathf.Rad2Deg * reflectionAngleRad - 180;
            endPoint = hit.point +new Vector2(Mathf.Cos(reflectionAngle), Mathf.Sin(reflectionAngle)) * (hit.distance - rayLength);
            // 更新终点和方向          

            // 设置 LineRenderer 的点数和位置
            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, hit.point);
            lineRenderer.SetPosition(2, endPoint);

            Debug.Log("Ray hit at point: " + hit.point + ", normal: " + normal + ", reflection: " + reflection);
        }
        else
        {
            // 没有碰撞时设置 LineRenderer 的点数和位置
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);

            Debug.Log("Ray endpoint: " + endPoint);

        }
    }
        void Move()
        {
        //获取输入
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        //计算移动方向
        Vector2 targetVelocity = new Vector2(horizontalInput, verticalInput).normalized * MoveSeppd;
        //移动
        rb.velocity = targetVelocity;
        }   
    void IRay.Ray()
    {
        
    }
}
