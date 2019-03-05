using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SnapScrollChooseLocations : MonoBehaviour
{
    //объект в котором находятся все элементы скролла
    //а сам PARENT_content назначен в компонент ScrollRect в поле Content
    public Transform PARENT_content;
    //это пустой GameObject который нужно создать как дочерний объект к вашему объекту на котором назначен ScrollRect
    //далее этот centralPoint нужно расположить на канвасе так как нужно вам - относительно этой точки и будет снаппинг элементов скролла
    public Transform centralPoint;
    //тут будем хранить расстояние между центром и каждым элементом скролла
    private List<float> distancesToCenter = new List<float>();
    //тут храним расстояние между родительским объектом элементов скролла и всех элем. скролла
    public List<float> distancesToScroll = new List<float>();
    //индекс элемента с минимальным расстоянием до центра
    public int indexMinDist;
    //происходит ли драг скролла
    private bool dragging;
    bool isFirstDrag = true;

    private void Start()
    {
        transform.GetChild(1).GetChild(0).transform.localScale = transform.GetChild(indexMinDist).transform.localScale;
    }

    private void Update()
    {
        CalcDistanceScrollToPhoto();
        photoSnap();
    }

    //это метод нужно вызвать через компонент Event Trigger -> на событие Begin Drag
    //Event Trigger нужно назначить на объект в котором компонент ScrollRect
    //этот метод будет вызван на момент начала драга
    public void BeginDrag()
    {
        if (isFirstDrag)
        {
            transform.GetChild(1).GetChild(0).transform.localScale = new Vector2(0.86f, 0.86f);
            isFirstDrag = false;
        }
        dragging = true;
        distancesToCenter.Clear();
        transform.GetChild(indexMinDist).GetChild(0).transform.localScale = new Vector2(0.86f, 0.86f);
    }

    //это метод нужно вызвать через компонент Event Trigger -> на событие End Drag
    //Event Trigger нужно назначит на объект в котором компонент ScrollRect
    //этот метод будет вызван на момент конца драга (когда отпустят кнопку мыши)
    public void EndDragPhoto()
    {
        //вычисляем дистанцию между скролл-контент точкой и фото
        CalcDistanceScrollToPhoto();

        //получаем дистанцию между центром и каждой фотографией
        for (int i = 0; i < PARENT_content.childCount; i++)
        {
            //вычисляем и получаем абсолютное значение
            distancesToCenter.Add(Mathf.Abs(centralPoint.position.x - PARENT_content.GetChild(i).position.x));
        }
        //вычисляем минимальный индекс к центру
        for (int i = 0; i < distancesToCenter.Count; i++)
        {
            if (distancesToCenter[i] == distancesToCenter.Min()) { indexMinDist = i; }
        }

        transform.GetChild(indexMinDist).GetChild(0).transform.localScale = transform.GetChild(indexMinDist).transform.localScale;
        dragging = false;
    }

    //снаппинг (подгонка) фото к центру
    void photoSnap()
    {
        //если скролл не драгается
        if (dragging == false)
        {
            //перемещаем контент-объект с со всеми элементами (центровка самого ближнего к центру элемента)
            float posX = Mathf.Lerp(PARENT_content.position.x, centralPoint.position.x - distancesToScroll[indexMinDist], Time.deltaTime * 20);
            Vector2 pos = new Vector2(posX, PARENT_content.position.y);
            PARENT_content.position = pos;
        }
    }

    //получаем дистанцию каждой фотографии до скролл-объекта
    public void CalcDistanceScrollToPhoto()
    {
        distancesToScroll.Clear();
        for (int i = 0; i < PARENT_content.childCount; i++)
        {
            //вычисляем дистанцию между скролл-контент точкой и фото
            distancesToScroll.Add(Mathf.Abs(PARENT_content.position.x - PARENT_content.GetChild(i).position.x));
        }
    }


}
