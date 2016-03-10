/* Begins recording Audio using msdn Waveform. It will continue to do so until the user requests StopRecord()
*  The audio is not saved anywhere unless the user calls PollAudio every time frame.
*/
bool StartRecord()
{
	//declare							
	static WAVEFORMATEX wfx;

	//init
	pWaveHdr1 = (PWAVEHDR)malloc(sizeof(WAVEHDR));
	pWaveHdr2 = (PWAVEHDR)malloc(sizeof(WAVEHDR));
	pSaveBuffer = (PBYTE)malloc(1);

	//record
	pBuffer1 = (PBYTE)malloc(INP_BUFFER_SIZE);
	pBuffer2 = (PBYTE)malloc(INP_BUFFER_SIZE);
	if (!pBuffer1 || !pBuffer2)
	{
		if (pBuffer1) free(pBuffer1);
		if (pBuffer2) free(pBuffer2);
		printf("Error allocating memory!");
		return false;
	}

	wfx.wFormatTag = WAVE_FORMAT_PCM;
	wfx.nChannels = 1;
	wfx.nSamplesPerSec = FREQUENCY;
	wfx.nAvgBytesPerSec = FREQUENCY;
	wfx.nBlockAlign = 1; //wfx.wBitsPerSample * wfx.nChannels / 8;
	wfx.wBitsPerSample = 8;
	wfx.cbSize = 0;

	if (waveInOpen(&hWaveIn, WAVE_MAPPER, &wfx, NULL, NULL, WAVE_FORMAT_DIRECT))
	{
		free(pBuffer1);
		free(pBuffer2);
		printf("Error opening waveform in!");
		return false;
	}

	pWaveHdr1->lpData = (LPSTR)pBuffer1;
	pWaveHdr1->dwBufferLength = INP_BUFFER_SIZE;
	pWaveHdr1->dwBytesRecorded = 0;
	pWaveHdr1->dwUser = 0;
	pWaveHdr1->dwFlags = 0;
	pWaveHdr1->dwLoops = 1;
	pWaveHdr1->lpNext = NULL;
	pWaveHdr1->reserved = 0;

	waveInPrepareHeader(hWaveIn, pWaveHdr1, sizeof(WAVEHDR));

	pWaveHdr2->lpData = (LPSTR)pBuffer2;
	pWaveHdr2->dwBufferLength = INP_BUFFER_SIZE;
	pWaveHdr2->dwBytesRecorded = 0;
	pWaveHdr2->dwUser = 0;
	pWaveHdr2->dwFlags = 0;
	pWaveHdr2->dwLoops = 1;
	pWaveHdr2->lpNext = NULL;
	pWaveHdr2->reserved = 0;

	waveInPrepareHeader(hWaveIn, pWaveHdr2, sizeof(WAVEHDR));

	waveInAddBuffer(hWaveIn, pWaveHdr1, sizeof(WAVEHDR));
	waveInAddBuffer(hWaveIn, pWaveHdr2, sizeof(WAVEHDR));

	// Begin sampling
	waveInStart(hWaveIn);
	return true;
}

/* Closes the waveform and returns data to user as a PBYTE
*/
PBYTE StopRecord(int* dwDataLength)
{
	if (pWaveHdr1->dwBytesRecorded > 0)//if header is full
	{
		pNewBuffer = (PBYTE)realloc(pSaveBuffer, *dwDataLength + (pWaveHdr1)->dwBytesRecorded);
		if (pNewBuffer == NULL)
		{
			waveInClose(hWaveIn);
			printf("Error allocating memory!");
			return NULL;
		}
		pSaveBuffer = pNewBuffer;
		CopyMemory(pSaveBuffer + *dwDataLength, (pWaveHdr1)->lpData, (pWaveHdr1)->dwBytesRecorded);
		*dwDataLength += (pWaveHdr1)->dwBytesRecorded;
		// Send out a new buffer
		waveInAddBuffer(hWaveIn, pWaveHdr1, sizeof(WAVEHDR));
	}
	if (pWaveHdr2->dwBytesRecorded > 0)//if header is full
	{
		pNewBuffer = (PBYTE)realloc(pSaveBuffer, *dwDataLength + (pWaveHdr2)->dwBytesRecorded);
		if (pNewBuffer == NULL)
		{
			waveInClose(hWaveIn);
			printf("Error allocating memory!");
			return NULL;
		}
		pSaveBuffer = pNewBuffer;
		CopyMemory(pSaveBuffer + *dwDataLength, (pWaveHdr2)->lpData, (pWaveHdr2)->dwBytesRecorded);
		*dwDataLength += (pWaveHdr2)->dwBytesRecorded;
		// Send out a new buffer			
	}

	//close wavein, recording complete	
	waveInReset(hWaveIn);//!actually important for data to terminate properly
	waveInUnprepareHeader(hWaveIn, pWaveHdr1, sizeof(WAVEHDR));
	waveInUnprepareHeader(hWaveIn, pWaveHdr2, sizeof(WAVEHDR));
	waveInClose(hWaveIn);
	free(pBuffer1);
	free(pBuffer2);
	free(pWaveHdr1);
	free(pWaveHdr2);
	return pSaveBuffer;
}
