/*
 * Written by Sam Arutyunyan Summer 2013
 * this class creates the graph, creates an object of gameView, sets view to the game layout. 
 * connects game view to the right layout, and assigns game view's nodes array to the one created here. 
 */
package com.example.research;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.lang.reflect.Field;
import java.util.ArrayList;

import com.example.research.GraphNode.NodeState;
import com.example.research.R.raw;
import android.app.Activity;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.os.Bundle;
import android.os.SystemClock;
import android.preference.PreferenceManager;
import android.view.View;
import android.widget.ImageButton;
import android.widget.LinearLayout;

public class PlayGame extends Activity
{
	GameView gameView;
	Graph myGraph;
	
	long timeWhenStopped = 0;//used for pausing chronometer timer	
	ImageButton b_clear;
	
	SharedPreferences prefs;
	Editor editor;

	@Override
	protected void onCreate(Bundle savedInstanceState) 
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.game);
		
		prefs =  PreferenceManager.getDefaultSharedPreferences(this);
		editor = prefs.edit(); 
		
		myGraph = new Graph(false);
		gameView = new GameView(this);//must be created before GenerateGraph is called. 
		
		GenerateGraph();						
		
		gameView.nodes = myGraph.nodes;	
		gameView.edges = myGraph.edges;
		
		//connect view to layout
		((LinearLayout)findViewById(R.id.l_right)).addView(gameView);			
	
		((ImageButton)findViewById(R.id.b_quit)).setOnClickListener(new View.OnClickListener() 
		{			
			@Override
			public void onClick(View v) 
			{					
				finish();							
			}
		});
		
		b_clear = ((ImageButton)findViewById(R.id.b_clear));
		
		b_clear.setOnClickListener(new View.OnClickListener() 
		{			
			@Override
			public void onClick(View v) 
			{	
				if(!gameView.gameWon)
				{										
					if(gameView.clearMode)//this is second button press: wipe out path
					{
						for(GraphNode curNode : myGraph.nodes)
						{
							curNode.nodeState = NodeState.Deactive;
							//curNode.active = false;//replaced?
						}
						for(ArrayList<GraphEdge> curEdgeList : myGraph.edges)
						{
							for(GraphEdge curEdge : curEdgeList)
							{
								curEdge.active = false;
							}				
						}
						gameView.numActiveNodes = 0;
						gameView.clearMode = false;
						b_clear.setBackgroundResource(R.drawable.b_clear);
					}
					else
					{
						gameView.clearMode = true;
						b_clear.setBackgroundResource(R.drawable.b_clear_active);
					}
					
					gameView.invalidate();// calls onDraw
				}//end if(!gamewon)
			}//end onClick
		});
		
		
	}//end onCreate

	
	
	@Override
	protected void onPause() 
	{
		super.onPause();
		if(!gameView.gameWon)//if game is won we shouldnt have to stop timer, it should already be stopped
		{		
			timeWhenStopped = gameView.timer.getBase() - SystemClock.elapsedRealtime();
			gameView.timer.stop();
		}
	}

	@Override
	protected void onResume() 
	{
		super.onResume();
		if(!gameView.gameWon)
		{
			gameView.timer.setBase(SystemClock.elapsedRealtime() + timeWhenStopped);
			gameView.timer.start();
		}
	}

	void GenerateGraph()
	{		
		try
		{						
			Class<raw> res = R.raw.class;
		    Field field = res.getField("puzzle_" + PuzzleSelect.puzzleIndex);//static variable from class PuzzleSelect
			int rawId = field.getInt(null);
			InputStream inputStream =  getResources().openRawResource(rawId);	    
						
		    InputStreamReader inputStreamReader = new InputStreamReader(inputStream);
		    BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
		    String line;	
		    
		    line = bufferedReader.readLine();//read the first line
		    
		    String[] stringArray = new String[5];//holds a broken up string of the first and second number seperated by whitespace	    
		    stringArray = line.split("\\s+");//split by whitespace
		    
		    int numNodes = Integer.parseInt(stringArray[0]);
		    gameView.nodeScale = Float.parseFloat(stringArray[1]);
		    gameView.nodeSpacing = Float.parseFloat(stringArray[2]);
		    float spacingMultiplier = Float.parseFloat(stringArray[2]) * 10f;
		    float leftMost = Float.parseFloat(stringArray[3]);
		    float topMost = Float.parseFloat(stringArray[4]);
		    
		    for(int i = 0; i < numNodes; i++)//read each set of node coords
		    {
		    	line = bufferedReader.readLine();
		    	stringArray = line.split("\\s+");//split by whitespace
		    	myGraph.AddNode(new GraphNode(myGraph.nextNodeIndex, new Vector2d(Float.parseFloat(stringArray[0]) * spacingMultiplier - leftMost , 
		    			Float.parseFloat(stringArray[1]) * spacingMultiplier - topMost)));		
		    	
		        myGraph.edges.add(new ArrayList<GraphEdge>());		    	
		    }
		    
		    while ((line = bufferedReader.readLine()) != null) //now we should be at the edges and we'll just read until we can't read no more...
		    {
		    	stringArray = line.split("\\s+");//split by whitespace
		    	//.get(from) - (from, to)
		    	myGraph.AddEdge(new GraphEdge(Integer.parseInt(stringArray[0]), Integer.parseInt(stringArray[1])));
		    }
		    
		}catch( Exception e){}	
		
		
        
	}//end GenerateGraph()	
	

}
