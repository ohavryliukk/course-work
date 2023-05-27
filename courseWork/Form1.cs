using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using ExifLib;

namespace courseWork
{
    public partial class Form1 : Form
    {
        Image img;
        OpenFileDialog ofd = new OpenFileDialog();
        Point a;
        Point b;
        Boolean mouseDown = false;
        Rectangle Rect = new Rectangle();
        private static string json_secret_file = @".\client_secret.json";
        private static string application_name = @"Desktop client 1";

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            String device, model;
            DateTime dateTime;
            pictureBox1.Image = null;
            ofd.Filter = "images|*.png;*.jpg;*.jpeg;*.gif";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(ofd.FileName);
                pictureBox2.Image = new Bitmap(ofd.FileName);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                txt_imgpath.Text = ofd.FileName;
                img = Image.FromFile(ofd.FileName);
                lbl_size.Text = pictureBox1.Image.Size.ToString(); // To displaye the image size//
                txt_imgName.Text = Path.GetFileName(ofd.FileName);
                txt_imgTheme.Text = ofd.Title;

                ExifReader reader = new ExifReader(ofd.FileName);

                if (reader.GetTagValue<String>(ExifTags.Model, out model))
                    txt_imgModel.Text = model.ToString();
                if (reader.GetTagValue<String>(ExifTags.Make, out device))
                    txt_imgDevice.Text = device.ToString();
                if (reader.GetTagValue<DateTime>(ExifTags.DateTime, out dateTime))
                    txt_imgDate.Text = dateTime.ToString();
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG|*.JPG|PNG|*.PNG|GIF|*.GIF|BMP|*.BMP|All files (*.*)|*.*";
            sfd.FileName = txt_imgName.Text;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Bitmap bitmap = new Bitmap(pictureBox1.Image))
                    {
                        bitmap.Save(sfd.FileName, ImageFormat.Jpeg);
                    }
                    MessageBox.Show("Image Saved Successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(" Error " + ex);
                }
            }
        }

        new Image Resize(Image image, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics graphic = Graphics.FromImage(bmp);
            graphic.DrawImage(image, 0, 0, width, height);
            graphic.Dispose();
            return bmp;
        }

        private void btn_resize_Click(object sender, EventArgs e)
        {
            int width = Convert.ToInt32(txt_width.Text), height = Convert.ToInt32(txt_height.Text);
            img = Resize(img, width, height);
            pictureBox1.Image = img;
            pictureBox2.Image = img;
            txt_width.Text = "";
            txt_height.Text = "";
            lbl_size.Text = img.Size.ToString();
        }

        void reload()
        {
            img = Image.FromFile(ofd.FileName);
            pictureBox1.Image = img;
        }

        private void btn_reload_Click(object sender, EventArgs e)
        {
            txt_width.Text = "";
            txt_height.Text = "";
            trk_hue.Value = 0;
            trk_contrast.Value = 100;
            trk_bright.Value = 0;
            reload();
        }

        private void btn_rotate_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = pictureBox1.Image;
            pictureBox1.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
            pictureBox1.Image.RotateFlip(RotateFlipType.Rotate90FlipX);
            pictureBox1.Refresh();
        }

        private void btn_normal_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
        }

        private void btn_stretch_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void btn_autosize_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        private void btn_center_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void btn_boxsize_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            a = e.Location;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown == true)
            {
                b = e.Location;
                Refresh();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDown == true)
            {
                b = e.Location;
                mouseDown = false;
            }
        }

        private Rectangle GetRect()
        {
            Rect = new Rectangle();
            Rect.X = Math.Min(a.X, b.X);
            Rect.Y = Math.Min(a.Y, b.Y);
            Rect.Width = Math.Abs(a.X - b.X);
            Rect.Height = Math.Abs(a.Y - b.Y);
            return Rect;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (Rect != null)
            {
                e.Graphics.DrawRectangle(Pens.Aqua, GetRect());
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (Rect.Width != 0 && Rect.Height != 0)
            {
                Bitmap bitm = new Bitmap(pictureBox1.Image, pictureBox1.Width, pictureBox1.Height);
                Bitmap crop = new Bitmap(Rect.Width, Rect.Height);
                Graphics g = Graphics.FromImage(crop);
                g.DrawImage(bitm, 0, 0, Rect, GraphicsUnit.Pixel);
                pictureBox1.Image = crop;
                pictureBox2.Image = crop;
                Rect = new Rectangle();
            }
        }

        void f1()
        {
            img = pictureBox1.Image;
            Bitmap bmpinverted = new Bitmap(img.Width, img.Height);
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix cmpicture = new ColorMatrix(new float[][] {
            new float[] {0,0,1,0,0},
            new float[] {0,1,0,0,0},
            new float[] {0,0,0,0,0},
            new float[] {0,0,0,1,0},
            new float[]{0,0,0,0,1} });
            ia.SetColorMatrix(cmpicture);
            Graphics grps = Graphics.FromImage(bmpinverted);
            grps.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
            grps.Dispose();
            pictureBox1.Image = bmpinverted;
        }

        private void btn_f1_Click(object sender, EventArgs e)
        {
            reload();
            f1();
        }

        void f2()
        {
            img = pictureBox1.Image;
            Bitmap bmpinverted = new Bitmap(img.Width, img.Height);
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix cmpicture = new ColorMatrix(new float[][] {
              new float[]{.393f, .349f, .272f, 0, 0},
              new float[]{.769f, .686f, .534f, 0, 0},
              new float[]{.189f, .168f, .131f, 0, 0},
              new float[]{0, 0, 0, 1, 0},
              new float[]{0, 0, 0, 0, 1} });
            ia.SetColorMatrix(cmpicture);
            Graphics grps = Graphics.FromImage(bmpinverted);
            grps.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
            grps.Dispose();
            pictureBox1.Image = bmpinverted;
        }

        private void btn_f2_Click(object sender, EventArgs e)
        {
            reload();
            f2();
        }

        void f3()
        {
            img = pictureBox1.Image;
            Bitmap bmpinverted = new Bitmap(img.Width, img.Height);
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix cmpicture = new ColorMatrix(new float[][] {
              new float[]{.3f, .3f, .3f, 0, 0},    //Grayscale Filter//
              new float[]{.59f, .59f, .59f, 0, 0},
              new float[]{.11f, .11f, .11f, 0, 0},
              new float[]{0, 0, 0, 1, 0},
              new float[]{0, 0, 0, 0, 1} });
            ia.SetColorMatrix(cmpicture);
            Graphics grps = Graphics.FromImage(bmpinverted);
            grps.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
            grps.Dispose();
            pictureBox1.Image = bmpinverted;
        }

        private void btn_f3_Click(object sender, EventArgs e)
        {
            reload();
            f3();
        }

        void f4()
        {
            img = pictureBox1.Image;
            Bitmap bmpinverted = new Bitmap(img.Width, img.Height);
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix cmpicture = new ColorMatrix(new float[][] {
             new float[] {1,1,0,0,0,0},
             new float[] {0,0,1,0,0},
             new float[] {0,0,0,0,0},
             new float[] {0,0,0,1,0},
             new float[]{0,0,0,0,1} });
            ia.SetColorMatrix(cmpicture);
            Graphics grps = Graphics.FromImage(bmpinverted);
            grps.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
            grps.Dispose();
            pictureBox1.Image = bmpinverted;
        }

        private void btn_f4_Click(object sender, EventArgs e)
        {
            reload();
            f4();
        }

        void f5()
        {
            img = pictureBox1.Image;
            Bitmap bmpinverted = new Bitmap(img.Width, img.Height);
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix cmpicture = new ColorMatrix(new float[][] {
             new float[] {1,1,1,0,0},
             new float[] {0,0,1,1,0},
             new float[] {0,0,0,0,0},
             new float[] {0,0,0,1,0},
             new float[]{0,0,0,0,1} });
            ia.SetColorMatrix(cmpicture);
            Graphics grps = Graphics.FromImage(bmpinverted);
            grps.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
            grps.Dispose();
            pictureBox1.Image = bmpinverted;
        }

        private void btn_f5_Click(object sender, EventArgs e)
        {
            reload();
            f5();
        }

        void hue()
        {
            img = pictureBox1.Image;
            float hue = 0;
            hue = 0.1f * trk_hue.Value;
            Bitmap bmpinverted = new Bitmap(img.Width, img.Height);
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix cmpicture = new ColorMatrix(new float[][] {
             new float[] {1,0,(hue),0,0,0,0},
             new float[] {0,1,0,0,0},
             new float[] {0,0,0,0,0},
             new float[] {0,0,0,1,0},
             new float[] {0,0,0,0,1} });
            ia.SetColorMatrix(cmpicture);
            Graphics grps = Graphics.FromImage(bmpinverted);
            grps.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
            grps.Dispose();
            pictureBox1.Image = bmpinverted;
            pictureBox2.Image = pictureBox1.Image;

        }

        private void trk_hue_ValueChanged(object sender, EventArgs e)
        {
            hue();
        }

        void contrast()
        {
            float contrast = 0;
            contrast = trk_contrast.Value / 100f;
            Bitmap bmpinverted = new Bitmap(img.Width, img.Height);
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix cmpicture = new ColorMatrix(new float[][] {
              new float[]{contrast ,0f,0f,0f,0f },
              new float[]{0f,contrast,0f,0f,0f },
              new float[]{0f,0f,contrast,0f,0f },
              new float[]{0f,0f,0f,1f,0f },
              new float[]{0.001f,0.001f,0.001f,0f,1f} });
            ia.SetColorMatrix(cmpicture);
            Graphics grps = Graphics.FromImage(bmpinverted);
            grps.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
            grps.Dispose();
            pictureBox1.Image = bmpinverted;
        }

        private void trk_contrast_ValueChanged(object sender, EventArgs e)
        {
            contrast();
        }

        private void trk_bright_ValueChanged_1(object sender, EventArgs e)
        {
            adjustBrightness();
        }

        private void adjustBrightness()
        {
            float brightnessValue = trk_bright.Value;
            Bitmap adjustedImage = new Bitmap(img.Width, img.Height);

            using (Graphics graphics = Graphics.FromImage(adjustedImage))
            {
                float brightness = brightnessValue / 100f;
                float[][] colorMatrixElements =
                {
                    new float[] { 1, 0, 0, 0, 0 },
                    new float[] { 0, 1, 0, 0, 0 },
                    new float[] { 0, 0, 1, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { brightness, brightness, brightness, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);

                graphics.DrawImage(
                    img,
                    new Rectangle(0, 0, img.Width, img.Height),
                    0, 0, img.Width, img.Height,
                    GraphicsUnit.Pixel,
                    imageAttributes);
            }
            pictureBox1.Image = adjustedImage;
        }
    }
}
