using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownShifterSmooth : MonoBehaviour
{
    [Header("UI References（按顺序）")]
    public GameObject currentCountdown;
    public GameObject n1; // n-1
    public GameObject n2; // n-2
    public GameObject n3; // n-3
    public GameObject p1; // n+1
    public GameObject p2; // n+2
    public GameObject p3; // n+3

    [Header("动画设置")]
    public float transitionDuration = 0.5f; // 移动 & 渐变时间

    private List<GameObject> objects = new List<GameObject>();
    private int currentNumber = 0;
    private bool isAnimating = false;

    void Start()
    {
        objects.Add(currentCountdown); // index 0
        objects.Add(n1); // index 1
        objects.Add(n2); // index 2
        objects.Add(n3); // index 3
        objects.Add(p1); // index 4
        objects.Add(p2); // index 5
        objects.Add(p3); // index 6

        if (currentCountdown.TryGetComponent(out Text textComp))
        {
            int.TryParse(textComp.text, out currentNumber);
        }

        StartCoroutine(LoopShift());
    }

    IEnumerator LoopShift()
    {
        while (true)
        {
            if (!isAnimating)
            {
                yield return StartCoroutine(ShiftOnceSmooth());
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator ShiftOnceSmooth()
    {
        isAnimating = true;

        List<Vector3> startPositions = new List<Vector3>();
        List<Vector3> targetPositions = new List<Vector3>();
        List<Color> targetColors = new List<Color>();
        List<string> newTexts = new List<string>();

        for (int i = 0; i < objects.Count; i++)
        {
            RectTransform rt = objects[i].GetComponent<RectTransform>();
            startPositions.Add(rt.localPosition);

            int fromIndex = (i + 1) % objects.Count;
            RectTransform fromRT = objects[fromIndex].GetComponent<RectTransform>();
            targetPositions.Add(fromRT.localPosition);

            // 缓存透明度
            if (objects[fromIndex].TryGetComponent(out Text fromText))
            {
                targetColors.Add(fromText.color);
            }
            else
            {
                targetColors.Add(Color.white);
            }
        }

        // 获取新的中心数字
        if (objects[1].TryGetComponent(out Text futureCurrent))
        {
            int.TryParse(futureCurrent.text, out currentNumber);
        }

        // 准备新文字内容
        for (int i = 0; i < objects.Count; i++)
        {
            int newValue = currentNumber + OffsetFromCenter(i);
            newTexts.Add(newValue.ToString());
        }

        // 平滑动画
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;

            for (int i = 0; i < objects.Count; i++)
            {
                RectTransform rt = objects[i].GetComponent<RectTransform>();
                rt.localPosition = Vector3.Lerp(startPositions[i], targetPositions[i], t);

                if (objects[i].TryGetComponent(out Text text))
                {
                    Color c = text.color;
                    c.a = Mathf.Lerp(c.a, targetColors[i].a, t);
                    text.color = c;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 最终状态
        for (int i = 0; i < objects.Count; i++)
        {
            RectTransform rt = objects[i].GetComponent<RectTransform>();
            rt.localPosition = targetPositions[i];

            if (objects[i].TryGetComponent(out Text text))
            {
                text.text = newTexts[i];
                text.color = targetColors[i];
            }
        }

        isAnimating = false;
    }

    // index → offset
    int OffsetFromCenter(int index)
    {
        switch (index)
        {
            case 0: return 0;
            case 1: return -1;
            case 2: return -2;
            case 3: return -3;
            case 4: return +1;
            case 5: return +2;
            case 6: return +3;
            default: return 0;
        }
    }
}
