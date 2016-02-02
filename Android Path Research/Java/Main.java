/*
 * Written by Sam Arutyunyan Summer 2013
 */
package com.example.research;


import android.os.Bundle;
import android.preference.PreferenceManager;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.view.KeyEvent;
import android.view.View;
import android.view.WindowManager;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;

public class Main extends Activity
{
	//Button b_playGame; //gonna do direct connetion instead
	EditText et_playerName;	
	SharedPreferences prefs;
	Editor editor;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) 
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.main);
		et_playerName = ((EditText)findViewById(R.id.et_playername));
		prefs =  PreferenceManager.getDefaultSharedPreferences(this);
		editor = prefs.edit(); 
		et_playerName.setText(prefs.getString("playerName", ""));	
		
		//prevent auto keyboard open
		getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_ALWAYS_HIDDEN);
		
		((Button)findViewById(R.id.b_puzzleSelect)).setOnClickListener(new View.OnClickListener() 
		{			
			@Override
			public void onClick(View v) 
			{		
				//editor.clear();
				//editor.commit();
				startActivity(new Intent("android.intent.action.PUZZLESELECT"));
			}
		});
		
		((Button)findViewById(R.id.b_btimes)).setOnClickListener(new View.OnClickListener() 
		{			
			@Override
			public void onClick(View v) 
			{		
				startActivity(new Intent("android.intent.action.BESTTIMES"));				
			}
		});
		
		
		et_playerName.setOnKeyListener(new View.OnKeyListener()
		{			
			@Override
			public boolean onKey(View v, int keyCode, KeyEvent event)
			{			        
	            if (keyCode==KeyEvent.KEYCODE_ENTER) 
	            { 
	            	if (et_playerName.getText().toString().length() != 0) 
					{
		            	et_playerName.setText(String.valueOf(et_playerName.getText()));
					   	editor.putString( "playerName", String.valueOf(et_playerName.getText()));
					   	editor.commit();					   				   	
					}
	            	//hide keyboard		
	            	InputMethodManager img = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
				   	img.hideSoftInputFromWindow(et_playerName.getWindowToken(), 0);
				   	return true;
	            }
	            return false;
			}
		});		

	}

}
