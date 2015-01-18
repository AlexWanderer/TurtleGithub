/*
* Written by Sam Arutyunyan, 2014
* Game Engine entry point
*/
#define WIN32_MEAN_AND_LEAN
#define WIN32_EXTRA_LEAN

#include <windows.h>
#include <GL\GL.h>
#include "engine.h"
#include "constants.h"

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
	Engine* engine = new Engine();

	//tries to register window. if fails, we don't run.
	if(engine->init(hInstance))
	{
		engine->run();
	}	
	engine->shutdown();
	SAFE_DELETE(engine);
	return 0;
}
