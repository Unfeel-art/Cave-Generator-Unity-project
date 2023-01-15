using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisjointSetUnioin{
    private List<int> parent = new List<int>(0);
    private List<int> size = new List<int>(0);
    public void setNodeNumber(int numberOfNodes)
        {
        for (int i = 0; i < numberOfNodes; i++)
         {
            parent.Add(i);
            size.Add(1);
        }
    }
    private void swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }
    public int findParent(int node)
    {
        if (parent[node] == node) return node;

        return parent[node] = findParent(parent[node]);
    }
    public void uniteSets(int node1, int node2)
    {
        node1 = findParent(node1);
        node2 = findParent(node2);
        if (node1 == node2) return;
        if (size[node1] < size[node2]) swap(ref node1, ref node2);
        parent[node2] = node1;
        size[node1] += size[node2];
    }
}
