using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class CurvedMoveEffect : MonoBehaviour
{
    private Action callback = null;
    [SerializeField]
    private GameObject startPos, endPos;

    private void Start()
    {
        MoveTo(startPos.transform.position, endPos.transform.position, 0.6f, 1, null);
    }

    private void OnComplete()
    {
        if (callback != null)
        {
            callback.Invoke();
        }
        Destroy(gameObject);
    }

    /// <param name="centerOffset">���ĵ�ƫ������0.5Ϊ��׼����������0.01������ʵ�ʵ���</param>
    public void MoveTo(Vector3 startPos, Vector3 endPos, float centerOffset, float time = 0.5f, Action callback = null)
    {
        this.callback = callback;
        gameObject.transform.position = startPos;
        gameObject.SetActive(true);
        StartCoroutine(Move(startPos, endPos, centerOffset, time, OnComplete));
    }

    //�������βЧ�����ӳ�һ��ʱ������ʾ������β�ͻ��в�Ӱ
    IEnumerator Move(Vector3 startPos, Vector3 endPos, float centerOffset, float time = 0.5f, Action onComplete = null)
    {
        TrailRenderer trail = gameObject.GetComponentInChildren<TrailRenderer>(true);
        yield return new WaitForSeconds(trail ? trail.time : 0);
        //���·������٣�����ʹ��CatmullRom���߲�ֵ�����˶���Ĭ��PathType.Linear
        //ͬʱ��������easetype��ʹ�ò�ͬ���˶���ʽ������easetype:https://blog.csdn.net/w0100746363/article/details/83587485
        //�����������һֱ����·��������Լ���SetLookAt(0):DOPath(path, time).SetLookAt(0);
        Tween tween = gameObject.transform.DOPath(Path(startPos, endPos, centerOffset, 6), time, PathType.CatmullRom).SetEase(Ease.OutSine);

        if (onComplete != null)
        {
            tween.OnComplete(new TweenCallback(onComplete));
        }
        if (trail)
            trail.enabled = true;
    }

    private Vector3[] Path(Vector3 startTrans, Vector3 endTrans, float centerCtl, int segmentNum)
    {
        var startPoint = startTrans;
        var endPoint = endTrans;
        //�м�㣺�����յ����������ٳ�һ��ϵ����ϵ�����Ը���ʵ�ʵ���
        var bezierControlPoint = (startPoint + endPoint) * centerCtl;

        //segmentNumΪint���ͣ�·����������ֵԽ��·����Խ�࣬����Խƽ��
        Vector3[] path = new Vector3[segmentNum];
        for (int i = 0; i < segmentNum; i++)
        {
            var time = (i + 1) / (float)segmentNum;//�黯��0~1��Χ
            path[i] = GetBezierPoint(time, startPoint, bezierControlPoint, endPoint);//ʹ�ñ��������ߵĹ�ʽȡ��tʱ��·����
        }
        return path;
    }

    /// <param name="t">0��1��ֵ��0��ȡ���ߵ���㣬1������ߵ��յ�</param>
    /// <param name="start">���ߵ���ʼλ��</param>
    /// <param name="center">����������״�Ŀ��Ƶ�</param>
    /// <param name="end">���ߵ��յ�</param>
    private Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
    {
        return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
    }
}
