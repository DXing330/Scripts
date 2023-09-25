using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    private int bigInt = 999999;
    private int size = 0;
    // Actual tile index.
    public List<int> nodes = new List<int>();
    // Move cost of tile.
    public List<int> weights = new List<int>();
    
    public void Reset()
    {
        size  = 0;
        nodes.Clear();
        weights.Clear();
    }

    public void InitialCapacity(int capacity)
    {
        nodes = new List<int>(new int[capacity]);
        weights = new List<int>(new int[capacity]);
    }

    private int getLeftChildIndex(int parentIndex)
    {
        return ((parentIndex*2)+1);
    }

    private int getRightChildIndex(int parentIndex)
    {
        return ((parentIndex*2)+2);
    }

    private int getParentIndex(int childIndex)
    {
        return ((childIndex-1)/2);
    }

    private bool hasLeftChild(int index)
    {
        return (getLeftChildIndex(index) < size);
    }

    private bool hasRightChild(int index)
    {
        return (getRightChildIndex(index) < size);
    }

    private bool hasParent(int index)
    {
        if (index <= 0)
        {
            return false;
        }
        return true;
    }
    // Child/Parents are sorted by weight.

    private int leftChild(int index)
    {
        return weights[getLeftChildIndex(index)];
    }

    private int rightChild(int index)
    {
        return weights[getRightChildIndex(index)];
    }

    private int parent(int index)
    {
        return weights[getParentIndex(index)];
    }

    // Need to swap on both lists.
    private void Swap(int indexOne, int indexTwo)
    {
        int temp = weights[indexOne];
        weights[indexOne] = weights[indexTwo];
        weights[indexTwo] = temp;
        int temp2 = nodes[indexOne];
        nodes[indexOne] = nodes[indexTwo];
        nodes[indexTwo] = temp2;
    }

    private void EnsureCapacity()
    {
        if (weights.Count < size + 1)
        {
            for (int i = 0; i < size; i++)
            {
                nodes.Add(-1);
                weights.Add(bigInt);
            }
        }
    }

    // When looking/pulling you care about the actual tile not the move cost.
    public int Peek()
    {
        if (size == 0)
        {
            return -1;
        }
        return nodes[0];
    }

    public int PeekWeight()
    {
        if (size == 0)
        {
            return -1;
        }
        return weights[0];
    }

    public int Pull()
    {
        if (size == 0)
        {
            return -1;
        }
        int node = nodes[0];
        nodes[0] = nodes[size-1];
        weights[0] = weights[size-1];
        size--;
        HeapifyDown();
        return node;
    }

    public void AddNodeWeight(int newNode, int newWeight)
    {
        EnsureCapacity();
        weights[size] = newWeight;
        nodes[size] = newNode;
        size++;
        HeapifyUp();
    }

    private void HeapifyUp()
    {
        int index = size - 1;
        while (parent(index) > weights[index])
        {
            Swap(getParentIndex(index), index);
            index = getParentIndex(index);
        }
    }

    private void HeapifyDown()
    {
        int index = 0;
        while (hasLeftChild(index))
        {
            int smallerChildIndex = getLeftChildIndex(index);
            if (hasRightChild(index) && weights[getRightChildIndex(index)] < weights[smallerChildIndex])
            {
                smallerChildIndex = getRightChildIndex(index);
            }
            if (weights[index] < weights[smallerChildIndex])
            {
                break;
            }
            Swap(index, smallerChildIndex);
            index = smallerChildIndex;
        }
    }
}
