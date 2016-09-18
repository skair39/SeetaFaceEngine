using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetectionDemo
{
    using Geb.Image;
    using Geb.Utils.WinForm;

    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private ImageU8 _imgGray;
        private ImageRgb24 _img;
        private FaceDetectSession _session;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            this.OpenImageFile((String path) => {
                this.tbPath.Text = path;

                _img = new ImageRgb24(path);
                _imgGray = _img.ToGrayscaleImage();

                this.box.Image = _img.ToBitmap();
            });
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            if (_imgGray == null) return;

            List<Rect> list = _session.Detect(_imgGray);

            if(list != null)
            {
                ImageRgb24 img = _img.Clone();
                foreach(var item in list)
                {
                    img.DrawRect(new RectF(item.X,item.Y,item.Width,item.Height), Rgb24.RED);
                }
                this.box.Image = img.ToBitmap();
            }
            MessageBox.Show("检测完毕");
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            _session = new FaceDetectSession("seeta_fd_frontal_v1.0.bin");
        }
    }
}
