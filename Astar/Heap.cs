using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BinaryHeap<T> where T : AstarNode
{

    T[] heap;
    int curCount;
    int leftchildIndex(T i) => i.HeapIndex * 2 + 1;
    int rightchildIndex(T i) => i.HeapIndex * 2 + 2;
    int parentIndex(T i) => (i.HeapIndex - 1) / 2;
    public int Length { get { return curCount; } }
    public BinaryHeap(int maxSize)
    {
        
        heap = new T[maxSize];
        curCount = 0;
    }
    // if is cause heapify error, problem must be unit is too close to wall so it detect
    // null node
    public void Enqueue(T data)
    {
        data.HeapIndex = curCount;
        heap[curCount] = data;
        heapifyup(data);
        curCount++;
    }
    public T Dequeue()
    {
        T firstItem = heap[0]; // 루트노드 복사
        curCount--;
        heap[0] = heap[curCount]; // 마지막원소를 루트로 이동
        heap[0].HeapIndex = 0; // Index 초기화
        heapifydown(heap[0]); // 위치교환 시작
        return firstItem;
    }
    public bool Contains(T item)
    {
        if (item.HeapIndex < curCount)
            return Equals(heap[item.HeapIndex], item);
        else
            return false;
    }

    public void UpdateItem(T item)
    {
        heapifyup(item);
    }

    public void Clear()
    {
        curCount = 0;
    }


    void Swap(T itemA, T itemB)
    {
        heap[itemA.HeapIndex] = itemB;
        heap[itemB.HeapIndex] = itemA;
        int temp = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = temp;
    }

   

    void heapifyup(T i)
    {
        while(true)
        {
            T parent = heap[parentIndex(i)];
            if (parent.Fcost > i.Fcost)
            {
                Swap(i, parent);
            }
            else break;
        }
    }

    void heapifydown(T i)
    {
        while(true)
        {

            int swapIndex;
            if (leftchildIndex(i) < curCount)
            {
                swapIndex = leftchildIndex(i);
                if (rightchildIndex(i) < curCount)
                {
                    if (heap[leftchildIndex(i)].Fcost > heap[rightchildIndex(i)].Fcost)
                    {
                        swapIndex = rightchildIndex(i);
                    }
                }
                if (i.Fcost > heap[swapIndex].Fcost)
                {
                    Swap(i, heap[swapIndex]);
                }
                else return;
            }
            else return;
        }
    }
}