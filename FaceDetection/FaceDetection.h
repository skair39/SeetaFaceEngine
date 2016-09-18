#pragma once

#include "common.h"

extern "C" {
	SEETA_API void* CreateFaceDetectionSession(const char* pModelFilePath, int minFaceSize, double threshold, double paramidScaleFactor, int step);

	SEETA_API void DeleteFaceDetectionSession(void** pSession);

	SEETA_API int Detect(void* pSession, unsigned char* pData, int width, int height, int maxCount, int* pResult);
}


