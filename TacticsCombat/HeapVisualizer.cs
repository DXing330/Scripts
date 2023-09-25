using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeapVisualizer : MonoBehaviour
{
    public Text top;
    public Text leftChild;
    public Text rightChild;
    public Text leftLeftChild;
    public Text leftRightChild;
    public Text rightRightChild;
    public TerrainPathfinder pathfinder;

    void Update()
    {
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        top.text = pathfinder.heap.weights[0].ToString();
        leftChild.text = pathfinder.heap.weights[1].ToString();
        rightChild.text = pathfinder.heap.weights[2].ToString();
        leftLeftChild.text = pathfinder.heap.weights[3].ToString();
        leftRightChild.text = pathfinder.heap.weights[4].ToString();
        rightRightChild.text = pathfinder.heap.weights[5].ToString();
    }
}
