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
    public int maxReflections = 5; // 最大反射次数

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
        else if (verticalInput > 0)
        {
            lastInputDirection = Vector2.up; // 向上
        }
        else if (verticalInput < 0)
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

        // 存储射线段的起点和终点
        List<Vector3> rayPoints = new List<Vector3>();

        // 初始射线起点
        startPoint = rb.position;
        rayPoints.Add(startPoint);

        // 初始射线方向和剩余长度
        Vector2 currentDirection = direction;
        float remainingLength = rayLength;
        int reflections = 0; // 当前反射次数

        while (remainingLength > 0 && reflections < maxReflections)
        {
            // 发射射线并检测碰撞
            hit = Physics2D.Raycast(startPoint, currentDirection, remainingLength, collisionLayer);

            if (hit.collider != null && hit.collider.tag == "Reflection")
            {
                // 碰撞点和法向量
                Vector2 hitPoint = hit.point;
                Vector2 normal = hit.normal;

                // 计算反射向量
                Vector2 reflection = Vector2.Reflect(currentDirection, normal);

                // 打印法向量、入射向量和反射向量以便调试
                Debug.Log("Hit normal: " + normal + ", Incident vector: " + currentDirection + ", Reflection vector: " + reflection);

                // 更新起点和方向
                startPoint = hitPoint;
                currentDirection = reflection;

                // 添加碰撞点
                rayPoints.Add(hitPoint);

                // 减少剩余长度
                remainingLength -= hit.distance;
                reflections++; // 增加反射次数

                Debug.Log("Ray hit at point: " + hit.point + ", normal: " + normal + ", reflection direction: " + reflection);
            }
            else
            {
                // 如果没有碰撞，延伸到剩余长度的终点
                endPoint = startPoint + currentDirection * remainingLength;
                rayPoints.Add(endPoint);
                remainingLength = 0;

                Debug.Log("Ray endpoint: " + endPoint);
            }
        }

        // 设置 LineRenderer 的点数和位置
        lineRenderer.positionCount = rayPoints.Count;
        for (int i = 0; i < rayPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, rayPoints[i]);
        }

        // 启动协程来清除线条
        StartCoroutine(ClearLineAfterDelay(lineDuration));

        // 确认 LineRenderer 的位置设置是否正确
        Debug.Log("LineRenderer positions set: " + string.Join(", ", rayPoints));
    }

    private IEnumerator ClearLineAfterDelay(float delay)
    {
        // 等待指定的时间
        yield return new WaitForSeconds(delay);

        // 清除 LineRenderer 的点数
        lineRenderer.positionCount = 0;
    }
}

