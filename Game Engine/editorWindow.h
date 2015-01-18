/*
* Written by Sam Arutyunyan, 2014
* Game Engine editor window
*/
#ifndef EDITOR_WINDOW_H
#define EDITOR_WINDOW_H

#include <windows.h>
#include <gl\gl.h>
#include <gl\glu.h>

class __declspec(dllexport) EditorWindow
{
public:		
	int width;
	int height;
	int aspect;

protected:
	HWND		hWndMain;		// window handle
	HWND		hWndConsole;	// window handle
	HDC			hDC;			// device context
	HGLRC		hGLRC;			// rendering context

public:
	EditorWindow() {}
	EditorWindow(const char *szName, int w, int h, HINSTANCE hInst);
	~EditorWindow();

	// this must be called before the class is used
	static bool RegisterWindow(HINSTANCE hInst);	
	void Render();

private:
	friend LRESULT CALLBACK WndProcMain(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);
	friend LRESULT CALLBACK  WndProcConsole(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

	void SetupPixelFormat();		// setup the pixel format

	// Windows message handling functions
	bool Create();					// WM_CREATE
	void Destroy();					// WM_DESTROY
	void Size();					// WM_SIZE
	
};

#endif