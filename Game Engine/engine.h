/*
* Written by Sam Arutyunyan, 2014
* Game Engine core
*/
#ifndef ENGINE_H
#define ENGINE_H
#include <Windows.h>
#include "editorWindow.h"
#include "constants.h"

class __declspec(dllexport) Engine
{
public:
	EditorWindow* mainWindow;
public:
	Engine();
	~Engine();
	bool init(HINSTANCE hInstance);
	LRESULT run(); // holds main loop
	void shutdown();

};

#endif