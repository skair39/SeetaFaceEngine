// FaceDetection.cpp : Defines the exported functions for the DLL application.
//

#include "FaceDetection.h"
#include "face_detection.h"

void* CreateFaceDetectionSession(const char* pModelFilePath, int minFaceSize, double threshold, double paramidScaleFactor, int step)
{
	seeta::FaceDetection* detector = new seeta::FaceDetection(pModelFilePath);
	detector->SetMinFaceSize(minFaceSize);
	detector->SetScoreThresh(threshold);
	detector->SetImagePyramidScaleFactor(paramidScaleFactor);
	detector->SetWindowStep(step, step);
	return detector;
};

void DeleteFaceDetectionSession(void** pSession)
{
	if (*pSession == 0) return;

	seeta::FaceDetection* detector = (seeta::FaceDetection*)(*pSession);

	delete detector;

	*pSession = 0;
};

int Detect(void* pSession, unsigned char* pData, int width, int height, int maxCount, int* pResult)
{
	if (pSession == 0 || pResult == 0) return -1;

	seeta::FaceDetection* detector = (seeta::FaceDetection*)(pSession);

	seeta::ImageData img_data;
	img_data.data = pData;
	img_data.width = width;
	img_data.height = height;
	img_data.num_channels = 1;

	std::vector<seeta::FaceInfo> faces = detector->Detect(img_data);

	int num_face = faces.size();
	if (num_face > maxCount) num_face = maxCount;

	for (int i = 0; i < num_face; i++) {
		int idx = i * 4;
		seeta::Rect rect = faces[i].bbox;
		pResult[idx] = rect.x;
		pResult[idx + 1] = rect.y;
		pResult[idx + 2] = rect.width;
		pResult[idx + 3] = rect.height;
	}

	return num_face;
};

