/*
* Written by Sam Arutyunyan, Summer 2013
* node graph and path finder for AI practice...
*/
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GraphGenerator : MonoBehaviour
{
    Graph myGraph;
    int width = 10;//x, j
    int depth = 10;//z, i
    float nodeSize = 1;//how far apart each node is

    //for display feedback
    List<GraphEdge> mySPT;//local copy of the search tree since the class will be destroyed after start

    // Use this for initialization
    void Start()
    {                 
        myGraph = new Graph(false);
        mySPT = new List<GraphEdge>();

        //auto graph-----------------

        //create all nodes first
        for (int i = 0; i < depth; ++i)
        {
            for (int j = 0; j < width; ++j)//i equates to z axis (depth), j to x axis(width)
            {               
                myGraph.AddNode(new NavGraphNode(myGraph.GetNextFreeNodeIndex(), new Vector3(nodeSize * j, 0, nodeSize * -i)));//- makes it create downwards
                myGraph.edges.Add(new List<GraphEdge>());//create a list that will hold this node's edges
                // Debug.Log("added node at index: " + myGraph.GetNextFreeNodeIndex() - 1);
                //Debug.Log("added node at position: " + (myGraph.nodes[myGraph.GetNextFreeNodeIndex() - 1] as NavGraphNode).position);
            }
        }

        //now create all connections: //the AddEdge() takes care of digraph
        for (int i = 0; i < depth; ++i)
        {
            for (int j = 0; j < width; ++j)//i equates to z axis (depth), j to x axis(width)
            {
                //diagnoal connections: at each node, create a node at +1i,+1j and +1i,-1j. always creating downwards. 

                //j is always between 0 and the max width
                if (j < width - 1)//width is 1 bigger than the index range
                {
                    myGraph.AddEdge(new GraphEdge(j + (width * i), j + (width * i) + 1));//edge to the node to the right

                    if (i < depth - 1)//add diagonal to bottom right
                    {
                        myGraph.AddEdge(new GraphEdge(j + (width * i), j + (width * i) + width + 1));//if +width is right below, then +1 is to the right of right below
                    }
                }
                if (i < depth - 1)//depth is 1 bigger than the index range
                {
                    myGraph.AddEdge(new GraphEdge(j + (width * i), j + (width * i) + width));//edge to the node to the bottom

                    //if j is > 0 create bottom-left diagonal (we cant go left if we are all the way on the left side)
                    if (j > 0)
                    {
                        myGraph.AddEdge(new GraphEdge(j + (width * i), j + (width * i) + width - 1));//if +width is right below, then -1 is to the left of right below
                    }
                }
            }
        }
       
        /*/add a cost to a specific edge
        foreach(GraphEdge ed in myGraph.edges[20])//from
        {
            if (ed.to == 21)//to
            {
                ed.Cost = 5f;
            }
        }
        */       

        //----------Search
        Graph_SearchAStar s = new Graph_SearchAStar(myGraph, 0, 56);
        List<int> resultPath = new List<int>();
        resultPath = s.GetPathToTarget();
        foreach (int p in resultPath)
        {
            Debug.Log(myGraph.nodes[p].Index + "->");//could have also just printed p
        }

        //debug the path. to do this in gizmo we have to copy tempList into mySPT one at a time
        //actually not sure, test it out. a theory that if a reference is assigned and teh class deleted, maybe the memory holds on
        List<GraphEdge> tempList = new List<GraphEdge>();
        tempList = s.GetSPT();
        foreach (GraphEdge edge in tempList)
        {
            if (edge != null)
            {
                if (edge.from != -1 && edge.to != -1)
                {
                    Debug.DrawLine((myGraph.nodes[edge.From] as NavGraphNode).position, (myGraph.nodes[edge.To] as NavGraphNode).position, Color.red, 30);
                }
            }
        }        
    }

    void OnDrawGizmos()//runs whenever you update teh scene view with mouse movement
    {
        if (myGraph == null) return;
 //       Debug.Log("called");
        Gizmos.color = new Color(.46f, .53f, .57f, 1);

        foreach (List<GraphEdge> edgeList in myGraph.edges)//this is for a digraph
        {
            foreach (GraphEdge edge in edgeList)
            {            
                Gizmos.DrawLine( (myGraph.nodes[edge.From] as NavGraphNode).position, (myGraph.nodes[edge.To] as NavGraphNode).position);
                Gizmos.DrawIcon((myGraph.nodes[edge.From] as NavGraphNode).position, "graph_dot.jpg");//should be inside a Gizmos Folder
            }
        }

        //draw teh search tree?
        //Gizmos.color = Color.red;

       /* foreach (GraphEdge edge in mySPT)
        {
            if (edge != null)
            {
                if(edge.from != -1 && edge.to != -1)
                Gizmos.DrawLine((myGraph.nodes[edge.From] as NavGraphNode).position, (myGraph.nodes[edge.To] as NavGraphNode).position);
            }
        }
        * */
    }
}

/*Manual graph: 
        myGraph.nodes.Add(new NavGraphNode(0, Vector3.zero));
        myGraph.edges.Add(new List<GraphEdge>());
        myGraph.nodes.Add(new NavGraphNode(1, Vector3.forward));
        myGraph.edges.Add(new List<GraphEdge>());
        myGraph.nodes.Add(new NavGraphNode(2, new Vector3(1, 0, 1)));
        myGraph.edges.Add(new List<GraphEdge>());
        myGraph.nodes.Add(new NavGraphNode(3, new Vector3(2, 0, .5f)));
        myGraph.edges.Add(new List<GraphEdge>());

        //connect edges
        myGraph.edges[0].Add(new GraphEdge(0, 1, 5));
        myGraph.edges[0].Add(new GraphEdge(0, 2, 6));
        myGraph.edges[1].Add(new GraphEdge(1, 2, 4));
        myGraph.edges[2].Add(new GraphEdge(2, 3, 5));
*/