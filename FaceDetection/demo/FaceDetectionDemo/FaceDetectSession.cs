using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionDemo
{
    using Geb.Image;

    public unsafe class FaceDetectSession : IDisposable
    {
        [DllImport("FaceDetection.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe void* CreateFaceDetectionSession(String pModelFilePath, int minFaceSize, double threshold, double paramidScaleFactor, int step);

        [DllImport("FaceDetection.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe void DeleteFaceDetectionSession(void** pSession);

        [DllImport("FaceDetection.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe int Detect(void* pSession, Byte* pData, int width, int height, int maxCount, int* pResult);

        private unsafe void* _pSession;

        /// <summary>
        /// 人脸检测Session
        /// </summary>
        /// <param name="modelPath">模型文件路径</param>
        /// <param name="minFaceSize">最小人脸尺寸，单位是像素</param>
        /// <param name="threshold">检测阈值。阈值越小，检测出来的人脸越多</param>
        /// <param name="paramidScaleFactor">金字塔缩放系数</param>
        /// <param name="step">扫描步长（单位是像素）</param>
        public FaceDetectSession(String modelPath, int minFaceSize = 20, double threshold = 1, double paramidScaleFactor = 0.8, int step = 4)
        {
            _pSession = CreateFaceDetectionSession(modelPath, minFaceSize, threshold, paramidScaleFactor, step);
        }

        public List<Rect> Detect(String path)
        {
            using (ImageU8 img = new ImageU8(path))
            {
                return Detect(img);
            }
        }

        public List<Rect> Detect(ImageU8 img)
        {
            const int MAX_COUNT = 256;

            int[] result = new int[MAX_COUNT * 4];
            fixed(int* pResult = result)
            {
                int count = Detect(_pSession, img.Start, img.Width, img.Height, MAX_COUNT, pResult);
                if (count <= 0) return null;

                List<Rect> list = new List<Rect>(count);
                for(int i = 0; i < count; i++)
                {
                    int idx = 4 * i;
                    Rect rect = new Rect(pResult[idx], pResult[idx + 1], pResult[idx + 2], pResult[idx + 3]);
                    list.Add(rect);
                }
                return list;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
                if(_pSession != null)
                {
                    void* pSession = _pSession;
                    DeleteFaceDetectionSession(&pSession);
                    _pSession = null;
                }
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
