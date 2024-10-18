using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayInterface;

public class Light : MonoBehaviour, IRay
{
    public float MoveSeppd = 5f; // 移动速度
    private Rigidbody2D rb; // 获取刚体
    public LayerMask collisionLayer; // 射线碰撞的图层
    public float rayLength = 10f; // 限制射线长度
    public float lineDuration = 1f; // 线条持续时间    

    private Vector2 startPoint; // 射线起始点
    private Vector2 endPoint; // 射线终点
    private RaycastHit2D hit; // 射线碰撞信息
    private Vector2 direction = Vector2.right; // 射线初始方向
    private Vector2 lastInputDirection; // 最后一次按下的方向
    private LineRenderer lineRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        // 设置 LineRenderer 的材质和宽度
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // 确认 LineRenderer 已正确初始化
        Debug.Log("LineRenderer initialized with startWidth: " + lineRenderer.startWidth + " and endWidth: " + lineRenderer.endWidth);
    }

    void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.J))
        {          
                // 调用 Ray() 方法
                ((IRay)this).Ray();                    
        }
        
    }

    void Move()
    {
        // 获取输入
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        // 更新最后一次按下的方向
        if (horizontalInput > 0)
        {
            lastInputDirection = Vector2.right; // 向右
        }
        else if (horizontalInput < 0)
        {
            lastInputDirection = Vector2.left; // 向左
        }
        else if (verticalInput < 0)
        {
            lastInputDirection = Vector2.up; // 向上
        }
        else if (verticalInput > 0)
        {
            lastInputDirection = Vector2.down; // 向下
        }
        // 计算移动方向
        Vector2 targetVelocity = new Vector2(horizontalInput, verticalInput).normalized * MoveSeppd;

        // 移动
        rb.velocity = targetVelocity;
    }

    void IRay.Ray()
    {
        // 使用最后一次按下的方向射出射线
        direction = lastInputDirection;
        // 计算射线起始点和终点
        startPoint = rb.position;
        endPoint = startPoint + direction * rayLength;
        
        // 发射射线并检测碰撞
        hit = Physics2D.Raycast(startPoint, direction, rayLength, collisionLayer);

        if (hit.collider != null)
        {
            Vector2 normal = hit.normal;
            Vector2 reflection = Vector2.Reflect(direction, normal); // 反射向量

            // 更新方向和终点
            endPoint = hit.point+reflection*(rayLength-hit.distance);

            // 设置 LineRenderer 的点数和位置
            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, hit.point);
            lineRenderer.SetPosition(2, endPoint);

            Debug.Log("Ray hit at point: " + hit.point + ", normal: " + normal + ", reflection direction: " + reflection);
        }
        else
        {
            // 没有碰撞时设置 LineRenderer 的点数和位置
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);

            Debug.Log("Ray endpoint: " + endPoint);
        }
        // 启动协程来清除线条
        StartCoroutine(ClearLineAfterDelay(lineDuration));

        // 确认 LineRenderer 的位置设置是否正确
        Debug.Log("LineRenderer positions set: [" + startPoint + ", " + hit.point + ", " + endPoint + "]");
    }
    private IEnumerator ClearLineAfterDelay(float delay)
    {
        // 等待指定的时间
        yield return new WaitForSeconds(delay);

        // 清除 LineRenderer 的点数
        lineRenderer.positionCount = 0;
    }

}